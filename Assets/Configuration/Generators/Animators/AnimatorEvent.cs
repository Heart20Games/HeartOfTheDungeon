using MyBox;
using Spine;
using System;
using System.Collections;
using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    [Flags] public enum Event { EnterState, ExitState }
    [Flags] public enum Target { None = 0, Owner = 1 << 0, Parent = 1 << 2 }

    public string methodName;
    public string eventName;
    public Event eventType;
    public Target targets = Target.Owner;
    [Range(0, 10)] public float waitForTime = 0;

    private Coroutine waitCoroutine;

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

    private void WaitAndSendMessage(Animator animator)
    {
        if (waitForTime == 0)
        {
            SendMessage(animator);
        }
        else
        {
            // May fail to behave as expected should the arbitrary MonoBehaviour not be enabled, or if the object is set inactive.
            waitCoroutine ??= animator.GetComponent<MonoBehaviour>().StartCoroutine(WaitForTime(waitForTime, animator));
        }
    }

    private IEnumerator WaitForTime(float time, Animator animator)
    {
        yield return new WaitForSeconds(time);
        if (waitCoroutine != null)
        {
            SendMessage(animator);
        }
        waitCoroutine = null;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HasFlag(Event.EnterState, eventType))
        {
            WaitAndSendMessage(animator);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HasFlag(Event.ExitState, eventType))
        {
            WaitAndSendMessage(animator);
        }
    }
}
