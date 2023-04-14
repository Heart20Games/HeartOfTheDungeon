using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionable
{
    public void SetOrigin(Transform source, Transform target);
    public void SetOffset(Vector3 offset, float rOffset);
}
