using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : BaseMonoBehaviour
{
    public static Crosshair main;

    public float maxDistance = 1000;

    [Header("Debugging")]
    [SerializeField] private bool debug = true;
    [SerializeField] private bool drawCameraToOriginRay = false;
    [SerializeField] private float drawRayLifetime = 2.5f;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public static Vector3 GetTargetedPosition(Transform referenceObject = null)
    {
        if (main != null)
        {
            return main.TargetedPosition(referenceObject);
        }
        else
        {
            Transform camera = Camera.main.transform;
            float originDistance = referenceObject == null ? 0 : (referenceObject.position - camera.position).magnitude;
            Vector3 origin = camera.position + (originDistance * camera.forward);
            return origin + (camera.forward * 1000);
        }
    }

    public Vector3 TargetedPosition(Transform referenceObject = null)
    {
        Transform camera = Camera.main.transform;
        Vector3 direction = (transform.position - camera.position).normalized;
        float originDistance = referenceObject == null ? 0 : (referenceObject.position - camera.position).magnitude;
        Vector3 origin = camera.position + (originDistance * direction);
        Ray ray = new(origin, direction);
        if (debug && drawCameraToOriginRay) Debug.DrawRay(camera.position, ray.direction * originDistance, Color.red, drawRayLifetime);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 hitDirection = (hitInfo.point - ray.origin).normalized;
            Color color = Vector3.Dot(ray.direction, hitDirection) >= 0 ? Color.green : Color.magenta;
            if (debug) Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, color, drawRayLifetime);
            return hitInfo.point;
        }
        else
        {
            if (debug) Debug.DrawRay(ray.origin, direction * maxDistance, Color.yellow, drawRayLifetime);
            return camera.position + (direction * maxDistance);
        }
    }
}
