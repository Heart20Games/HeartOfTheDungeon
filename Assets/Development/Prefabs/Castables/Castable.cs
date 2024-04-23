using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    using Body;
    using MyBox;
    using System;
    using static global::Body.Behavior.ContextSteering.CSIdentity;
    using static HotD.Castables.CastableToLocation;

    public class Castable : CastableProperties, ICastable
    {
        // Positioning
        //[Foldout("Positioning and Following", true)]
        //[Header("Positioning and Following")]
        //public CastableItem item;
        //public CastableItem GetItem() { return item; }
        //public Transform weaponArt;
        //public Transform pivot;
        //[ReadOnly][SerializeField] Vector3 pivotDirection;
        //public float rOffset = 0;
        //[Foldout("Positioning and Following")]
        //public bool followBody = true;
        //[HideInInspector] public Character source;

        //[SerializeField] private bool debug;
        //private Vector3 direction;
        //public virtual Vector3 Direction { get => direction; set => direction = value; }

        //[Foldout("Power Level", true)]
        //[Header("Power Level")]
        //[ReadOnly][SerializeField] private float powerLevel;
        //public float PowerLevel { get => powerLevel; set => SetPowerLevel(value); }
        //public void SetPowerLevel(float powerLevel) { this.powerLevel = powerLevel; onSetPowerLevel.Invoke(this.powerLevel); }
        //public UnityEvent<float> onSetPowerLevel;

        //[ReadOnly][SerializeField] private int maxPowerLevel = 1;
        //public int MaxPowerLevel { get => maxPowerLevel; set => maxPowerLevel = value; }
        //public void SetMaxPowerLevel(int maxPowerLevel) { this.maxPowerLevel = maxPowerLevel; onSetMaxPowerLevel.Invoke(this.maxPowerLevel); }
        //public UnityEvent<int> onSetMaxPowerLevel;

        //[Foldout("Positionables", true)]
        //[Header("Things to Position")]
        //public List<ToLocation<Positionable>> toLocations = new();
        //public List<Transform> positionables;
        //public List<CastedVFX> effects = new();

        //// Statuses
        //[Foldout("Statuses", true)]
        //[Header("Statuses")]
        //public List<Status> triggerStatuses;
        //public List<Status> castStatuses;
        //public List<Status> hitStatuses;

        //// Damage
        //[Foldout("Identity and Damage", true)]
        //[Header("Identity and Damage")]
        //private Identity identity = Identity.Neutral;
        //public Identity Identity
        //{
        //    get => identity;
        //    set
        //    {
        //        identity = value;
        //        onSetIdentity.Invoke(identity);
        //    }
        //}
        //public UnityEvent<Identity> onSetIdentity;
        //private Damager damager;

        // Events
        [Foldout("Casting", true)]
        [Header("Casting")]
        public List<GameObject> castingMethods = new();
        public bool casting = false;
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        public bool unCastOnRelease = false;
        //public UnityEvent onTrigger;
        //public UnityEvent<Vector3> onCast;
        //public UnityEvent onRelease;
        //public UnityEvent onUnCast;
        //public UnityEvent onCasted;

        public bool debug;


        // Initialization
        //private void Awake()
        //{
        //    damager = GetComponent<Damager>();
        //}

        public virtual void Initialize(Character owner)
        {
            Initialize(owner, null);
        }
        public override void Initialize(ICastCompatible owner, CastableItem item = null, int action=0)
        {
            //this.item = item;
            //this.source = source;
            //Identity = source.Identity;
            //if (damager != null) { damager.Ignore(source.body); }
            //owner.artRenderer.DisplayWeapon(weaponArt);

            base.Initialize(owner, item, action);

            // Positioning
            //ReportOriginToPositionables();
            if (owner.Body != null)
            {
                ReportExceptionsToCollidables(owner.Body.GetComponents<Collider>());
                PositionCastable();
            }
            foreach (GameObject method in castingMethods)
            {
                method.SetActive(false);
            }
            foreach (var toLocation in fields.toLocations)
            {
                Transform toSource = toLocation.GetSourceTransform(owner as Character);
                Transform toTarget = toLocation.GetTargetTransform(owner as Character);
                toLocation.toMove.SetOrigin(toSource, toTarget);
                if (toLocation.toMove.TryGetComponent<CastedVFX>(out var vfx))
                {
                    fields.effects.Add(vfx);
                    //owner.vfxController.AddVFX(vfx);
                }
            }
            MaxPowerLevel = 3;
        }

        // ICastable (State-Based)
        public void QueueAction(CastAction action)
        {
            switch (action)
            {
                case CastAction.Equip: Equip(); break;
                case CastAction.UnEquip: UnEquip(); break;
                case CastAction.Trigger: Trigger(); break;
                case CastAction.Release: Release(); break;
                case CastAction.Start: Cast(); break;
                case CastAction.End: UnCast(); break;
            }
        }

        public virtual bool CanCast() { return !casting; }
        public virtual UnityEvent OnCasted() { return onCasted; }

        // Equipping
        //public virtual void Disable() { }
        //public virtual void Enable() { }
        private void Equip()
        {
            foreach (var effect in fields.effects)
            {
                effect.equipped = true;
            }
        }
        private void UnEquip()
        {
            foreach (var effect in fields.effects)
            {
                effect.equipped = false;
            }
            Item.UnEquip(); Destroy(gameObject);
        }


        // Triggering

        public virtual void Trigger()
        {
            foreach (Status status in fields.triggerStatuses)
            {
                status.effect.Apply(Owner as Character, status.strength);
            }
            onTrigger.Invoke();
            if (castOnTrigger) Cast();
        }

        public virtual void Release()
        {
            foreach (Status status in fields.triggerStatuses)
            {
                status.effect.Remove(Owner as Character);
            }
            onRelease.Invoke();
            if (castOnRelease) Cast();
            if (unCastOnRelease) UnCast();
        }

        // Casting

        public virtual void Cast()
        {
            casting = true;
            if (fields.pivot != null)
            {
                //pivot.SetRotationWithVector(Direction); //.XZVector());
                fields.pivot.forward = Direction == Vector3.zero ? Vector3.forward : Direction;
                fields.pivotDirection = fields.pivot.forward;
            }
            foreach (Status status in fields.castStatuses)
            {
                status.effect.Apply(Owner as Character, status.strength);
            }
            if (debug) { Debug.Log($"{name} casting in {fields.direction} direction."); }
            onCast.Invoke(fields.direction);
        }

        public virtual void UnCast()
        {
            casting = false;
            foreach (Status status in fields.castStatuses)
            {
                status.effect.Remove(Owner as Character);
            }
            onUnCast.Invoke();
            onCasted.Invoke();
        }



        // Extras

        private void PositionCastable()
        {
            if (fields.pivot != null)
            {
                Transform origin = fields.followBody ? Owner.Body : transform;
                Vector3 pivotLocalPosition = fields.pivot.localPosition;
                fields.pivot.SetParent(origin, false);
                fields.pivot.localPosition = pivotLocalPosition;
            }
        }

        private void ReportOriginToPositionables()
        {
            Transform effectParent = fields.followBody ? Owner.Body : Owner.Transform;
            ReportOriginAmong(onTrigger, effectParent);
            ReportOriginAmong(onCast, effectParent);
            ReportOriginAmong(onUnCast, effectParent);
            ReportOriginAmong(onRelease, effectParent);
        }

        private void ReportOriginAmong(UnityEventBase uEvent, Transform effectParent)
        {
            for (int l = 0; l < uEvent.GetPersistentEventCount(); l++)
            {
                object target = uEvent.GetPersistentTarget(l);
                if (target is IPositionable positionable)
                {
                    positionable.SetOrigin(effectParent, Owner.Body);
                    if (Owner.WeaponLocation != null)
                        positionable.SetOffset(Owner.WeaponLocation.localPosition, fields.rOffset);
                }
            }
        }

        private void ReportExceptionsToCollidables(Collider[] exceptions)
        {
            ReportExceptionsAmong(onTrigger, exceptions);
            ReportExceptionsAmong(onCast, exceptions);
        }

        private void ReportExceptionsAmong(UnityEventBase uEvent, Collider[] exceptions)
        {
            for (int l = 0; l < uEvent.GetPersistentEventCount(); l++)
            {
                object target = uEvent.GetPersistentTarget(l);
                if (target is ICollidables collidable)
                {
                    collidable.SetExceptions(exceptions);
                }
            }
        }
    }
}