using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : CastedCollider, ICollisionExceptions
    {
        [Foldout("Projectile", true)]
        [ReadOnly][SerializeField] private Vector3 localPosition;
        public Vector3 direction = new();
        public float speed = 0;

        protected new Rigidbody rigidbody;
        private bool shouldIgnoreDodgeLayer;

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

        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
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
    }
}