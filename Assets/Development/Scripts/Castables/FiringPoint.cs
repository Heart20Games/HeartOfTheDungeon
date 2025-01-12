using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPoint : BaseMonoBehaviour
{
    [SerializeField] private Transform trackpoint;
    [SerializeField] private Transform firepoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool trackOnStart;
    [ReadOnly][SerializeField] private bool tracking = true;

    private float yMax, xzMax;

    private void Awake()
    {
        ResetTracking();
        if (trackOnStart)
            StartTracking();
        else
            StopTracking();
    }

    private void FixedUpdate()
    {
        if (!tracking) return;

        if (trackpoint == null) return;

        Vector3 relative = trackpoint.position - transform.position;
        Vector3 xzRelative = relative.XZVector3();
        Vector3 direction = xzRelative.normalized;

        yMax = Mathf.Max(yMax, relative.y);
        xzMax = Mathf.Max(xzMax, xzRelative.magnitude);

        Vector3 final = (Vector3.up * (yMax + offset.y)) + (direction * (xzMax + offset.x));

        if (firepoint == null) return;

        firepoint.transform.position = transform.position + final;
    }

    [ButtonMethod]
    public void StartTracking()
    {
        tracking = true;
    }

    [ButtonMethod]
    public void StopTracking()
    {
        tracking = false;
    }

    [ButtonMethod]
    public void ResetTracking()
    {
        yMax = int.MinValue;
        xzMax = int.MinValue;
    }
}
