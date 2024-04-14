using Cinemachine;
using CustomUnityEvents;
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
                if (finder != null)
                    finder.enabled = false;
                finder = value;
                if (finder != null)
                    finder.enabled = !targetLock;
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
            }
        }


        // Swaps and Locks
        private int zoomLevel = 0;
        public void SetZoom(int level)
        {
            zoomLevels[level].gameObject.SetActive(true);
            zoomLevels[zoomLevel].gameObject.SetActive(false);
            zoomLevel = level;
            virtualCamera = zoomLevels[zoomLevel];
        }
        public void Zoom(bool zoomIn)
        {
            int dir = zoomIn ? -1 : 1;
            if (zoomLevel + dir == Mathf.Clamp(zoomLevel + dir, 0, zoomLevels.Count-1))
                SetZoom(zoomLevel + dir);
        }

        public bool HasTarget()
        {
            return hoveringOver.Count > 0;
        }

        public void SetTargetLock(bool targetLock)
        {
            this.targetLock = targetLock;
            Finder.enabled = !targetLock;
            Finder.SetLockOn(targetLock);
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

        public void SwitchTargets(bool left)
        {
            Print($"Switch targets {(left ? "left" : "right")}.", debug);
            if (finder.selectables.Count > 1)
            {
                // Set selected to the next nearest selectable in the given direction.
                DeSelect();
                finder.TargetIdx += (left ? -1 : 1);
                Select();
                onTargetSet.Invoke(selected);
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
                targetGroup.m_Targets[1].target = selected.transform;
        }

        public override void DeSelect()
        {
            base.DeSelect();
            targetGroup.m_Targets[1].target = null;
        }
    }
}
