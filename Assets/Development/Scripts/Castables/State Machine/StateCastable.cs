using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotD.Castables
{
    public enum CastState { None, Init, Equipped, Activating, Executing }

    [Serializable]
    public struct StateTransition
    {
        public StateTransition(CastState source, CastAction actions, CastState destination)
        {
            this.source = source;
            this.triggerActions = actions;
            this.destination = destination;
        }
        public CastState source;
        public CastAction triggerActions;
        public CastState destination;
    }

    [Serializable]
    public struct StateAction : IEquatable<StateAction>
    {
        public StateAction(CastState state, CastAction action)
        {
            this.state = state;
            this.action = action;
        }
        public CastState state;
        public CastAction action;

        public readonly bool Equals(StateAction other)
        {
            return state == other.state && action == other.action;
        }
    }

    [RequireComponent(typeof(Damager))]
    public class StateCastable : CastableProperties, ICastable
    {
        // State
        [Foldout("State", true)]
        public CastState state;
        [ReadOnly] public int executorCount;
        public List<StateTransition> transitions = new();
        public List<StateAction> queuedActions = new();
        public List<StateAction> dequeuedActions = new();
        public Dictionary<StateAction, StateTransition> transitionBank = new();
        public List<ICastStateExecutor> executorList = new();
        [Foldout("State")] public Dictionary<CastState, List<ICastStateExecutor>> executorBank = new();

        public bool CanCast { get => state == CastState.Equipped; }

        public bool debug;

        public void Awake()
        {
            foreach (CastState state in Enum.GetValues(typeof(CastState)))
            {
                executorBank.Add(state, new());
            }
            executorCount = 0;
            foreach (Transform child in transform)
            {
                var executor = child.GetComponent<CastStateExecutor>();
                if (executor)
                {
                    executorList.Add(executor);
                    executorBank[executor.State].Add(executor);
                    executorCount++;
                    executor.ActionPerformer = ActionPerformed;
                    executor.ActionReporter = QueueAction;
                    executor.SetActive(executor.State == CastState.None);
                }
            }
            transitionBank.Clear();
            foreach (var transition in transitions)
            {
                foreach (CastAction value in Enum.GetValues(typeof(CastAction)))
                {
                    if (value != CastAction.None && HasAction(value, transition.triggerActions))
                    {
                        Print($"Added to transition bank: ({transition.source} / {value}) : {transition}", debug, this);
                        transitionBank.Add(new(transition.source, value), transition);
                    }
                }
            }
            SetState(CastState.None);
        }

        public override void Initialize(ICastCompatible owner, CastableItem item, int actionIndex = 0)
        {
            base.Initialize(owner, item, actionIndex);
            foreach (var executor in executorList)
            {
                executor.Initialize(fields);
            }
            SetState(CastState.Init);
        }

        // Helpers

        private bool HasAction(CastAction action, CastAction actions)
        {
            return (action & actions) == action;
        }

        // State

        public void SetState(CastState state)
        {
            foreach (var keyState in executorBank.Keys)
            {
                foreach (var executor in executorBank[keyState])
                {
                    Print($"SetActive({executor.State == state}) on {keyState} executor. (Seeking {state})", debug);
                    executor.SetActive(executor.State == state);
                    if (executor.State == state)
                    {
                        executor.PerformAction(new(state, CastAction.Start));
                        QueueAction(CastAction.End, false, state);
                    }
                    else if (executor.State == this.state)
                    {
                        executor.PerformAction(new(this.state, CastAction.End));
                    }
                }
            }
            queuedActions.RemoveAll(
                (StateAction stateAction) => { return stateAction.state != state; }
            );
            this.state = state;
        }

        // Action Queue

        public void QueueAction(CastAction action)
        {
            QueueAction(action, true, CastState.None);
        }
        public void QueueAction(CastAction action, bool transitionIfNotWaiting = true, CastState state = CastState.None)
        {
            state = state == CastState.None ? this.state : state;
            StateAction stateAction = new(state, action);
            if (transitionBank.TryGetValue(stateAction, out StateTransition transition))
            {
                bool waitingOnExecutor = false;
                
                Print($"Attempting {transition.triggerActions} transition from {transition.source} to {transition.destination}.", debug);
                var executors = executorBank[state];
                if (executors.Count > 0)
                {
                    foreach (var executor in executors)
                    {
                        if (executor.PerformAction(stateAction))
                        {
                            Print($"Executor performing {stateAction.action} on {stateAction.state}.", debug);
                            queuedActions.Add(stateAction);
                            waitingOnExecutor = true;
                        }
                    }
                    DequeueFirst();
                }

                if (!waitingOnExecutor && transitionIfNotWaiting)
                {
                    Print($"No executors to wait on, setting state to {transition.destination}.", debug);
                    SetState(transition.destination);
                }
            }
        }

        public void DequeueFirst()
        {
            if (queuedActions.Count > 0)
            {
                var nextAction = queuedActions.First();
                if (dequeuedActions.Remove(nextAction))
                {
                    ActionPerformed(nextAction);
                }
            }
        }

        public void ActionPerformed(StateAction stateAction)
        {
            if (queuedActions.Count > 0)
            {
                if (queuedActions.First().Equals(stateAction))
                {
                    queuedActions.RemoveAt(0);
                    if (transitionBank.TryGetValue(stateAction, out StateTransition transition))
                    {
                        SetState(transition.destination);
                    }
                    DequeueFirst();
                }
                else
                {
                    dequeuedActions.Add(stateAction);
                }
            }
        }

        // Transition Set Up

        private bool FindTransitionBase(StateTransition transition, out int idx)
        {
            for (idx = 0; idx < transitions.Count; idx++)
            {
                var trans = transitions[idx];
                if (trans.source == transition.source && trans.destination == transition.destination)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddTransition(StateTransition transition)
        {
            if (FindTransitionBase(transition, out int index))
            {
                CastAction actions = transition.triggerActions | transitions[index].triggerActions;
                transitions[index] = new(transition.source, actions, transition.destination);
            }
            else
            {
                transitions.Add(transition);
            }
        }

        [ButtonMethod]
        public void AddBaseTransitions()
        {
            AddTransition(new(CastState.Init, CastAction.Equip, CastState.Equipped));
            AddTransition(new(CastState.Equipped, CastAction.UnEquip, CastState.Init));
        }

        [ButtonMethod]
        public void AddChargeTransitions()
        {
            AddBaseTransitions();
            AddTransition(new(CastState.Equipped, CastAction.Trigger, CastState.Activating));
            AddTransition(new(CastState.Activating, CastAction.Release | CastAction.End, CastState.Executing));
            AddTransition(new(CastState.Executing, CastAction.End, CastState.Equipped));
        }

        [ButtonMethod]
        public void AddComboTransitions()
        {
            AddBaseTransitions();
            AddTransition(new(CastState.Equipped, CastAction.Trigger, CastState.Activating));
            AddTransition(new(CastState.Activating, CastAction.End, CastState.Executing));
            AddTransition(new(CastState.Executing, CastAction.End, CastState.Equipped));
        }
    }
}