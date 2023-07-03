using Body;
using Body.Behavior;
using Body.Behavior.ContextSteering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Possy : BaseMonoBehaviour
{
    [Header("Members")]
    public Character leader;
    public List<Character> characters = new();
    public List<Character> pets = new();

    static List<Possy> possies = new();
    static Possy mainPossy;
    public bool isMainPossy = false;

    [Header("Events")]
    public bool aggroed = false;
    public bool allDead = false;
    public UnityEvent onAggro;
    public UnityEvent onAllDead;

    [Header("Follow Target")]
    public Transform followTargeter;
    public bool useLeaderAsFollowTargeter;
    public Transform defaultFollowTarget;
    private Possy targetPossy;
    public Possy TargetPossy { get => targetPossy; set => SetTargetPossy(value); }

    [Header("Noise and Scaling")]
    public MovementNoise noise;
    public float destinationScale = 1f;

    public bool debug = false;


    private void Awake()
    {
        possies.Add(this);
    }

    private void Start()
    {
        if (useLeaderAsFollowTargeter)
            followTargeter = leader.body.transform;
        if (isMainPossy)
            mainPossy = this;
        if (leader != null)
        {
            defaultFollowTarget = leader.body.transform;
            SetFollowTarget(defaultFollowTarget);
        }
        foreach (var character in characters)
        {
            if (followTargeter)
                character.brain.Target = followTargeter;
            if (noise != null)
            {
                character.Controller.noise = noise;
                character.Controller.destinationScale = destinationScale;
            }
            character.Controller.onFoeContextActive.AddListener(CharacterAggroed);
            character.onDmg.AddListener(CharacterDamaged);
            character.onDeath.AddListener(CharacterDied);
            character.onControl.AddListener(CharacterControlled);
        }
    }


    // Actions

    public void Refresh()
    {

    }

    public void Respawn()
    {

    }

    public void SetTargetPossy(Possy target, bool preferNew=true)
    {
        if (preferNew || targetPossy == null)
        {
            targetPossy = mainPossy;
            SetFollowTarget(mainPossy.followTargeter);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        if (followTargeter.TryGetComponent(out Brain brain))
            brain.Target = target;
    }


    // Events

    public void RivalPossyDied()
    {
        aggroed = false;
        TargetPossy = null;
        SetFollowTarget(defaultFollowTarget);
    }

    public void CharacterControlled(bool controlled)
    {
        if (debug) print("Controlled");
        if (mainPossy && leader != null)
        {
            foreach (var character in characters)
            {
                if (character.controllable)
                    leader = character;
            }
        }
    }

    public void CharacterAggroed()
    {
        if (debug) print("Aggroed");
        if (!aggroed)
        {
            if (debug) print("Aggro!");
            aggroed = true;
            foreach (var character in characters)
            {
                character.Controller.destinationScale = 1f;
                character.Controller.useNoise = false;
            }
            if (mainPossy != this)
            {
                SetTargetPossy(mainPossy);
                mainPossy.SetTargetPossy(this, false);
            }
            onAggro.Invoke();
        }
    }

    public void CharacterDamaged()
    {
        if (debug) print("Damaged");
        CharacterAggroed();
    }

    public void CharacterDied()
    {
        if (debug) print("Died");
        foreach (var character in characters)
        {
            if (character.alive) return;
        }
        aggroed = false;
        if (TargetPossy != null)
            TargetPossy.RivalPossyDied();
        TargetPossy = null;
        allDead = true;
        onAllDead.Invoke();
    }
}
