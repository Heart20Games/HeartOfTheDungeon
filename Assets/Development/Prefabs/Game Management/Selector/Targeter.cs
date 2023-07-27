using System.Collections.Generic;
using UnityEngine;

namespace Selection
{
    public class Targeter : Selector, ILooker
    {
        public static Targeter main;

        public float range = 10f;
        [ReadOnly][SerializeField] public List<ASelectable> selectableBank = new();
        public bool targetLock = false;

        private TargetFinder finder;
        public TargetFinder Finder
        {
            get => finder;
            set { finder = value; finder.enabled = !targetLock; }
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
            if (targetLock) Select();
            else DeSelect();
        }

        public void SwitchTargets(bool left)
        {
            if (finder.selectables.Count > 1)
            {
                // Set selected to the next nearest selectable in the given direction.
                DeSelect();
                finder.TargetIdx += left ? -1 : 1;
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
