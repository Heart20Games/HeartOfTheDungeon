using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Brain;
using static LeafNode;

[CreateAssetMenu(fileName = "BrainModifiers", menuName = "ScriptableObjects/BrainModifiers", order = 1)]
public class BrainModifiers : ScriptableObject
{
    public float baseOffset = 0f;

    // Steering
    public float speed = 3.5f;
    public float angularSpeed = 120f;
    public float acceleration = 8f;
    public float stoppingDistance = 5f;
    public bool autoBreaking = true;

    // Obstacle Avoidance
    public float radius = 0.5f;
    public float height = 2f;

    public void InitializeBrain(Brain brain)
    {
        brain.agent.baseOffset = baseOffset;
        
        // Steering
        brain.agent.speed = speed;
        brain.agent.angularSpeed = angularSpeed;
        brain.agent.acceleration = acceleration;
        brain.agent.stoppingDistance = stoppingDistance;
        brain.agent.autoBraking = autoBreaking;
        
        // Obstacle Avoidance
        brain.agent.radius = radius;
        brain.agent.height = height;
    }
}