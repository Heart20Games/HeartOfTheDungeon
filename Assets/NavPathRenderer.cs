using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavPathRenderer : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform body;
    public Transform arrowhead;
    private LineRenderer line;

    public Vector3 offset = Vector3.zero;
    public bool matchOffsetToTransform = true;

    private Vector3 lastEndPosition;
    private Vector3 lastPrevPosition;

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
        if (matchOffsetToTransform)
        {
            offset = transform.position - body.position;
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
            DrawPath(agent.path);
        }
        else
        {
            if (arrowhead != null)
            {
                arrowhead.gameObject.SetActive(false);
            }
        }
    }

    public void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return;

        int buffer = path.corners.Length == 2 ? 1 : 0;
        line.positionCount = path.corners.Length + buffer;
        line.SetPosition(0, path.corners[0] + offset);


        if (buffer > 0)
        {
            Vector3 midpoint = Vector3.Lerp(path.corners[0] + offset, path.corners[1] + offset, 0.5f);
            line.SetPosition(1, midpoint);
        }

        for (int i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i+buffer, path.corners[i] + offset);
        }

        if (arrowhead != null)
        {
            Vector3 endPoint = line.GetPosition(line.positionCount - 1);
            Vector3 prevPoint = line.GetPosition(line.positionCount - 2);
            if (endPoint != lastEndPosition || prevPoint != lastPrevPosition)
            {
                lastEndPosition = endPoint;
                lastPrevPosition = prevPoint;
                Vector3 direction = (prevPoint - endPoint).normalized;
                arrowhead.gameObject.SetActive(true);
                arrowhead.position = endPoint;
                print(endPoint);
                Vector2 flatDirection = new Vector2(direction.x, direction.z);
                arrowhead.SetRotationWithVector(flatDirection, 90, 0.1f);
            }
        }
    }
}
