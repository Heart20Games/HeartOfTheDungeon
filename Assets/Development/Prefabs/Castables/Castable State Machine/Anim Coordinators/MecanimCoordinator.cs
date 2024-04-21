using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MecanimCoordinator : AnimationCoordinator
{
    [ReadOnly][SerializeField] protected Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Print("Animator not found.", animator == null, this);
    }

    // Warnings
    protected bool HasValidParameter(string kind, string parameter)
    {
        Assert.IsNotNull(animator);

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

    public override float GetFloat(string parameter)
    {
        if (HasValidParameter("Float", parameter))
        {
            return animator.GetFloat(parameter);
        }
        return 0;
    }

    public override int GetInt(string parameter)
    {
        if (HasValidParameter("Int", parameter))
            animator.GetInteger(parameter);
        return 0;
    }

    public override bool GetBool(string parameter)
    {
        if (HasValidParameter("Bool", parameter))
            return animator.GetBool(parameter);
        return false;
    }

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
