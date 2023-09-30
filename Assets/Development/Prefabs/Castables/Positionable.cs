using Pixeye.Unity;
using UnityEngine;

public class Positionable : BaseMonoBehaviour, IPositionable
{
    //[Header("Positionable")]
    [Foldout("Positionable", true)]
    public bool applyOnSet = true;
    public Transform source;
    public Transform target;
    public Vector3 offset=new();
    public float rOffset=0;

    public virtual void SetOrigin(Transform source, Transform target)
    {
        print($"Set Origin on {name}");
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
        print($"Apply on {name} ({toMove.name} -> {source})");
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
