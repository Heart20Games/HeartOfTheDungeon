using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using static LeafNode;
using UnityEngine.InputSystem;
using static CSMapping;

public class Brain : MonoBehaviour, ITimeScalable
{
    public bool Enabled
    {
        get => enabled;
        set { SetEnabled(value); }
    }

    [SerializeField]
    private Transform target = null;
    public Transform Target
    {
        get { return target; }
        set { target = value.TryGetComponent(out Character targetChar) ? targetChar.body : value; }
    }
    
    private Character character;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] private CSController controller;

    public float navUpdate = 1f;
    public float followingDistance = 0f;
    public float baseOffset = 0f;
    public bool useAgent = true;

    public enum Type { Leaf, Condition, Selector, Sequence }
    public enum Action { Idle, Chase }
    private Dictionary<Action, Tick> actions;
    public Dictionary<Action, Tick> Actions {
        get { return actions ??= new Dictionary<Action, Tick>() { { Action.Idle, Idle }, { Action.Chase, Chase } }; }
    }
    public BehaviorTree tree;
    [HideInInspector] public BehaviorNode root;
    private BehaviorNode.Status status;

    public BrainModifiers modifiers;

    private float timeScale = 1f;
    public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }

    private float timeKeeper = 0f;

    public bool debug = false;

    private void Awake()
    {
        character = GetComponent<Character>();
        agent = character.body.GetComponent<NavMeshAgent>();
        controller = character.body.GetComponent<CSController>();
        agent.baseOffset = baseOffset;
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
        Debug.Log("Tree Name: " + root.name);

        //SelectorNode hasTarget = new SelectorNode("Has Target");
        //LeafNode idle = new LeafNode("Idle", Idle);
        //LeafNode interest = new LeafNode("Chase", Chase);

        //tree.AddChild(hasTarget);
        //hasTarget.AddChild(interest);
        //hasTarget.AddChild(idle);

        root.PrintTree();
        Enabled = Enabled;
    }

    private void Update()
    {
        if (debug) Debug.Log("Updating Brain");
        if (status == BehaviorNode.Status.FAILURE)
        {
            Debug.LogWarning("Behavior Tree reached fail state");
        }
        status = root.Process();
    }

    
    // Outside Interaction

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

    public void SetTarget(Transform _target)
    {
        target = _target;
    }


    // Checks

    public bool HasTarget()
    {
        if (debug) Debug.Log("Has Target? " + (target != null ? "Yes" : "No") + " (" + target + ")");
        return target != null;
    }


    // Actions

    public BehaviorNode.Status Idle()
    {
        if (debug) Debug.Log("Idling...");
        return BehaviorNode.Status.SUCCESS;
    }

    public BehaviorNode.Status Chase()
    {
        if (!HasTarget()) return BehaviorNode.Status.FAILURE;

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
