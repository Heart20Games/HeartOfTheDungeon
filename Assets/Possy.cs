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
    public Character Leader { get => leader; set => SetLeader(value); }
    public List<Character> characters = new();
    public List<Character> pets = new();

    static List<Possy> possies = new();
    static Possy mainPossy;
    public bool isMainPossy = false;

    [Header("Events")]
    public bool aggroed = false;
    public bool allDead = false;
    public UnityEvent onAggro = new();
    public UnityEvent onAllDead = new();
    private bool aggroedThisFrame = false;

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

    private void FixedUpdate()
    {
        if (aggroed && !aggroedThisFrame)
        {
            SetAggroed(false);
        }
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
        foreach (var character in characters)
            character.Refresh();
    }

    public void Respawn()
    {
        foreach (var character in characters)
            character.Respawn();
    }

    public void SetTargetPossy(Possy target, bool preferNew=true)
    {
        if (preferNew || targetPossy == null)
        {
            targetPossy = target;
            if (targetPossy != null)
                targetPossy.onAllDead.AddListener(RivalPossyDied);
            SetFollowTarget(target == null ? null : target.followTargeter);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        if (followTargeter.TryGetComponent(out Brain brain))
            brain.Target = target;
    }

    public void SetLeader(Character character)
    {
        leader = character;
        defaultFollowTarget = leader.body.transform;
    }

    public void SetAggroed(bool aggro)
    {
        if (debug) print("Aggro!");
        if (aggro)
        {
            aggroedThisFrame = true;
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
            aggroed = true;
        }
        else
        {
            aggroed = false;
            TargetPossy = null;
            SetFollowTarget(defaultFollowTarget);
        }
    }


    // Events

    public void RivalPossyDied()
    {
        if (debug) print("Rival Possy Died");
        if (isMainPossy)
            Refresh();
        SetAggroed(false);
    }

    public void CharacterControlled(bool controlled)
    {
        if (debug) print("Controlled");
        if (mainPossy && leader != null)
        {
            foreach (var character in characters)
            {
                if (character.controllable)
                    Leader = character;
            }
        }
    }

    public void CharacterAggroed()
    {
        if (debug) print("Aggroed");
        if (!aggroed)
        {
            SetAggroed(true);
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
        allDead = true;
        aggroed = false;
        TargetPossy = null;
        onAllDead.Invoke();
        onAllDead.RemoveAllListeners();
    }
}
