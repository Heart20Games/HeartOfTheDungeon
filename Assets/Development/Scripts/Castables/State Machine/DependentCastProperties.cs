using HotD.Castables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependentCastProperties : CastProperties
{
    public CastProperties initializeOffOf;

    private void Awake()
    {
        Initialize(initializeOffOf.fields);
    }
}
