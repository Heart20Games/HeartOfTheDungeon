using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Colliders;

namespace HotD.Castables
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : CastedCollider, ICollidable
    {
        [Foldout("Projectile", true)]
        [ReadOnly][SerializeField] private Vector3 localPosition;
        public Vector3 direction = new();
        public float speed = 0;

        private bool shouldIgnoreDodgeLayer;

        [Foldout("Collision", true)]
        private new Rigidbody rigidbody;
        [SerializeField] private Collider[] colliders;
        private Collider[] Colliders { get { return colliders ?? InitializeColliders(); } }

        [Foldout("Tracking", true)]
        public Transform trackingTarget;
        public Transform TrackingTarget { get => trackingTarget; set => trackingTarget = value; }
        [Foldout("Tracking")] public float damping;

        public bool ShouldIgnoreDodgeLayer
        {
            get => shouldIgnoreDodgeLayer;
            set => shouldIgnoreDodgeLayer = value;
        }

        private void LateUpdate()
        {
            if (trackingTarget != null)
                transform.LookToward(target, damping);
        }

        private Collider[] InitializeColliders()
        {
            colliders = GetComponentsInChildren<Collider>();
            Print($"Initialized Projectile Colliders ({colliders.Length})", debug, this);
            return colliders;
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            InitializeColliders();
        }

        private void Start()
        {
            rigidbody.position = transform.position;
            rigidbody.velocity = speed * Time.fixedDeltaTime * direction;
        }

        private void FixedUpdate()
        {
            rigidbody.velocity = speed * Time.fixedDeltaTime * direction;
        }

        public void SetSpeed(float val)
        {
            speed = val;
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
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void QueueCleanup(Transform pInstance, Projectile bInstance, float lifeSpan, List<Projectile> projectiles)
        {
            StartCoroutine(CleanupInstance(pInstance, bInstance, lifeSpan, projectiles));
        }

        private IEnumerator CleanupInstance(Transform pInstance, Projectile bInstance, float lifeSpan, List<Projectile> projectiles)
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

        // Collision Exceptions
        public void AddException(Collider exception)
        {
            Print($"Adding Projectile Collision Exception", debug, this);
            ChangeException(Colliders, exception, true);
        }
        public void RemoveException(Collider exception)
        {
            Print($"Removing Projectile Collision Exception", debug, this);
            ChangeException(Colliders, exception, false);
        }
        public void AddExceptions(Collider[] exceptions)
        {
            Print($"Adding Projectile Collision Exceptions ({exceptions.Length})", debug, this);
            ChangeExceptions(Colliders, exceptions, true);
        }
        public void RemoveExceptions(Collider[] exceptions)
        {
            Print($"Removing Projectile Collision Exceptions ({exceptions.Length})", debug, this);
            ChangeExceptions(Colliders, exceptions, false);
        }
    }
}