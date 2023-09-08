using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;
using static Body.Behavior.ContextSteering.CSIdentity;
using System.Runtime.CompilerServices;

public class Castable : BaseMonoBehaviour, ICastable
{

    // Positioning
    [Header("Positioning and Following")]
    public CastableItem item;
    public CastableItem GetItem() { return item; }
    public Transform weaponArt;
    public Transform pivot;
    public float rOffset = 0;
    public bool followBody = true;
    [HideInInspector] public Character source;

    // Statuses
    [Header("Statuses")]
    public List<Status> castStatuses;
    public List<Status> hitStatuses;

    // Damage
    [Header("Identity and Damage")]
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
    public UnityEvent<Identity> onSetIdentity;
    private Damager damager;

    public List<Transform> positionables;

    // Events
    [Header("Casting")]
    public bool casting = false;
    public UnityEvent<Vector3> doCast;
    public UnityEvent onCast;
    public UnityEvent onUnCast;
    public UnityEvent onCasted;


    // Iniaitlization
    private void Awake()
    {
        damager = GetComponent<Damager>();
    }

    public virtual void Initialize(Character source)
    {
        Initialize(source, null);
    }
    public virtual void Initialize(Character source, CastableItem item=null)
    {
        this.item = item;
        this.source = source;
        Identity = source.Identity;
        if (damager != null) { damager.Ignore(source.body); }
        ReportOriginToPositionables();
        if (source.body != null)
        {
            ReportExceptionsToCollidables(source.body.GetComponents<Collider>());
            PositionCastable();
        }
        source.artRenderer.DisplayWeapon(weaponArt);
    }


    // Equipping
    public virtual void Disable() { }
    public virtual void Enable() { }
    public virtual void UnEquip() { item.UnEquip(); Destroy(gameObject); }
    

    // Casting

    public virtual bool CanCast() { return !casting; }

    public virtual void Cast(Vector3 direction)
    {
        doCast.Invoke(direction);
        if (pivot != null)
            pivot.SetRotationWithVector(direction.XZVector());
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

    public virtual UnityEvent OnCasted() { return onCasted; }


    // Extras

    private void PositionCastable()
    {
        if (pivot != null)
        {
            Transform origin = followBody ? source.body : transform;
            Vector3 pivotLocalPosition = pivot.localPosition;
            pivot.SetParent(origin, false);
            pivot.localPosition = pivotLocalPosition;
        }
    }

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