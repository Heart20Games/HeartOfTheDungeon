using UnityEngine;
using UnityEngine.AI;

public class BalancedPathfinder : MonoBehaviour
{
    public NavMeshPath path;
    public Transform target;
    
    public bool hasPath = false;
    public float pathLength = 0f;

    public bool debug = false;

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
            if (hasPath)
            {
                pathLength = 0f;
                for (int i = 0; i < path.corners.Length-1; i++)
                {
                    pathLength += (path.corners[i] - path.corners[i + 1]).magnitude;
                }
                if (debug) print($"Nav Path has {path.corners.Length} corners, with distance {pathLength}.");
            }
        }
    }

    public bool NextPoint(out Vector3 next)
    {
        next = path.corners.Length > 1 ? path.corners[1] : new();
        return hasPath;
    }
}
