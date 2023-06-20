using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : BaseMonoBehaviour
{
    public Transform target;
    public Vector3 up = Vector3.up;
    private Vector3 rotationOffset;

    private void Awake()
    {
        //rotationOffset = transform.rotation.eulerAngles;
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        transform.LookAt(target.position);
    }
}
