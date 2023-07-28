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

    public void OrientBody(Vector2 vector)
    {
        body.SetLocalRotationWithVector(vector);
    }
}
