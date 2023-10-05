using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using static CastableStats;
using System;
using MyBox;

namespace HotD.Castables
{
    public class Casted : Positionable, ICollidables
    {
        public CastableStats stats;
        [SerializeField] private bool debug;

        [Foldout("Base Events", true)]
        public UnityEvent onStart = new();
        public UnityEvent onEnable = new();
        public UnityEvent onDisable = new();

        [Foldout("Cast Events", true)]
        public UnityEvent onTrigger = new();
        public UnityEvent onRelease = new();
        public UnityEvent<Vector3> onCast = new();
        public UnityEvent onUnCast = new();
        public void Trigger() { onTrigger.Invoke(); }
        public void Release() { onRelease.Invoke(); }
        public void Cast(Vector3 vector) { onCast.Invoke(vector); }
        public void UnCast() { onUnCast.Invoke(); }

        [Foldout("Power Level", true)]
        public bool canUpdatePowerLevel = false;
        public Vector2 powerRange;
        public UnityEvent<bool> onHasCharge = new();
        public UnityEvent<float> onSetPowerLevel = new();
        public UnityEvent<int> onSetPowerLimit = new();
        [ReadOnly][SerializeField] private float powerLevel = 0f;
        [ReadOnly][SerializeField] private int powerLimit = 0;
        [ReadOnly][SerializeField] private float actualLevel = 0f;
        [ReadOnly][SerializeField] private float clampedPower = 0f;

        [Foldout("Combo Step", true)]
        public Vector2 comboRange;
        [Foldout("Combo Step")] public UnityEvent<int> onSetComboStep = new();
        [ReadOnly][SerializeField] private int comboStep = 0;
        [ReadOnly][SerializeField] private float clampedCombo = 0f;

        [Foldout("Collision", true)] public UnityEvent<Collider[]> onSetExceptions = new();

        [Serializable] public struct CastStatEvent
        {
            public string name;
            public CastableStat stat;
            public float modifier;
            public float multiplier;
            public UnityEvent<float> finalValue;
        }
        public List<CastStatEvent> castStatEvents = new();

        private void Awake()
        {
            if (stats != null)
            {
                stats.chargeLimit.updatedFinal.AddListener(SetPowerLimit);
                SetPowerLimit(stats.ChargeLimit);
            }
        }

        private void Start()
        {
            ReportStats();
            onStart.Invoke();
        }
        private void OnEnable()
        {
            ReportStats();
            onEnable.Invoke();
        }
        private void OnDisable()
        {
            onDisable.Invoke();
        }

        public void ReportStats()
        {
            foreach (var e in castStatEvents)
            {
                e.finalValue.Invoke((stats.GetAttribute(e.stat).FinalValue + e.modifier) * e.multiplier);
            }
        }

        public void SetExceptions(Collider[] exceptions)
        {
            onSetExceptions.Invoke(exceptions);
        }

        // Power Level
        public void SetPowerLevel(float powerLevel)
        {
            if (canUpdatePowerLevel)
            {
                this.powerLevel = powerLevel;
                onHasCharge.Invoke(powerLevel > 0);
                onSetPowerLevel.Invoke(powerLevel);
                UpdateEnabled();
            }
        }
        public void SetPowerLimit(float powerLimit)
        {
            SetPowerLimit(Mathf.RoundToInt(powerLimit));
        }
        public void SetPowerLimit(int powerLimit)
        {
            this.powerLimit = powerLimit;
            onSetPowerLimit.Invoke(powerLimit);
            UpdateEnabled();
        }

        // Combo Step
        public void SetComboStep(int step)
        {
            this.comboStep = step;
            onSetComboStep.Invoke(step);
            UpdateEnabled();
        }

        // Enable / Disable
        public void UpdateEnabled()
        {
            clampedCombo = Mathf.Clamp(comboStep, comboRange.x, comboRange.y);
            bool correctCombo = comboRange.y <= 0 || comboStep == clampedCombo;
            actualLevel = powerLimit * Mathf.Clamp01(powerLevel);
            clampedPower = Mathf.Clamp(actualLevel, powerRange.x, powerRange.y);
            bool correctPower = powerRange.y <= 0 || actualLevel == clampedPower;
            gameObject.SetActive(correctCombo && correctPower);
            enabled = correctCombo && correctPower;
        }

        // Cast Events
        public virtual void OnTrigger()
        {
            canUpdatePowerLevel = true;
            SetPowerLevel(0);
            onTrigger.Invoke();
        }
        public virtual void OnRelease()
        {
            SetPowerLevel(0);
            canUpdatePowerLevel = false;
            onRelease.Invoke();
        }
        public virtual void OnCast(Vector3 vector)
        {
            if (isActiveAndEnabled)
            {
                if (debug) { Debug.Log($"{name} casting using {vector} vector."); }
                onCast.Invoke(vector);
            }
                
        }
        public virtual void OnUnCast()
        {
            if (isActiveAndEnabled)
            {
                onUnCast.Invoke();
            }
        }
    }
}