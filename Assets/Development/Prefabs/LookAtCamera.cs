using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : BaseMonoBehaviour
{
    public Transform target;
    public Vector3 up = Vector3.up;

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
        if (up == Vector3.zero)
        {
            transform.TrueLookAt(target.position);
        }
        else
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 relativePosition = transform.position + new Vector3(direction.x, 0f, direction.z);
            transform.LookAt(relativePosition, up);
        }

        //Vector3 direction = (target.position - transform.position).normalized;
        //Debug.DrawRay(transform.position, direction, Color.red, Time.fixedDeltaTime);
        //Vector2 xzDirection = direction.XZVector();
        //transform.SetRotationWithVector(xzDirection);

        //Vector2 fyDirection = new(direction.y, xzDirection.magnitude);
        //Vector3 cross = Vector3.Cross(Vector3.up, direction);
        //transform.SetRotationWithVector(fyDirection, cross);
    }
}
