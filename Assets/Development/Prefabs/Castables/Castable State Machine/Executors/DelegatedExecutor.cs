using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class DelegatedExecutor : CastStateExecutor
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

        public List<StateAction> actionsToPerform;

        [Header("Tests")]
        public StateAction testAction;

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (Coordinator)
                Coordinator.RegisterActionListener(FinishAction, active);
        }

        public override bool PerformAction(StateAction stateAction)
        {
            if (SupportsAction(stateAction.action))
            {
                actionsToPerform.Add(stateAction);
                bool wait = StartAction(stateAction.action);
                if (!wait)
                    actionsToPerform.Remove(stateAction);
                return wait;
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
        public void TestFinishAction()
        {
            if (actionsToPerform.Count > 0)
            {
                FinishAction(testAction);
            }
        }

        public void FinishAction(StateAction stateAction)
        {
            if (actionsToPerform.Contains(stateAction))
            {
                actionsToPerform.Remove(stateAction);
                actionPerformed.Invoke(stateAction);
            }
        }

        public void FinishAction(CastAction action)
        {
            foreach (StateAction stateAction in actionsToPerform)
            {
                if (stateAction.action == action)
                {
                    FinishAction(stateAction);
                }
            }
        }

        public void ReportAction(CastAction action)
        {
            actionReported.Invoke(action);
        }

        public void End()
        {
            ReportAction(CastAction.End);
        }

        [ButtonMethod]
        public void TestReportAction()
        {
            ReportAction(testAction.action);
        }
    }
}