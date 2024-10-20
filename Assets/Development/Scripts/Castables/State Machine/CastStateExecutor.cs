using MyBox;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    using static Coordination;

    public interface ICastStateExecutor : ICastProperties
    {
        public CastState State { get; set; }
        public bool PerformAction(StateAction stateAction, out CastAction waitOn);
        public void ReportAction(CastAction action);

        public UnityAction<StateAction> ActionPerformer { get; set; }
        public UnityAction<CastAction> ActionReporter { get; set; }
        public UnityAction<Triggers> ToTriggerListeners { get; set; }

    }

    public class CastStateExecutor : CastProperties, ICastStateExecutor
    {
        [SerializeField] protected CastState state;

        public CastState State { get => state; set => state = value; }
        public UnityAction<StateAction> ActionPerformer { get => actionPerformed; set => actionPerformed = value; }
        public UnityAction<CastAction> ActionReporter { get => actionReported; set => actionReported = value; }
        public UnityAction<Triggers> ToTriggerListeners { get => toTriggerListeners; set => toTriggerListeners = value; }

        [Foldout("Events")]
        public UnityEvent<CastAction> onAction;
        public UnityEvent<Triggers> onTriggers;
        protected UnityAction<StateAction> actionPerformed;
        protected UnityAction<CastAction> actionReported;
        protected UnityAction<Triggers> toTriggerListeners;

        public virtual bool PerformAction(StateAction stateAction, out CastAction waitOn)
        {
            waitOn = CastAction.None;
            if (stateAction.state == state)
            {
                actionPerformed.Invoke(stateAction);
                onAction.Invoke(stateAction.action);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void ReportAction(CastAction action)
        {
            actionReported.Invoke(action);
        }
    }
}