using Body;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;
using static HotD.Castables.CastableToLocation;
using static UnityEditor.Progress;

namespace HotD.Castables
{
    public enum CastableState { None, Init, Equipped, Activating, Executing }
    public enum CastableAction { None, Equip, Trigger, Release, End, UnEquip }

    [Serializable]
    public struct StateTransition
    {
        public CastableAction triggerAction;
        public CastableState source;
        public CastableState destination;
    }

    public struct StateAction : IEquatable<StateAction>
    {
        public StateAction(CastableState state, CastableAction action)
        {
            this.state = state;
            this.action = action;
        }
        public CastableState state;
        public CastableAction action;

        public bool Equals(StateAction other)
        {
            return state == other.state && action == other.action;
        }
    }

    [Serializable]
    public class CastableFields
    {
        [Header("Positioning")]
        public float rOffset;
        public Transform weaponArt;
        public Transform pivot;
        [ReadOnly] public Character owner;
        public Vector3 direction;
        public bool followBody;

        [Header("Settings")]
        public CastableItem item;
        public int maxPowerLevel;
        public int curPowerLevel;
        public bool casting = false;
        public bool castOnTrigger = true;
        public bool castOnRelease = false;
        public bool unCastOnRelease = false;
        public List<GameObject> castingMethods = new();

        [Header("Positionables")]
        public List<ToLocation<Positionable>> toLocations = new();
        public List<Transform> positionables;
        public List<CastedVFX> effects = new();

        [Header("Damage")]
        public Identity identity = Identity.Neutral;
        public Damager damager;

        [Header("Status Effects")]
        public List<Status> triggerStatuses;
        public List<Status> castStatuses;
        public List<Status> hitStatuses;

        [Header("Diagnostics")]
        [ReadOnly] public Vector3 pivotDirection;
    }

    [RequireComponent(typeof(Damager))]
    public class CastableStateMachine : BaseMonoBehaviour
    {
        public CastableFields fields = new();

        // Properties
        public virtual Vector3 Direction { get => fields.direction; set => fields.direction = value; }
        public Character Owner { get => fields?.owner; set => fields.owner = value; }
        public CastableItem Item { get => fields?.item; set => fields.item = value; }
        public int PowerLevel
        { 
            get => fields.curPowerLevel;
            set { fields.curPowerLevel = value; onSetPowerLevel.Invoke(value); }
        }
        public int MaxPowerLevel
        {
            get => fields.maxPowerLevel;
            set { fields.maxPowerLevel = value; onSetMaxPowerLevel.Invoke(value); }
        }
        public Identity Identity
        {
            get => fields.identity;
            set { fields.identity = value; onSetIdentity.Invoke(value); }
        }

        // Events
        [Foldout("Events", true)]
        public UnityEvent<int> onSetPowerLevel;
        public UnityEvent<int> onSetMaxPowerLevel;
        public UnityEvent<Identity> onSetIdentity;
        public UnityEvent onTrigger;
        public UnityEvent<Vector3> onCast;
        public UnityEvent onRelease;
        public UnityEvent onUnCast;
        [Foldout("Events")] public UnityEvent onCasted;

        // State
        public CastableState state;
        [ReadOnly] public List<ICastableStateExecutor> executors;
        public List<StateTransition> transitions;
        public List<StateAction> queuedActions;
        public List<StateAction> dequeuedActions;
        public Dictionary<StateAction, StateTransition> transitionBank;

        public void Awake()
        {
            fields.damager = GetComponent<Damager>();
            foreach (Transform child in transform)
            {
                var behaviours = child.GetComponents<BaseMonoBehaviour>();
                foreach (var behaviour in behaviours)
                {
                    if (behaviour is ICastableStateExecutor)
                    {
                        executors.Add(behaviour as ICastableStateExecutor);
                    }
                }
            }
            SetState(CastableState.None);
        }

        public void Initialize(Character owner, CastableItem item)
        {
            Owner = owner;
            Item = item;

            if (fields.damager != null)
                fields.damager.Ignore(owner.body);
            Identity = owner.Identity;
            owner.artRenderer.DisplayWeapon(fields.weaponArt);

            SetState(CastableState.Init);
        }

        // State

        public void SetState(CastableState state)
        {
            foreach (var executor in executors)
            {
                executor.SetActive(executor.State == state);
            }
        }

        // Action Queue

        public void QueueAction(CastableAction action)
        {
            StateAction stateAction = new(state, action);
            if (transitionBank.TryGetValue(stateAction, out StateTransition transition))
            {
                foreach (var executor in executors)
                {
                    if (executor.PerformAction(stateAction, ActionPerformed))
                        queuedActions.Add(stateAction);
                }
                DequeueFirst();
            }
        }

        public void DequeueFirst()
        {
            var nextAction = queuedActions.First();
            if (dequeuedActions.Remove(nextAction))
            {
                ActionPerformed(nextAction);
            }
        }

        public void ActionPerformed(StateAction stateAction)
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

        // Tests
        [Header("Tests")]
        public CastableAction testAction;
        public CastableState testState;

        public void TestQueueAction()
        {
            QueueAction(testAction);
        }
        public void TestSetState()
        {
            SetState(testState);
        }
    }
}