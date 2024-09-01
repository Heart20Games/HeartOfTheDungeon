using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class SortGroupByDistance : MonoBehaviour
{
    public Transform target;
    public bool useMainCamera;
    public float scale = 100f;
    const int MAX_SORT = 32767;

    private SortingGroup group;

    private void Awake()
    {
        group = GetComponent<SortingGroup>();
    }

    private void Update()
    {
        Transform finalTarget = target != null && !useMainCamera ? target : Camera.main.transform;
        int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, finalTarget.position) * scale);
        group.sortingOrder = MAX_SORT - Mathf.Clamp(distance, 0, MAX_SORT);
    }
}
