using Body;
using Body.Behavior;
using Body.Behavior.ContextSteering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class Party : BaseMonoBehaviour
{
    [Header("Party Members")]
    public Character leader;
    public Character Leader { get => leader; set => SetLeader(value); }
    public List<Character> characters = new();
    public List<Character> pets = new();

    static List<Party> parties = new();
    static Party mainParty;
    public bool isMainParty = false;

    [Space()]
    [Header("Status Events")]
    [ReadOnly] public bool aggroed = false;
    public UnityEvent onAggro = new();
    [ReadOnly] public bool allDead = false;
    public UnityEvent onAllDead = new();
    private bool aggroedThisFrame = false;

    [Header("Follow Target")]
    public Transform followTargeter;
    public bool useLeaderAsFollowTargeter;
    public Transform defaultFollowTarget;
    [ReadOnly][SerializeField] private Party targetParty;
    public Party TargetParty { get => targetParty; set => SetTargetParty(value); }

    [Space()]
    [Header("Noise and Scaling")]
    public MovementNoise noise;
    public float tightness = 1f;
    public float tightnessIdle = 1f;
    public float tightnessAggro = 2f;
    public float Tightness { get => tightness; set => SetTightness(value); }

    public bool debug = false;


    private void Awake()
    {
        parties.Add(this);
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
        if (isMainParty)
            mainParty = this;
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
                character.Controller.destinationScale = tightnessIdle;
            }
            character.Controller.onFoeContextActive.AddListener(CharacterAggroed);
            character.onDmg.AddListener(CharacterDamaged);
            character.onDeath.AddListener(CharacterDied);
            character.onControl.AddListener((bool controlled) => CharacterControlled(controlled, character));
            if (debug) print($"Character {character.name} connected to party {name}.");
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

    public void SetTargetParty(Party target, bool preferNew=true)
    {
        if (preferNew || targetParty == null)
        {
            if (targetParty != null)
                targetParty.onAllDead.RemoveListener(RivalPartyDied);
            targetParty = target;
            if (targetParty != null)
                targetParty.onAllDead.AddListener(RivalPartyDied);
            SetFollowTarget(target == null ? defaultFollowTarget : target.followTargeter);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        print($"Set {name}'s follow target: {(transform == null ? "none" : transform.name)}");
        if (followTargeter.TryGetComponent(out Brain brain))
            brain.Target = target;
    }

    public void SetLeader(Character character)
    {
        leader = character;
        defaultFollowTarget = leader.body.transform;
        if (!aggroed)
            SetFollowTarget(defaultFollowTarget);
    }

    public void SetAggroed(bool aggro)
    {
        if (aggro)
        {
            if (debug) print($"{name} Aggroed!");
            Tightness = tightnessAggro;
            aggroedThisFrame = true;
            foreach (var character in characters)
            {
                character.Controller.destinationScale = 1f;
                character.Controller.useNoise = false;
            }
            if (mainParty != this)
            {
                SetTargetParty(mainParty);
                mainParty.SetTargetParty(this, false);
            }
            onAggro.Invoke();
            aggroed = true;
        }
        else
        {
            if (debug) print($"{name} No Longer Aggroed.");
            Tightness = tightnessIdle;
            aggroed = false;
            TargetParty = null;
        }
    }

    public void SetTightness(float tightness)
    {
        foreach (var character in characters)
        {
            if (noise != null)
            {
                character.Controller.destinationScale = tightnessIdle;
            }
        }
    }


    // Events

    public void RivalPartyDied()
    {
        if (debug) print($"Rival Party {targetParty} Died.");
        if (isMainParty)
            Refresh();
        SetAggroed(false);
    }

    public void CharacterControlled(bool controllable, Character controlled)
    {
        if (debug) print($"Control changed for {controlled.body.name}.");
        if (controllable)
        {
            if (mainParty)
            {
                Leader = controlled;
            }
            else
            {
                foreach (var character in characters)
                {
                    if (!character.controllable)
                        Leader = character;
                }
            }
        }
    }

    public void CharacterAggroed()
    {
        if (!aggroed)
            SetAggroed(true);
    }

    public void CharacterDamaged()
    {
        if (debug) print($"{name} Damaged");
        CharacterAggroed();
    }

    public void CharacterDied(Character character)
    {
        Assert.IsTrue(character.alive == false);
        if (debug) print($"Character {character.body.name} Died.");
        foreach (var other in characters)
        {
            if (other.alive)
            {
                print($"Character {other.body.name} is still alive.");
                return;
            }
        }
        if (debug) print($"Party {name} Died.");
        allDead = true;
        SetAggroed(false);
        onAllDead.Invoke();
    }
}
