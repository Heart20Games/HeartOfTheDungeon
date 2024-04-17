using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class DelegatedSingleExecutor : CastStateExecutor
    {
        [Serializable]
        public struct ActionEvent
        {
            public string name;
            public CastAction action;
            public bool waitForPerformance;
            public UnityEvent startAction;
        }

        public List<ActionEvent> supportedActions;

        private UnityAction<StateAction> actionPerformed;
        private StateAction stateAction;

        public override bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed)
        {
            if (actionPerformed == null && SupportsAction(stateAction.action))
            {
                actionPerformed = onActionPerformed;
                this.stateAction = stateAction;
                return StartAction(stateAction.action);
            }
            else
            {
                return false;
            }
        }

        private bool StartAction(CastAction action)
        {
            foreach (ActionEvent actionEvent in supportedActions)
            {
                if (actionEvent.action == action)
                {
                    actionEvent.startAction.Invoke();
                    return actionEvent.waitForPerformance;
                }
            }
            return false;
        }

        private bool SupportsAction(CastAction action)
        {
            foreach (ActionEvent actionEvent in supportedActions)
            {
                if (actionEvent.action == action)
                {
                    return true;
                }
            }
            return false;
        }

        [ButtonMethod]
        public void FinishAction()
        {
            actionPerformed.Invoke(stateAction);
        }
    }
}