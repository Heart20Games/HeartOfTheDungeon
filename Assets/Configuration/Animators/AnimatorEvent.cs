using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    public enum Event { EnterState, ExitState }

    public string methodName;
    public string eventName;
    public Event eventType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage(methodName, eventName);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage(methodName, eventName);
    }
}
