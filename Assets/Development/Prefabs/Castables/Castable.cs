using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;
using static Body.Behavior.ContextSteering.CSIdentity;

public class Castable : BaseMonoBehaviour, ICastable
{
    [HideInInspector] public Body.Character source;

    public UnityEvent<Vector3> doCast;
    public UnityEvent onCast;
    public UnityEvent onUnCast;
    public UnityEvent onCasted;
    public bool casting = false;
    public UnityEvent<Identity> onSetIdentity;

    public List<Status> castStatuses;
    public List<Status> hitStatuses;

    private Identity identity = Identity.Neutral;
    public Identity Identity
    {
        get => identity;
        set
        {
            identity = value;
            onSetIdentity.Invoke(identity);
        }
    }

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
        Identity = source.Identity;
        if (damager != null) { damager.Ignore(source.body); }
        ReportOriginToPositionables();
        if (source.body != null)
        {
            ReportExceptionsToCollidables(source.body.GetComponents<Collider>());
        }
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
                positionable.SetOffset(source.weaponOffset.localPosition, rOffset);
            }
        }
    } 

    private void ReportExceptionsToCollidables(Collider[] exceptions)
    {
        ReportExceptionsAmong(onCast, exceptions);
        ReportExceptionsAmong(doCast, exceptions);
    }

    private void ReportExceptionsAmong(UnityEventBase uEvent, Collider[] exceptions)
    {
        for (int l = 0; l < uEvent.GetPersistentEventCount(); l++)
        {
            object target = uEvent.GetPersistentTarget(l);
            if (target is ICollidables collidable)
            {
                collidable.SetExceptions(exceptions);
            }
        }
    } 
}