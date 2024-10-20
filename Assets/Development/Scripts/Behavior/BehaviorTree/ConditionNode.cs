using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Body.Behavior.Tree
{
    public class ConditionNode : LeafNode
    {
        public ConditionNode() { }

        public ConditionNode(string n, Tick pm)
        {
            name = n;
            ProcessMethod = pm;
        }
    }
}