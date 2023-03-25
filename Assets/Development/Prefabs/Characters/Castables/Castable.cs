using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Castable : MonoBehaviour, ICastable
{
    public UnityEvent onCast;
    public UnityEvent onUnCast;
    public UnityEvent onCasted;
    public bool casting = false;

    public abstract void Initialize(Character source);
    
    public virtual bool CanCast() { return !casting; }

    public virtual void Cast(Vector3 direction)
    {
        casting = true;
        onCast.Invoke();
    }

    public virtual void UnCast()
    {
        casting = false;
        onUnCast.Invoke();
        onCasted.Invoke();
    }

    public virtual void Disable() { }

    public virtual void Enable() { }

    public virtual UnityEvent OnCasted() { return onCasted; }

    public virtual void UnEquip() { Destroy(gameObject); }
}
