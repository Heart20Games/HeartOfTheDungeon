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

    [RequireComponent(typeof(Brain))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Talker))]
    [RequireComponent(typeof(Caster))]
    public class Character : AIdentifiable, IDamageable, IControllable
    {
        // Movement and Positioning
        [Foldout("Parts", true)]
        [Header("Movement and Positioning")]
        public Transform body;
        public Transform pivot;
        public Pivot moveReticle;
        [HideInInspector] public Movement movement;
        [HideInInspector] public float baseOffset;

        // State
        [Foldout("State", true)]
        [Header("State")]
        public CharacterSettings settings;
        //public bool controllable = true;
        public bool aimActive = false;
        public bool alive = false;
        [Space]
        public UnityEvent<bool> onControl;
        public UnityEvent<bool> onSpectate;

        // Appearance
        [Foldout("Parts", true)]
        [Header("Appearance")]
        public ArtRenderer artRenderer;
        public VFXEventController vfxController;
        public CinemachineVirtualCamera virtualCamera;
        public CharacterBlock statBlock;

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
        [HideInInspector] public Talker talker;

        // Behaviour
        [Header("Parts")]
        [HideInInspector] public Brain brain;
        public CSController Controller { get => brain.controller; }

        // Casting
        [Foldout("Casting", true)]
        [Header("Casting")]
        [HideInInspector] public Caster caster;
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
        public UnityEvent<Character> onDeath;
        public UnityEvent onRespawn;
        public UnityEvent onDespawn;
        [Foldout("Death and Respawning")] public UnityEvent<bool> onAlive;

        public bool debug = false;

        public override void Awake()
        {
            base.Awake();

            // Body Initialization
            transform.rotation = new(0, 0, 0, 0);
            if (!firingLocation) firingLocation = transform;
            Awarn.IsNotNull(body, "Character has no Character");
            InitBody();

            // Components
            brain = GetComponent<Brain>();
            movement = GetComponent<Movement>();
            talker = GetComponent<Talker>();
            caster = GetComponent<Caster>();

            // Connections
            ConnectHealth();

            // Initialization
            InitMovement();
            InitializeCastables();
            InitializeSpawn();
            SetSpectatable(false);

            // Statblock connections
            statBlock.Initialize();
            statBlock.healthMax.updatedFinalInt.AddListener(health.max.SetValue); // max health dependent attribute;
            statBlock.armorClass.updatedFinalInt.AddListener(armor.max.SetValue); // armor class dependent attribute;
        }

        private void Start()
        {
            // Healthbar subscription
            ConnectPips(pips); // Can't run on awake; requires other bits to be initialized.
            if (healthPopup != null)
                health.current.Subscribe(healthPopup.PopupChange);

            // Value Initialization
            Identity = Identity;

            MaxHealth = statBlock.MaxHealth;
            CurrentHealth = statBlock.MaxHealth;

            armor.max.Value = statBlock.ArmorClass;
            armor.current.Value = statBlock.ArmorClass;

            SetMode(ControlMode.None);

        }

        private void InitMovement()
        {
            movement.body = body;
            movement.pivot = pivot;
            movement.artRenderer = artRenderer;
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
        public bool Controllable
        { 
            get => mode.Controllable;
            set => SetMode(value ? ControlMode.Player : !Controllable ? mode.controlMode : ControlMode.None);
        }
        public bool Alive { get => mode.liveMode == LiveMode.Alive; }
        public void SetMode<T>(T subMode) where T : Enum
        {
            if (settings.TryGetMode(subMode, out var mode))
                SetMode(mode);
            else
                Debug.LogWarning($"Can't find live mode for \"{subMode}\"");
        }
        public void SetMode(CharacterMode mode)
        {
            if (this.mode.name != mode.name)
            {
                movement.SetMoveVector(new());
                brain.Enabled = mode.controlMode == ControlMode.Brain;
                movement.canMove = mode.moveMode == MovementMode.Active;
                movement.applyGravity = mode.moveMode != MovementMode.Disabled;
                SetNonNullActive(artRenderer, mode.displayable);
                SetNonNullActive(moveReticle, mode.useMoveReticle);
                SetNonNullEnabled(interactor, mode.useInteractor);
                SetNonNullEnabled(caster, mode.useCaster);
                if (pips != null) pips.SetDisplayMode(mode.pipMode);
                bool alive = mode.liveMode == LiveMode.Alive;
                brain.Alive = alive;
                if (artRenderer != null) artRenderer.Dead = !alive;
                SetNonNullEnabled(aliveCollider, alive);
                SetNonNullEnabled(deadCollider, !alive);
                switch (mode.liveMode)
                {
                    case LiveMode.Alive:
                        {
                            switch (this.mode.liveMode)
                            {
                                case LiveMode.Dead: Refresh(); break;
                                case LiveMode.Despawned: Respawn(); break;
                            }
                            break;
                        }
                    case LiveMode.Dead: Die(autoDespawn, autoRespawn); break;
                    case LiveMode.Despawned: Despawn(); break;
                }
                if ((mode.Controllable || this.mode.Controllable) && this.mode.controlMode != mode.controlMode)
                {
                    onControl.Invoke(mode.Controllable);
                }
                this.mode = mode;
            }
        }
        public void SetNonNullActive(Component component, bool active)
        {
            if (component != null)
                component.gameObject.SetActive(active);
        }
        public void SetNonNullEnabled(Behaviour component, bool enabled)
        {
            if (component != null)
                component.enabled = enabled;
        }
        public void SetNonNullEnabled(Collider collider, bool enabled)
        {
            if (collider != null)
                collider.enabled = enabled;
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
            onSpectate.Invoke(_spectatable);
        }

        // Life, Death and Spawning

        private Coroutine autoRespawnCoroutine;
        private Coroutine autoDespawnCoroutine;

        public void Die(bool autoDespawn, bool autoRespawn)
        {
            // Timers
            void respawnAfterDelay() => autoRespawnCoroutine ??= CallAfterDelay(TriggerRespawn, autoRespawnDelay);
            if (autoDespawn)
                autoDespawnCoroutine ??= CallAfterDelay(TriggerDespawn, autoDespawnDelay, respawnAfterDelay);
            else if (autoRespawn)
                respawnAfterDelay();

            Emotion = "dead";
            onDeath.Invoke(this);
        }

        public void Refresh()
        {
            CurrentHealth = MaxHealth;
            Emotion = "neutral";
        }

        public void TriggerRespawn(bool finishCoroutine = false, UnityAction afterAction = null)
        {
            if (finishCoroutine)
                autoRespawnCoroutine = null;
            SetMode(LiveMode.Alive);
            afterAction?.Invoke();
        }
        public void Respawn()
        {
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
            Print($"Despawn {Name}", true);
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
            if (amount <= 0f && alive) SetMode(LiveMode.Dead);
        }

        public void SetDamagePosition(Vector3 damagePosition)
        {
            this.damagePosition.position = damagePosition;
        }

        public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
        {
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
            print($"Dealing {testDamageAmount} Damage from {testDamageIdentity} Identity.");
            TakeDamage(testDamageAmount, testDamageIdentity);
        }
        [ButtonMethod]
        public void TestHealDamage()
        {
            print($"Healing {testDamageAmount} Damage from {testDamageIdentity} Identity.");
            HealDamage(testDamageAmount, testDamageIdentity);
        }

        // Castables

        public readonly CastableItem[] castableItems = new CastableItem[5];
        public readonly Castable[] castables = new Castable[5];
        public void InitializeCastables()
        {
            if (Loadout != null)
            {
                for (int i = 0; i < Mathf.Min(Loadout.abilities.Count, 2); i++)
                {
                    SetCastable(i, Loadout.abilities[i]);
                }
                for (int i = 0; i < Mathf.Min(Loadout.weapons.Count, 2); i++)
                {
                    SetCastable(2 + i, Loadout.weapons[i]);
                }
                SetCastable(4, Loadout.mobility);
            }
            if (brain != null)
                brain.RegisterCastables(castableItems);
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

        public void TriggerCastable(ICastable castable)
        {
            if (caster != null && caster.enabled)
            {
                caster.Castable = castable;
                caster.Trigger();
            }
        }

        public void ReleaseCastable(ICastable castable)
        {
            if (caster != null && caster.enabled && caster.Castable == castable)
            {
                caster.Release();
            }
            else castable?.Release();
        }

        public void SetAimModeActive(bool active)
        {
            if (virtualCamera.TryGetComponent(out CinemachineInputProvider cip))
            {
                cip.enabled = !active;
            }
            aimActive = active;
        }


        // Actions
        public void MoveCharacter(Vector2 input) { movement.SetMoveVector(input); caster.SetFallback(movement.moveVector.FullY(), true); }
        public void Aim(Vector2 input, bool aim=false) { if (aimActive || aim) caster.SetVector(input.FullY()); }
        public void TriggerCastable(int idx) { TriggerCastable(castables[idx]); }
        public void ReleaseCastable(int idx) { ReleaseCastable(castables[idx]); }
        public void Interact() { talker.Talk(); }
        public void AimMode(bool active) { SetAimModeActive(active); }


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