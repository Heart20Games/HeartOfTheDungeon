using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorNode
{
    public enum Status { SUCCESS, RUNNING, FAILURE };
    public Status status;
    public List<BehaviorNode> children = new List<BehaviorNode>();
    public int currentChild = 0;
    public string name;

    public BehaviorNode() { }

    public BehaviorNode(string n)
    {
        name = n;
    }

    public void AddChild(BehaviorNode n)
    {
        children.Add(n);
    }
}
