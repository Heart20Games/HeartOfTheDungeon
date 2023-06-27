using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Body.Behavior.Tree
{
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
            if (ProcessMethod != null)
            {
                return ProcessMethod();
            }
            return Status.FAILURE;
        }
    }
}