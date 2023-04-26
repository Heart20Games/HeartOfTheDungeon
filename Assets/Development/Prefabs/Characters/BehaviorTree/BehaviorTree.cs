using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Brain;
using static LeafNode;

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "ScriptableObjects/BehaviorTree", order = 1)]
public class BehaviorTree : ScriptableObject
{
    [System.Serializable]
    public class Node
    {
        public Node (string name, BehaviorNode behavior)
        {
            this.name = name;
            this.behavior = behavior;
        }
        public string name;
        public Type type;
        public Action action;
        public int childCount;
        [HideInInspector] public BehaviorNode behavior;
    }

    public Node[] nodes;

    public BehaviorNode GenerateTree(Brain brain)
    {
        BehaviorNode rootNode = new BehaviorNode("Root");
        Stack<Node> stack = new Stack<Node>();
        stack.Push(new Node("Root", rootNode));
        foreach (Node node in nodes)
        {
            Node pNode = stack.Pop();
            BehaviorNode parent = pNode.behavior;
            Tick pm = brain.Actions[node.action];
            BehaviorNode newNode = null;
            switch (node.type)
            {
                case Type.Leaf: newNode = new LeafNode(node.name, pm); break;
                case Type.Selector: newNode = new SelectorNode(node.name); break;
                case Type.Sequence: newNode = new SequenceNode(node.name); break;
                case Type.Condition: newNode = new ConditionNode(node.name, pm); break;
            }
            node.behavior = newNode;
            parent.AddChild(newNode);
            if (parent.children.Count < pNode.childCount)
            {
                stack.Push(pNode);
            }
            if (node.childCount > 0)
            {
                stack.Push(node);
            }
        }
        return rootNode;
    }
}
