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

    public interface ICasted : IPositionable
    {
        // Triggers / Events
        public void Release();
        public void OnTrigger();
        public void OnRelease();
        public void OnStartCast(Vector3 vector);
        public void OnEndCast();

        // Initialization
        public void InitializeUnityEvents();
        public bool Equipped { get; set; }
        
        // Stats
        public CastableStats Stats { get; set; }
        public Vector2 PowerRange { get; set; }
        public DependentAttribute PowerLimit { get; set; }
        public Vector2 ComboRange { get; set; }
        public void SetPowerLevel(float powerLevel);
    }

    public class Casted : Positionable, ICasted
    {
        protected CastableStats stats;
        [SerializeField] protected bool debug;
        [SerializeField] protected bool equipped = false;
        [SerializeField] protected bool forceUpdate = false;
        private readonly bool castOnEnable = true;
        public CastableStats Stats { get => stats; set => stats = value; }
        public bool Equipped { get => equipped; set => equipped = value; }


        [Foldout("Base Events", true)]
        [SerializeField] protected UnityEvent onStart = new();
        [SerializeField] protected UnityEvent onEnable = new();
        [SerializeField] protected UnityEvent onDisable = new();

        [Foldout("Cast Events", true)]
        [SerializeField] protected UnityEvent onTrigger = new();
        [SerializeField] protected UnityEvent onRelease = new();
        public UnityEvent<Vector3> onCast = new();
        [SerializeField] protected UnityEvent onEndCast = new();
        public void Release() { onRelease.Invoke(); }
        private void StartCast(Vector3 vector) { onCast.Invoke(vector); }

        [Foldout("Power Level", true)]
        [ReadOnly][SerializeField] protected Vector2 powerRange;
        [SerializeField] protected UnityEvent<bool> onHasCharge = new();
        [SerializeField] protected UnityEvent<float> onSetPowerLevel = new();
        [SerializeField] protected UnityEvent<int> onSetPowerLimit = new();
        [SerializeField] protected DependentAttribute powerLimit;
        [ReadOnly][SerializeField] protected float powerLevel = 0f;
        [ReadOnly][SerializeField] protected float actualLevel = 0f;
        [ReadOnly][SerializeField] protected float clampedPower = 0f;
        public Vector2 PowerRange { get => powerRange; set => powerRange = value; }
        public DependentAttribute PowerLimit { get => powerLimit; set => powerLimit = value; }

        [Foldout("Combo Step", true)]
        [SerializeField] protected Vector2 comboRange;
        [SerializeField] protected UnityEvent<int> onSetComboStep = new();
        [ReadOnly][SerializeField] protected int comboStep = 0;
        [ReadOnly][SerializeField] protected float clampedCombo = 0f;
        public Vector2 ComboRange { get => comboRange; set => comboRange = value; }

        [Foldout("Collision")]
        [SerializeField] protected UnityEvent<Collider[]> onSetExceptions = new();


        [Serializable] private struct CastStatEvent
        {
            public string name;
            public CastableStat stat;
            public float modifier;
            public float multiplier;
            public UnityEvent<float> finalValue;
        }
        [Header("Reporting")]
        [SerializeField] private List<CastStatEvent> castStatEvents = new();

        protected virtual void Awake()
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
            if (castOnEnable)
            {
                StartCast(Vector3.forward); // TODO: Need to know what the correct vector is.
            }
        }

        private void OnDisable()
        {
            onDisable.Invoke();
        }

        public void InitializeUnityEvents()
        {
            onTrigger ??= new();
            onRelease ??= new();
            onCast ??= new();
            onEndCast ??= new();
            onSetPowerLevel ??= new();
            onSetPowerLimit ??= new();
        }

        private void ReportStats()
        {
            foreach (var e in castStatEvents)
            {
                e.finalValue.Invoke((stats.GetAttribute(e.stat).FinalValue + e.modifier) * e.multiplier);
            }
        }

        protected void AddListenerController(bool active = false, UnityAction<int> powerLimitListener = null, UnityAction triggerListener = null, UnityAction releaseListener = null, UnityAction<Vector3> startCastListener = null, UnityAction endCastListener = null)
        {
            gameObject.SetActive(active);
            onSetPowerLimit.AddListener(powerLimitListener);
            onTrigger.AddListener(triggerListener);
            onRelease.AddListener(releaseListener);
            onCast.AddListener(startCastListener);
            onEndCast.AddListener(endCastListener);
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
        private void HasCharge(bool hasCharge)
        {
            onHasCharge.Invoke(hasCharge);
        }
        private void OnSetPowerLimit(float powerLimit) { OnSetPowerLimit((int) powerLimit); }
        private void OnSetPowerLimit(int powerLimit)
        {
            onSetPowerLimit.Invoke(powerLimit);
            UpdateEnabled();
        }

        // Combo Step
        private void SetComboStep(int step)
        {
            this.comboStep = step;
            onSetComboStep.Invoke(step);
            UpdateEnabled();
        }

        // Enable / Disable
        private void UpdateEnabled()
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
            onTrigger.Invoke();
        }
        public virtual void OnRelease()
        {
            HasCharge(false);
            onRelease.Invoke();
        }
        public virtual void OnStartCast(Vector3 vector)
        {
            if (isActiveAndEnabled)
            {
                if (debug) { Debug.Log($"{name} casting using {vector} vector."); }
                onCast.Invoke(vector);
            }
        }
        public virtual void OnEndCast()
        {
            if (isActiveAndEnabled)
            {
                onEndCast.Invoke();
            }
        }
    }
}