using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positionable : BaseMonoBehaviour, IPositionable
{
    public Transform source;
    public Transform target;
    public Vector3 offset=new();
    public float rOffset=0;

    public virtual void SetOrigin(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
    }

    public virtual void SetOffset(Vector3 offset=new(), float rOffset=0)
    {
        this.offset = offset;
        this.rOffset = rOffset;
    }

    public virtual void MoveToOrigin()
    {
        MoveToOrigin(null);
    }

    public virtual void MoveToOrigin(Transform toMove)
    {
        toMove = toMove != null ? toMove : transform;
        toMove.SetParent(source);
        toMove.position = target.position;
    }
}
