using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Selection
{
    public class Targeter : Selector, ILooker
    {
        public static Targeter main;


        [ReadOnly] public List<ASelectable> selectableBank = new();

        [Header("Targeting")]
        public CinemachineVirtualCamera virtualCamera;
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

        private void Awake()
        {
            main = this;
            selectableBank.AddRange(FindObjectsByType<ASelectable>(FindObjectsSortMode.None));
        }


        // Swaps and Locks
        public void SetTargetLock(bool targetLock)
        {
            this.targetLock = targetLock;
            Finder.enabled = !targetLock;
            virtualCamera.enabled = targetLock;
            if (targetLock) Select();
            else DeSelect();
            targetGroup.m_Targets[0].target = finder == null ? null : finder.transform;
            targetGroup.m_Targets[1].target = selected == null ? null : selected.transform;
        }

        public void SwitchTargets(bool left)
        {
            if (finder.selectables.Count > 1)
            {
                // Set selected to the next nearest selectable in the given direction.
                DeSelect();
                finder.TargetIdx += (left ? -1 : 1);
                Select();
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
        }

        public override void DeSelect()
        {
            base.DeSelect();
        }
    }
}
