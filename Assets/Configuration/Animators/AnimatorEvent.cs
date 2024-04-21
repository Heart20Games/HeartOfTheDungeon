using MyBox;
using Spine;
using System;
using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    [Flags] public enum Event { EnterState, ExitState }
    [Flags] public enum Target { None = 0, Owner = 1 << 0, Parent = 1 << 2 }

    public string methodName;
    public string eventName;
    public Event eventType;
    public Target targets = Target.Owner;

    private bool HasFlag<T>(T flag, T flags) where T : Enum
    {
        return ((int)(object)flags & (int)(object)flag) == (int)(object)flag;
    }

    private void SendMessage(Animator animator)
    {
        if (HasFlag(Target.Owner, targets))
        {
            animator.gameObject.SendMessage(methodName, eventName);
        }
        if (HasFlag(Target.Parent, targets))
        {
            animator.gameObject.SendMessageUpwards(methodName, eventName);
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HasFlag(Event.EnterState, eventType))
        {
            SendMessage(animator);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HasFlag(Event.ExitState, eventType))
        {
            SendMessage(animator);
        }
    }
}
