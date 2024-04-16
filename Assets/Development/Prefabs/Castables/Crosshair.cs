using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : BaseMonoBehaviour
{
    public static Crosshair main;

    public float maxDistance = 1000;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public static Vector3 GetTargetedPosition()
    {
        if (main != null)
        {
            return main.TargetedPosition();
        }
        else
        {
            Transform camera = Camera.main.transform;
            return camera.position + (camera.forward * 1000);
        }
    }

    public Vector3 TargetedPosition()
    {
        Transform camera = Camera.main.transform;
        Vector3 direction = (transform.position - camera.position).normalized;
        Ray ray = new(camera.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return hitInfo.point;
        }
        else
        {
            return camera.position + (direction * maxDistance);
        }
    }
}
