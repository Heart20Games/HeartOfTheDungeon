using MyBox;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public interface ICastStateExecutor : ICastableProperties
    {
        public CastState State { get; set; }
        public bool PerformAction(StateAction stateAction);
        public void ReportAction(CastAction action);

        public UnityAction<StateAction> ActionPerformer { get; set; }
        public UnityAction<CastAction> ActionReporter { get; set; }
    }

    public class CastStateExecutor : CastableProperties, ICastStateExecutor
    {
        [SerializeField] protected CastState state;

        public CastState State { get => state; set => state = value; }
        public UnityAction<StateAction> ActionPerformer { get => actionPerformed; set => actionPerformed = value; }
        public UnityAction<CastAction> ActionReporter { get => actionReported; set => actionReported = value; }

        [Foldout("Events")]
        public UnityEvent<CastAction> onAction;
        protected UnityAction<StateAction> actionPerformed;
        protected UnityAction<CastAction> actionReported;

        public virtual bool PerformAction(StateAction stateAction)
        {
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