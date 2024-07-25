using CustomUnityEvents;
using HotD.Body;
using MyBox;
using Sisus.ComponentNames;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Impact : Validator
{
    [Serializable]
    public struct ImpactEvents
    {
        public BinaryEvent<Impact> impact;
        public BinaryEvent<GameObject> other;
        public BinaryEvent<ASelectable> selectable;
        public void InvokeEnter(Impact impact)
        {
            this.impact.enter.Invoke(impact);
            this.other.enter.Invoke(impact.other);
            this.selectable.enter.Invoke(impact.selectable);
        }
        public void InvokeExit(Impact impact)
        {
            this.impact.exit.Invoke(impact);
            this.other.exit.Invoke(impact.other);
            this.selectable.exit.Invoke(impact.selectable);
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
    [SerializeField] private Character character;
    [Foldout("Events")] public ImpactEvents onImpact;

    // Tracking
    [ReadOnly] public List<GameObject> touching = new();
    [HideInInspector] public GameObject other;
    [HideInInspector] public Vector3 impactLocation;

    public Character _Character => character;

    private void OnEnable() {}

    // Events

    private void OnEventEnter(GameObject other, UnityEvent onEvent)
    {
        if (isActiveAndEnabled)
        {
            this.other = other;
            if ((!oneShot || !hasCollided) && Validate(other) && !touching.Contains(other))
            {
                Print($"Valid Other: {other.name} ({this.GetName()})", debug);
                touching.Add(other);
                hasCollided = true;
                onEvent.Invoke();
                onImpact.InvokeEnter(this);
            }
            else
            {
                Print($"Invalid Other: {other.name} ({this.GetName()})", debug);
            }
        }
    }

    private void OnEventExit(GameObject other, UnityEvent onEvent)
    {
        if (isActiveAndEnabled)
        {
            this.other = other;
            if (Validate(other))
            {
                touching.Remove(other);
                onEvent.Invoke();
                onImpact.InvokeExit(this);
            }
        }
    }


    // Signals
    
    private void OnTriggerExit(Collider other)
    {
        OnEventExit(other.gameObject, onTrigger.exit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
            impactLocation = collision.GetContact(0).point;
        OnEventEnter(collision.gameObject, onCollision.enter);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.contacts.Length > 0)
            impactLocation = collision.GetContact(0).point;
        OnEventExit(collision.gameObject, onCollision.exit);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEventEnter(other.gameObject, onTrigger.enter);
    }
}
