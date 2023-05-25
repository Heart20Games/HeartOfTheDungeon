using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavPathRenderer : BaseMonoBehaviour
{
    public NavMeshAgent agent;
    public Transform body;
    public Transform arrowhead;
    private LineRenderer line;

    public Vector3 offset = Vector3.zero;
    public bool matchOffsetToTransform = true;
    public float edgeBuffer = 0f;
    public float arrowBuffer = 0f;

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
            Vector3 midpoint = Vector3.Lerp(path.corners[0] + offset, path.corners[1], 0.5f);
            line.SetPosition(1, midpoint);
        }

        for (int i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i+buffer, path.corners[i]);
        }

        BufferEdges(edgeBuffer);

        Vector3 endPoint = line.GetPosition(line.positionCount - 1);
        Vector3 prevPoint = line.GetPosition(line.positionCount - 2);
        PositionArrowhead(endPoint, prevPoint);
    }

    private void BufferEdges(float buffer)
    {
        if (buffer > 0)
        {
            Vector3 startPoint = line.GetPosition(0);
            Vector3 nextPoint = line.GetPosition(1);
            Vector3 endPoint = line.GetPosition(line.positionCount - 1);
            Vector3 prevPoint = line.GetPosition(line.positionCount - 2);

            float startDistance = Vector3.Distance(startPoint, nextPoint);
            float endDistance = Vector3.Distance(endPoint, prevPoint);

            float startBuffer = buffer < startDistance ? buffer : startDistance - 0.001f;
            Vector3 newStart = Vector3.Lerp(startPoint, nextPoint, startBuffer/startDistance);
            line.SetPosition(0, newStart);

            buffer = buffer + arrowBuffer;
            float endBuffer = buffer < endDistance ? buffer : endDistance - 0.001f;
            Vector3 newEnd = Vector3.Lerp(endPoint, prevPoint, endBuffer/endDistance);
            line.SetPosition(line.positionCount - 1, newEnd);
        }
    }

    private void PositionArrowhead(Vector3 endPoint, Vector3 prevPoint)
    {
        if (arrowhead != null)
        {
            if (endPoint != lastEndPosition || prevPoint != lastPrevPosition)
            {
                lastEndPosition = endPoint;
                lastPrevPosition = prevPoint;
                Vector3 direction = (prevPoint - endPoint).normalized;
                arrowhead.gameObject.SetActive(true);
                arrowhead.position = endPoint;
                Vector2 flatDirection = new Vector2(direction.x, direction.z);
                arrowhead.SetRotationWithVector(flatDirection, 90);
            }
        }
    }
}
