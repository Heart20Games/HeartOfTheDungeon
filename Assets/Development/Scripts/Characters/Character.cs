using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace HotD.Body
{
    using HotD.Castables;
    using Modifiers;
    using MyBox;
    using Selection;
    using HotD;
    using UIPips;
    using System.Collections;
    using static HotD.CharacterModes;
    using System;
    using global::Body.Behavior.ContextSteering;
    using global::Body.Behavior;
    using static global::Body.Behavior.ContextSteering.CSIdentity;

    public interface ICharacterInputs
    {
        public void MoveCharacter(Vector2 input);
        public void Aim(Vector2 input, bool aim = false);
        public void TriggerCastable(int idx);
        public void ReleaseCastable(int idx);
        public void Interact();
        public void FlipCamera();
    }
    public interface ICharacterStatus
    {
        public List<Status> Statuses { get; }
        public MaxModField<int> Armor { get; }
        public int CurrentHealth { get; set; }
        public bool AutoRespawn { get; set; }
        public bool AutoDespawn { get; set; }
    }
    public interface ICharacter : ICharacterStatus, ICharacterInputs, IIdentifiable, IDamageReceiver, IControllable, ICastCompatible
    {
        public CharacterBlock StatBlock { get; set; }
        public Transform Pivot { get; }
        public IMovement Movement { get; }
        public ArtRenderer ArtRenderer { get; }
        public TargetFinder TargetFinder { get; }
        public IBrain Brain { get; }
        public CSController Controller { get; }
        public void ListenForControlChanged(UnityAction<bool> action);
        public void ListenForDamage(UnityAction action);
        public void ListenForDeath(UnityAction<Character> action);
    }

    //[RequireComponent(typeof(Brain))]
    //[RequireComponent(typeof(Movement))]
    //[RequireComponent(typeof(Talker))]
    //[RequireComponent(typeof(Caster))]
    public class Character : AIdentifiable, ICharacter
    {
        [Foldout("Identity")]
        [SerializeField] private CharacterBlock statBlock;

        // Movement and Positioning
        [Foldout("Parts", true)]
        [Header("Movement and Positioning")]
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform body;
        [SerializeField] private Pivot moveReticle;
        private float baseOffset;
        private IMovement movement;


        // Properties
        public CharacterBlock StatBlock { get => statBlock; set => statBlock = value; }
        public Transform Pivot { get => pivot; }
        public IMovement Movement { get => movement; }
        public Transform Body { get => body; }
        //public new Transform Transform { get => transform; } // Contained in BaseMonoBehaviour
        public Transform WeaponLocation { get => weaponLocation; }
        public IWeaponDisplay WeaponDisplay { get => artRenderer; }
        public CastCoordinator Coordinator { get => artRenderer; }

        // State
        [Foldout("State", true)]
        [Header("State")]
        [SerializeField] private CharacterSettings settings;
        //public bool controllable = true;
        [ReadOnly][SerializeField] private bool aimActive = false;
        [Space]
        private readonly UnityEvent<bool> controlChanged = new();
        public void ListenForControlChanged(UnityAction<bool> action)
        {
            controlChanged.AddListener(action);
        }

        // Appearance
        [Foldout("Appearance", true)]
        [Header("Appearance")]
        [SerializeField] private ArtRenderer artRenderer;
        //public VFXEventController vfxController;
        [SerializeField] private LookAtCamera cameraPivot;
        [SerializeField] private CinemachineVirtualCamera orbitalCamera;
        [SerializeField] private CinemachineVirtualCamera aimCamera;

        public CinemachineVirtualCamera AimCamera => aimCamera;

        public ArtRenderer ArtRenderer { get => artRenderer; }

        // Collision
        [Foldout("Parts", true)]
        [Header("Collision")]
        [SerializeField] private Collider aliveCollider;
        [SerializeField] private Collider deadCollider;

        // Interaction, Selection, and Targeting
        [Foldout("Parts", true)]
        [Header("Interaction, Selection, and Targeting")]
        [SerializeField] private Interactor interactor;
        [SerializeField] private TargetFinder targetFinder;
        [SerializeField] private Selectable selectable;
        private ITalker talker;

        public TargetFinder TargetFinder { get => targetFinder; }

        // Behaviour
        [Header("Parts")]
        [HideInInspector] private IBrain brain;
        public IBrain Brain { get => brain; }
        public CSController Controller { get => brain.Controller; }

        public Collider AliveCollider => aliveCollider;

        // Casting
        [Foldout("Casting", true)]
        [Header("Casting")]
        [SerializeField] private Transform weaponLocation;
        [SerializeField] private Transform firingLocation;
        private ICaster caster;
        [SerializeField] private Loadout Loadout { get => statBlock == null ? null : statBlock.loadout; }
        public Transform FiringLocation { get => firingLocation; }

        // Identifiable
        public override Identity Identity
        {
            get => identity;
            set { identity = value; brain.Identity = value; }
        }
        public override string Name { get => statBlock == null ? null : statBlock.characterName; set => statBlock.characterName = value; }
        public override MaxModField<int> Health { get => health; }
        public override MaxModField<int> Armor { get => armor; }

        private int castableID;
        public int CastableID => castableID;

        // Status Effects
        [Foldout("Status Effects", true)]
        [Header("Status Effects")]
        [SerializeField] private List<Status> statuses;
        public List<Status> Statuses { get => statuses; }

        // Health and Damage
        [Foldout("Health and Damage", true)]
        [SerializeField] private PipGenerator pips;
        [SerializeField] private NumberPopup healthPopup;
        [SerializeField] private Transform damagePosition;
        [SerializeField] private MaxModField<int> health = new("Health", 5, 5);
        [SerializeField] private MaxModField<int> armor = new("Armor", 1, 1);
        [Space]
        [SerializeField] private UnityEvent onDamage;
        public void ListenForDamage(UnityAction action)
        {
            onDamage.AddListener(action);
        }
        public int CurrentHealth { get => Health?.current?.Value ?? -1; set { if (Health?.current != null) { Health.current.Value = value; } } }
        private int MaxHealth { get => Health?.max?.Value ?? -1; set { if (Health?.max != null) { Health.max.Value = value; } } }

        // Death and Respawning
        [Foldout("Death and Respawning", true)]
        [Header("Death and Respawning")]
        [SerializeField] private Transform spawn;
        [Space]
        // Respawn / Despawn
        [SerializeField] private bool autoRespawn;
        [ConditionalField("autoRespawn")] public float autoRespawnDelay;
        [SerializeField] private bool autoDespawn;
        [ConditionalField("autoDespawn")] public float autoDespawnDelay;
        [Space]
        // Events
        [SerializeField] private UnityEvent onRespawn;
        [SerializeField] private UnityEvent onDespawn;
        [Foldout("Death and Respawning")]
        [SerializeField] private UnityEvent<bool> onAlive;
        private UnityAction<Character> onDeath;
        public void ListenForDeath(UnityAction<Character> action)
        {
            onDeath += action;
        }
        public bool AutoRespawn { get => autoRespawn; set => autoRespawn = value; }
        public bool AutoDespawn { get => autoDespawn; set => autoDespawn = value; }

        [SerializeField] protected bool debug = false;

        // Actions
        public void MoveCharacter(Vector2 input) { movement.MoveVector = input; caster.SetFallback(movement.MoveVector.FullY(), true); }
        public void Aim(Vector2 input, bool aim = false) { if (aimActive || aim) caster.SetVector(input.FullY()); }
        public void TriggerCastable(int idx) { if (castables[idx] != null) caster.TriggerCastable(castables[idx]); castableID = idx; }
        public void ReleaseCastable(int idx) { if (castables[idx] != null) caster.ReleaseCastable(castables[idx]); castableID = idx; }
        public void Interact() { talker.Talk(); }
        public void FlipCamera() { if (cameraPivot != null) cameraPivot.FlipOverX(); }

        // Initialization
        public override void Awake()
        {
            base.Awake();

            // Body Initialization
            transform.rotation = new(0, 0, 0, 0);
            if (!firingLocation) firingLocation = transform;
            Awarn.IsNotNull(body, "Character has no Character (no body)", this);
            InitBody();

            // Components
            brain = GetIComponent<IBrain>();
            movement = GetIComponent<IMovement>();
            talker = GetIComponent<ITalker>();
            caster = GetIComponent<ICaster>();

            // Connections
            ConnectHealth();

            // Initialization
            movement.SetCharacter(this);
            InitializeCastables();
            InitializeSpawn();
            SetSpectatable(false);

            // Statblock connections
            if (statBlock != null)
            {
                statBlock.Initialize();
                statBlock.healthMax.updatedFinalInt.AddListener(health.max.SetValue); // max health dependent attribute;
                statBlock.armorClass.updatedFinalInt.AddListener(armor.max.SetValue); // armor class dependent attribute;
            }
        }

        private void Start()
        {
            // Healthbar subscription
            ConnectPips(pips); // Can't run on awake; requires other bits to be initialized.
            if (healthPopup != null)
                health.current.Subscribe(healthPopup.PopupChange);

            // Value Initialization
            Identity = Identity;

            if (statBlock != null)
            {
                MaxHealth = statBlock.MaxHealth;
                CurrentHealth = statBlock.MaxHealth;

                armor.max.Value = statBlock.ArmorClass;
                armor.current.Value = statBlock.ArmorClass;
            }

            SetMode(ControlMode.Brain);
        }

        private void OnDestroy()
        {
            DisconnectHealth();
            DisconnectPips(pips);
            if (healthPopup != null)
                health.current.UnSubscribe(healthPopup.PopupChange);
        }

        private void InitBody()
        {
            if (body != null)
            {
                CapsuleCollider capsuleCollider = body.GetComponent<CapsuleCollider>();
                baseOffset = capsuleCollider.height / 2;
                capsuleCollider.center = new Vector3(capsuleCollider.center.x, baseOffset, capsuleCollider.center.z);
            }
        }

        public void InitializeSpawn()
        {
            if (spawn != null)
            {
                spawn.SetPositionAndRotation(body.position, body.rotation);
                spawn.localScale = body.localScale;
            }
        }

        // Character Mode

        [SerializeField] private CharacterMode baseMode;
        public CharacterMode mode;
        public List<CharacterModifier> modifiers;
        public bool PlayerControlled
        {
            get => mode.PlayerControlled;
            set => SetPlayerControlled(value);
        }
        public void SetPlayerControlled(bool playerControlled, bool ignoreDeath=false)
        {
            if (ignoreDeath || mode.Alive)
            {
                SetMode(playerControlled ? ControlMode.Player : ControlMode.Brain); //!Controllable ? mode.controlMode : ControlMode.None);
            }
        }
        public bool Alive { get => mode.liveMode == LiveMode.Alive; }
        [ButtonMethod]
        public void ReapplyMode()
        {
            SetMode(baseMode);
        }
        public void AddOrRemoveModifier<T>(T key, bool add)
        {
            if (settings.TryGetModifier(key, out var modifier))
            {
                if (add) modifiers.Add(modifier);
                else modifiers.Remove(modifier);

                SetMode(baseMode);
            }
        }

        [SerializeField] private bool debugMode = false;
        public void SetMode<T>(T subMode) where T : Enum
        {
            if (settings.TryGetMode(subMode, out var mode, debugMode))
            {
                Print($"Switching to {mode.name}.", debugMode, this);
                SetMode(mode);
            }
            else
                Debug.LogWarning($"Can't find mode for \"{subMode}\"");

            Print($"{subMode}:{this.mode.name}", debugMode, this);
            Break(debugMode, this);
        }
        private void SetMode(CharacterMode new_mode)
        {
            // Keep Track of Old Mode and Base Mode
            CharacterMode oldMode = mode;
            baseMode = new_mode;

            // Apply Modifiers and Set Mode
            foreach (var modifier in modifiers)
            {
                new_mode = modifier.ModifyMode(new_mode);
            }
            mode = new_mode;

            // Apply the Chosen Mode

            // Movement
            Assert.IsNotNull(movement);
            movement.StopMoving();
            movement.MoveVector = new();
            movement.CanMove = new_mode.canMove;
            movement.UseGravity = new_mode.useGravity;

            // Behavior
            SetNonNullEnabled(brain, new_mode.controlMode == ControlMode.Brain);
            SetNonNullEnabled(caster, new_mode.useCaster);
            SetNonNullEnabled(interactor, new_mode.useInteractor);
            
            // Displays
            SetNonNullActive(moveReticle, new_mode.useMoveReticle);
            if(!artRenderer.KeepActive) SetNonNullActive(artRenderer, new_mode.displayable);
            if (pips != null) pips.SetDisplayMode(new_mode.pipMode);
            
            // Animation / Selection (Alive?)
            bool alive = new_mode.liveMode == LiveMode.Alive;
            brain.Alive = alive;
            if (artRenderer != null) artRenderer.Dead = !alive;
            SetNonNullEnabled(aliveCollider, alive);
            SetNonNullEnabled(deadCollider, !alive);
            selectable.IsSelectable = alive;
            
            // Death and Respawning
            if (new_mode.liveMode != oldMode.liveMode)
            {
                switch (new_mode.liveMode)
                {
                    case LiveMode.Alive:
                        {
                            switch (oldMode.liveMode)
                            {
                                case LiveMode.Dead: break;
                                case LiveMode.Despawned: Respawn(); break;
                            }
                            break;
                        }
                    case LiveMode.Dead: Die(autoDespawn, autoRespawn); break;
                    case LiveMode.Despawned: Despawn(); break;
                }
            }
            
            // Control Mode
            if ((new_mode.PlayerControlled || oldMode.PlayerControlled) && oldMode.controlMode != new_mode.controlMode)
            {
                controlChanged.Invoke(new_mode.PlayerControlled);
            }   
        }

        // Status Ticks

        private void Update()
        {
            StatusTick();
        }

        private void StatusTick()
        {
            foreach (Status status in statuses)
            {
                Assert.IsFalse(status.effect == null);
                status.effect.Tick(status.strength, this);
            }
        }

        // Controllabe, Spectatable, Displayable

        public void SetSpectatable(bool _spectatable)
        {
            if (!_spectatable)
            {
                Print($"Do not spectate {Name}.", debug);
                SetCamerasActive(false, false);
            }
            else if (PrimaryTargetingMethod(out var method))
            {
                Print($"Spectate {Name} based on primary targeting method: {method}", debug);
                SetCamerasActive
                (
                    method != AimingMethod.OverTheShoulder,
                    method == AimingMethod.OverTheShoulder
                );
            }
            else
            {
                Print($"Spectate {Name} using default camera (orbital).", debug);
                SetCamerasActive(true, false);
            }
        }

        private void SetCamerasActive(bool orbital, bool aim)
        {
            if (orbitalCamera != null)
                orbitalCamera.gameObject.SetActive(orbital);
            if (aimCamera != null)
                aimCamera.gameObject.SetActive(aim);
        }

        // Life, Death and Spawning

        private Coroutine autoRespawnCoroutine;
        private Coroutine autoDespawnCoroutine;

        public void Die(bool autoDespawn, bool autoRespawn)
        {
            Print($"{Name} died -- {mode.liveMode}", debug, this);
            Emotion = "dead";
            onDeath?.Invoke(this);
            onAlive?.Invoke(false);
            
            // Timers
            void respawnAfterDelay() => autoRespawnCoroutine ??= CallAfterDelay(TriggerRespawn, autoRespawnDelay);
            if (autoDespawn)
            {
                autoDespawnCoroutine ??= CallAfterDelay(TriggerDespawn, autoDespawnDelay, autoRespawn ? respawnAfterDelay : null);
            }
            else if (autoRespawn)
            {
                respawnAfterDelay();
            }
        }

        public void Refresh()
        {
            Print($"Refreshing {Name}.", debug);
            CurrentHealth = MaxHealth;
            Emotion = "neutral";
        }

        public void TriggerRespawn(bool finishCoroutine = false, UnityAction afterAction = null)
        {
            if (finishCoroutine)
                autoRespawnCoroutine = null;
            SetMode(LiveMode.Alive);
            afterAction?.Invoke();
            Respawn();
        }
        public void Respawn()
        {
            Print($"Respawing {Name}.", debug);
            if (mode.liveMode != LiveMode.Alive) SetMode(LiveMode.Alive);
            Print($"Respawn {Name}", debug);
            body.SetPositionAndRotation(spawn.position, spawn.rotation);
            body.localScale = spawn.localScale;
            //SetDisplayable(true);
            Refresh();
            onAlive?.Invoke(true);
            onRespawn?.Invoke();
        }

        public void TriggerDespawn(bool finishCoroutine = false, UnityAction afterAction = null)
        {
            if (finishCoroutine)
                autoDespawnCoroutine = null;
            SetMode(LiveMode.Despawned);
            afterAction?.Invoke();
        }
        public void Despawn()
        {
            Print($"Despawning {Name}");
            //SetDisplayable(false);
            onDespawn?.Invoke();
        }
        
        // Time Helpers
        public Coroutine CallAfterDelay(UnityAction<bool, UnityAction> action, float delay, UnityAction afterAction = null)
        {
            return StartCoroutine(CallAfterDelayCoroutine(action, delay, afterAction));
        }
        public IEnumerator CallAfterDelayCoroutine(UnityAction<bool, UnityAction> action, float delay, UnityAction afterAction = null)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke(true, afterAction);
        }

        // Health and Damage

        private int ApplyArmor(int oldValue, int newValue)
        {
            int change = newValue - oldValue;
            if (change< 0)
                change = Mathf.Min(change + armor.current.Value, 0);
            return oldValue + change;
        }

    private void ConnectHealth()
        {
            health.current.Subscribe(HealthChanged);
            health.current.Subscribe(ApplyArmor);
        }

        private void DisconnectHealth()
        {
            health.current.UnSubscribe(HealthChanged);
            health.current.UnSubscribe(ApplyArmor);
        }

        public void HealthChanged(int amount)
        {
            if (amount < CurrentHealth)
            {
                artRenderer.Hit();
                onDamage.Invoke();
            }
            if (amount <= 0f && Alive) SetMode(LiveMode.Dead);
        }

        public void SetDamagePosition(Vector3 damagePosition)
        {
            this.damagePosition.position = damagePosition;
        }

        public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
        {
            Print($"{Name} taking {damageAmount} from {id}. ({RelativeIdentity(id, Identity) != Identity.Friend})", debug);
            if (RelativeIdentity(id, Identity) != Identity.Friend)
            {
                CurrentHealth -= damageAmount;
            }
        }

        public void HealDamage(int damageAmount, Identity id = Identity.Neutral)
        {
            if (RelativeIdentity(id, Identity) != Identity.Foe)
            {
                CurrentHealth += damageAmount;
            }
        }

        // Damage Testing
        [Foldout("Health and Damage", true)] [SerializeField] private Identity testDamageIdentity = Identity.Neutral;
        [Foldout("Health and Damage")] [SerializeField] private int testDamageAmount = 1;

        [ButtonMethod]
        public void TestTakeDamage()
        {
            Print($"Dealing {testDamageAmount} Damage from {testDamageIdentity} Identity.", debug);
            TakeDamage(testDamageAmount, testDamageIdentity);
        }
        [ButtonMethod]
        public void TestHealDamage()
        {
            Print($"Healing {testDamageAmount} Damage from {testDamageIdentity} Identity.", debug);
            HealDamage(testDamageAmount, testDamageIdentity);
        }

        // Castables

        [Foldout("Casting", true)]
        [ReadOnly] public CastableItem[] castableItems = new CastableItem[5];
        [Foldout("Casting")][ReadOnly] public ICastable[] castables = new ICastable[5];
        public void InitializeCastables()
        {
            if (Loadout != null)
            {
                for (int i = 0; i < Mathf.Min(Loadout.weapons.Count, 2); i++)
                {
                    SetCastable(i, Loadout.weapons[i]);
                }
                for (int i = 0; i < Mathf.Min(Loadout.abilities.Count, 2); i++)
                {
                    SetCastable(2 + i, Loadout.abilities[i]);
                }
                SetCastable(4, Loadout.mobility);
            }
            brain?.RegisterCastables(castableItems);
        }

        public bool PrimaryTargetingMethod(out AimingMethod method)
        {
            if (castableItems?.Length > 0)
            {
                CastableItem item = castableItems[(int)Loadout.Slot.Weapon1];
                Awarn.IsNotNull(item, $"Weapon 1 Slot on {Name} is null.", this);
                if (item != null)
                {
                    method = item.aimingMethod;
                    return true;
                }
                else
                {
                    method = AimingMethod.Centered;
                    return false;
                }
            }
            else
            {
                method = AimingMethod.Centered;
                return false;
            }
        }

        public void SetCastable(int idx, CastableItem item)
        {
            if (castableItems[idx] != null)
                castables[idx]?.QueueAction(CastAction.UnEquip);
            castableItems[idx] = null;
            castables[idx] = null;

            if (Loadout != null && item != null)
            {
                if (item == null)
                    Debug.LogWarning($"Loadout {Loadout.name} has null item at index {idx}.", this);
                else if (item.prefab == null)
                    Debug.LogWarning($"Castable Item {item.name} has null prefab.", this);
                else
                {
                    castableItems[idx] = item;
                    castables[idx] = Instantiate(item.prefab, transform).GetComponent<ICastable>();
                    castables[idx].Initialize(this, item);
                    item.Equip(statBlock);
                }
            }
        }

        //public void SetAimModeActive(bool active)
        //{
        //    if (virtualCamera.TryGetComponent(out CinemachineInputProvider cip))
        //    {
        //        cip.enabled = !active;
        //    }
        //    aimActive = active;
        //}


        // Debugging

        public void PrintAbilities(List<Ability> abilities)
        {
            string toPrint = "";
            foreach (Ability _ability in abilities)
            {
                toPrint += _ability.gameObject.name + "\n";
            }
            print(toPrint);
        }

        public void PrintWeapons(List<Weapon> weapons)
        {
            string toPrint = "";
            foreach (Weapon _weapon in weapons)
            {
                toPrint += _weapon.gameObject.name + "\n";
            }
            print(toPrint);
        }
    }
}