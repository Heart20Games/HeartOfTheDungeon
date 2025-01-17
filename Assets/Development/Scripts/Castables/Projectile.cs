using MyBox;
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
            SetDirection(target, source, 1.5f);

            rigidbody.position = transform.position;
        }

        private void FixedUpdate()
        {
            rigidbody.velocity = speed * Time.fixedDeltaTime * direction;

            if(rigidbody.velocity != Vector3.zero) rigidbody.rotation = Quaternion.LookRotation(rigidbody.velocity);
        }

        public void SetDirection(Transform toTarget, Transform origin, float offSet)
        {
            if (toTarget == null) return;
            if (origin == null) return;

            Vector3 distance = new Vector3(toTarget.position.x - origin.position.x, (toTarget.position.y + offSet) - origin.position.y, toTarget.position.z - origin.position.z).normalized;

            direction = distance;
        }

        public void SetSource(Transform trans)
        {
            source = trans;
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