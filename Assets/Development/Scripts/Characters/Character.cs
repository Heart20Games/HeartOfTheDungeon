using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Body
{
    using Behavior;
    using Body.Behavior.ContextSteering;
    using HotD.Castables;
    using Modifiers;
    using MyBox;
    using Selection;
    using HotD;
    using UIPips;
    using static Body.Behavior.ContextSteering.CSIdentity;
    using System.Collections;
    using static HotD.CharacterModes;
    using System;
    using HotD.Body;

    //[RequireComponent(typeof(Brain))]
    //[RequireComponent(typeof(Movement))]
    //[RequireComponent(typeof(Talker))]
    //[RequireComponent(typeof(Caster))]
    public class Character : AIdentifiable, IDamageable, IControllable
    {
        [Foldout("Identity")]
        public CharacterBlock statBlock;

        // Movement and Positioning
        [Foldout("Parts", true)]
        [Header("Movement and Positioning")]
        public Transform body;
        public Transform pivot;
        public Pivot moveReticle;
        [HideInInspector] public IMovement movement;
        [HideInInspector] public float baseOffset;

        // State
        [Foldout("State", true)]
        [Header("State")]
        public CharacterSettings settings;
        //public bool controllable = true;
        public bool aimActive = false;
        [Space]
        public UnityEvent<bool> onControl;

        // Appearance
        [Foldout("Appearance", true)]
        [Header("Appearance")]
        public ArtRenderer artRenderer;
        public VFXEventController vfxController;
        public LookAtCamera cameraPivot;
        public CinemachineVirtualCamera orbitalCamera;
        public CinemachineVirtualCamera aimCamera;

        // Collision
        [Foldout("Parts", true)]
        [Header("Collision")]
        public Collider aliveCollider;
        public Collider deadCollider;

        // Interaction, Selection, and Targeting
        [Foldout("Parts", true)]
        [Header("Interaction, Selection, and Targeting")]
        public Interactor interactor;
        public TargetFinder targetFinder;
        public Selectable selectable;
        [HideInInspector] public ITalker talker;

        // Behaviour
        [Header("Parts")]
        [HideInInspector] public IBrain brain;
        public CSController Controller { get => brain.Controller; }

        // Casting
        [Foldout("Casting", true)]
        [Header("Casting")]
        [HideInInspector] public ICaster caster;
        public Transform weaponLocation;
        public Transform firingLocation;
        public Loadout Loadout { get => statBlock == null ? null : statBlock.loadout; }

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

        // Status Effects
        [Foldout("Status Effects", true)]
        [Header("Status Effects")]
        public List<Status> statuses;

        // Health and Damage
        [Foldout("Health and Damage", true)]
        public PipGenerator pips;
        public NumberPopup healthPopup;
        public Transform damagePosition;
        public MaxModField<int> health = new("Health", 5, 5);
        public MaxModField<int> armor = new("Armor", 1, 1);
        [Space]
        public UnityEvent onDmg;
        public int CurrentHealth { get => health.current.Value; set => health.current.Value = value; }
        public int MaxHealth { get => health.max.Value; set => Health.max.Value = value; }

        // Death and Respawning
        [Foldout("Death and Respawning", true)]
        [Header("Death and Respawning")]
        public Transform spawn;
        [Space]
        // Respawn / Despawn
        public bool autoRespawn;
        [ConditionalField("autoRespawn")] public float autoRespawnDelay;
        public bool autoDespawn;
        [ConditionalField("autoDespawn")] public float autoDespawnDelay;
        [Space]
        // Events
        public UnityAction<Character> onDeath;
        public UnityEvent onRespawn;
        public UnityEvent onDespawn;
        [Foldout("Death and Respawning")] public UnityEvent<bool> onAlive;

        public bool debug = false;

        public int CastableID => castableID;

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
            Awarn.IsNotNull(body, "Character has no Character (no body)");
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

        public CharacterMode mode;
        public ModField<bool> brainDead = new("Brain Dead", false);
        public bool BrainDead
        {
            get => brainDead.Value;
            set
            {
                brainDead.Value = value;
                SetMode(mode);
            }
        }
        public bool PlayerControlled
        {
            get => mode.PlayerControlled;
            set => SetMode(value ? ControlMode.Player : ControlMode.Brain); //!Controllable ? mode.controlMode : ControlMode.None);
        }
        public bool Alive { get => mode.liveMode == LiveMode.Alive; }
        public void SetMode<T>(T subMode) where T : Enum
        {
            if (settings.TryGetMode(subMode, out var mode))
                SetMode(mode);
            else
                Debug.LogWarning($"Can't find live mode for \"{subMode}\"");
        }
        private void SetMode(CharacterMode new_mode)
        {
            CharacterMode oldMode = this.mode;
            this.mode = new_mode;

            movement.StopMoving();
            movement.MoveVector = new();
            movement.CanMove = new_mode.moveMode == MovementMode.Active && !BrainDead;
            movement.UseGravity = new_mode.moveMode != MovementMode.Disabled;
            SetNonNullEnabled(brain, !new_mode.PlayerControlled && !BrainDead);
            SetNonNullEnabled(caster, new_mode.useCaster && !BrainDead);
            SetNonNullActive(artRenderer, new_mode.displayable);
            SetNonNullActive(moveReticle, new_mode.useMoveReticle && !BrainDead);
            SetNonNullEnabled(interactor, new_mode.useInteractor && !BrainDead);
            if (pips != null) pips.SetDisplayMode(new_mode.pipMode);
                
            bool alive = new_mode.liveMode == LiveMode.Alive;
            brain.Alive = alive;
            if (artRenderer != null) artRenderer.Dead = !alive;
            SetNonNullEnabled(aliveCollider, alive);
            SetNonNullEnabled(deadCollider, !alive);
            selectable.IsSelectable = alive;
            
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
                
            if ((new_mode.PlayerControlled || oldMode.PlayerControlled) && oldMode.controlMode != new_mode.controlMode)
            {
                onControl.Invoke(new_mode.PlayerControlled);
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
            Print($"{Name} died -- {mode.liveMode}");
            Emotion = "dead";
            onDeath?.Invoke(this);
            
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
            Print($"Refreshing {Name}.");
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
            Print($"Respawing {Name}.");
            SetMode(LiveMode.Alive);
            Print($"Respawn {Name}", debug);
            body.SetPositionAndRotation(spawn.position, spawn.rotation);
            body.localScale = spawn.localScale;
            //SetDisplayable(true);
            Refresh();
            onRespawn.Invoke();
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
            onDespawn.Invoke();
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

        private void ConnectHealth()
        {
            health.current.Subscribe(HealthChanged);
            health.current.Subscribe((int oldValue, int newValue) =>
            {
                int change = newValue - oldValue;
                if (change < 0)
                    change = (int)Mathf.Min(change + armor.current.Value, 0);
                return oldValue + change;
            });
        }

        public void HealthChanged(int amount)
        {
            if (amount < CurrentHealth)
            {
                artRenderer.Hit();
                onDmg.Invoke();
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
        [Foldout("Casting")][ReadOnly] public Castable[] castables = new Castable[5];
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
            if (brain != null)
                brain.RegisterCastables(castableItems);
        }

        public bool PrimaryTargetingMethod(out AimingMethod method)
        {
            if (castableItems?.Length > 0)
            {
                CastableItem item = castableItems[(int)Loadout.Slot.Weapon1];
                Awarn.IsNotNull(item, $"Weapon 1 Slot on {Name} is null.");
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
                castables[idx]?.UnEquip();
            castableItems[idx] = null;
            castables[idx] = null;

            if (Loadout != null && item != null)
            {
                if (item == null)
                    Debug.LogWarning($"Loadout {Loadout.name} has null item at index {idx}.");
                else if (item.prefab == null)
                    Debug.LogWarning($"Castable Item {item.name} has null prefab.");
                else
                {
                    castableItems[idx] = item;
                    castables[idx] = Instantiate(item.prefab, transform);
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