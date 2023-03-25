using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 up = Vector3.up;
    private Vector3 rotationOffset;

    private void Awake()
    {
        rotationOffset = transform.rotation.eulerAngles;
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        Vector3 vector = transform.position - target.position;
        Vector3 targetDir = vector.normalized;
        float step = 1f;
        Vector3 currentDir = transform.rotation.eulerAngles;
        Vector3 newDir = Vector3.Lerp(currentDir, targetDir, step);
        transform.rotation = Quaternion.Euler(newDir);
        transform.Rotate(rotationOffset);
    }
}
