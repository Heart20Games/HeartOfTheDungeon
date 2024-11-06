using Body.Behavior.ContextSteering;
using Body.Behavior.Tree;
using Body.Behavior;
using HotD.Castables;
using HotD;
using System.Collections.Generic;
using UnityEngine;
using HotD.Body;
using Selection;
using Modifiers;
using UnityEngine.Events;
using UIPips;
using static Body.Behavior.ContextSteering.CSIdentity;

public abstract class AIdentifiableStub : BaseMonoBehaviour, IIdentifiable
{
    // Fields
    public Portraits portraits = null;
    public Identity identity = Identity.Neutral;
    public ModField<string> emotion = null;

    // Events
    public UnityEvent<PortraitImage> onPortrait;
    public UnityEvent<Sprite> onImage;

    // Properties
    public virtual Identity Identity { get => Identity.Neutral; set => NULL(); }
    public virtual PortraitImage Portrait
    {
        get => new();
        set => NULL();
    }
    public virtual Sprite Image
    {
        get => null;
        set => NULL();
    }
    public virtual string Emotion { get => ""; set => NULL(); }
    public virtual string Description { get => ""; set => NULL(); }

    // Mod Fields
    public virtual MaxModField<int> Health { get => null; }
    public virtual MaxModField<int> Armor { get => null; }

    protected void NULL() { /* Do Nothing */ }

    // Portrait Connections
    private void UpdatePortraits(string _emotion) { }
    public void ConnectPortrait(UnityAction<PortraitImage> action, bool initialize = false) { }
    public void DisconnectPortrait(UnityAction<PortraitImage> action) { }

    public void ConnectImage(UnityAction<Sprite> action, bool initialize = false) { }
    public void DisconnectImage(UnityAction<Sprite> action) { }


    // Pip Connections
    public void ConnectPipType(MaxModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total, bool initialize = false) { }
    public virtual void ConnectPips(PipGenerator generator, bool initialize = false) { }

    public void DisconnectPipType(MaxModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total) { }
    public virtual void DisconnectPips(PipGenerator generator) { }
}

public class CharacterStub : AIdentifiableStub, ICharacter
{
    public CharacterBlock StatBlock { get => null; set => NULL(); }
    public Transform Pivot => null;
    public IMovement Movement => null;
    public ArtRenderer ArtRenderer => null;
    public TargetFinder TargetFinder => null;
    public IBrain Brain => null;
    public CSController Controller => null;

    private int currentHealth = 1;
    private bool autoRespawn = false;
    private bool autoDespawn = false;
    public List<Status> Statuses => new();
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public bool AutoRespawn { get => autoRespawn; set => autoRespawn = value; }
    public bool AutoDespawn { get => autoDespawn; set => autoDespawn = value; }

    private bool playerControlled = false;
    public bool PlayerControlled { get => playerControlled; set => SetPlayerControlled(value); }
    public IWeaponDisplay WeaponDisplay => null;
    public Transform Body => null;
    public Transform WeaponLocation => null;
    public Transform FiringLocation => null;
    public CastCoordinator Coordinator => null;
    public int CastableID => 0;

    public void Aim(Vector2 input, bool aim = false) { }
    public void MoveCharacter(Vector2 input) { }
    public void FlipCamera() { }
    public void Interact() { }
    public void ReleaseCastable(int idx) { }
    public void TriggerCastable(int idx) { }

    public void ListenForControlChanged(UnityAction<bool> action) { }
    public void ListenForDamage(UnityAction action) { }
    public void ListenForDeath(UnityAction<Character> action) { }

    public void SetDamagePosition(Vector3 damagePosition) { }
    public void TakeDamage(int amount, CSIdentity.Identity id = CSIdentity.Identity.Neutral) { }
    public void SetPlayerControlled(bool playerControlled, bool ignoreDeath=false) { }
    public void SetSpectatable(bool spectable) { }
}

public class MovementStub : BaseMonoBehaviour, IMovement
{
    private readonly MovementTemplate settingsTemplate;
    private MoveSettings settings;
    private protected List<MoveModifier> modifiers;
    private bool useGravity;
    private bool canMove;
    private Vector2 moveVector;
    private bool shouldFlip;
    private float timeScale;
    protected bool settingsInitialized = false;
    protected bool settingsModified = false;

    public MoveSettings Settings
    {
        get
        {
            if (!settingsInitialized && settingsTemplate != null)
            {
                modifiers?.AddRange(settingsTemplate.modifiers);
                settingsModified = false;
                settingsInitialized = true;
            }
            if (!settingsModified)
            {
                settings = settingsTemplate.settings.Modified(modifiers);
                settingsModified = true;
            }
            return settings;
        }
        set => settings = value.Modified(modifiers);
    }
    public List<MoveModifier> Modifiers { get => modifiers; set => modifiers = value; }
    public bool UseGravity { get => useGravity; set => useGravity = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public Vector2 MoveVector { get => moveVector; set => moveVector = value; }
    public bool ShouldFlip { get => shouldFlip; set => shouldFlip = value; }
    public float TimeScale { get => timeScale; set => SetTimeScale(value); }

    public void AddOrRemoveModifiers(List<MoveModifier> modifiers, bool add)
    {
        if (add)
        {
            Modifiers.AddRange(modifiers);
            settingsModified = modifiers.Count > 0;
        }
        else
        {
            foreach (var modifier in modifiers)
            {
                for (int i = Modifiers.Count - 1; i >= 0; i--)
                {
                    if (Modifiers[i].Equals(modifier))
                    {
                        Modifiers.RemoveAt(i);
                        settingsModified = false;
                    }
                }
            }
        }
    }

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