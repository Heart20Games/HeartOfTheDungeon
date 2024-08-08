using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    using static Coordination;

    public enum CastState { None, Init, Equipped, Activating, Executing, Cooldown }

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
            this.name = $"[{state} -> {action}]";
            this.state = state;
            this.action = action;
        }
        
        public string Name()
        {
            return $"[{state} -> {action}]";
        }

        public string name;
        public CastState state;
        public CastAction action;

        public readonly bool Equals(StateAction other)
        {
            return state == other.state && action == other.action;
        }
    }

    [RequireComponent(typeof(Damager))]
    public class StateCastable : CastProperties, ICastable
    {
        // State
        [Foldout("State", true)]
        public CastState state;
        [Tooltip("Executors are found among child objects on Awake.")]
        [ReadOnly][SerializeField] private int executorCount;
        public List<StateTransition> transitions = new();
        public List<StateAction> queuedActions = new();
        public List<StateAction> dequeuedActions = new();
        public Dictionary<StateAction, StateTransition> transitionBank = new();
        public List<ICastStateExecutor> executorList = new();
        [Foldout("State")] public Dictionary<CastState, List<ICastStateExecutor>> executorBank = new();
        [SerializeField] protected bool debugCastable = false;

        public bool CanCast { get => state == CastState.Equipped; }

        public void Awake()
        {
            foreach (CastState state in Enum.GetValues(typeof(CastState)))
            {
                executorBank.Add(state, new());
            }
            // Finds Executors Among Children At Runtime
            executorCount = 0;
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<CastStateExecutor>(out var executor))
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
                        Print($"Added to transition bank: ({transition.source} / {value}) : {transition}", debugCastable, this);
                        transitionBank.Add(new(transition.source, value), transition);
                    }
                }
            }
            SetState(CastState.None);
        }

        public override void Initialize(ICastCompatible owner, CastableItem item)
        {
            base.Initialize(owner, item);
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
            // End the ones we don't want.
            foreach (var keyState in executorBank.Keys)
            {
                foreach (var executor in executorBank[keyState])
                {
                    if (executor.State != state)
                    {
                        Print($"- Remove {keyState} executor. (Seeking {state})", debugCastable);

                        // End it first
                        if (executor.State == this.state)
                        {
                            PerformAndWait(executor, new(this.state, CastAction.End));
                        }

                        // Then deactivate
                        executor.SetActive(false);
                    }
                }
            }

            // Now Activate the one(s) we want.
            foreach (var keyState in executorBank.Keys)
            {
                foreach (var executor in executorBank[keyState])
                {
                    if (executor.State == state)
                    {
                        Print($"+ Add {keyState} executor. (Seeking {state})", debugCastable);

                        // Activate first
                        executor.SetActive(true);

                        // Then start
                        PerformAndWait(executor, new(state, CastAction.Start));
                        //QueueAction(CastAction.End, false, state);
                    }
                }
            }

            // Remove actions queued for the state we just entered (give us a fresh start).
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
            
            // Find the first relevant transition found in the transition bank.
            if (transitionBank.TryGetValue(stateAction, out StateTransition transition))
            {
                bool waitingOnExecutor = false;

                // Attempt that transition.
                Print($"? Attempting {transition.triggerActions} transition from {transition.source} to {transition.destination}.", debugCastable);
                var executors = executorBank[state];
                if (executors.Count > 0)
                {
                    // Perform the transition on each executor in the given state.
                    foreach (var executor in executors)
                    {
                        waitingOnExecutor = PerformAndWait(executor, stateAction);
                    }
                    DequeueFirst();
                }

                // We're all done here if we're not waiting on anything.
                if (!waitingOnExecutor && transitionIfNotWaiting)
                {
                    Print($"No executors to wait on, setting state to {transition.destination}.", debugCastable);
                    SetState(transition.destination);
                }
            }
        }

        // Perform the state transition, add any actions it needs to wait for onto the queue. Returns whether we're waiting for something.
        private bool PerformAndWait(ICastStateExecutor executor, StateAction stateAction)
        {
            if (executor.PerformAction(stateAction, out CastAction waitOn))
            {
                StateAction waitStateAction = new(stateAction.state, waitOn);
                Print($"... Executor performing {waitStateAction.action} on {waitStateAction.state}.", debugCastable);
                queuedActions.Add(waitStateAction);
                return true;
            }
            return false;
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

        private void RemoveTransition(StateTransition transition)
        {
            if (FindTransitionBase(transition, out int index))
            {
                if (transition.triggerActions == transitions[index].triggerActions)
                {
                    transitions.RemoveAt(index);
                }
                else
                {
                    // ( toRemove XOR alreadyHave ) AND alreadyHave -- result includes only bits not in the removed transition actions.
                    CastAction actions = (transition.triggerActions ^ transitions[index].triggerActions) & transitions[index].triggerActions;
                    transitions[index] = new(transitions[index].source, actions, transitions[index].destination);
                }
            }
        }

        // Quick Set-up Methods

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

        [ButtonMethod]
        public void AddInstantCastOnTriggerTransitions()
        {
            AddBaseTransitions();
            AddTransition(new(CastState.Equipped, CastAction.Trigger, CastState.Executing));
            AddTransition(new(CastState.Executing, CastAction.Release | CastAction.End, CastState.Equipped));
        }

        [ButtonMethod]
        public void AddCooldownTransitions()
        {
            RemoveTransition(new(CastState.Executing, CastAction.End, CastState.Equipped));
            AddTransition(new(CastState.Executing, CastAction.End, CastState.Cooldown));
            AddTransition(new(CastState.Cooldown, CastAction.End, CastState.Equipped));
        }

        [ButtonMethod]
        public void CreateCastExecutor()
        {
            GameObject parent = new("Cast");
            parent.transform.SetParent(gameObject.transform);
            var executor = parent.AddComponent<DelegatedExecutor>();

            executor.State = CastState.Executing;
            
            executor.supportedTransitions.Add(new(
                "Cast on Start", CastAction.Start,
                Triggers.StartCast, Triggers.None
            ));
            executor.supportedTransitions.Add(new(
                "Finish", CastAction.End, 
                Triggers.None, Triggers.None, CastAction.End
            ));
        }

        [ButtonMethod]
        public void CreateChargeThenCastExecutors()
        {
            GameObject parent = new("Charge");
            parent.transform.SetParent(gameObject.transform);
            var executor = parent.AddComponent<DelegatedExecutor>();

            executor.State = CastState.Activating;

            executor.supportedTransitions.Add(new(
                "Charge on Start", CastAction.Start,
                Triggers.StartAction, Triggers.None
            ));
            executor.supportedTransitions.Add(new(
                "Cast on Release", CastAction.Release,
                Triggers.None, Triggers.None, CastAction.End
            ));

            var charger = parent.AddComponent<Charger>();
            
            charger.resetOnBegin = true;

            charger.onCharged = new();
            UnityEvent onCharged = charger.onCharged;
            UnityEventTools.AddPersistentListener(onCharged, executor.End);

            UnityEvent startAction = executor.supportedTransitions[0].startAction;
            UnityEventTools.AddPersistentListener(startAction, charger.Begin);

            CreateCastExecutor();
        }
    }
}