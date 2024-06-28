using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static CastCoordinator;

namespace HotD.Castables
{
    public class DelegatedExecutor : CastStateExecutor
    {
        [SerializeField] protected bool debugExecutor = false;
        public List<ActionEvent> supportedActions = new();
        public List<StateAction> actionsToPerform = new();

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (Coordinator)
                Coordinator.RegisterActionListener(FinishAction, active);
            if (!active)
            {
                actionsToPerform.Clear();
                ResetSupportedActions();
            }
        }

        public void ResetSupportedActions()
        {
            for (int i = 0; i < supportedActions.Count; i++)
            {
                supportedActions[i] = supportedActions[i].MarkFired(false);
            }
        }

        // The core of the executor. Tries to do the thing, then reports back whether it needs to wait to do the thing.
        public override bool PerformAction(StateAction stateAction, out CastAction waitOn)
        {
            waitOn = CastAction.None;
            if (SupportsAction(stateAction.action))
            {
                Print($"Found supported action {stateAction.Name()}.", debugExecutor, this);
                actionsToPerform.Add(stateAction);
                waitOn = StartAction(stateAction.action);
                actionsToPerform.Remove(stateAction);
                if (waitOn != CastAction.None)
                {
                    actionsToPerform.Add(new(stateAction.state, waitOn));
                    return true;
                }
            }
            else
            {
                Print($"Discarding unsupported action {stateAction.Name()}.", debugExecutor, this);
            }
            return false;
        }

        private bool HasAction(CastAction actions, CastAction action)
        {
            return (actions & action) == action;
        }

        // Returns a StateAction to wait on.
        private CastAction StartAction(CastAction action)
        {
            Assert.IsFalse(action == CastAction.None);
            for (int i = 0; i < supportedActions.Count; i++)
            {
                ActionEvent actionEvent = supportedActions[i];
                
                if (HasAction(actionEvent.triggerAction, action))
                {
                    if (actionEvent.CanFire())
                    {
                        Print($"Firing action event {actionEvent.name}.", debugExecutor, this);
                        supportedActions[i] = actionEvent.MarkFired(true);
                        actionEvent.startAction.Invoke();
                        Coordinator.Coordinate(actionEvent.sendToCoordinator);
                        return !actionEvent.waitForPerformance ? CastAction.None : actionEvent.waitAction;
                    }
                    else
                    {
                        Print($"Discarding one-shot {actionEvent.name}.", debugExecutor, this);
                    }
                }
            }
            return CastAction.None;
        }

        private bool SupportsAction(CastAction action)
        {
            foreach (ActionEvent actionEvent in supportedActions)
            {
                if (HasAction(actionEvent.triggerAction, action))
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
            Print($"Finish Cast Action: {action}", debugExecutor, this);
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

        [Serializable]
        public struct ActionEvent
        {
            public ActionEvent(string name, CastAction triggerAction, Triggers sendToCoordinator, bool waitForPerformance, CastAction waitAction = CastAction.None, bool oneShot = true)
            {
                this.name = name;
                this.triggerAction = triggerAction;
                this.sendToCoordinator = sendToCoordinator;
                this.waitForPerformance = waitForPerformance;
                this.waitAction = waitAction;
                this.oneShot = oneShot;
                this.fired = false;
                this.startAction = new();
            }

            public ActionEvent(ActionEvent old, bool fired)
            {
                this.name = old.name;
                this.triggerAction = old.triggerAction;
                this.sendToCoordinator = old.sendToCoordinator;
                this.waitForPerformance = old.waitForPerformance;
                this.waitAction = old.waitAction;
                this.oneShot = old.oneShot;
                this.fired = fired;
                this.startAction = old.startAction;
            }

            public readonly bool CanFire()
            {
                return (!oneShot) || (!fired);
            }

            public readonly ActionEvent MarkFired(bool fired)
            {
                return this.fired == fired ? this : new ActionEvent(this, fired);
            }

            public string name;
            public CastAction triggerAction;
            public Triggers sendToCoordinator;
            public bool waitForPerformance;
            [ConditionalField("waitForPerformance")]
            public CastAction waitAction;
            public bool oneShot;
            [ReadOnly][SerializeField] public readonly bool fired;
            public UnityEvent startAction;
        }
    }
}