using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using static CastableStats;
using System;
using MyBox;
using Attributes;

namespace HotD.Castables
{
    public struct CastedMechanic
    {
        public bool disableMovementWhileCasting;
    }

    public class Casted : Positionable, ICollidables
    {
        public CastableStats stats;
        [SerializeField] private bool debug;

        protected bool casting = false;

        [Foldout("Base Events", true)]
        public UnityEvent onStart = new();
        public UnityEvent onEnable = new();
        public UnityEvent onDisable = new();

        [Foldout("Cast Events", true)]
        public UnityEvent onTrigger = new();
        public UnityEvent onRelease = new();
        public UnityEvent<Vector3> onCast = new();
        public UnityEvent onEndCast = new();
        public void Trigger() { onTrigger.Invoke(); }
        public void Release() { onRelease.Invoke(); }
        public void Cast(Vector3 vector) { casting = true; onCast.Invoke(vector); }
        public void EndCast() { casting = false; onEndCast.Invoke(); }

        [Foldout("Power Level", true)]
        public bool canUpdatePowerLevel = false;
        public Vector2 powerRange;
        public UnityEvent<bool> onHasCharge = new();
        public UnityEvent<float> onSetPowerLevel = new();
        public UnityEvent<int> onSetPowerLimit = new();
        public DependentAttribute powerLimit;
        [ReadOnly][SerializeField] private float finalPowerLimit = 0f;
        [ReadOnly][SerializeField] private float powerLevel = 0f;
        [ReadOnly][SerializeField] private float actualLevel = 0f;
        [ReadOnly][SerializeField] private float clampedPower = 0f;

        [Foldout("Combo Step", true)]
        public Vector2 comboRange;
        [Foldout("Combo Step")] public UnityEvent<int> onSetComboStep = new();
        [ReadOnly][SerializeField] private int comboStep = 0;
        [ReadOnly][SerializeField] private float clampedCombo = 0f;

        [Foldout("Collision")] public UnityEvent<Collider[]> onSetExceptions = new();

        public bool equipped = false;
        public bool forceUpdate = false;

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
                OnSetPowerLimit(powerLimit.FinalValue);
                stats.chargeLimit.updatedFinalFloat.AddListener(OnSetPowerLimit);
            }
        }

        private void Start()
        {
            ReportStats();
            onStart.Invoke();
        }

        private void Update()
        {
            if (forceUpdate)
            {
                forceUpdate = false;
                OnSetPowerLimit(powerLimit.FinalValue);
            }
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
            if (debug) print($"Set Power Level {powerLevel}");
            this.powerLevel = powerLevel;
            HasCharge(powerLevel > 0);
            onSetPowerLevel.Invoke(powerLevel);
            UpdateEnabled();
        }
        public void HasCharge(bool hasCharge)
        {
            onHasCharge.Invoke(hasCharge);
        }
        public void OnSetPowerLimit(float powerLimit) { OnSetPowerLimit((int) powerLimit); }
        public void OnSetPowerLimit(int powerLimit)
        {
            finalPowerLimit = powerLimit;
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
            actualLevel = powerLevel;
            clampedPower = Mathf.Clamp(actualLevel, powerRange.x, powerRange.y);
            bool correctPower = powerRange.y <= 0 || actualLevel == clampedPower;
            gameObject.SetActive(correctCombo && correctPower);
            enabled = correctCombo && correctPower;
        }

        // Cast Events
        public virtual void OnTrigger()
        {
            canUpdatePowerLevel = true;
            onTrigger.Invoke();
        }
        public virtual void OnRelease()
        {
            canUpdatePowerLevel = false;
            HasCharge(false);
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
                onEndCast.Invoke();
            }
        }
    }
}