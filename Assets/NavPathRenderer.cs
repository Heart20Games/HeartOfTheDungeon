using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavPathRenderer : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform body;
    private LineRenderer line;

    public Vector3 offset = Vector3.zero;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (body == null)
        {
            body = agent.transform;
        }
    }

    private void FixedUpdate()
    {
        if (agent != null)
        {
            DisplayPath();
        }
    }

    public void DisplayPath()
    {
        line.enabled = agent.hasPath;
        if (line.enabled)
        {
            line.SetPosition(0, body.position);
            DrawPath(agent.path);
        }
    }

    public void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return;

        line.positionCount = path.corners.Length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i] + offset);
        }
    }
}
