using Body.Behavior;
using CustomUnityEvents;
using Sorting;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Selection
{
    [RequireComponent(typeof(Selectable))]
    public class TargetFinder : Validator
    {
        public Targeter Main { get => Targeter.main; }

        [Header("Settings")]
        public float range = 10f;
        public Vector3 offset = Vector3.up;
        public LayerMask obstacleMask;
        public bool requireClearPath = false;

        [Header("Selectables")]
        [ReadOnly] public List<ASelectable> selectables = new();
        [ReadOnly][SerializeField] private ASelectable attachedSelectable;
        ObjectDistanceSort<ASelectable> distanceSort;
        [ReadOnly][SerializeField] private int targetIdx = 0;
        [ReadOnly][SerializeField] private int numValid = 0;
        [ReadOnly][SerializeField] private int numInRange = 0;
        public int TargetIdx { get => targetIdx; set => SetTargetIdx(value); }
        [ReadOnly][SerializeField] private ASelectable lastTarget;
        public UnityEvents<Transform> onSetTarget;
        public UnityEvent<bool> onSetLockOn;

        private void Awake()
        {
            enabled = false;
            attachedSelectable = GetComponent<ASelectable>();
            distanceSort = new(selectables, Camera.main.transform);
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

        public void SetLockOn(bool lockOn)
        {
            onSetLockOn.Invoke(lockOn);
        }

        private void SetTargetIdx(int newIdx)
        {
            targetIdx = selectables.Count > 0 ? Mod(newIdx, selectables.Count) : newIdx;

            // Unhover the old, Hover the new
            ASelectable newTarget = GetSelectable(targetIdx);
            if (lastTarget != null && (targetIdx < 0 || lastTarget != newTarget))
                Main.UnHover(lastTarget);
            if (newTarget != null && lastTarget != newTarget)
                Main.Hover(newTarget);

            if (lastTarget != newTarget)
            {
                if (debug) print($"Changed targets: {lastTarget} -> {newTarget}");
            }

            Transform targetTransform = newTarget == null ? null : newTarget.transform;
            onSetTarget.Invoke(targetTransform);
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
            numValid = 0;
            numInRange = 0;
            selectables.Clear();
        }

        private void FillSelectables()
        {
            for (int i = 0; i < Main.selectableBank.Count; i++)
            {
                ASelectable selectable = Main.selectableBank[i];
                if (selectable != attachedSelectable && Main.Validate(selectable))
                {
                    numValid += 1;
                    Vector3 origin = transform.position + offset;
                    Vector3 vector = (selectable.transform.position + selectable.offset) - origin;
                    // Ignore things that aren't in range.
                    if (vector.magnitude <= range)
                    {
                        numInRange += 1;
                        if (requireClearPath)
                        {
                            // Ignore things that are blocked.
                            if (Physics.Raycast(origin, vector, out var hit, obstacleMask))
                                if (hit.collider.gameObject == selectable.gameObject)
                                    AddSelectable(selectable);
                            Debug.DrawRay(origin, vector.normalized * hit.distance, Color.green);
                            Debug.DrawRay(origin + (vector.normalized * hit.distance), vector.normalized * (vector.magnitude - hit.distance), Color.red);
                        }
                        else
                        {
                            AddSelectable(selectable);
                        }
                    }
                }
            }
        }

        private void AddSelectable(ASelectable selectable)
        {
            if (Validate(selectable.gameObject, attachedSelectable.Identity))
            {
                if (debug) print("Found a valid selectable!");
                selectables.Add(selectable);
            }
        }

        private void SortSelectables()
        {
            distanceSort.Sort();
            for (int i = 0; i < selectables.Count; i++)
            {
                selectables[i].last = selectables[Mod(i - 1, selectables.Count)];
                selectables[i].next = selectables[Mod(i + 1, selectables.Count)];
            }
        }

        private int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
