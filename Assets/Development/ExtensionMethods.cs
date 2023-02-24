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
}
