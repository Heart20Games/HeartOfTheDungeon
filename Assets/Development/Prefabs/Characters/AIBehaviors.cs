using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting;

public class AIBehaviors : MonoBehaviour
{

    public Transform target;
    
    private Character character;
    private NavMeshAgent agent;

    public float navUpdate = 1f;
    public float followingDistance = 0f;
    public float baseOffset = 0f;

    public BehaviorNode tree = new BehaviorNode();
    private BehaviorNode.Status status;

    private float timeKeeper = 0f;

    private void Awake()
    {
        character = GetComponent<Character>();
        agent = character.body.GetComponent<NavMeshAgent>();
        agent.baseOffset = agent.gameObject.transform.position.y + baseOffset;

        SelectorNode hasTarget = new SelectorNode("Has Target");
        LeafNode idle = new LeafNode("Idle", Idle);
        LeafNode chase = new LeafNode("Chase", Chase);

        tree.AddChild(hasTarget);
        hasTarget.AddChild(chase);
        hasTarget.AddChild(idle);

        tree.PrintTree();
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
}
