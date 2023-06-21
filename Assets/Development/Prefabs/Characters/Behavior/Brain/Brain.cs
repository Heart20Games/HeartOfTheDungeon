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

    public class Brain : BaseMonoBehaviour, ITimeScalable
    {
        // Enabled
        public bool Enabled
        {
            get => enabled;
            set { SetEnabled(value); }
        }

        // Target
        [SerializeField]
        private Transform target = null;
        public Transform Target
        {
            get { return target; }
            set { target = value.TryGetComponent(out Character targetChar) ? targetChar.body : value; }
        }

        // Components
        private Character character;
        [HideInInspector] public NavMeshAgent agent;
        [HideInInspector] private CSController controller;
        [HideInInspector] private BalancedPathfinder pathFinder;

        // Agent
        public float navUpdate = 1f;
        public float followingDistance = 0f;
        public float baseOffset = 0f;
        public bool useAgent = true;
        public Modifiers modifiers;

        // Tree
        public enum Type { Leaf, Condition, Selector, Sequence }
        public enum Action { Idle, Chase, Duel }
        private Dictionary<Action, Tick> actions;
        public Dictionary<Action, Tick> Actions
        {
            get { return actions ??= new Dictionary<Action, Tick>() { { Action.Idle, Idle }, { Action.Chase, Chase }, { Action.Duel, Duel } }; }
        }
        public BehaviorTree tree;
        [HideInInspector] public BehaviorNode root;
        private BehaviorNode.Status status;

        // Castable Contexts
        public Dictionary<Identity, List<Context>> castableMap = new();

        // TimeScale
        private float timeScale = 1f;
        public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }

        // Helpers
        private float timeKeeper = 0f;
        public bool debug = false;

        // Initialization
        private void Awake()
        {
            character = GetComponent<Character>();
            agent = character.body.GetComponent<NavMeshAgent>();
            controller = character.body.GetComponent<CSController>();
            pathFinder = character.body.GetComponent<BalancedPathfinder>();
            agent.baseOffset = baseOffset;
            if (character != null && character.loadout != null)
            {
                RegisterCastables(character.loadout.weapons);
                RegisterCastables(character.loadout.abilities);
            }
            controller.Context.castableContext = castableMap;
            if (target != null)
            {
                Target = target;
            }
            if (tree != null)
            {
                root = tree.GenerateTree(this);
            }
            if (agent != null && modifiers != null)
            {
                modifiers.InitializeBrain(this);
            }
            //Debug.Log("Tree Name: " + root.name);

            //SelectorNode hasTarget = new SelectorNode("Has Target");
            //LeafNode idle = new LeafNode("Idle", Idle);
            //LeafNode interest = new LeafNode("Chase", Chase);

            //tree.AddChild(hasTarget);
            //hasTarget.AddChild(interest);
            //hasTarget.AddChild(idle);

            root.PrintTree();
            Enabled = Enabled;
        }


        // Update
        private void Update()
        {
            //if (debug) Debug.Log("Updating Brain");
            if (status == BehaviorNode.Status.FAILURE)
            {
                Debug.LogWarning("Behavior Tree reached fail state");
            }
            status = root.Process();
        }

        // Castable Contexts
        public void RegisterCastables(List<CastableItem> castables)
        {
            for(int i = 0; i < castables.Count; i++)
            {
                CastableItem item = castables[i];
                RegisterCastable(item);
            }
        }

        public void RegisterCastable(CastableItem item)
        {
            if (debug) print("Register Castable: " + item.name);
            if (!castableMap.TryGetValue(item.context.identity, out List<Context> contexts))
            {
                contexts = new();
                castableMap.Add(item.context.identity, contexts);
            }
            if (!contexts.Contains(item.context))
            {
                contexts.Add(item.context);
            }
        }

        // Enabled
        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (agent != null)
            {
                agent.enabled = useAgent && enabled;
            }
            if (controller != null)
            {
                controller.Active = !useAgent && enabled;
            }
        }

        // Target
        public void SetTarget(Transform _target)
        {
            target = _target;
        }


        // Contexts
        //public void SetBaseContext(Action action)
        //{
        //    controller.Context.baseContext = controller.Preset.GetContext(action);
        //}


        // Checks

        public bool HasTarget()
        {
            if (debug) Debug.Log("Has Target? " + (target != null ? "Yes" : "No") + " (" + target + ")");
            return target != null;
        }

        public bool HasFoeInRange(Range range)
        {
            return controller.HasActiveContext(Identity.Foe, range);
        }

        // Actions

        public BehaviorNode.Status Idle()
        {
            //SetBaseContext(Action.Idle);

            pathFinder.target = target;

            //if (debug) Debug.Log("Idling...");

            return BehaviorNode.Status.SUCCESS;
        }

        public BehaviorNode.Status Chase()
        {
            if (!HasFoeInRange(Range.InRange)) return BehaviorNode.Status.FAILURE;

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
                pathFinder.target = target;
                if (pathFinder.NextPoint(out Vector3 destination))
                {
                    controller.Destination = destination;
                }
                else
                {
                    controller.following = false;
                }
            }

            return BehaviorNode.Status.SUCCESS;
        }
        
        public BehaviorNode.Status Duel()
        {
            if (!HasFoeInRange(Range.InAttackRange)) return BehaviorNode.Status.FAILURE;

            if (debug) Debug.Log("Dueling...");

            if (!useAgent)
            {
                controller.following = false;
                character.AimCharacter(-controller.CurrentVector);
                character.ActivateWeapon();
            }

            return BehaviorNode.Status.SUCCESS;
        }


        // TimeScaling
        private float tempSpeed;
        private float tempAngularSpeed;
        private float tempAcceleration;
        public void SetTimeScale(float timeScale)
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
        }

        //public class ActionComparer : IEqualityComparer<Action>
        //{
        //    public bool Equals(Action x, Action y)
        //    {
        //        return x == y;
        //    }

        //    public int GetHashCode(Action x)
        //    {
        //        return (int)x;
        //    }
        //}
    }
}