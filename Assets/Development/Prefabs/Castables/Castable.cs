using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Member;

public class Castable : MonoBehaviour, ICastable
{
    [HideInInspector] public Character source;

    public UnityEvent<Vector3> doCast;
    public UnityEvent onCast;
    public UnityEvent onUnCast;
    public UnityEvent onCasted;
    public bool casting = false;

    public List<Status> castStatuses;
    public List<Status> hitStatuses;

    public float rOffset = 0;
    public bool followBody = true;

    private Damager damager;

    private void Awake()
    {
        damager = GetComponent<Damager>();
    }

    public virtual void Initialize(Character source)
    {
        this.source = source;
        if (damager != null) { damager.Ignore(source.body); }
        ReportOriginToPositionables();
    }
    
    public virtual bool CanCast() { return !casting; }

    public virtual void Cast(Vector3 direction)
    {
        doCast.Invoke(direction);
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

    // Extras
    private void ReportOriginToPositionables()
    {
        Transform effectParent = followBody ? source.body : source.transform;
        ReportOriginAmong(onCast, effectParent);
        ReportOriginAmong(doCast, effectParent);
    }

    private void ReportOriginAmong(UnityEventBase uEvent, Transform effectParent)
    {
        for (int l = 0; l < uEvent.GetPersistentEventCount(); l++)
        {
            object target = uEvent.GetPersistentTarget(l);
            if (target is IPositionable positionable)
            {
                positionable.SetOrigin(effectParent, source.body);
                positionable.SetOffset(source.weaponOffset, rOffset);
            }
        }
    } 
}