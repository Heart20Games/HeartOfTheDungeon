using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Body.Behavior.Tree
{
    public class SequenceNode : BehaviorNode
    {
        public SequenceNode() { }

        public SequenceNode(string n)
        {
            name = n;
        }

        public override Status Process()
        {
            Debug.Log("Process sequence: " + name);
            Status childStatus = children[currentChild].Process();
            if (childStatus != Status.SUCCESS) return childStatus;
            currentChild++;
            if (currentChild >= children.Count)
            {
                currentChild = 0;
                return Status.SUCCESS;
            }
            return Status.RUNNING;
        }
    }
}