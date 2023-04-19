using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    public Transform body;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponentInChildren<Transform>();
        }
    }
}
