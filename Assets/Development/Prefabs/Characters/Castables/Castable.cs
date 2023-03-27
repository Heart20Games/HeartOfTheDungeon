using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Castable : MonoBehaviour, ICastable
{
    [HideInInspector] public Character source;

    public UnityEvent onCast;
    public UnityEvent onUnCast;
    public UnityEvent onCasted;
    public bool casting = false;

    public List<Status> castStatuses;
    public List<Status> hitStatuses;

    public virtual void Initialize(Character source)
    {
        this.source = source;
    }
    
    public virtual bool CanCast() { return !casting; }

    public virtual void Cast(Vector3 direction)
    {
        casting = true;
        foreach (Status status in castStatuses)
        {
            status.effect.Apply(source, status.strength);
        }
        onCast.Invoke();
    }

    public virtual void UnCast()
    {
        casting = false;
        foreach (Status status in castStatuses)
        {
            status.effect.Remove(source);
        }
        onUnCast.Invoke();
        onCasted.Invoke();
    }

    public virtual void Disable() { }

    public virtual void Enable() { }

    public virtual UnityEvent OnCasted() { return onCasted; }

    public virtual void UnEquip() { Destroy(gameObject); }
}
