using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;
using static Body.Behavior.ContextSteering.CSIdentity;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

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

    private Vector3 direction;
    public virtual Vector3 Direction { get => direction; set => direction = value; }

    [ReadOnly][SerializeField] private float powerLevel;
    public void SetPowerLevel(float powerLevel) { this.powerLevel = powerLevel; }

    // Statuses
    [Header("Statuses")]
    public List<Status> triggerStatuses;
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
    public bool castOnTrigger = true;
    public bool castOnRelease = false;
    public bool unCastOnRelease = false;
    public UnityEvent<Vector3> onCast;
    public UnityEvent onTrigger;
    public UnityEvent onRelease;
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
    

    // Triggering

    public virtual void Trigger()
    {
        foreach (Status status in triggerStatuses)
        {
            status.effect.Apply(source, status.strength);
        }
        onTrigger.Invoke();
        if (castOnTrigger) Cast();
    }

    public virtual void Release()
    {
        foreach (Status status in triggerStatuses)
        {
            status.effect.Remove(source);
        }
        onRelease.Invoke();
        if (castOnRelease) Cast();
        if (unCastOnRelease) UnCast();
    }

    // Casting

    public virtual bool CanCast() { return !casting; }

    public virtual void Cast()
    {
        casting = true;
        if (pivot != null)
            pivot.SetRotationWithVector(direction.XZVector());
        foreach (Status status in castStatuses)
        {
            status.effect.Apply(source, status.strength);
        }
        onCast.Invoke(direction);
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
        ReportOriginAmong(onTrigger, effectParent);
        ReportOriginAmong(onCast, effectParent);
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
        ReportExceptionsAmong(onTrigger, exceptions);
        ReportExceptionsAmong(onCast, exceptions);
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