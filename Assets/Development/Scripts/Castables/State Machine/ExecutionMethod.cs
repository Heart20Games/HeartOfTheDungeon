using HotD.Body;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class ExecutionMethod : DependentCastProperties
    {
        public bool aimAtCrosshair = true;
        public List<Positionable> positionables = new();
        public UnityEvent onEnable = new();
        private ISetCollisionExceptions[] collidables;

        public override void InitializeEvents()
        {
            base.InitializeEvents();
            onEnable ??= new();
        }

        protected new void Awake()
        {
            base.Awake();

            fieldEvents.onSetOwner.Invoke(Owner);
            collidables = GetComponentsInChildren<ISetCollisionExceptions>(true);
            foreach (var collidable in collidables)
            {
                if (collidable == null)
                {
                    Debug.LogWarning("Found null collidable.", this);
                }
            }
        }

        protected new void OnEnable()
        {
            Print("Execution Method Enabled", true, this);

            base.OnEnable();

            fieldEvents.onSetOwner.Invoke(Owner);
            UpdatePositionables();
            UpdateCollisionExceptions();
            ApplyOrRemoveStatuses(GetStatuses(statusType), true);
            
            onEnable.Invoke();
        }

        private void OnDisable()
        {
            ApplyOrRemoveStatuses(GetStatuses(statusType), false);
        }

        public enum StatusType { None, Trigger, Cast, Hit }
        public StatusType statusType;
        private List<Status> GetStatuses(StatusType type)
        {
            return type switch
            {
                StatusType.Trigger => fields.triggerStatuses,
                StatusType.Cast => fields.castStatuses,
                StatusType.Hit => fields.hitStatuses,
                _ => null
            };
        }

        private void ApplyOrRemoveStatuses(List<Status> statuses, bool apply)
        {
            if (statuses != null && Owner != null && Owner is Character)
            {
                foreach (var status in statuses)
                {
                    if (status.effect != null)
                    {
                        if (apply) status.effect.Apply(Owner as Character, status.strength);
                        else status.effect.Remove(Owner as Character);
                    }
                }
            }
        }

        private void UpdatePositionables()
        {
            if (Owner == null)
            {
                Debug.LogWarning($"Owner Null (Execution Method)", this);
            }
            Assert.IsNotNull(Crosshair.main);

            Transform source = Owner == null ? transform : Owner.Transform;
            Transform body = Owner != null ? Owner.Body : transform;
            Transform location = (Owner == null ? transform :
                (Owner.FiringLocation != null ? Owner.FiringLocation :
                    (Owner.WeaponLocation != null ? Owner.WeaponLocation :
                        (Owner.Body != null ? Owner.Body : Owner.Transform
            ))));
            Vector3 target = (aimAtCrosshair ? Crosshair.main.TargetedPosition() : location.position);
            
            foreach (var positionable in positionables)
            {
                positionable.SetOrigin(source, location);
                positionable.SetTargetPosition(target);
            }
        }

        private void UpdateCollisionExceptions()
        {
            foreach (var collidable in collidables)
            {
                collidable?.SetExceptions(fields.CollisionExceptions);
            }
        }

        [ButtonMethod]
        public void TestCollisionExceptions()
        {
            fields.CollisionExceptions = fields.CollisionExceptions;
        }
    }
}
