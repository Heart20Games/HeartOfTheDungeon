using UnityEngine;

public interface IPositionable
{
    public Vector3 OriginPosition { get; }
    public void SetOrigin(Transform source, Transform target);
    public void SetOffset(Vector3 offset, float rOffset);
}
