using CustomUnityEvents;
using MyBox;
using Sisus.ComponentNames;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using Body;

public class Impact : Validator
{
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
        public Other(GameObject gameObject, Collision collision, Collider collider)
        {
            this.gameObject = gameObject;
            this.collision = collision;
            this.collider = collider;
        }

        public GameObject gameObject;
        public Collision collision;
        public Collider collider;

        public Vector3 ImpactLocation
        {
            get
            {
                if (collision != null && collision.contacts.Length > 0)
                    return collision.GetContact(0).point;
                else
                    return Vector3.zero;
            }
        }
    }

    // Settings
    //[Header("Settings")]
    public bool oneShot = false;
    public bool hasCollided = false;

    [Foldout("Events", true)]
    [Header("Connections")]
    public BinaryEvent onCollision;
    public BinaryEvent onTrigger;
    [SerializeField] private Body.Character character;
    [Foldout("Events")] public ImpactEvents onImpact;

    public Other other;

    // Tracking
    [ReadOnly] public List<GameObject> touching = new();

    public Body.Character _Character => character;

    private void OnEnable() {}

    // On Event Enter

    private void OnEventEnter(Collider collider, UnityEvent onEvent)
    {
        Other other = new(collider.gameObject, null, collider);
        OnEventEnter(other, onEvent);
    }

    private void OnEventEnter(Collision collision, UnityEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider);
        OnEventEnter(other, onEvent);
    }

    private void OnEventEnter(Other other, UnityEvent onEvent)
    {
        if (isActiveAndEnabled)
        {
            this.other = other;
            this.other.gameObject = other.gameObject;
            if ((!oneShot || !hasCollided) && Validate(other.gameObject) && !touching.Contains(other.gameObject))
            {
                Print($"Valid Other: {other.gameObject.name} ({this.GetName()})", debug, this);
                touching.Add(other.gameObject);
                hasCollided = true;
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
        Other other = new(collider.gameObject, null, collider);
        OnEventExit(other, onEvent);
    }

    private void OnEventExit(Collision collision, UnityEvent onEvent)
    {
        Other other = new(collision.gameObject, collision, collision.collider);
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
}
