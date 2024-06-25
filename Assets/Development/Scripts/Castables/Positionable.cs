using MyBox;
using UnityEngine;

public class Positionable : BaseMonoBehaviour, IPositionable
{
    //[Header("Positionable")]
    [Foldout("Positionable", true)]
    public bool applyOnSet = true;
    [SerializeField] protected Transform source;
    [SerializeField] protected Transform target;
    [SerializeField] protected Vector3 offset=new();
    [Foldout("Positionable")]
    [SerializeField] protected float rOffset=0;

    public Vector3 OriginPosition
    {
        get
        {
            return target ? target.position : (source ? source.position : transform.position);
        }
    }

    public virtual void SetOrigin(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
        if (applyOnSet) Apply();
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
        toMove.position = target.position + offset;
    }

    public virtual void MoveToOrigin()
    {
        MoveToOrigin(null);
    }

    public virtual void MoveToOrigin(Transform toMove)
    {
        toMove = toMove == null ? transform : toMove;
        toMove.SetParent(source);
        toMove.position = target.position;
    }
}
