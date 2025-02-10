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

public class Impactor : Validator
{
    // Settings
    //[Header("Settings")]
    [SerializeField] private bool oneShot = false;
    [SerializeField] private bool hasCollided = false;

    [Foldout("Events", true)]
    [Header("Connections")]
    [SerializeField] private BinaryEvent onCollision;
    [SerializeField] private BinaryEvent onTrigger;
    [SerializeField] private Character character;
    [Foldout("Events")][SerializeField] private ImpactEvents onImpact;

    public Other other;

    // Tracking
    [ReadOnly][SerializeField] private List<GameObject> touching = new();
    [ReadOnly][SerializeField] private List<Other> others = new();

    public Character _Character => character;


    // Enable / Disable

    private void OnEnable()
    {
        foreach (var other in others)
        {
            InvokeImpactEnter(other);
        }
    }

    private void OnDisable()
    {
        foreach (var other in others)
        {
            InvokeImpactExit(other);
        }
        others.Clear();
    }


    // Impact

    private void InvokeImpactEnter(Other other)
    {
        if (AlertOtherNull(other)) return;

        Print($"Valid Other: {other.gameObject.name} ({this.GetName()})", debug, this);
        touching.Add(other.gameObject);
        hasCollided = true;
        Print($"Details: {other.ImpactLocation}", debug, this);
        other.onEvent?.exit?.Invoke();
        onImpact.InvokeEnter(this);
    }

    private void InvokeImpactExit(Other other)
    {
        if (AlertOtherNull(other)) return;

        touching.Remove(other.gameObject);
        other.onEvent?.exit?.Invoke();
        onImpact.InvokeExit(this);
    }

    private bool AlertOtherNull(Other other)
    {
        if (other.gameObject == null)
        {
            Debug.LogWarning("GameObject on Other is Null!");
            return true;
        }
        else return false;
    }

    // On Event Enter

    private void OnEventEnter(Collider collider, BinaryEvent onEvent)
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
        Other other = new(collider.gameObject, null, collider, onEvent, point);
        OnEventEnter(other);
    }

    private void OnEventEnter(Collision collision, BinaryEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider, onEvent, new());
        OnEventEnter(other);
    }

    private void OnEventEnter(Other other)
    {
        if (isActiveAndEnabled)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DodgeZone")) return; // Hardcoded Ignore of the DodgeZone

            if (other.gameObject.GetComponent<MagicShieldImpact>()) // Hardcoded Ignore of the MagicShieldImpact VFX
            {
                if (gameObject.GetComponent<Projectile>())
                {
                    if (gameObject.GetComponent<Projectile>().ShouldIgnoreDodgeLayer) return;
                }

                if (other.gameObject.transform.parent.GetComponent<CastListenerDistributor>()) return;
            }

            this.other = other;
            this.other.gameObject = other.gameObject;
            if ((!oneShot || !hasCollided) && Validate(other.gameObject) && !touching.Contains(other.gameObject))
            {
                InvokeImpactEnter(other);
                others.Add(other);
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

    private void OnEventExit(Collider collider, BinaryEvent onEvent)
    {
        Other other = new(collider.gameObject, null, collider, onEvent, new());
        OnEventExit(other);
    }

    private void OnEventExit(Collision collision, BinaryEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider, onEvent, new());
        OnEventExit(other);
    }

    private void OnEventExit(Other other)
    {
        if (isActiveAndEnabled)
        {
            this.other = other;
            if (Validate(other.gameObject))
            {
                InvokeImpactExit(other);
                others.Remove(other);
            }
        }
    }

    // Signals

    private void OnTriggerExit(Collider other)
    {
        Print("OnTriggerExit", debug, this);
        OnEventExit(other, onTrigger);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Print("OnCollisionEnter", debug, this);
        OnEventEnter(collision, onCollision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Print("OnCollsionExit", debug, this);
        OnEventExit(collision, onCollision);
    }

    private void OnTriggerEnter(Collider other)
    {
        Print("OnTriggerEnter", debug, this);
        OnEventEnter(other, onTrigger);
    }

    // Structs

    [Serializable]
    public struct ImpactEvents
    {
        public BinaryEvent<Impactor> impact;
        public BinaryEvent<GameObject> other;
        public BinaryEvent<ASelectable> selectable;
        public BinaryEvent<Other> otherFull;
        public void InvokeEnter(Impactor impact)
        {
            this.impact.enter.Invoke(impact);
            this.other.enter.Invoke(impact.other.gameObject);
            this.selectable.enter.Invoke(impact.selectable);
            this.otherFull.enter.Invoke(impact.other);
        }
        public void InvokeExit(Impactor impact)
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

        public Other(GameObject gameObject, Collision collision, Collider collider, Vector3 normal, Vector3 position, BinaryEvent onEvent = null)
        {
            this.gameObject = gameObject;
            this.collision = collision;
            this.collider = collider;
            this.onEvent = onEvent;
            this.contactPoint = new(normal, position);
        }

        public Other(GameObject gameObject, Collision collision, Collider collider, BinaryEvent onEvent = null, ContactPoint contactPoint = new())
        {
            this.gameObject = gameObject;
            this.collision = collision;
            this.collider = collider;
            this.onEvent = onEvent;
            this.contactPoint = contactPoint;
        }

        public GameObject gameObject;
        public Collision collision;
        public Collider collider;
        public BinaryEvent onEvent;
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