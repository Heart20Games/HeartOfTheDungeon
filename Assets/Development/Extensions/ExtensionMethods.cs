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
    public static Vector3 Orient(this Vector3 _vector, Vector3 front)
    {
        Vector3 right = Vector3.Cross(front, Vector3.up);
        Vector3 up = Vector3.Cross(right, front);
        return front * _vector.z + right * _vector.x + up * _vector.y;
    }

    // Vector2
    public static Vector3 FullY(this Vector2 _vector)
    {
        return new(_vector.x, 0, _vector.y);
    }
    public static Vector2 Orient(this Vector2 _vector, Vector2 front)
    {
        Vector2 right = -Vector2.Perpendicular(front);
        return front * _vector.y + right * _vector.x;
    }
    public static Vector2 Orient(this Vector2 _vector, Vector3 front)
    {
        return _vector.Orient(front.normalized.XZVector());
    }

    // Transform
    public static void TrueLookAt(this Transform _transform, Vector3 target)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 right = Vector3.Cross(cameraPosition, Vector3.up);
        Vector3 cameraUp = Vector3.Cross(cameraPosition, right);
        _transform.LookAt(cameraPosition, cameraUp);
    }

    // Set Rotation With Vector
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

    // Set Local Rotation WIth Vector
    public static void SetLocalRotationWithVector(this Transform _transform, Vector2 vector, float rotationOffset = 0, float threshold = 0.5f)
    {
        SetLocalRotationWithVector(_transform, vector, Vector3.up, Vector3.right, Vector3.forward, rotationOffset, threshold);
    }
    public static void SetLocalRotationWithVector(this Transform _transform, Vector2 vector, Vector3 up, float rotationOffset = 0, float threshold = 0.5f)
    {
        SetLocalRotationWithVector(_transform, vector, up, Vector3.right, Vector3.forward, rotationOffset, threshold);
    }
    public static void SetLocalRotationWithVector(this Transform _transform, Vector2 vector, Vector3 up, Vector3 right, Vector3 forward, float rotationOffset = 0, float threshold = 0.5f)
    {
        Vector3 direction = right * vector.x + forward * vector.y;
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, up);
            _transform.localRotation = newRotation;
            _transform.Rotate(0, rotationOffset, 0);
            Vector3 euler = _transform.localEulerAngles;
            _transform.localEulerAngles = new Vector3(euler.x, euler.y + rotationOffset, euler.z);
        }
    }
}
