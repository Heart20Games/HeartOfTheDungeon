using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : BaseMonoBehaviour
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
