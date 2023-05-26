using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BalancedPathfinder : MonoBehaviour
{
    public NavMeshPath path;
    public Transform target;

    private void Awake()
    {
        path = new();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }
    }

    public bool NextPoint(out Vector3 next)
    {
        next = path.corners.Length > 0 ? path.corners[0] : new();
        return path.corners.Length > 0;
    }
}
