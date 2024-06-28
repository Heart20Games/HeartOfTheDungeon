using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : BaseMonoBehaviour
{
    public Transform target;
    public Vector3 up = Vector3.up;

    private void OnEnable()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isActiveAndEnabled)
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
        }
    }

    [ButtonMethod]
    public void FlipOverX()
    {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
    }
}
