using Body;
using HotD.Body;
using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

namespace HotD.Castables
{
    public enum CastState { None, Init, Equipped, Activating, Executing }
    public enum CastAction { None, Equip, Start, Trigger, Release, End, UnEquip }

    [Serializable]
    public struct StateTransition
    {
        public CastState source;
        public CastAction triggerAction;
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

        public bool Equals(StateAction other)
        {
            return state == other.state && action == other.action;
        }
    }

    [RequireComponent(typeof(Damager))]
    public class CastableStateMachine : CastableProperties
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
            foreach (var transition in transitions)
            {
                transitionBank.Add(new(transition.source, transition.triggerAction), transition);
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
            StateAction stateAction = new(state, action);
            if (transitionBank.TryGetValue(stateAction, out StateTransition transition))
            {
                bool waitingOnExecutor = false;
                
                Print($"Performing {transition.triggerAction} transition from {transition.source} to {transition.destination}.", debug);
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

                if (!waitingOnExecutor)
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

        // Tests
        [Header("Tests")]
        public CastAction testAction;
        public CastState testState;
        public TestCastCompatible testCastCompatible;
        public CastableItem testItem;

        [ButtonMethod]
        public void TestQueueAction()
        {
            QueueAction(testAction);
        }
        [ButtonMethod]
        public void TestSetState()
        {
            SetState(testState);
        }
        [ButtonMethod]
        public void TestInitialize()
        {
            Initialize(testCastCompatible, testItem);
        }
    }
}