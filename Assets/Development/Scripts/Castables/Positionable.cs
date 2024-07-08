using MyBox;
using UnityEngine;

public class Positionable : BaseMonoBehaviour, IPositionable
{
    //[Header("Positionable")]
    [Foldout("Positionable", true)]
    public bool applyOnSet = true;
    [SerializeField] protected Transform source;
    [SerializeField] protected Transform location;
    [SerializeField] protected Transform target;
    [SerializeField] protected Vector3 locationOverride;
    [SerializeField] protected Vector3 targetOverride;
    [SerializeField] protected Vector3 offset=new();
    [SerializeField] protected bool useLocationOverride = false;
    [SerializeField] protected bool useTargetOverride = false;
    [Foldout("Positionable")]
    [SerializeField] protected float rOffset=0;

    public Vector3 OriginPosition
    {
        get
        {
            return (useLocationOverride ? locationOverride : 
                (location ? location.position : 
                    (source ? source.position : transform.position))
            );
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return (useTargetOverride ? targetOverride :
                (target ? target.position : OriginPosition)
            );
        }
    }

    public virtual void SetOrigin(Transform source, Transform location)
    {
        this.source = source;
        this.location = location;
        if (applyOnSet) Apply();
    }

    public virtual void SetOrigin(Transform source, Vector3 locationOverride)
    {
        this.source = source;
        this.locationOverride = locationOverride;
        this.useLocationOverride = true;
        if (applyOnSet) Apply();
    }

    public virtual void SetTarget(Transform target)
    {
        this.target = target;
    }

    public virtual void SetTarget(Vector3 targetOverride)
    {
        this.targetOverride = targetOverride;
        this.useTargetOverride = true;
    }

    public virtual void SetOffset(Vector3 offset=new(), float rOffset=0)
    {
        this.offset = offset;
        this.rOffset = rOffset;
        if (applyOnSet) Apply();
    }

    public virtual void Apply()
    {
        ApplyTo(null);
    }

    public virtual void ApplyTo(Transform toMove)
    {
        toMove = toMove == null ? transform : toMove;
        toMove.SetParent(source);
        toMove.position = OriginPosition + offset;
    }

    public virtual void MoveToOrigin()
    {
        MoveToOrigin(null);
    }

    public virtual void MoveToOrigin(Transform toMove)
    {
        toMove = toMove == null ? transform : toMove;
        toMove.SetParent(source);
        toMove.position = OriginPosition;
    }
}
