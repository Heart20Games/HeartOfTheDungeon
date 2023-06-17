using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class ExtensionMethods
{
    // Animator
    public static bool HasParameter(this Animator _animator, string parameterName)
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.name == parameterName) return true;
        }
        return false;
    }

    // Vector3
    public static Vector2 XZVector(this Vector3 _vector)
    {
        return new(_vector.x, _vector.z);
    }

    // Transform
    public static void SetRotationWithVector(this Transform _transform, Vector2 vector, float rotationOffset=0, float threshold=0.5f)
    {
        SetRotationWithVector(_transform, vector, Vector3.up, Vector3.right, Vector3.forward, rotationOffset, threshold);
    }
    public static void SetRotationWithVector(this Transform _transform, Vector2 vector, Vector3 up, float rotationOffset=0, float threshold=0.5f)
    {
        SetRotationWithVector(_transform, vector, up, Vector3.right, Vector3.forward, rotationOffset, threshold);
    }
    public static void SetRotationWithVector(this Transform _transform, Vector2 vector, Vector3 up, Vector3 right, Vector3 forward, float rotationOffset=0, float threshold=0.5f)
    {
        Vector3 direction = right * vector.x + forward * vector.y;
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, up);
            _transform.rotation = newRotation;
            _transform.Rotate(0, rotationOffset, 0);
            Vector3 euler = _transform.eulerAngles;
            _transform.eulerAngles = new Vector3(euler.x, euler.y + rotationOffset, euler.z);
        }
    }
}
