using Sorting;
using System.Collections.Generic;
using UnityEngine;

namespace Selection
{
    public class TargetFinder : BaseMonoBehaviour
    {
        public Targeter Main { get => Targeter.main; }

        public float range = 10f;
        public LayerMask obstacleMask;
        [ReadOnly] public List<ASelectable> selectables = new();
        private ASelectable attachedSelectable;
        ObjectDistanceSort<ASelectable> distanceSort;

        private int targetIdx = 0;
        public int TargetIdx { get => targetIdx; set => SetTargetIdx(value); }
        private ASelectable lastTarget;

        private void Awake()
        {
            attachedSelectable = GetComponent<ASelectable>();
            distanceSort = new(selectables, transform);
        }

        public void FixedUpdate()
        {
            if (isActiveAndEnabled)
            {
                lastTarget = GetSelectable(targetIdx);
                ClearSelectables();
                FillSelectables();
                SortSelectables();
                TargetIdx = selectables.IndexOf(Main.selected);
            }
        }

        private void SetTargetIdx(int newIdx)
        {
            targetIdx = (newIdx < 0 && selectables.Count > 0) ? 0 : newIdx;

            // Unhover the old, Hover the new
            ASelectable newTarget = GetSelectable(targetIdx);
            if (lastTarget != null && (targetIdx < 0 || lastTarget != newTarget))
                Main.UnHover(lastTarget);
            if (newTarget != null && lastTarget != newTarget)
                Main.Hover(newTarget);

            lastTarget = newTarget;
        }

        private ASelectable GetSelectable(int idx)
        {
            if (idx >= 0 && idx < selectables.Count)
                return selectables[targetIdx];
            else return null;
        }

        private void ClearSelectables()
        {
            for (int i = 0; i < selectables.Count; i++)
            {
                selectables[i].last = null;
                selectables[i].next = null;
            }
            selectables.Clear();
        }

        private void FillSelectables()
        {
            for (int i = 0; i < Main.selectableBank.Count; i++)
            {
                ASelectable selectable = Main.selectableBank[i];
                if (selectable != attachedSelectable && Main.Validate(selectable))
                {
                    Vector3 origin = transform.position;
                    Vector3 vector = selectable.transform.position - origin;
                    // Ignore things that aren't in range.
                    if (vector.magnitude <= range)
                    {
                        // Ignore things that are blocked.
                        if (!Physics.Raycast(origin, vector.normalized, vector.magnitude, obstacleMask))
                            selectables.Add(selectable);
                    }
                }
            }
        }

        private void SortSelectables()
        {
            distanceSort.Sort();
            for (int i = 0; i < selectables.Count; i++)
            {
                selectables[i].last = selectables[(i - 1) % selectables.Count];
                selectables[i].next = selectables[(i + 1) % selectables.Count];
            }
        }
    }
}
