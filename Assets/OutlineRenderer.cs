using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class OutlineRenderer : MonoBehaviour
{
    public Transform toRender;
    [Layer] public int defaultMask;
    [Layer] public int outlineMask;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        int mask = active ? outlineMask : defaultMask;
        toRender.ApplyLayerRecursive(mask);
        FlipX(flipped);
    }

    [Header("Flipping")]
    public bool autoflip;
    public Transform flipTarget;
    private void FixedUpdate()
    {
        if (autoflip && flipTarget != null)
        {
            FlipX(flipTarget.localScale.x < 0);
        }
    }

    [SerializeField] private bool invert = false;
    [SerializeField] private bool flipped = false;
    public void FlipX(bool flip)
    {
        flipped = flip;
        float magnitude = Mathf.Abs(transform.localScale.x);
        float sign = (flip != invert) ? -1 : 1;
        transform.localScale = new Vector3(magnitude * sign, transform.localScale.y, transform.localScale.z);
    }
}
