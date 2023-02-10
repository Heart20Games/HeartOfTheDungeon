using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting;

public class AgentFollower : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public bool isFollowing = true;
    public float navUpdate = 1f;

    public float followingDistance = 0f;

    private float timeKeeper = 0f;

    private void Awake()
    {
        if (isFollowing)
        {
            // StartCoroutine(UpdateDestination());
            agent = GetComponent<NavMeshAgent>();
            agent.destination = player.position;
        }

       
    }

    private void Update()
    {
        if (isFollowing)
        {
            if (Vector3.Distance(transform.position, player.position) < followingDistance)
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
                Vector3 playerPosition = player.position;
                agent.destination = playerPosition;
            }
        }
        else
        {
            // We can handle what happens when we want to send them somewhere else.

        }
    }
    
}
