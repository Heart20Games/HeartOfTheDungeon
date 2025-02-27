using Body;
using Body.Behavior;
using Body.Behavior.ContextSteering;
using HotD;
using HotD.Body;
using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static HotD.CharacterModes;

public class Party : BaseMonoBehaviour
{
    public new string Name { get => gameObject.name; set => gameObject.name = value; }

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
            followTargeter = leader.Body.transform;
        if (isMainParty)
            mainParty = this;
        if (leader != null)
        {
            defaultFollowTarget = leader.Body.transform;
            SetFollowTarget(defaultFollowTarget);
        }
        foreach (var character in members)
        {
            if (followTargeter)
                character.Brain.Target = followTargeter;
            if (noise != null)
            {
                character.Controller.noise = noise;
                character.Controller.destinationScale = tightnessIdle;
            }
            if (autoRespawnAll)
            {
                character.AutoRespawn = true;
                character.autoRespawnDelay = autoRespawnDelay;
            }
            if (autoDespawnAll)
            {
                character.AutoDespawn = true;
                character.autoDespawnDelay = autoDespawnDelay;
            }
            character.Controller.onFoeContextActive.AddListener(CharacterAggroed);
            character.ListenForDamage(MemberDamaged);
            character.ListenForDeath(MemberDied);
            character.ListenForControlChanged((bool controlled) => CharacterControlled(controlled, character));
            Print($"Character {character.name} connected to party {name}.", debug);
        }
    }


    // Actions

    [ButtonMethod]
    public void Refresh()
    {
        foreach (var character in members)
        {
            Print($"Refresh {character.Name}.", debug, this);
            character.Refresh();
        }
    }

    [ButtonMethod]
    public void Respawn()
    {
        foreach (var character in members)
        {
            Print($"Respawn {character.Name}.", debug, this);
            character.Respawn();
        }
    }

    [ButtonMethod]
    public void Despawn()
    {
        foreach (Character character in members)
        {
            Print($"Despawn {character.Name}.", debug, this);
            character.SetMode(LiveMode.Despawned);
        }
    }

    public void ResetAttributes(int baseSkillPoints = 0)
    {
        foreach (var member in members)
        {
            member.StatBlock.strength.BaseValue = 0;
            member.StatBlock.constitution.BaseValue = 0;
            member.StatBlock.intelligence.BaseValue = 0;
            member.StatBlock.dexterity.BaseValue = 0;
            member.StatBlock.skillPoints = baseSkillPoints;
        }
    }

    public void LevelUp(bool pullUpCharacterSheet = false, int amount = 1)
    {
        foreach (var member in members)
            member.StatBlock.skillPoints += amount;

        if (isMainParty && pullUpCharacterSheet)
            Game.main.SetMode(GameModes.Menu.CharacterSheet);
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
        Print($"Set {name}'s follow target: {(transform == null ? "none" : transform.name)}", debug);
        if (followTargeter.TryGetComponent(out Brain brain))
            brain.Target = target;
    }

    public void SetLeader(Character character)
    {
        leader = character;
        defaultFollowTarget = leader.Body.transform;
        if (!aggroed)
            SetFollowTarget(defaultFollowTarget);
    }

    public void SetAggroed(bool aggro)
    {
        if (aggro)
        {
            Print($"{name} Aggroed!", debug);
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
            Print($"{name} No Longer Aggroed.", debug);
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
        Print($"Rival Party {targetParty} Died.", debug);
        if (isMainParty)
            Refresh();
        SetAggroed(false);
    }

    public void CharacterControlled(bool controllable, Character controlled)
    {
        Print($"Control changed for {controlled.Body.name}.", debug);
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
                    if (!character.PlayerControlled)
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

    public void MemberDamaged()
    {
        Print($"{name} Damaged", debug);
        CharacterAggroed();
    }

    [HideInInspector] public UnityAction<Character> onMemberDeath;
    public void MemberDied(Character character)
    {
        Print($"Character {character.Body.name} Died. ({character.mode.liveMode})", debug);
        Assert.IsFalse(character.Alive);

        bool anyAlive = false;
        foreach (var other in members)
        {
            if (other.Alive)
            {
                Print($"Character {other.Body.name} is still alive.", debug);
                anyAlive = true;
            }
        }
        
        if (!anyAlive)
        {
            Print($"Party {name} Died.", debug);
            allDead = true;
            SetAggroed(false);
            onAllDead.Invoke();
        }

        onMemberDeath?.Invoke(character);
    }
}
