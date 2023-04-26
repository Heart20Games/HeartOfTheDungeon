using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeafNode : BehaviorNode
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public LeafNode() { }

    public LeafNode(string n, Tick pm)
    {
        name = n;
        ProcessMethod = pm;
    }

    public override Status Process()
    {
        Debug.Log("Process leaf: " + name);
        if (ProcessMethod != null)
        {
            return ProcessMethod();
        }
        return Status.FAILURE;
    }
}
