using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BalancedPathfinder : MonoBehaviour
{
    public NavMeshPath path;
    public Transform target;
    public bool hasPath = false;

    private void Awake()
    {
        path = new();
    }

    private void FixedUpdate()
    {
        hasPath = false;
        if (target != null)
        {
            hasPath = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }
    }

    public bool NextPoint(out Vector3 next)
    {
        next = path.corners.Length > 1 ? path.corners[1] : new();
        return hasPath;
    }
}
