using Body;
using Body.Behavior.ContextSteering;
using HotD.Body;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;
using static HotD.Castables.CastableToLocation;

namespace HotD.Castables
{
    public interface ICastableProperties
    {
        public void SetActive(bool active);
        public void Initialize(CastableFields field);
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
        public CastableFields fields = new();

        // Properties
        public virtual Vector3 Direction { get => fields.direction; set => fields.direction = value; }
        public ICastCompatible Owner { get => fields?.owner; set => fields.owner = value; }
        public CastCoordinator Coordinator { get => fields?.owner?.Coordinator; }
        public CastableItem Item { get => fields?.item; set => fields.item = value; }
        public Damager Damager { get => GetComponent<Damager>(); }
        public int PowerLevel
        {
            get => fields.curPowerLevel;
            set { fields.curPowerLevel = value; onSetPowerLevel.Invoke(value); }
        }
        public int MaxPowerLevel
        {
            get => fields.maxPowerLevel;
            set { fields.maxPowerLevel = value; onSetMaxPowerLevel.Invoke(value); }
        }
        public Identity Identity
        {
            get => fields.identity;
            set { fields.identity = value; onSetIdentity.Invoke(value); }
        }

        // Events
        [Foldout("Events", true)]
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        public bool unCastOnRelease = false;
        public UnityEvent<int> onSetPowerLevel = new();
        public UnityEvent<int> onSetMaxPowerLevel = new();
        public UnityEvent<Identity> onSetIdentity = new();
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
                onSetPowerLevel.AddListener((int powerLevel) => { Coordinator.SetInt("Level", powerLevel); });
                if (active)
                {
                    Print($"Set Action Index on Coordinator: {fields.actionIndex}");
                    Coordinator.SetActionIndex(fields.actionIndex);
                }
            }
        }

        public virtual void Initialize(CastableFields fields)
        {
            this.fields = fields;
            this.fields ??= new();
        }

        public virtual void Initialize(ICastCompatible owner, CastableItem item, int actionIndex = 0)
        {
            fields ??= new();
            onSetIdentity ??= new();

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
    public class CastableFields
    {
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
        public bool casting = false;
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        public bool unCastOnRelease = false;
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
    }
}