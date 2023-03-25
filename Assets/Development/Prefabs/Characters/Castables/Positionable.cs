using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Positionable : MonoBehaviour, IPositionable
{
    public Transform source;
    public Transform target;

    public virtual void SetOrigin(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
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
