using HotD.Castables;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependentCastProperties : CastProperties
{
    public CastProperties initializeOffOf;

    protected void Awake()
    {
        InitializeFields();
    }

    protected void OnEnable()
    {
        if (fields != initializeOffOf.fields)
        {
            InitializeFields();
        }
    }

    [ButtonMethod]
    public void InitializeFields()
    {
        Print($"Initializing Fields on {Name}");
        Initialize(initializeOffOf.fields);
    }
}
