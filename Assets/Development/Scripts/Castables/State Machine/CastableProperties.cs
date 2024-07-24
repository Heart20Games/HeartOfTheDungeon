using MyBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;
using static HotD.Castables.CastableFields;
using static HotD.Castables.CastableToLocation;
using static UnityEngine.Rendering.DebugUI;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace HotD.Castables
{
    public interface ICastableProperties
    {
        public void SetActive(bool active);
        public void Initialize(CastableFieldsEditor field);
        public void Initialize(ICastCompatible owner, CastableItem item);

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
        [SerializeField] protected bool debugProperties = false;

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
            Print($"Setting Power Level on {name}", debugProperties, this);
            PowerLevel = value;
        }
        public void ResetPowerLevel()
        {
            PowerLevel = 0;
        }
        public void SetComboStep(int value)
        {
            ComboStep = value;
        }
        public void IncrementComboStep()
        {
            IncrementComboStep(1);
        }
        public void IncrementComboStep(int step = 1)
        {
            ComboStep += step;
        }
        public void ResetComboStep()
        {
            ComboStep = 0;
        }

        // Events
        [Foldout("Events", true)]
        public bool connectToFieldEvents = false;
        public FieldEvents fieldEvents = new("Local Events");
        [Foldout("Events")] public CastEvents castEvents = new("Cast Events");
        

        private void Awake()
        {
            fields.damager = GetComponent<Damager>();
        }

        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (Coordinator)
            {
                fieldEvents.onSetPowerLevel.AddListener(Coordinator.SetPowerLevel);
                fieldEvents.onSetComboStep.AddListener(Coordinator.SetComboLevel);
                if (active)
                {
                    Print($"Set Action Index on Coordinator: {fields.ActionType}", debugProperties, this);
                    Coordinator.ActionIndex = fields.ActionType;
                }
            }
        }

        public virtual void Initialize(CastableFieldsEditor fields)
        {
            this.fields = fields;
            this.fields ??= new();
            if (connectToFieldEvents)
            {
                Print($"Connecting Field Events on {name}.", debugProperties, this);
                this.fields.ConnectFieldEvents(fieldEvents);
            }
            this.fields.InitializeConnections();
        }

        public virtual void InitializeEvents()
        {
            fieldEvents = new("Local Events");
            castEvents = new("Cast Events");
            connectToFieldEvents = true;
        }

        public virtual void Initialize(ICastCompatible owner, CastableItem item)
        {
            fields ??= new();

            Owner = owner;
            Item = item;

            if (fields.damager != null)
                fields.damager.Ignore(owner.Body);

            Identity = owner.Identity;
            if (owner.Body != null)
            {
                fields.CollisionExceptions = owner.Body.GetComponents<Collider>();
            }
            owner?.WeaponDisplay?.DisplayWeapon(fields.weaponArt);
        }
    }

    [Serializable]
    public struct CastEvents
    {
        public string name;
        public UnityEvent onTrigger;
        public UnityEvent<Vector3> onStartCast;
        public UnityEvent onRelease;
        public UnityEvent onEndCast;
        public UnityEvent onCasted;

        public CastEvents(string name)
        {
            this.name = name;
            onTrigger = new();
            onRelease = new();
            onStartCast = new();
            onEndCast = new();
            onCasted = new();
        }
    }

    [Serializable]
    public class CastableFieldsEditor : CastableFields
    {
        [SerializeField] private bool debugFieldsEditor = false;

        // Properties
        public int PowerLevel
        {
            get => curPowerLevel;
            set
            { 
                curPowerLevel = value; 
                events.onSetPowerLevel.Invoke(value);
                Print($"Power Level set: {value}", debugFieldsEditor);
            }
        }
        public int MaxPowerLevel
        {
            get => maxPowerLevel;
            set 
            { 
                maxPowerLevel = value; 
                events.onSetMaxPowerLevel.Invoke(value);
                Print($"Max Power Level set: {value}", debugFieldsEditor);
            }
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
                    Print($"Combo Step set: {value}", debugFieldsEditor);
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
                    Print($"Max Combo Step set: {value}", debugFieldsEditor);
                }
            }
        }
        public float Cooldown
        {
            get => stats.stats.Cooldown;
            set
            {
                events.onSetCooldown.Invoke(value);
                Print($"Cooldown set: {value}", debugFieldsEditor);
            }
        }
        public Identity Identity
        {
            get => identity;
            set { identity = value; }
        }
        public void SetMaxPowerLevel(int level) { MaxPowerLevel = level; }
        public void SetCooldown(float length) { Cooldown = length; }
        
        // Collisions
        public Collider[] CollisionExceptions
        {
            get => collisionExceptions;
            set
            {
                collisionExceptions = value;
                collisionExceptions ??= new Collider[0];
                events.onSetCollisionExceptions.Invoke(collisionExceptions);
                Print($"Collision Exceptions set: {collisionExceptions.Length}", debugFieldsEditor);
            }
        }
        public void SetCollisionExceptions(Collider[] exceptions) { CollisionExceptions = exceptions; }

        // Stats
        public CastableStats Stats
        {
            get => stats.stats;
            set { SetStats(value); }
        }
        public void SetStats(CastableStats stats)
        {
            // Disconnect
            if (this.stats.stats != null)
            {
                this.stats.stats.chargeLimit.updatedFinalInt.RemoveListener(SetMaxPowerLevel);
                this.stats.stats.cooldown.updatedFinalFloat.RemoveListener(SetCooldown);
            }
            this.stats = new(stats);
        }

        public void InitializeConnections()
        {
            if (this.stats.stats != null)
            {
                this.stats.stats.chargeLimit.updatedFinalInt.AddListener(SetMaxPowerLevel);
                this.stats.stats.cooldown.updatedFinalFloat.AddListener(SetCooldown);
            }
        }

        public void ConnectFieldEvents(FieldEvents events)
        {
            Print($"Connecting Field Events {events.name} -> {this.events.name}", debugFieldsEditor);
            
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

            // Cooldown
            this.events.onSetCooldown.AddListener(events.onSetCooldown.Invoke);

            // Collision Exceptions
            this.events.onSetCollisionExceptions.AddListener(events.onSetCollisionExceptions.Invoke);
        }
    }

    [Serializable]
    public class CastableFields
    {
        // Events
        [SerializeField] protected bool debug = false;
        [HideInInspector] public FieldEvents events = new("Events");
        [HideInInspector] public FieldStats stats;

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        protected void Print(object message, bool debug = true)
        {
            if (debug) Debug.Log(message);
        }

        [Header("Configuration")]
        public float rOffset;
        public Transform weaponArt;
        public Transform pivot;
        public Vector3 direction;
        public bool followBody;
        public ICastCompatible owner;
        public ActionType ActionType
        {
            get { return item != null ? item.actionType : ActionType.Passive; }
        }

        [Header("Settings")]
        public CastableItem item;
        public int maxPowerLevel;
        public int curPowerLevel;
        public int maxComboStep;
        public int curComboStep;
        public bool usePowerLevelAsComboStep = false;
        public List<GameObject> castingMethods = new();

        [Header("Positionables")]
        public List<ToLocation<Positionable>> toLocations = new();
        public List<Transform> positionables;
        public List<CastedVFX> effects = new();

        [Header("Collisions")]
        public Collider[] collisionExceptions;

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
            public UnityEvent<float> onSetCooldown;
            public UnityEvent<Collider[]> onSetCollisionExceptions;

            public FieldEvents(string name)
            {
                this.name = name;
                onSetPowerLevel = new();
                onSetMaxPowerLevel = new();
                onSetComboStep = new();
                onSetMaxComboStep = new();
                onSetIdentity = new();
                onSetCooldown = new();
                onSetCollisionExceptions = new();
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