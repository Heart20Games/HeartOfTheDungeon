using UnityEngine;

public static class ExtensionMethods
{
    public static bool HasParameter(this Animator _animator, string parameterName)
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.name == parameterName) return true;
        }
        return false;
    }

    public static void SetRotationWithVector(this Transform _transform, Vector2 vector)
    {
        Vector3 direction = Vector3.forward;
        if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f)
        {
            direction = Vector3.right * -vector.x + Vector3.forward * -vector.y;
        }
        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
            _transform.rotation = newRotation;
        }
    }
}
