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
    using System.Collections;
    using static Body.Behavior.ContextSteering.CSIdentity;

    [RequireComponent(typeof(Brain))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Talker))]
    [RequireComponent(typeof(Caster))]
    public class Character : AIdentifiable, IDamageable, IBrainable
    {
        [Foldout("Movement and Positioning", true)]
        [Header("Movement and Positioning")]
        public Transform body;
        public Transform pivot;
        public Pivot moveReticle;
        [HideInInspector] public Movement movement;
        [HideInInspector] public float baseOffset;

        [Foldout("State", true)]
        [Header("State")]
        public bool controllable = true;
        public bool aimActive = false;
        public bool alive = false;
        [Space]
        public UnityEvent<bool> onControl;

        [Foldout("Appearance", true)]
        [Header("Appearance")]
        public ArtRenderer artRenderer;
        public VFXEventController vfxController;
        public CinemachineVirtualCamera virtualCamera;
        public CharacterBlock statBlock;

        [Foldout("Collision", true)]
        [Header("Collision")]
        public Collider aliveCollider;
        public Collider deadCollider;

        [Foldout("Interaction, Selection, and Targeting", true)]
        [Header("Interaction, Selection, and Targeting")]
        public Interactor interactor;
        public TargetFinder targetFinder;
        [HideInInspector] public Talker talker;

        [Foldout("Behaviour", true)]
        [Header("Behaviour")]
        [HideInInspector] public Brain brain;
        public CSController Controller { get => brain.controller; }

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
            set
            {
                identity = value;
                brain.Identity = value;
            }
        }
        public override string Name { get => statBlock == null ? null : statBlock.characterName; set => statBlock.characterName = value; }
        public override ModField<int> Health { get => health; }

        [Foldout("Status Effects", true)]
        [Header("Status Effects")]
        public List<Status> statuses;

        [Foldout("Health and Damage", true)]
        [Header("Health and Damage")]
        public HealthPips healthBar;
        public bool alwaysHideHealth = false;
        public float hideHealthWaitTime = 15f;
        public ModField<int> health = new("Health", 5, 5);
        [Space]
        public UnityEvent onDmg;
        public int CurrentHealth
        {
            get { return health.current.Value; }
            set { SetCurrentHealth(value); }
        }
        public int MaxHealth
        {
            get { return health.max.Value; }
            set { SetMaxHealth(value); }
        }

        [Foldout("Death and Respawning", true)]
        [Header("Death and Respawning")]
        public Transform spawn;
        [Space]
        public UnityEvent<Character> onDeath;
        public UnityEvent onRespawn;
        [Foldout("Death and Respawning")] public UnityEvent<bool> onAlive;


        // Initialization
        private void Awake()
        {
            // Body Initialization
            transform.rotation = new(0, 0, 0, 0);
            Awarn.IsNotNull(body, "Character has no Character");
            InitBody();

            // Components
            brain = GetComponent<Brain>();
            movement = GetComponent<Movement>();
            talker = GetComponent<Talker>();
            caster = GetComponent<Caster>();

            // Connections
            ConnectAlive();
            ConnectControl();

            // Initialization
            InitializeCastables();
            InitializeSpawn();
            SetControllable(false);
        }

        private void Start()
        {
            // Healthbar subscription
            if (healthBar != null)
            {
                //healthBar.enabled = false;
                //healthBar.SetHealthBase(CurrentHealth, MaxHealth);
                Health.Subscribe(healthBar.SetHealth, healthBar.SetHealthTotal);
            }

            // Value Initialization
            Identity = Identity;
            SetAlive(true);
            MaxHealth = statBlock.MaxHealth;
            CurrentHealth = statBlock.MaxHealth;

            // Attribute subscriptions
            statBlock.healthMax.updatedFinalInt.AddListener(health.max.SetValue); // max health dependent attribute
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
                spawn.position = body.position;
                spawn.rotation = body.rotation;
                spawn.localScale = body.localScale;
            }
        }


        // Respawning and Refreshing

        public void Refresh()
        {
            CurrentHealth = MaxHealth;
            SetAlive(true);
        }

        public void Respawn()
        {
            body.SetPositionAndRotation(spawn.position, spawn.rotation);
            body.localScale = spawn.localScale;
            Refresh();
        }

        // Updates

        private void Update()
        {
            StatusTick();
        }


        // Statuses

        private void StatusTick()
        {
            foreach (Status status in statuses)
            {
                Assert.IsFalse(status.effect == null);
                status.effect.Tick(status.strength, this);
            }
        }


        // State

        public void SetDisplayable(bool _displayable)
        {
            artRenderer.enabled = _displayable;
        }

        public void SetControllable(bool _controllable)
        {
            brain.Enabled = !_controllable;
            controllable = _controllable;
            onControl.Invoke(_controllable);
        }

        public void SetBrainable(bool brainable)
        {
            brain.Enabled = brainable;
            movement.enabled = brainable;
        }

        public void ConnectControl()
        {
            if (moveReticle != null) onControl.AddListener(moveReticle.gameObject.SetActive);
            if (virtualCamera != null) onControl.AddListener(virtualCamera.gameObject.SetActive);
            if (interactor != null) onControl.AddListener(DoEnable(interactor));
        }
        private UnityAction<bool> DoEnable(Behaviour behaviour, bool disable=false) { return (bool enable) => { behaviour.enabled = enable && !disable; }; }


        // Health

        private void ConnectAlive()
        {
            onAlive.AddListener((bool alive) => { brain.Alive = alive; });
            onAlive.AddListener(DoEnable(movement));
            onAlive.AddListener(DoEnable(caster));

            if (artRenderer != null) onAlive.AddListener((bool alive) => { artRenderer.Dead = !alive; });
            if (aliveCollider != null) onAlive.AddListener((bool alive) => { aliveCollider.enabled = alive; });
            if (deadCollider != null) onAlive.AddListener((bool alive) => { deadCollider.enabled = alive; });
        }

        public void SetAlive(bool alive)
        {
            movement.SetMoveVector(new());
            bool died = !alive && this.alive;
            this.alive = alive;
            onAlive.Invoke(alive);
            if (died)
                onDeath.Invoke(this);
        }

        public void SetMaxHealth(int amount)
        {
            Health.max.Value = amount;
        }

        public void SetCurrentHealth(int amount)
        {
            int prevHealth = Health.current.Value;
            Health.current.Value = Mathf.Min(amount, Health.max.Value);
            if (prevHealth != Health.current.Value && healthBar != null)
            {
                healthBar.enabled = !alwaysHideHealth;
            }
            if (prevHealth > Health.current.Value)
            {
                artRenderer.Hit();
                onDmg.Invoke();
                if (CurrentHealth <= 0f && alive) SetAlive(false);
            }
        }


        // Damagable

        public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
        {
            if (RelativeIdentity(id, Identity) != Identity.Friend)
            {
                CurrentHealth -= damageAmount;
            }
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