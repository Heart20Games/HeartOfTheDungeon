using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : BehaviorNode
{
    public SelectorNode() { }

    public SelectorNode(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Debug.Log("Process Selector: " + name);
        Status childStatus = children[currentChild].Process();

        switch (childStatus)
        {
            case Status.RUNNING:
                return Status.RUNNING;
            case Status.SUCCESS:
                currentChild = 0;
                return Status.SUCCESS;
            case Status.FAILURE:
                currentChild++;
                if (currentChild >= children.Count)
                {
                    currentChild = 0;
                    return Status.FAILURE;
                }
                break;
        }
        return Status.RUNNING;
    }
}
