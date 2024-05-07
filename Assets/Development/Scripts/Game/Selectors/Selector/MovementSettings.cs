using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Character/Movement Settings", order = 1)]
public class MovementSettings : ScriptableObject
{
    [Header("Physics")]
    public float speed = 700f;
    public float maxVelocity = 10f;
    public float footstepVelocity = 1f;
    public float moveDrag = 0.5f;
    public float stopDrag = 7.5f;
    public bool useGravity = true;
    public float normalForce = 0.1f;
    public float gravityForce = 1f;
    public float groundDistance = 0.01f;
}