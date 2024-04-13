using Body;
using Body.Behavior;
using Body.Behavior.ContextSteering;
using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static HotD.CharacterModes;

public class Party : BaseMonoBehaviour
{
    public string Name { get => gameObject.name; set => gameObject.name = value; }

    [Foldout("Party Members", true)]
    [Header("Party Members")]
    public Character leader;
    public Character Leader { get => leader; set => SetLeader(value); }
    public List<Character> members = new();
    [Foldout("Party Members")] public List<Character> pets = new();

    static List<Party> parties = new();
    public static Party mainParty;
    public bool isMainParty = false;

    [Foldout("Status Events", true)]
    [Header("Status Events")]
    [ReadOnly] public bool aggroed = false;
    public UnityEvent onAggro = new();
    [ReadOnly] public bool allDead = false;
    public UnityEvent onAllDead = new();
    [Foldout("Status Events")] private bool aggroedThisFrame = false;

    [Foldout("Follow Target", true)]
    [Header("Follow Target")]
    public Transform followTargeter;
    public bool useLeaderAsFollowTargeter;
    public Transform defaultFollowTarget;
    [Foldout("Follow Target")][ReadOnly][SerializeField]
    private Party targetParty;
    public Party TargetParty { get => targetParty; set => SetTargetParty(value); }

    [Foldout("Noise and Scaling", true)]
    [Header("Noise and Scaling")]
    public MovementNoise noise;
    public float tightness = 1f;
    public float tightnessIdle = 1f;
    [Foldout("Noise and Scaling")] public float tightnessAggro = 2f;
    public float Tightness { get => tightness; set => SetTightness(value); }

    [Foldout("Config", true)]
    [Header("Config")]
    [Space()]
    public bool autoRespawnAll;
    [ConditionalField("autoRespawnAll")] public float autoRespawnDelay;
    [Space()]
    public bool autoDespawnAll;
    [Foldout("Config")][ConditionalField("autoDespawnAll")] public float autoDespawnDelay;

    [Space()]
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
        foreach (var character in members)
        {
            if (followTargeter)
                character.brain.Target = followTargeter;
            if (noise != null)
            {
                character.Controller.noise = noise;
                character.Controller.destinationScale = tightnessIdle;
            }
            if (autoRespawnAll)
            {
                character.autoRespawn = true;
                character.autoRespawnDelay = autoRespawnDelay;
            }
            if (autoDespawnAll)
            {
                character.autoDespawn = true;
                character.autoDespawnDelay = autoDespawnDelay;
            }
            character.Controller.onFoeContextActive.AddListener(CharacterAggroed);
            character.onDmg.AddListener(CharacterDamaged);
            character.onDeath.AddListener(CharacterDied);
            character.onControl.AddListener((bool controlled) => CharacterControlled(controlled, character));
            if (debug) print($"Character {character.name} connected to party {name}.");
        }
    }


    // Actions

    [ButtonMethod]
    public void Refresh()
    {
        foreach (var character in members)
            character.Refresh();
    }

    [ButtonMethod]
    public void Respawn()
    {
        foreach (var character in members)
            character.Respawn();
    }

    [ButtonMethod]
    public void Despawn()
    {
        foreach (Character character in members)
        {
            character.SetMode(LiveMode.Despawned);
        }
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
        if (debug) print($"Set {name}'s follow target: {(transform == null ? "none" : transform.name)}");
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
            foreach (var character in members)
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
        foreach (var character in members)
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
                foreach (var character in members)
                {
                    if (!character.Controllable)
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
        Assert.IsTrue(character.Alive == false);
        if (debug) print($"Character {character.body.name} Died.");
        foreach (var other in members)
        {
            if (other.Alive)
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
