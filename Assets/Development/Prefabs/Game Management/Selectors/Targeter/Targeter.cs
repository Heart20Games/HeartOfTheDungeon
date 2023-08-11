using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Selection
{
    public class Targeter : Selector, ILooker
    {
        public static Targeter main;


        [ReadOnly] public List<ASelectable> selectableBank = new();
        [SerializeField] private bool debug = false;

        [Header("Targeting")]
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
        public BinaryEvent<ASelectable> onTargetSet;

        private void Awake()
        {
            if (virtualCamera != null)
                cmCollider = virtualCamera.GetComponent<CinemachineCollider>();
            main = this;
            selectableBank.AddRange(FindObjectsByType<ASelectable>(FindObjectsSortMode.None));
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
            if (debug) print($"Targeter Looking ({vector})");
            lookVector = vector;
            cmCollider.m_MinimumDistanceFromTarget = minDistance;
            cmCollider.m_DistanceLimit = maxDistance;
        }

        private void FixedUpdate()
        {
            if (lookVector != Vector2.zero)
            {
                if (debug) print("Targeter Zooming");
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


        // Swaps and Locks
        public void SetTargetLock(bool targetLock)
        {
            this.targetLock = targetLock;
            Finder.enabled = !targetLock;
            virtualCamera.gameObject.SetActive(targetLock);
            if (targetLock)
            {
                Select();
                onTargetSet.enter.Invoke(selected);
            }
            else
            {
                onTargetSet.exit.Invoke(selected);
                DeSelect();
            } 
            targetGroup.m_Targets[0].target = finder == null ? null : finder.transform;
            targetGroup.m_Targets[1].target = selected == null ? null : selected.transform;
        }

        public void SwitchTargets(bool left)
        {
            if (debug) print($"Switch targets {(left ? "left" : "right")}.");
            if (finder.selectables.Count > 1)
            {
                // Set selected to the next nearest selectable in the given direction.
                DeSelect();
                finder.TargetIdx += (left ? -1 : 1);
                Select();
                onTargetSet.enter.Invoke(selected);
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
            targetGroup.m_Targets[1].target = selected.transform;
        }

        public override void DeSelect()
        {
            base.DeSelect();
            targetGroup.m_Targets[1].target = null;
        }
    }
}
