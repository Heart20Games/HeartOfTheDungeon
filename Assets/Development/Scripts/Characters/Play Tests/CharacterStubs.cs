using Body.Behavior.ContextSteering;
using Body.Behavior.Tree;
using Body.Behavior;
using HotD.Castables;
using HotD;
using System.Collections.Generic;
using UnityEngine;
using HotD.Body;
using Body;

public class MovementStub : BaseMonoBehaviour, IMovement
{
    private MovementSettings settings;
    private bool useGravity;
    private bool canMove;
    private Vector2 moveVector;
    private bool shouldFlip;
    private float timeScale;

    public MovementSettings Settings { get => settings; set => settings = value; }
    public bool UseGravity { get => useGravity; set => useGravity = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public Vector2 MoveVector { get => moveVector; set => moveVector = value; }
    public bool ShouldFlip { get => shouldFlip; set => shouldFlip = value; }
    public float TimeScale { get => timeScale; set => SetTimeScale(value); }

    public void SetCharacter(Character character) { }
    public float SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        return timeScale;
    }
    public void StopMoving() { }
}

public class BrainStub : BaseMonoBehaviour, IBrain
{
    private bool alive;
    private CSIdentity.Identity identity;
    private Transform target;
    private CSController controller;
    private Dictionary<Action, LeafNode.Tick> actions = new();
    private float timeScale;

    public bool Alive { get => alive; set => alive = value; }
    public CSIdentity.Identity Identity { get => identity; set => identity = value; }
    public Transform Target { get => target; set => target = value; }
    public CSController Controller => controller;
    public Dictionary<Action, LeafNode.Tick> Actions => actions;
    public float TimeScale { get => timeScale; set => SetTimeScale(value); }
    
    public void RegisterCastables(CastableItem[] items) { }
    public float SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        return timeScale;
    }
}

public class TalkerStub : BaseMonoBehaviour, ITalker
{
    public void CompleteTalking() { }
    public void Talk() { }
    public void Talk(string targetNode) { }
}

public class CasterStub : BaseMonoBehaviour, ICaster
{
    public void ReleaseCastable(ICastable castable) { }
    public void SetFallback(Vector3 fallback, bool isAimVector = false, bool setOverride = false) { }
    public Vector3 SetVector(Vector3 aimVector)
    {
        return aimVector;
    }
    public void TriggerCastable(ICastable castable) { }
}