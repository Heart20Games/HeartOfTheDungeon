using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimCoordinator : AnimationCoordinator
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Warnings
    protected bool HasValidParameter(string kind, string parameter)
    {
        if (animator.HasParameter(parameter))
        {
            return true;
        }
        else
        {
            InvalidParameterWarning(kind, parameter);
            return false;
        }
    }

    // Parameter Helpers

    public override void SetFloat(string parameter, float value)
    {
        if (HasValidParameter("Float", parameter))
            animator.SetFloat(parameter, value);
    }

    public override void SetInt(string parameter, int value)
    {
        if (HasValidParameter("Int", parameter))
            animator.SetInteger(parameter, value);
    }

    public override void SetBool(string parameter, bool value)
    {
        if (HasValidParameter("Bool", parameter))
            animator.SetBool(parameter, value);
    }

    public override void SetTrigger(string parameter)
    {
        if (HasValidParameter("Trigger", parameter))
        animator.SetTrigger(parameter);
    }
}
