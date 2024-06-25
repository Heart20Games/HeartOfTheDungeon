using Attributes;
using Body;
using Body.Behavior.ContextSteering;
using HotD.Body;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;
using static HotD.Castables.CastableFields;
using static HotD.Castables.CastableToLocation;
using static UnityEngine.Rendering.DebugUI;

namespace HotD.Castables
{
    public interface ICastableProperties
    {
        public void SetActive(bool active);
        public void Initialize(CastableFieldsEditor field);
        public void Initialize(ICastCompatible owner, CastableItem item, int actionIndex = 0);

        public CastableItem Item { get; set; }
        public Damager Damager { get; }

        //public Vector3 Direction { get; set; }
        //public ICastCompatible Owner { get; }
        //public CastCoordinator Coordinator { get; }
        //public int PowerLevel { get; set; }
        //public int MaxPowerLevel { get; set; }
        //public Identity Identity { get; set; }
    }

    public class CastableProperties : BaseMonoBehaviour, ICastableProperties
    {
        public CastableFieldsEditor fields = new();
        [SerializeField] protected bool debug = false;

        // Properties
        public virtual Vector3 Direction { get => fields.direction; set => fields.direction = value; }
        public ICastCompatible Owner { get => fields?.owner; set => fields.owner = value; }
        public CastCoordinator Coordinator { get => fields?.owner?.Coordinator; }
        public CastableItem Item { get => fields?.item; set => fields.item = value; }
        public Damager Damager { get => GetComponent<Damager>(); }
        public int PowerLevel
        {
            get => fields.PowerLevel;
            set => fields.PowerLevel = value; //onSetPowerLevel.Invoke(value); }
        }
        public int MaxPowerLevel
        {
            get => fields.MaxPowerLevel;
            set => fields.MaxPowerLevel = value; //onSetMaxPowerLevel.Invoke(value); }
        }
        public int ComboStep
        {
            get => fields.ComboStep;
            set => fields.ComboStep = value; //onSetComboStep.Invoke(value); }
        }
        public int MaxComboStep
        {
            get => fields.MaxComboStep;
            set => fields.MaxComboStep = value; // onSetMaxComboStep.Invoke(value); }
        }
        public Identity Identity
        {
            get => fields.Identity;
            set { fields.Identity = value; }
        }

        // Setters
        public void SetPowerLevel(float value)
        {
            SetPowerLevel((int)value);
        }
        public void SetPowerLevel(int value)
        {
            Print($"Setting Power Level on {name}", debug, this);
            PowerLevel = value;
        }
        public void SetComboStep(int value)
        {
            ComboStep = value;
        }

        // Events
        [Foldout("Events", true)]
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        public bool unCastOnRelease = false;
        public bool connectToFieldEvents = false;
        public FieldEvents fieldEvents = new("Local Events");
        public UnityEvent onTrigger = new();
        public UnityEvent<Vector3> onCast = new();
        public UnityEvent onRelease = new();
        public UnityEvent onUnCast = new();
        [Foldout("Events")] public UnityEvent onCasted = new();

        private void Awake()
        {
            fields.damager = GetComponent<Damager>();
        }

        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (Coordinator)
            {
                fieldEvents.onSetPowerLevel.AddListener((int powerLevel) => { Coordinator.SetInt("Level", powerLevel); });
                if (active)
                {
                    Print($"Set Action Index on Coordinator: {fields.actionIndex}");
                    Coordinator.SetActionIndex(fields.actionIndex);
                }
            }
        }

        public virtual void Initialize(CastableFieldsEditor fields)
        {
            this.fields = fields;
            this.fields ??= new();
            if (connectToFieldEvents)
            {
                Print($"Connecting Field Events on {name}.", debug, this);
                this.fields.ConnectFieldEvents(fieldEvents);
            }
            this.fields.InitializeConnections();
        }

        public void InitializeEvents()
        {
            fieldEvents = new("Local Events");
            onCast ??= new();
            onTrigger ??= new();
            onRelease ??= new();
            onUnCast ??= new();
            onCasted ??= new();
            connectToFieldEvents = true;
        }

        public virtual void Initialize(ICastCompatible owner, CastableItem item, int actionIndex = 0)
        {
            fields ??= new();

            Owner = owner;
            Item = item;

            if (fields.damager != null)
                fields.damager.Ignore(owner.Body);

            Identity = owner.Identity;
            owner?.WeaponDisplay?.DisplayWeapon(fields.weaponArt);
            fields.actionIndex = actionIndex;
        }
    }

    [Serializable]
    public class CastableFieldsEditor : CastableFields
    {
        // Properties
        public int PowerLevel
        {
            get => curPowerLevel;
            set { curPowerLevel = value; events.onSetPowerLevel.Invoke(value); }
        }
        public int MaxPowerLevel
        {
            get => maxPowerLevel;
            set { maxPowerLevel = value; events.onSetMaxPowerLevel.Invoke(value); }
        }
        public int ComboStep
        {
            get { return usePowerLevelAsComboStep ? PowerLevel : curComboStep; }
            set 
            {
                if (usePowerLevelAsComboStep)
                {
                    PowerLevel = value;
                }
                else
                {
                    curComboStep = value; events.onSetComboStep.Invoke(value);
                }
            }
        }
        public int MaxComboStep
        {
            get { return usePowerLevelAsComboStep ? MaxPowerLevel : curComboStep; }
            set
            { 
                if (usePowerLevelAsComboStep)
                {
                    MaxPowerLevel = value;
                }
                else
                {
                    maxComboStep = value; 
                    events.onSetMaxComboStep.Invoke(value); 
                }
            }
        }
        public Identity Identity
        {
            get => identity;
            set { identity = value; }
        }

        public CastableStats Stats
        {
            get => stats.stats;
            set { SetStats(value); }
        }

        public void SetMaxPowerLevel(int level) { MaxPowerLevel = level; }

        public void SetStats(CastableStats stats)
        {
            // Disconnect
            if (this.stats.stats != null)
            {
                this.stats.stats.chargeLimit.updatedFinalInt.RemoveListener(SetMaxPowerLevel);
            }
            this.stats = new(stats);
        }

        public void InitializeConnections()
        {
            if (this.stats.stats != null)
            {
                this.stats.stats.chargeLimit.updatedFinalInt.AddListener(SetMaxPowerLevel);
            }
        }

        public void ConnectFieldEvents(FieldEvents events)
        {
            Debug.Log($"Connecting Field Events {events.name} -> {this.events.name}");
            
            // Power Level
            this.events.onSetPowerLevel.AddListener(events.onSetPowerLevel.Invoke);
            this.events.onSetMaxPowerLevel.AddListener(events.onSetMaxPowerLevel.Invoke);

            // Combo Steps
            this.events.onSetComboStep.AddListener(events.onSetComboStep.Invoke);
            this.events.onSetMaxComboStep.AddListener(events.onSetMaxComboStep.Invoke);
            if (usePowerLevelAsComboStep)
            {
                this.events.onSetPowerLevel.AddListener(events.onSetComboStep.Invoke);
                this.events.onSetMaxPowerLevel.AddListener(events.onSetMaxComboStep.Invoke);
            }
            
            // Identity
            this.events.onSetIdentity.AddListener(events.onSetIdentity.Invoke);
        }
    }

    [Serializable]
    public class CastableFields
    {
        // Events
        [SerializeField] protected bool debug = false;
        [HideInInspector] public FieldEvents events = new("Events");
        [HideInInspector] public FieldStats stats;

        [Header("Configuration")]
        public float rOffset;
        public Transform weaponArt;
        public Transform pivot;
        public Vector3 direction;
        public bool followBody;
        public ICastCompatible owner;
        public int actionIndex;

        [Header("Settings")]
        public CastableItem item;
        public int maxPowerLevel;
        public int curPowerLevel;
        public int maxComboStep;
        public int curComboStep;
        //public bool casting = false;
        //public bool castOnTrigger = true;
        //public bool castOnRelease = false;
        //public bool unCastOnRelease = false;
        public bool usePowerLevelAsComboStep = false;
        public List<GameObject> castingMethods = new();

        [Header("Positionables")]
        public List<ToLocation<Positionable>> toLocations = new();
        public List<Transform> positionables;
        public List<CastedVFX> effects = new();

        [Header("Damage")]
        public Identity identity = Identity.Neutral;
        public Damager damager;

        [Header("Status Effects")]
        public List<Status> triggerStatuses;
        public List<Status> castStatuses;
        public List<Status> hitStatuses;

        [Header("Diagnostics")]
        [ReadOnly] public Vector3 pivotDirection;

        [Serializable]
        public struct FieldEvents
        {
            public string name;
            public UnityEvent<int> onSetPowerLevel;
            public UnityEvent<int> onSetMaxPowerLevel;
            public UnityEvent<int> onSetComboStep;
            public UnityEvent<int> onSetMaxComboStep;
            public UnityEvent<Identity> onSetIdentity;

            public FieldEvents(string name)
            {
                this.name = name;
                onSetPowerLevel = new();
                onSetMaxPowerLevel = new();
                onSetComboStep = new();
                onSetMaxComboStep = new();
                onSetIdentity = new();
            }
        }

        [Serializable]
        public struct FieldStats
        {
            public string name;
            public CastableStats stats;

            public FieldStats(CastableStats stats)
            {
                this.name = stats == null ? "[Unknown]" : stats.name;
                this.stats = stats;
            }
        }
    }
}