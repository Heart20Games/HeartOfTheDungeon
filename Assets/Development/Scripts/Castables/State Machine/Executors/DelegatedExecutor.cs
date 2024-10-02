using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HotD.Castables
{
    using static Coordination;

    public class DelegatedExecutor : CastStateExecutor
    {

        [SerializeField] protected bool debugExecutor = false;
        [FormerlySerializedAs("supportedActions")] public List<TransitionEvent> supportedTransitions = new();
        public List<StateAction> actionsToPerform = new();

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (Coordinator)
            {
                Print($"Found coordinator; {(active ? "" : "de")}register FinishAction. ({Name})", debugExecutor, this);
                Coordinator.RegisterActionListener(FinishAction, active);
            }
            else
            {
                Print($"Cannot access coordinator. ({Name})", debugExecutor, this);
            }
            if (!active)
            {
                actionsToPerform.Clear();
                ResetSupportedActions();
            }
        }

        public void ResetSupportedActions()
        {
            for (int i = 0; i < supportedTransitions.Count; i++)
            {
                supportedTransitions[i] = supportedTransitions[i].MarkFired(false);
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

        // Executes any events associated with the given CastAction. Returns a StateAction to wait on.
        private CastAction StartAction(CastAction action)
        {
            Assert.IsFalse(action == CastAction.None);
            for (int i = 0; i < supportedTransitions.Count; i++)
            {
                TransitionEvent transition = supportedTransitions[i];
                
                if (HasAction(transition.triggerAction, action))
                {
                    // Execute the event.
                    var waitOn = ExecuteEvent(transition, out var eventStatus);
                    supportedTransitions[i] = eventStatus;
                    return waitOn;
                }
            }
            return CastAction.None;
        }

        // Execute the given action event, and track whether the event has been fired. Returns the CastAction to wait for.
        private CastAction ExecuteEvent(TransitionEvent transition, out TransitionEvent eventStatus)
        {
            FinishAction(transition.triggerAction, false);
            if (transition.CanFire())
            {
                Print($"Firing action event {transition.name}.", debugExecutor, this);
                eventStatus = transition.MarkFired(true);
                transition.startAction.Invoke();
                Coordinator.Coordinate(transition.sendToCoordinator);
                actionExecuted?.Invoke(transition.sendToListener);
                return transition.waitAction;
            }
            else
            {
                Print($"Discarding one-shot {transition.name}.", debugExecutor, this);
                eventStatus = transition;
                return CastAction.None;
            }
        }

        private bool SupportsAction(CastAction action)
        {
            foreach (TransitionEvent actionEvent in supportedTransitions)
            {
                if (HasAction(actionEvent.triggerAction, action))
                {
                    return true;
                }
            }
            return false;
        }

        // Finishes the given action, if it's being waited on.
        public void FinishAction(StateAction stateAction, bool report = true)
        {
            if (actionsToPerform.Contains(stateAction))
            {
                actionsToPerform.Remove(stateAction);
                if (report)
                    actionPerformed.Invoke(stateAction);
            }
        }

        // Finishes the given action, if it's being waited on (finds the action from the actionsToPerform list).
        public void FinishAction(CastAction action)
        {
            FinishAction(action, true);
        }
        private void FinishAction(CastAction action, bool report)
        {
            Print($"Finish Cast Action: {action}", debugExecutor, this);
            Break(debugExecutor, this);
            
            StateAction[] actions = actionsToPerform.ToArray();
            foreach (StateAction stateAction in actions)
            {
                if (stateAction.action == action)
                {
                    FinishAction(stateAction, report);
                    break;
                }
            }
        }

        // Reporting
        [ButtonMethod]
        public void End()
        {
            ReportAction(CastAction.End);
        }

        [ButtonMethod]
        public void Continue()
        {
            ReportAction(CastAction.Continue);
            //FinishAction(CastAction.Continue);
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

        // Structs

        [Serializable]
        public struct TransitionEvent
        {
            // Fields
            public string name;
            public CastAction triggerAction;
            public Triggers sendToCoordinator;
            public Triggers sendToListener;
            public CastAction waitAction;
            public bool oneShot;
            [ReadOnly][SerializeField] public readonly bool fired;
            public UnityEvent startAction;

            // Construction
            public TransitionEvent(string name, CastAction triggerAction = CastAction.None, Triggers sendToCoordinator = Triggers.None, Triggers sendToListener = Triggers.None, CastAction waitAction = CastAction.None, bool oneShot = true)
            {
                this.name = name;
                this.triggerAction = triggerAction;
                this.sendToCoordinator = sendToCoordinator;
                this.sendToListener = sendToListener;
                this.waitAction = waitAction;
                this.oneShot = oneShot;
                this.fired = false;
                this.startAction = new();
            }

            public TransitionEvent(TransitionEvent old, bool fired)
            {
                this.name = old.name;
                this.triggerAction = old.triggerAction;
                this.sendToCoordinator = old.sendToCoordinator;
                this.sendToListener = old.sendToListener;
                this.waitAction = old.waitAction;
                this.oneShot = old.oneShot;
                this.fired = fired;
                this.startAction = old.startAction;
            }

            // Action Search
            public static bool TryFindActionEvent(CastAction action, List<TransitionEvent> supportedActions, out TransitionEvent actionEvent, TransitionEvent fallback = new())
            {
                actionEvent = fallback;
                foreach (var supportedAction in supportedActions)
                {
                    if (supportedAction.triggerAction == action)
                    {
                        actionEvent = supportedAction;
                        return true;
                    }
                }
                return false;
            }

            // Fired?
            public readonly bool CanFire()
            {
                return (!oneShot) || (!fired);
            }

            public readonly TransitionEvent MarkFired(bool fired)
            {
                return this.fired == fired ? this : new TransitionEvent(this, fired);
            }
        }
    }
}