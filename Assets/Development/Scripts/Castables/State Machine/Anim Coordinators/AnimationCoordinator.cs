using MyBox;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AnimationCoordinator : BaseMonoBehaviour
{
    // Warnings
    protected void InvalidParameterWarning(string kind, string property, bool debug=false)
    {
        if (debug) Debug.LogWarning($"No valid {kind} parameter named {property} found.", this);
    }

    // Parameter Helpers

    public abstract float GetFloat(string property);
    public abstract int GetInt(string property);
    public abstract bool GetBool(string property);

    public abstract void SetFloat(string property, float value);
    public abstract void SetInt(string property, int value);
    public abstract void SetBool(string property, bool value);
    public abstract void SetTrigger(string property);
}
