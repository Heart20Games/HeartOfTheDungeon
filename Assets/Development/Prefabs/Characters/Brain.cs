using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting;

public class Brain : MonoBehaviour, ITimeScalable
{
    public bool Enabled
    {
        get { return enabled; }
        set { SetEnabled(value); }
    }

    [SerializeField]
    private Transform target = null;
    public Transform Target
    {
        get { return target; }
        set {
            Character targetChar = value.GetComponent<Character>();
            if (targetChar != null)
            {
                target = targetChar.body;
            }
            else
            {
                target = value; 
            }
        }
    }
    
    private Character character;
    private NavMeshAgent agent;

    public float navUpdate = 1f;
    public float followingDistance = 0f;
    public float baseOffset = 0f;

    public BehaviorNode tree = new BehaviorNode();
    private BehaviorNode.Status status;

    private float timeScale = 1f;
    private float timeKeeper = 0f;

    private void Awake()
    {
        character = GetComponent<Character>();
        agent = character.body.GetComponent<NavMeshAgent>();
        agent.baseOffset = baseOffset;
        if (target != null)
        {
            Target = target;
        }

        SelectorNode hasTarget = new SelectorNode("Has Target");
        LeafNode idle = new LeafNode("Idle", Idle);
        LeafNode chase = new LeafNode("Chase", Chase);

        tree.AddChild(hasTarget);
        hasTarget.AddChild(chase);
        hasTarget.AddChild(idle);

        //tree.PrintTree();
    }

    private void Update()
    {
        if (status == BehaviorNode.Status.FAILURE)
        {
            Debug.LogWarning("Behavior Tree reached fail state");
        }
        status = tree.Process();
    }

    
    // Outside Interaction

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (agent != null)
        {
            agent.enabled = enabled;
        }
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }


    // Checks

    public bool HasTarget()
    {
        return target != null;
    }


    // Actions

    public BehaviorNode.Status Idle()
    {
        return BehaviorNode.Status.SUCCESS;
    }

    public BehaviorNode.Status Chase()
    {
        if (!HasTarget()) return BehaviorNode.Status.FAILURE;

        agent.destination = target.position;
        if (Vector3.Distance(transform.position, target.position) < followingDistance)
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
        this.timeScale = timeScale;
    }
}
