using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Colliders;

namespace HotD.Castables
{
    public class CastedCollider : Casted, IDamager, ICollisionExceptions
    {
        [Foldout("Damage", true)]
        [SerializeField] protected bool debugDamage;
        public UnityEvent<Impact> hitDamageable;
        public UnityEvent<Impact> leftDamageable;

        [Foldout("Collision", true)]
        [SerializeField] protected bool debugCollision;
        [SerializeField] protected Collider[] colliders;
        [Foldout("Collision")]
        [SerializeField] protected List<Collider> exceptions;
        protected Collider[] Colliders { get { return colliders ?? InitializeColliders(); } }
        public Collider[] Exceptions
        {
            get => exceptions.ToArray();
            set => SetExceptions(value);
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeColliders();
        }

        private Collider[] InitializeColliders()
        {
            colliders = GetComponentsInChildren<Collider>();
            Print($"Initialized Projectile Colliders ({colliders.Length})", debugCollision, this);
            return colliders;
        }

        public void HitDamageable(Impact impact)
        {
            Print($"Hit Damageable: {impact.other.collider.gameObject.name}", debugDamage, this);
            hitDamageable.Invoke(impact);
        }

        public void LeftDamageable(Impact impact)
        {
            Print($"Left Damageable: {impact.other.collider.gameObject.name}", debugDamage, this);
            leftDamageable.Invoke(impact);
        }

        // Collision Exceptions
        public void SetExceptions(Collider[] exceptions)
        {
            RemoveExceptions(Exceptions);
            AddExceptions(exceptions);
            onSetExceptions.Invoke(Exceptions);
        }
        public void AddException(Collider exception)
        {
            Print($"Adding Projectile Collision Exception", debugCollision, this);
            ChangeException(Colliders, exception, true);
            if (!exceptions.Contains(exception))
            {
                exceptions.Add(exception);
            }
        }
        public void RemoveException(Collider exception)
        {
            Print($"Removing Projectile Collision Exception", debugCollision, this);
            ChangeException(Colliders, exception, false);
            exceptions.Remove(exception);
        }
        public void AddExceptions(Collider[] exceptions)
        {
            Print($"Adding Projectile Collision Exceptions ({exceptions.Length})", debugCollision, this);
            foreach (Collider exception in exceptions)
            {
                AddException(exception);
            }
            //ChangeExceptions(Colliders, exceptions, true);
            //this.exceptions.AddRange(exceptions);
        }
        public void RemoveExceptions(Collider[] exceptions)
        {
            Print($"Removing Projectile Collision Exceptions ({exceptions.Length})", debugCollision, this);
            foreach (Collider exception in exceptions)
            {
                RemoveException(exception);
            }
            //ChangeExceptions(Colliders, exceptions, false);
            //foreach (var exception in exceptions)
            //{
            //    this.exceptions.Remove(exception);
            //}
        }
    }
}
