using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Body.Behavior
{
    using ContextSteering;
    using Tree;
    using static ContextSteering.CSIdentity;
    using static ContextSteering.CSContext;
    using static Tree.LeafNode;
    using HotD.Castables;
    using UnityEngine.InputSystem.XR;
    using HotD;

    public enum Action { Idle, Patrol, Chase, Duel }

    public interface IBrain : ITimeScalable, IEnableable
    {
        // Properties
        public bool Alive { get; set; }
        public Identity Identity { get; set; }
        public Transform Target { get; set; }

        // Context Steering
        public CSController Controller { get; }

        // Tree
        public Dictionary<Action, Tick> Actions { get; }

        // Castables
        public void RegisterCastables(CastableItem[] items);
    }

    public class Brain : BaseMonoBehaviour, IBrain
    {
        // Properties
        public new bool enabled
        {
            get => base.enabled;
            set => base.enabled = value;
        }
        
        public bool Alive
        {
            get => controller.Alive;
            set { if (controller != null) controller.Alive = value; }
        }

        public Identity Identity
        {
            get => controller.identity;
            set => controller.identity=value;
        }

        [SerializeField] private Transform target = null;
        public Transform Target
        {
            get => target;
            set => SetTarget(value);
        }

        // Components
        private CSController controller;
        protected Character character;
        private Transform body;
        public NavMeshAgent agent;
        protected BalancedPathfinder pathFinder;
        public CSController Controller => controller;

        // Agent
        public float navUpdate = 1f;
        public float followingDistance = 0f;
        public float baseOffset = 0f;
        public bool useAgent = true;
        public Modifiers modifiers;

        // Tree
        private Dictionary<Action, Tick> actions;
        public Dictionary<Action, Tick> Actions
        {
            get => actions ??= new Dictionary<Action, Tick>() { { Action.Idle, Idle }, { Action.Patrol, Patrol }, { Action.Chase, Chase }, { Action.Duel, Duel } };
        }
        private BehaviorTree tree;
        private BehaviorNode root;
        private BehaviorNode Root { get => root ??= tree != null ? tree.GenerateTree(this) : new(); set => root = value; }
        private BehaviorNode.Status status;

        // Castable Contexts
        private readonly Dictionary<Identity, List<Context>> castableMap = new();

        // Helpers
        private float timeKeeper = 0f;
        [SerializeField] protected bool debug = false;

        // Initialization
        public virtual void Awake()
        {
            if (debug) print("Awake!");
            if (TryGetComponent(out character))
            {
                body = character.body;
            }
            if (body == null)
                body = transform;
            if (body != null)
            {
                if (agent != null || body.TryGetComponent(out agent))
                    agent.baseOffset = baseOffset;
                if (controller != null || body.TryGetComponent(out controller))
                    controller.Context.castableContext = castableMap;
                if (pathFinder == null)
                    body.TryGetComponent(out pathFinder);
            }
            if (target != null)
                Target = target;
            Root = Root;
            if (agent != null && modifiers != null)
                modifiers.InitializeBrain(this);
            Alive = true;

            if (debug) root.PrintTree();
            //Enabled = Enabled;
        }


        // Update
        public virtual void Update()
        {
            //if (debug) Debug.Log("Updating Brain");
            if (status == BehaviorNode.Status.FAILURE)
            {
                Debug.LogWarning("Behavior Tree reached fail state");
            }
            status = Root.Process();
        }

        // Castable Contexts
        public void RegisterCastables(CastableItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    RegisterCastable(items[i]);
            }
        }

        private void RegisterCastable(CastableItem item)
        {
            if (debug) print("Register Castable: " + item.name);
            
            if (!castableMap.TryGetValue(item.context.identity, out List<Context> contexts))
            {
                contexts = new();
                castableMap.Add(item.context.identity, contexts);
            }

            if (!contexts.Contains(item.context))
                contexts.Add(item.context);
        }

        // Target
        private void SetTarget(Transform target)
        {
            this.target = target;
            if (this.target != null)
                this.target = (target.TryGetComponent(out Character targetChar) ? targetChar.body : target);
            if (agent != null && agent.isActiveAndEnabled)
                agent.destination = this.target == null ? new() : this.target.position;
            if (pathFinder != null)
                pathFinder.target = this.target;
        }


        // Checks

        private bool HasTarget()
        {
            if (debug) Debug.Log("Has Target? " + (target != null ? "Yes" : "No") + " (" + target + ")");
            return target != null;
        }

        private bool HasFoeInRange(Range range)
        {
            bool result = controller.HasActiveContext(Identity.Foe, range);
            if (debug) print($"Has Foe In Range: {range}");
            return result;
        }

        // Actions

        private BehaviorNode.Status Idle()
        {
            TargetNavigation();

            //if (debug) Debug.Log("Idling...");

            return BehaviorNode.Status.SUCCESS;
        }

        private BehaviorNode.Status Chase()
        {
            //if (!HasFoeInRange(Range.InRange)) return BehaviorNode.Status.FAILURE;

            if (debug) Debug.Log("Chasing...");

            if (useAgent)
            {
                agent.destination = target.position;
                if (Vector3.Distance(transform.position, target.position) < agent.stoppingDistance)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                    timeKeeper += Time.deltaTime;
                    if (timeKeeper > navUpdate)
                    {
                        timeKeeper = 0f;
                        agent.destination = target.position;
                    }
                }
            }
            else
            {
                TargetNavigation();
            }

            return BehaviorNode.Status.SUCCESS;
        }

        protected BehaviorNode.Status Patrol()
        {
            TargetNavigation();

            return BehaviorNode.Status.SUCCESS;
        }

        protected BehaviorNode.Status Duel()
        {
            //if (!HasFoeInRange(Range.InAttackRange)) return BehaviorNode.Status.FAILURE;

            Debug.Log("Dueling...");

            Debug.Log("Trying to attack");
            character.Aim(-controller.CurrentVector.normalized, true);

            // Find the closest-range weapon that I can currently use, then shoot with it.
            int closestIdx = FindClosestRangeUsableCastable();
            if (closestIdx >= 0)
            {
                Debug.Log("Actually attacking!");
                character.TriggerCastable(closestIdx);
            }

            return BehaviorNode.Status.SUCCESS;
        }

        private int FindClosestRangeUsableCastable()
        {
            int closest = int.MaxValue;
            int closestIdx = -1;
            for (int i = 0; i < character.castableItems.Length; i++)
            {
                CastableItem item = character.castableItems[i];
                if (item != null && controller.HasActiveContext(item.context))
                {
                    if (item.context.vector.deadzone.y < closest)
                    {
                        Debug.Log("Found something to attack with...");
                        closest = (int)item.context.vector.deadzone.y;
                        closestIdx = i;
                    }
                }
            }
            return closestIdx;
        }


        // Behavior

        private void TargetNavigation()
        {
            pathFinder.target = target;
            if (pathFinder.NextPoint(out Vector3 destination))
            {
                controller.SetDestination(destination, pathFinder.pathLength);
            }
        }


        // TimeScaling
        private float timeScale = 1f;
        public float TimeScale
        {
            get => timeScale;
            set => timeScale = SetTimeScale(value);
        }
        private float tempSpeed;
        private float tempAngularSpeed;
        private float tempAcceleration;
        public float SetTimeScale(float timeScale)
        {
            if (this.timeScale != timeScale)
            {
                if (useAgent)
                {
                    if (timeScale == 0)
                    {
                        tempSpeed = agent.speed;
                        tempAngularSpeed = agent.angularSpeed;
                        tempAcceleration = agent.acceleration;
                        agent.speed = 0;
                        agent.angularSpeed = 0;
                    }
                    else if (this.timeScale == 0)
                    {
                        agent.speed = tempSpeed;
                        agent.angularSpeed = tempAngularSpeed;
                    }
                    else
                    {
                        float ratio = timeScale / this.timeScale;
                        agent.speed *= ratio;
                        agent.angularSpeed *= ratio;
                    }
                }
            }
            this.timeScale = timeScale;
            return timeScale;
        }
    }
}