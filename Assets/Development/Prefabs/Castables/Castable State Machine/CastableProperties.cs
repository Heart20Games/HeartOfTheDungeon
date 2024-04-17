using Body;
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
    public class CastableProperties : BaseMonoBehaviour
    {
        public CastableFields fields = new();

        // Properties
        public virtual Vector3 Direction { get => fields.direction; set => fields.direction = value; }
        public Character Owner { get => fields?.owner; set => fields.owner = value; }
        public CastableItem Item { get => fields?.item; set => fields.item = value; }
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
        public UnityEvent<int> onSetPowerLevel;
        public UnityEvent<int> onSetMaxPowerLevel;
        public UnityEvent<Identity> onSetIdentity;
        public UnityEvent onTrigger;
        public UnityEvent<Vector3> onCast;
        public UnityEvent onRelease;
        public UnityEvent onUnCast;
        [Foldout("Events")] public UnityEvent onCasted;

        private void Awake()
        {
            fields.damager = GetComponent<Damager>();
        }

        public virtual void Initialize(Character owner, CastableItem item)
        {
            Owner = owner;
            Item = item;

            if (fields.damager != null)
                fields.damager.Ignore(owner.body);
            Identity = owner.Identity;
            owner.artRenderer.DisplayWeapon(fields.weaponArt);
        }
    }

    [Serializable]
    public class CastableFields
    {
        [Header("Positioning")]
        public float rOffset;
        public Transform weaponArt;
        public Transform pivot;
        [ReadOnly] public Character owner;
        public Vector3 direction;
        public bool followBody;

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