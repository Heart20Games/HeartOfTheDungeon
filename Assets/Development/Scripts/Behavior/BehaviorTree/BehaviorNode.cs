using System.Collections.Generic;
using UnityEngine;

namespace Body.Behavior.Tree
{
    public enum NodeType { Leaf, Condition, Selector, Sequence }

    public class BehaviorNode
    {
        public enum Status { SUCCESS, RUNNING, FAILURE };
        public Status status;
        public List<BehaviorNode> children = new();
        public int currentChild = 0;
        public string name;

        public BehaviorNode() { }

        public BehaviorNode(string n)
        {
            name = n;
        }

        public virtual Status Process()
        {
            return children[currentChild].Process();
        }

        public void AddChild(BehaviorNode n)
        {
            children.Add(n);
        }

        struct NodeLevel
        {
            public int level;
            public BehaviorNode node;
        }

        public void PrintTree()
        {
            string treePrintout = "";
            Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
            BehaviorNode currentNode = this;
            nodeStack.Push(new NodeLevel { level = 0, node = currentNode });

            while (nodeStack.Count != 0)
            {
                NodeLevel nextNode = nodeStack.Pop();
                treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";
                for (int i = nextNode.node.children.Count - 1; i >= 0; i--)
                {
                    nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
                }
            }

            Debug.Log(treePrintout);
        }
    }
}