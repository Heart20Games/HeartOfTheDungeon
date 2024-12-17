using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static Colliders;

namespace HotD.Castables
{
    public class CastedCollider : Casted, IDamager, ICollisionExceptions
    {
        [Foldout("Orientation", true)]
        [ReadOnly][SerializeField] protected Vector3 localPosition;
        public Vector3 direction = new();

        [Foldout("Damage", true)]
        [SerializeField] protected bool debugDamage;
        public UnityEvent<Impactor> hitDamageable;
        public UnityEvent<Impactor> leftDamageable;

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

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i].enabled = active;
            }
        }

        // Cleanup
        public virtual void QueueCleanup<T>(Transform pInstance, T bInstance, float lifeSpan, List<T> projectiles) where T : CastedCollider
        {
            StartCoroutine(CleanupInstance(pInstance, bInstance, lifeSpan, projectiles));
        }

        protected virtual IEnumerator CleanupInstance<T>(Transform pInstance, T bInstance, float lifeSpan, List<T> projectiles) where T : CastedCollider
        {
            Print("Waiting to cleanup Projectile instance.", debug, this);
            yield return new WaitForSeconds(lifeSpan);
            Print("Deleting Projectile instance...", debug, this);
            projectiles?.Remove(bInstance);
            if (pInstance != null)
            {
                Destroy(pInstance.gameObject);
            }
            Print("Cleaned up Projectile instance", debug, this);
        }

        // Collision
        private Collider[] InitializeColliders()
        {
            colliders = GetComponentsInChildren<Collider>();
            Print($"Initialized Projectile Colliders ({colliders.Length})", debugCollision, this);
            return colliders;
        }

        public void HitDamageable(Impactor impact)
        {
            Assert.IsNotNull(impact.other.collider);
            Print($"Hit Damageable: {impact.other.collider.gameObject.name}", debugDamage, this);
            hitDamageable.Invoke(impact);
        }

        public void LeftDamageable(Impactor impact)
        {
            if (impact.other.collider != null)
            {
                Print($"Left Damageable: {impact.other.collider.gameObject.name}", debugDamage, this);
            }
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
        }
        public void RemoveExceptions(Collider[] exceptions)
        {
            Print($"Removing Projectile Collision Exceptions ({exceptions.Length})", debugCollision, this);
            foreach (Collider exception in exceptions)
            {
                RemoveException(exception);
            }
        }
    }
}
