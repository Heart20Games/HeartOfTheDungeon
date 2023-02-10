using UnityEngine;
using UnityEngine.AI;

public class AgentFollower : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private float updateInterval = 1f;
    private float timeSinceLastUpdate = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            agent.destination = player.position;
            timeSinceLastUpdate = 0f;
        }
    }
}
