using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class ExecutionMethod : CastableProperties
    {
        public bool aimAtCrosshair = true;
        public List<Positionable> positionables = new();
        public UnityEvent onEnable = new();
        public CastableProperties initializeOffOf;

        public override void InitializeEvents()
        {
            base.InitializeEvents();
            onEnable ??= new();
        }

        private void Awake()
        {
            Initialize(initializeOffOf.fields);
        }

        private void OnEnable()
        {
            foreach (var positionable in positionables)
            {
                if (Owner == null)
                {
                    Debug.LogWarning($"Owner Null");
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
            onEnable.Invoke();
        }
    }
}
