using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static CastCoordinator;

namespace HotD.Castables
{
    public class DelegatedExecutor : CastStateExecutor
    {
        [Serializable]
        public struct ActionEvent
        {
            public ActionEvent (string name, CastAction triggerAction, Triggers sendToCoordinator, bool waitForPerformance)
            {
                this.name = name;
                this.triggerAction = triggerAction;
                this.sendToCoordinator = sendToCoordinator;
                this.waitForPerformance = waitForPerformance;
                this.startAction = new();
            }

            public string name;
            public CastAction triggerAction;
            public Triggers sendToCoordinator;
            public bool waitForPerformance;
            public UnityEvent startAction;
        }

        public List<ActionEvent> supportedActions = new();

        public List<StateAction> actionsToPerform = new();

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (Coordinator)
                Coordinator.RegisterActionListener(FinishAction, active);
            if (!active)
                actionsToPerform.Clear();
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
                if (actionEvent.triggerAction == action)
                {
                    actionEvent.startAction.Invoke();
                    Coordinator.Coordinate(actionEvent.sendToCoordinator);
                    return actionEvent.waitForPerformance;
                }
            }
            return false;
        }

        private bool SupportsAction(CastAction action)
        {
            foreach (ActionEvent actionEvent in supportedActions)
            {
                if (actionEvent.triggerAction == action)
                {
                    return true;
                }
            }
            return false;
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
            Print($"Finish Cast Action: {action}", debug, this);
            StateAction[] actions = actionsToPerform.ToArray();
            foreach (StateAction stateAction in actions)
            {
                if (stateAction.action == action)
                {
                    FinishAction(stateAction);
                }
            }
        }

        // Reporting
        [ButtonMethod]
        public void End()
        {
            ReportAction(CastAction.End);
        }

        // Testing
        [Header("Tests")]
        public bool debug;
        public StateAction testAction;

        [ButtonMethod]
        public void TestFinishAction()
        {
            if (actionsToPerform.Count > 0)
            {
                FinishAction(testAction.Equals(new StateAction()) ? testAction : actionsToPerform[0]);
            }
        }

        [ButtonMethod]
        public void TestReportAction()
        {
            ReportAction(testAction.action);
        }
    }
}