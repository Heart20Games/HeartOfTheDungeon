using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIPips
{
    using static PipGenerator;

    [Serializable]
    public class PipPartition
    {
        public string name;

        // Parts
        [ReadOnly] [SerializeField] private PipPartitionSettings settings;
        public PipGenerator generator;
        public List<Pip> pips = new();

        // Pips
        [Space]
        [SerializeField] private AutoPip prefab;
        [SerializeField] private int total = 5;
        [SerializeField] private int filled = 5;

        // Auto-Hide
        [Space]
        [ReadOnly][SerializeField] private float timeTillHide;
        private Coroutine coroutine;

        [Foldout("Events")] public UnityEvent<int> onSetTotal = new();
        [Foldout("Events")] public UnityEvent<int> onSetFilled = new();
        [Foldout("Events")] public UnityEvent<int> onChanged = new();

        // Old Values
        [Header("Old Values")]
        [ReadOnly] [SerializeField] private int lastTotal;
        [ReadOnly] [SerializeField] private int lastFilled;
        [ReadOnly][SerializeField] private bool lastGrouping;
        [ReadOnly][SerializeField] private int lastGroupCapacity;
        [ReadOnly][SerializeField] private int lastGroupThreshold;

        private int childOffset;

        // Properties
        public GameObject GeneratorObject { get => generator.gameObject; }
        public Transform Transform { get => generator.transform; }
        public int Count { get => pips.Count; }
        public PipType Type { get => settings.type; }
        public bool AutoHide { get => settings.autoHide; }
        public float HideDelay { get => settings.hideWaitTime; }
        public bool UseGrouping { get => settings.useGrouping; }
        public int GroupThreshold { get => settings.groupThreshold; }
        public int GroupCapacity { get => settings.groupCapacity; }

        // Initialization
        public PipPartition(PipPartitionSettings settings, PipGenerator generator)
        {
            this.name = settings.name;
            this.settings = settings;
            this.generator = generator;
            prefab = GeneratePip();
        }

        public AutoPip GeneratePip()
        {
            AutoPip pip = GameObject.Instantiate(generator.basePrefab, generator.transform);
            pip.Initialize(settings, generator.usedInWorldSpace);
            pip.gameObject.SetActive(false);
            return pip;
        }

        // MonoBehaviour
        public void Update(int childOffset = 0)
        {
            if (lastTotal != total || lastGrouping != UseGrouping)
            {
                SetTotal(total, childOffset);
                lastGrouping = UseGrouping;
            }
            if (lastFilled != filled)
            {
                SetFilled(filled);
            }
        }

        // Generation
        public void SetFilled(int filled)
        {
            this.filled = Mathf.Clamp(filled, 0, total);
            ChangePips(this.filled);
            onSetFilled.Invoke(this.filled);
            this.lastFilled = this.filled;
            UnHide();
        }

        public void SetTotal(int total, int childOffset = 0)
        {
            this.childOffset = childOffset;

            // Constrain
            this.total = total;
            this.filled = Mathf.Min(this.filled, this.total);

            // Clear then Add
            generator.ClearPips(pips);
            if (UseGrouping)
            {
                AddPipsGrouped(filled, GroupThreshold, GroupCapacity);
                AddPipsGrouped(total-filled, GroupThreshold, GroupCapacity, true);
            }
            else
            {
                AddPips(filled);
                AddPips(total - filled);
            }

            // Report
            this.lastTotal = total;
            onSetTotal.Invoke(total);
            UnHide();
        }

        // Add Pips
        private void AddPip(AutoPip prefab, int amount = 1)
        {
            AutoPip pip = GameObject.Instantiate(prefab, generator.pipTarget == null ? Transform : generator.pipTarget);
            pip.gameObject.SetActive(true);
            pip.Amount = amount;
            pip.transform.SetSiblingIndex(childOffset + pips.Count);
            pips.Add(pip);
        }

        private void AddPips(int number)
        {
            for (int i = 0; i < number; i++)
            {
                AddPip(prefab);
            }
        }

        private void AddPipsLabeled(int number, int amount)
        {
            for (int i = 0; i < number; i++)
            {
                AddPip(prefab, amount);
            }
        }

        private void AddPipsGrouped(int number, int threshold, int capacity, bool inverted = false)
        {
            int fullGroups = Mathf.Max(0, (number - threshold)) / capacity;
            int remainder = Mathf.Min(number, (number % capacity) + threshold);
            if (inverted)
            {
                AddPips(Mathf.Min(remainder, threshold));
                if (remainder > threshold)
                    AddPip(prefab, remainder - threshold);
                AddPipsLabeled(fullGroups, capacity);
            }
            else
            {
                AddPipsLabeled(fullGroups, capacity);
                if (remainder > threshold)
                    AddPip(prefab, remainder - threshold);
                AddPips(Mathf.Min(remainder, threshold));
            }
        }

        // Change Pips

        private void ChangePip(Pip pip, bool filled, int amount = 1)
        {
            pip.Filled = filled;
            pip.Amount = amount;
        }

        private void ChangePips(int first, int last, bool filled)
        {
            for (int i = first; i < Mathf.Min(last, pips.Count); i++)
            {
                ChangePip(pips[i], filled, pips[i].Amount);
            }
        }

        private void ChangePips(int filled)
        {
            ChangePips(0, filled, true);
            ChangePips(filled, pips.Count, false);
        }

        // Hiding

        public void Hide()
        {
            // Hide all the Pips
            foreach (var pip in pips)
                pip.gameObject.SetActive(false);
        }

        public void UnHide()
        {
            if (generator.displayMode != DisplayMode.Off)
            {
                // UnHide all the Pips
                foreach (var pip in pips)
                    pip.gameObject.SetActive(true);
                
                // Handle the Hide Delay
                if (generator.displayMode != DisplayMode.On)
                    StartHideDelay();
                else 
                    StopHideDelay();
            }
            else Hide();
        }

        public void StartHideDelay()
        {
            if (settings.autoHide && GeneratorObject.activeInHierarchy)
            {
                if (coroutine == null)
                    coroutine = generator.StartCoroutine(Deactivate(HideDelay));
                else
                    timeTillHide = HideDelay;
            }
        }

        public void StopHideDelay()
        {
            if (coroutine != null)
            {
                generator.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public IEnumerator Deactivate(float waitTime)
        {
            timeTillHide = waitTime;
            while (timeTillHide > 0)
            {
                float timeToWait = Mathf.Min(timeTillHide, 1f);
                yield return new WaitForSeconds(timeToWait);
                timeTillHide -= timeToWait;
            }
            Hide();
            coroutine = null;
        }
    }
}
