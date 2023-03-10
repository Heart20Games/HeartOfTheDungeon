using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "ScriptableObjects/BehaviorTree", order = 1)]
public class BehaviorTree : ScriptableObject
{
    public BehaviorNode rootNode;
}
