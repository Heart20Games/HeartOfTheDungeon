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

        // Cleanup
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}