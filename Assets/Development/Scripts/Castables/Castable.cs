using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;
using static Body.Behavior.ContextSteering.CSIdentity;

namespace HotD.Castables
{
    using HotD.Body;
    using MyBox;

    public class Castable : CastProperties, ICastable
    {
        // Events
        [Foldout("Casting", true)]
        [Header("Casting")]
        public List<GameObject> castingMethods = new();
        public bool casting = false;
        [Foldout("Events", true)]
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        [Foldout("Events")] public bool unCastOnRelease = false;
        public virtual bool CanCast { get => !casting; }
        [SerializeField] protected bool debugCastable = false;

        public virtual void Initialize(Character owner)
        {
            Initialize(owner, null);
        }
        public override void Initialize(ICastCompatible owner, CastableItem item = null)
        {
            base.Initialize(owner, item);

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
                if (owner is Character)
                {
                    Transform toSource = toLocation.GetSourceTransform(owner as Character);
                    Transform toTarget = toLocation.GetTargetTransform(owner as Character);
                    toLocation.toMove.SetOrigin(toSource, toTarget);
                }
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

        public virtual UnityEvent OnCasted() { return castEvents.onCasted; }

        // Equipping
        //public virtual void Disable() { }
        //public virtual void Enable() { }
        private void Equip()
        {
            foreach (var effect in fields.effects)
            {
                effect.Equipped = true;
            }
        }
        private void UnEquip()
        {
            foreach (var effect in fields.effects)
            {
                effect.Equipped = false;
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
            castEvents.onTrigger.Invoke();
            if (castOnTrigger) Cast();
        }

        public virtual void Release()
        {
            foreach (Status status in fields.triggerStatuses)
            {
                status.effect.Remove(Owner as Character);
            }
            castEvents.onRelease.Invoke();
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
            Print($"{name} casting in {fields.direction} direction.", debugCastable, this);
            castEvents.onStartCast.Invoke(fields.direction);
        }

        public virtual void UnCast()
        {
            casting = false;
            foreach (Status status in fields.castStatuses)
            {
                status.effect.Remove(Owner as Character);
            }
            castEvents.onEndCast.Invoke();
            castEvents.onCasted.Invoke();
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
            ReportOriginAmong(castEvents.onTrigger, effectParent);
            ReportOriginAmong(castEvents.onStartCast, effectParent);
            ReportOriginAmong(castEvents.onEndCast, effectParent);
            ReportOriginAmong(castEvents.onRelease, effectParent);
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
            ReportExceptionsAmong(castEvents.onTrigger, exceptions);
            ReportExceptionsAmong(castEvents.onStartCast, exceptions);
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