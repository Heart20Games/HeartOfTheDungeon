using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Explosion: BaseMonoBehaviour
{
    public enum CollisionMode { SphereCast, Collider }

    public Identity identity = Identity.Neutral;
    public float radius = 5f;
    public float force = 10f;
    public int damage = 10;
    public CollisionMode collisionMode = CollisionMode.SphereCast;

    public bool triggerExplosion = false;

    public bool debug;
    public float debugRayDuration = 0.01f;

    [Serializable]
    public struct Other
    {
        public Collider collider;
        public IDamageReceiver damageReceiver;

        public Other(Collider collider, IDamageReceiver damageReceiver)
        {
            this.collider = collider;
            this.damageReceiver = damageReceiver;
        }
    }

    [SerializeField] private List<Other> others = new();
    [SerializeField] private Collider collider;

    private void Awake()
    {
        if (this.collider == null)
        {
            foreach (Collider collider in GetComponents<Collider>())
            {
                if (collider.isTrigger)
                {
                    this.collider = collider;
                }
            }
        }
    }

    private void Update()
    {
        if (triggerExplosion)
        {
            triggerExplosion = false;
            Explode();
        }
    }

    public void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.forward * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.right * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.back * radius, Color.yellow, debugRayDuration);
        Debug.DrawRay(transform.position, Vector3.left * radius, Color.yellow, debugRayDuration);
    }

    public void Explode()
    {
        if (collisionMode == CollisionMode.SphereCast)
        {
            RaycastHit[] hits = Physics.SphereCastAll(new(transform.position, Vector3.forward), radius);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                PushThem(hit.collider);
                if (hit.collider.TryGetComponent(out IDamageReceiver damageable))
                {
                    DamageThem(new(hit.collider, damageable), hit.point);
                }
            }
        }
        else if (collisionMode == CollisionMode.Collider)
        {
            ValidateOthers();

            foreach (var other in others)
            {
                Ray ray = new(transform.position, other.collider.transform.position - transform.position);
                Vector3 hitPosition = other.collider.transform.position;
                if (other.collider.Raycast(ray, out var hit, 1000))
                {
                    hitPosition = hit.point;
                }
                PushThem(collider);
                DamageThem(other, hitPosition);
            }
        }
    }

    private void PushThem(Collider collider)
    {
        if (collider.TryGetComponent(out Rigidbody rigidbody))
        {
            Print($"Push them! {collider.gameObject.name}", debug);
            rigidbody.AddExplosionForce(force, transform.position, radius);
        }
    }

    private void DamageThem(Other other, Vector3 hitPosition)
    {
        Print($"Damage them! {other.collider.gameObject.name}", debug);
        other.damageReceiver.SetDamagePosition(hitPosition);
        other.damageReceiver.TakeDamage(damage, identity);
    }

    private void ValidateOthers()
    {
        for (int i = others.Count - 1; i >= 0; i--)
        {
            if (others[i].collider == null)
                others.Remove(others[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && other.gameObject.TryGetComponent<IDamageReceiver>(out var damageReceiver))
        {
            foreach (var item in others)
            {
                if (item.damageReceiver == damageReceiver) return;
            }
            others.Add(new(other, damageReceiver));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.gameObject.TryGetComponent<IDamageReceiver>(out var damageReceiver))
        {
            others.Remove(new(other, damageReceiver));
        }
    }
}
