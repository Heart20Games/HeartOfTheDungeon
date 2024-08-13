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
        private ICollidables[] collidables;

        public override void InitializeEvents()
        {
            base.InitializeEvents();
            onEnable ??= new();
        }

        protected new void Awake()
        {
            base.Awake();
            collidables = GetComponentsInChildren<ICollidables>(true);
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
            base.OnEnable();
            foreach (var positionable in positionables)
            {
                if (Owner == null)
                {
                    Debug.LogWarning($"Owner Null (Execution Method)", this);
                }
                
                Assert.IsNotNull(Crosshair.main);
                Transform source = (Owner == null ?
                    transform :
                    (Owner.Body !=  null ?
                        Owner.Body :
                        Owner.Transform
                    )
                );
                Transform location = (Owner == null ? 
                    transform :
                    (Owner.FiringLocation != null ?
                        Owner.FiringLocation :
                        (Owner.WeaponLocation != null ?
                            Owner.WeaponLocation :
                            (Owner.Body != null ?
                                Owner.Body :
                                Owner.Transform
                            )
                        )
                    )
                );
                Vector3 target = (aimAtCrosshair ? Crosshair.main.TargetedPosition() : location.position);
                positionable.SetOrigin(source, location);
                positionable.SetTargetPosition(target);
            }
            foreach (var collidable in collidables)
            {
                collidable?.SetExceptions(fields.CollisionExceptions);
            }
            onEnable.Invoke();
        }

        [ButtonMethod]
        public void TestCollisionExceptions()
        {
            fields.CollisionExceptions = fields.CollisionExceptions;
        }
    }
}
