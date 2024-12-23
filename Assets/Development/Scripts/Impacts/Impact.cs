using Body.Behavior.ContextSteering;
using CustomUnityEvents;
using HotD.Body;
using HotD.Castables;
using MyBox;
using Sisus.ComponentNames;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Impact : Validator
{
    // Settings
    //[Header("Settings")]
    [SerializeField] private bool oneShot = false;
    [SerializeField] private bool hasCollided = false;

    [Foldout("Events", true)]
    [Header("Connections")]
    [SerializeField] private BinaryEvent onCollision;
    [SerializeField] private BinaryEvent onTrigger;
    [SerializeField] private HotD.Body.Character character;
    [Foldout("Events")][SerializeField] private ImpactEvents onImpact;

    public Other other;

    // Tracking
    [ReadOnly][SerializeField] private List<GameObject> touching = new();

    public HotD.Body.Character _Character => character;

    // On Event Enter

    private void OnEventEnter(Collider collider, UnityEvent onEvent)
    {
        var normal = (collider.transform.position - transform.position).normalized;
        Other.ContactPoint point = new(normal, new());
        if (collider is SphereCollider || collider is BoxCollider || collider is CapsuleCollider)
        {
            var location = collider.ClosestPoint(collider.transform.position);
            point = new(normal, location);
        }
        else if (normal.magnitude != 0)
        {
            Ray ray = new(collider.transform.position, -normal.normalized);
            if (collider.Raycast(ray, out var hitInfo, normal.magnitude))
            {
                Print($"Raycast collision found.", debug, this);
                point = new(hitInfo.normal, hitInfo.point);
            }
        }
        Other other = new(collider.gameObject, null, collider, point);
        OnEventEnter(other, onEvent);
    }

    private void OnEventEnter(Collision collision, UnityEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider, new());
        OnEventEnter(other, onEvent);
    }

    private void OnEventEnter(Other other, UnityEvent onEvent)
    {
        if (isActiveAndEnabled)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DodgeZone")) return;

            if (other.gameObject.GetComponent<MagicShieldImpact>())
            {
                if (gameObject.GetComponent<Projectile>())
                {
                    if (gameObject.GetComponent<Projectile>().ShouldIgnoreDodgeLayer) return;
                }

                if (other.gameObject.transform.parent.GetComponent<CastListenerDistributor>()) return;
            }

            this.other = other;
            this.other.gameObject = other.gameObject;
            if(other.gameObject.tag == "Character")
            {
                touching.Add(other.gameObject);
                hasCollided = true;
                onEvent.Invoke();
                onImpact.InvokeEnter(this);
            }
            if ((!oneShot || !hasCollided) && Validate(other.gameObject) && !touching.Contains(other.gameObject))
            {
                Print($"Valid Other: {other.gameObject.name} ({this.GetName()})", debug, this);
                touching.Add(other.gameObject);
                hasCollided = true;
                Print($"Details: {other.ImpactLocation}", debug, this);
                onEvent.Invoke();
                onImpact.InvokeEnter(this);
            }
            else
            {
                Print($"Invalid Other: {other.gameObject.name} ({this.GetName()})", debug, this);
            }
        }
        else
        {
            Print($"Not active and enabled.", debug, this);
        }
    }

    // On Event Exit

    private void OnEventExit(Collider collider, UnityEvent onEvent)
    {
        Other other = new(collider.gameObject, null, collider, new());
        OnEventExit(other, onEvent);
    }

    private void OnEventExit(Collision collision, UnityEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider, new());
        OnEventExit(other, onEvent);
    }

    private void OnEventExit(Other other, UnityEvent onEvent)
    {
        if (isActiveAndEnabled)
        {
            this.other = other;
            if (Validate(other.gameObject))
            {
                touching.Remove(other.gameObject);
                onEvent.Invoke();
                onImpact.InvokeExit(this);
            }
        }
    }

    // Signals

    private void OnTriggerExit(Collider other)
    {
        Print("OnTriggerExit", debug, this);
        OnEventExit(other, onTrigger.exit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Print("OnCollisionEnter", debug, this);
        OnEventEnter(collision, onCollision.enter);
    }

    private void OnCollisionExit(Collision collision)
    {
        Print("OnCollsionExit", debug, this);
        OnEventExit(collision, onCollision.exit);
    }

    private void OnTriggerEnter(Collider other)
    {
        Print("OnTriggerEnter", debug, this);
        OnEventEnter(other, onTrigger.enter);
    }

    // Structs

    [Serializable]
    public struct ImpactEvents
    {
        public BinaryEvent<Impact> impact;
        public BinaryEvent<GameObject> other;
        public BinaryEvent<ASelectable> selectable;
        public BinaryEvent<Other> otherFull;
        public void InvokeEnter(Impact impact)
        {
            this.impact.enter.Invoke(impact);
            this.other.enter.Invoke(impact.other.gameObject);
            this.selectable.enter.Invoke(impact.selectable);
            this.otherFull.enter.Invoke(impact.other);
        }
        public void InvokeExit(Impact impact)
        {
            this.impact.exit.Invoke(impact);
            this.other.exit.Invoke(impact.other.gameObject);
            this.selectable.exit.Invoke(impact.selectable);
            this.otherFull.exit.Invoke(impact.other);
        }
    }

    [Serializable]
    public struct Other
    {
        public struct ContactPoint
        {
            public ContactPoint(Vector3 normal, Vector3 position)
            {
                this.normal = normal;
                this.position = position;
            }
            public Vector3 normal;
            public Vector3 position;
        }

        public Other(GameObject gameObject, Collision collision, Collider collider, Vector3 normal, Vector3 position)
        {
            this.gameObject = gameObject;
            this.collision = collision;
            this.collider = collider;
            this.contactPoint = new(normal, position);
        }

        public Other(GameObject gameObject, Collision collision, Collider collider, ContactPoint contactPoint = new())
        {
            this.gameObject = gameObject;
            this.collision = collision;
            this.collider = collider;
            this.contactPoint = contactPoint;
        }

        public GameObject gameObject;
        public Collision collision;
        public Collider collider;
        private ContactPoint contactPoint;

        public Vector3 ImpactLocation
        {
            get
            {
                if (collision != null && collision.contacts.Length > 0)
                    return collision.GetContact(0).point;
                else
                    return contactPoint.position;
            }
        }

        public Vector3 ImpactNormal
        {
            get
            {
                if (collision != null && collision.contacts.Length > 0)
                    return collision.GetContact(0).normal;
                else
                    return contactPoint.normal;
            }
        }
    }
}