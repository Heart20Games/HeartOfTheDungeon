using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting;

public class AgentFollower : MonoBehaviour
{

    public Transform target;

    private Character character;
    private NavMeshAgent agent;

    public float navUpdate = 1f;
    public float followingDistance = 0f;
    public float baseOffset = 0f;

    private float timeKeeper = 0f;

    private void Awake()
    {
        character = GetComponent<Character>();
        agent = character.body.GetComponent<NavMeshAgent>();
        agent.baseOffset = agent.gameObject.transform.position.y + baseOffset;
    }

    private void Update()
    {
        if (target != null)
        {
            agent.destination = target.position;

            if (Vector3.Distance(transform.position, target.position) < followingDistance)
            {
                agent.isStopped = true;
                return;
            }
            else
            {
                agent.isStopped = false;
            }
            timeKeeper += Time.deltaTime;
            if(timeKeeper > navUpdate)
            {
                timeKeeper = 0f;
                agent.destination = target.position;
            }
        }
        else
        {
            // We can handle what happens when we want to send them somewhere else.
        }
    }
    
}
