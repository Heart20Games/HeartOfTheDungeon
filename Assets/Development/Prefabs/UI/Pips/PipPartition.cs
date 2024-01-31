using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIPips
{
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

        // Properties
        public GameObject GeneratorObject { get => generator.gameObject; }
        public Transform Transform { get => generator.transform; }
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
        public void Update()
        {
            if (lastTotal != total || lastFilled != filled || lastGrouping != UseGrouping)
            {
                SetFilled(filled);
            }
        }

        // Generation
        public void SetFilled(int filled)
        {
            this.filled = filled;
            SetTotal(total);
            onSetFilled.Invoke(this.filled);
            this.lastFilled = this.filled;
        }

        public void SetTotal(int total)
        {
            // Constrain
            this.total = total;
            this.filled = Mathf.Min(this.filled, this.total);

            // Clear then Add
            generator.ClearPips(pips);
            AddPips(filled);
            AddPips(total - filled);

            // Report
            lastTotal = total;
            onSetTotal.Invoke(total);
        }

        // Add Pips
        private void AddPip(AutoPip prefab, int amount = 1)
        {
            AutoPip pip = GameObject.Instantiate(prefab, generator.pipTarget == null ? Transform : generator.pipTarget);
            pip.gameObject.SetActive(true);
            pip.Amount = amount;
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

        // Hiding

        public void Hide()
        {
            foreach (var pip in pips)
            {
                pip.gameObject.SetActive(false);
            }
        }

        public void UnHide()
        {
            foreach (var pip in pips)
            {
                pip.gameObject.SetActive(true);
            }

            if (settings.autoHide && GeneratorObject.activeInHierarchy)
            {
                if (coroutine == null)
                    coroutine = generator.StartCoroutine(Deactivate(HideDelay));
                else
                    timeTillHide = HideDelay;
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
