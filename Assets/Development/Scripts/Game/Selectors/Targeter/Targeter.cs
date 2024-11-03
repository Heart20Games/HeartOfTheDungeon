using Cinemachine;
using HotD;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Selection
{
    public class Targeter : Selector, ILooker
    {
        public static Targeter main;

        [ReadOnly] public List<ASelectable> selectableBank = new();
        [ReadOnly] public List<IPartySpawner> partySpawners = new();
        [SerializeField] private bool debug = false;

        [Header("Targeting")]
        public List<CinemachineVirtualCamera> zoomLevels = new();
        public CinemachineVirtualCamera virtualCamera;
        public CinemachineCollider cmCollider;
        public CinemachineTargetGroup targetGroup;
        [ReadOnly][SerializeField] private TargetFinder finder;
        [ReadOnly][SerializeField] private bool targetLock = false;
        public TargetFinder Finder
        {
            get => finder;
            set {
                finder = value;
                if (finder != null)
                    finder.enabled = true;
            }
        }
        public UnityEvent<ASelectable> onTargetSet;

        private void Awake()
        {
            if (virtualCamera != null)
                cmCollider = virtualCamera.GetComponent<CinemachineCollider>();
            main = this;
            selectableBank.AddRange(FindObjectsByType<ASelectable>(FindObjectsSortMode.None));
            if (!zoomLevels.Contains(virtualCamera))
                zoomLevels.Add(virtualCamera);
            partySpawners = new List<IPartySpawner>(FindObjectsOfType<BaseMonoBehaviour>().OfType<IPartySpawner>());
            foreach (IPartySpawner party in partySpawners)
            {
                party.RegisterPartyAdder(AddTarget);
                party.RegisterPartyRemover(RemoveTarget);
            }
        }

        // New Parties
        public void AddTarget(ASelectable selectable)
        {
            Print("Adding target!", debug);
            if (!selectableBank.Contains(selectable))
            {
                selectableBank.Add(selectable);
            }
        }

        public void RemoveTarget(ASelectable selectable)
        {
            Print("Removing target!", debug);
            selectableBank.Remove(selectable);
        }

        // Looking

        [Header("Looking")]
        //[SerializeField] private float zoomSpeed = 100f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float rotationSpeed = 1f;
        [ReadOnly][SerializeField] private Vector2 lookVector = Vector2.zero;

        [Header("Distances")]
        [SerializeField] private float far;
        [SerializeField] private float mid;
        [SerializeField] private float near;
        public void Look(Vector2 vector)
        {
            Print($"Targeter Looking ({vector})", debug);
            lookVector = vector;
            cmCollider.m_MinimumDistanceFromTarget = minDistance;
            cmCollider.m_DistanceLimit = maxDistance;
        }

        private void FixedUpdate()
        {
            if (targetLock)
            {
                if (lookVector != Vector2.zero)
                {
                    Print("Targeter Zooming", debug);

                    //virtualCamera.zoom += zoomSpeed * Time.fixedDeltaTime * Mathf.Sign(lookVector.y);
                    //cmCollider.m_DistanceLimit = Mathf.Clamp(cmCollider.m_DistanceLimit, cmCollider.m_MinimumDistanceFromTarget, cmCollider.m_DistanceLimit);
                }
                Transform target = targetGroup.m_Targets[0].target;
                if (targetGroup != null && target != null)
                {
                    var targetRotation = Quaternion.LookRotation(target.transform.position - targetGroup.transform.position);

                    // Smoothly rotate towards the target point.
                    targetGroup.transform.rotation = Quaternion.Slerp(targetGroup.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                    //targetGroup.transform.LookAt(targetGroup.m_Targets[0].target);
                }

                if(targetGroup.m_Targets[1].target == null)
                {
                    HotD.Game.main.CurCharacter.AimCamera.gameObject.SetActive(true);
                    SetTargetLock(false);
                    Game.main.GetComponent<GameInput>().TurnOffLockOn();
                    targetLock = false;
                }

                CheckDistance();
            }
        }


        // Swaps and Locks
        private int zoomLevel = 0;
        public void SetZoom(int level)
        {
            zoomLevels[zoomLevel].gameObject.SetActive(false);
            zoomLevel = level;
            zoomLevels[level].gameObject.SetActive(true);
            virtualCamera = zoomLevels[zoomLevel];
        }
        public void Zoom(bool zoomIn)
        {
            int dir = zoomIn ? -1 : 1;
            if (zoomLevel + dir == Mathf.Clamp(zoomLevel + dir, 0, zoomLevels.Count-1))
                SetZoom(zoomLevel + dir);
        }

        private void CheckDistance()
        {
            if(selected != null)
            {
                float distance = Vector3.Distance(targetGroup.m_Targets[0].target.transform.position, targetGroup.m_Targets[1].target.transform.position);

                if(distance >= far)
                {
                    if(zoomLevel != 2)
                        SetZoom(2);
                }
                if(distance >= mid && distance < far)
                {
                    if(zoomLevel != 1)
                        SetZoom(1);
                }
                if(distance >= near && distance < mid)
                {
                    if(zoomLevel != 0)
                        SetZoom(0);
                }
            }
        }

        public bool HasTarget()
        {
            return hoveringOver.Count > 0;
        }

        public void SetTargetLock(bool targetLock)
        {
            this.targetLock = targetLock;

            if (Finder == null)
            {
                Debug.LogWarning("No Target Finder found. Can't set Target Lock.", this);
                return;
            }

            Finder.SetLockOn(targetLock);
            ResetCameras();
            virtualCamera.gameObject.SetActive(targetLock);
            if (targetLock)
            {
                Select();
                onTargetSet.Invoke(selected);
                targetGroup.m_Targets[0].target = finder == null ? null : finder.transform;
                targetGroup.m_Targets[1].target = selected == null ? null : selected.transform;
            }
            else
            {
                onTargetSet.Invoke(null);
                DeSelect();
            }
        }

        private void ResetCameras()
        {
            foreach(CinemachineVirtualCamera cams in zoomLevels)
            {
                cams.gameObject.SetActive(false);
            }

            zoomLevel = 0;
            SetZoom(zoomLevel);
        }

        public void SwitchTargets(int newIdx)
        {
            // Set selected to the next nearest selectable in the given direction.
            DeSelect();
            finder.TargetIdx = newIdx % finder.selectables.Count;
            Select();
            onTargetSet.Invoke(selected);
        }
        public void SwitchTargets(bool left)
        {
            Print($"Switch targets {(left ? "left" : "right")}.", debug);
            if(finder.selectables.Count > 1)
            {
                int newIdx = finder.TargetIdx + (left ? -1 : 1);
                SwitchTargets(newIdx);
            }
        }

        public void TargetLost(bool stillThere=true)
        {
            Print($"Target lost? {(!stillThere ? "Yes" : "No")}", debug);
            if (!stillThere)
            {
                if (finder.selectables.Count > 1)
                    SwitchTargets(finder.TargetIdx + 1);
                else
                    DeSelect();
            }
        }

        // Validation
        public bool Validate(ASelectable selectable)
        {
            return selectable.visible && SelectableTypes.Contains(selectable.Type);
        }

        // Overrides
        public override void Select()
        {
            base.Select();
            if (selected != null)
            {
                targetGroup.m_Targets[1].target = selected.transform;
                selected.onIsSelectable.AddListener(TargetLost);
            }
        }

        public override void DeSelect()
        {
            if (selected != null)
            {
                selected.onIsSelectable.RemoveListener(TargetLost);
            }
            base.DeSelect();
            targetGroup.m_Targets[1].target = null;
        }
    }
}
