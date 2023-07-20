using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;

public class Impact : Validator
{
    // Settings
    [Header("Settings")]
    public bool oneShot = false;
    public bool hasCollided = false;

    [Header("Connections")]
    public BinaryEvent onCollision;
    public BinaryEvent onTrigger;

    // Tracking
    public readonly List<GameObject> touching = new();
    [HideInInspector] public GameObject other;

    // Events

    private void OnEventEnter(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if ((!oneShot || !hasCollided) && Validate(other) && !touching.Contains(other))
        {
            if (debug) print($"Other: {other.name}");
            touching.Add(other);
            onEvent.Invoke();
            hasCollided = true;
        }
    }

    private void OnEventExit(GameObject other, UnityEvent onEvent)
    {
        this.other = other;
        if (Validate(other))
        {
            touching.Remove(other);
            onEvent.Invoke();
        }
    }


    // Signals
    
    private void OnTriggerExit(Collider other)
    {
        OnEventExit(other.gameObject, onTrigger.exit);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnEventEnter(collision.gameObject, onCollision.enter);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnEventExit(collision.gameObject, onCollision.exit);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEventEnter(other.gameObject, onTrigger.enter);
    }
}
