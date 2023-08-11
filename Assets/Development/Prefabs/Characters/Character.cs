using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Body
{
    using Behavior;
    using Body.Behavior.ContextSteering;
    using Selection;
    using System.Collections;
    using static Body.Behavior.ContextSteering.CSIdentity;

    [RequireComponent(typeof(Brain))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Talker))]
    [RequireComponent(typeof(Caster))]
    public class Character : AIdentifiable, IDamageable, IControllable
    {
        [Header("Movement and Positioning")]
        public Transform body;
        public Transform pivot;
        public Pivot moveReticle;
        [HideInInspector] public Movement movement;
        [HideInInspector] public float baseOffset;

        [Header("State")]
        public bool controllable = true;
        public bool aimActive = false;
        public bool alive = false;
        [Space]
        public UnityEvent<bool> onControl;

        [Header("Appearance")]
        public ArtRenderer artRenderer;
        public CinemachineVirtualCamera virtualCamera;
        public CharacterUIElements characterUIElements;

        [Header("Collision")]
        public Collider aliveCollider;
        public Collider deadCollider;

        [Header("Interaction, Selection, and Targeting")]
        public Interactor interactor;
        public TargetFinder targetFinder;
        [HideInInspector] public Talker talker;

        [Header("Behaviour")]
        [HideInInspector] public Brain brain;
        public CSController Controller { get => brain.controller; }

        [Header("Casting")]
        public Loadout loadout;
        public Transform weaponOffset;
        [HideInInspector] public Caster caster;

        // Identifiable
        //[Header("Identity")]
        //[SerializeField] private Identity identity = Identity.Neutral;
        public override Identity Identity
        {
            get => identity;
            set
            {
                identity = value;
                brain.Identity = value;
            }
        }
        public override string Name { get => characterUIElements.characterName; set => characterUIElements.characterName = value; }

        [Header("Status Effects")]
        public List<Status> statuses;

        [Header("Health and Damage")]
        public Health healthBar;
        public bool alwaysHideHealth = false;
        public float hideHealthWaitTime = 15f;
        public Modified<int> maxHealth = new(20);
        public Modified<int> currentHealth = new(20);
        [Space]
        public UnityEvent onDmg;
        public int MaxHealth
        {
            get { return maxHealth.Value; }
            set { SetMaxHealth(value); }
        }
        public int CurrentHealth
        {
            get { return currentHealth.Value; }
            set { SetCurrentHealth(value); }
        }

        [Header("Death and Respawning")]
        public Transform spawn;
        [Space]
        public UnityEvent<Character> onDeath;
        public UnityEvent onRespawn;
        public UnityEvent<bool> onAlive;


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
            MaxHealth = MaxHealth;
            CurrentHealth = CurrentHealth;
            InitializeSpawn();
            SetControllable(false);
        }

        private void Start()
        {
            if (healthBar != null)
            {
                healthBar.enabled = false;
                healthBar.SetHealthBase(CurrentHealth, MaxHealth);
            }
            Identity = Identity;
            SetAlive(true);
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

        public void SetControllable(bool _controllable)
        {
            brain.Enabled = !_controllable;
            controllable = _controllable;
            onControl.Invoke(_controllable);
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
            maxHealth.value = amount;
            if (healthBar != null)
            {
                healthBar.SetHealthTotal(MaxHealth);
            }
        }

        public void SetCurrentHealth(int amount)
        {
            int prevHealth = currentHealth.Value;
            currentHealth.Value = Mathf.Min(amount, maxHealth.Value);
            if (prevHealth != currentHealth.Value && healthBar != null)
            {
                healthBar.enabled = !alwaysHideHealth;
                healthBar.SetHealth(CurrentHealth);
            }
            if (prevHealth > currentHealth.Value)
            {
                artRenderer.Hit();
                onDmg.Invoke();
                if (CurrentHealth <= 0f && alive) SetAlive(false);
                if (coroutine == null && healthBar != null)
                    coroutine = StartCoroutine(DeactivateHealthbar(hideHealthWaitTime));
                else
                    currentHideHealthTime = hideHealthWaitTime;
            }
        }


        // Damagable

        private Coroutine coroutine;
        private float currentHideHealthTime;
        public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
        {
            if (RelativeIdentity(id, Identity) != Identity.Friend)
            {
                CurrentHealth -= damageAmount;
            }
        }

        public IEnumerator DeactivateHealthbar(float waitTime)
        {
            Assert.IsNotNull(healthBar);
            currentHideHealthTime = waitTime;
            while (currentHideHealthTime > 0)
            {
                float timeToWait = Mathf.Min(currentHideHealthTime, 1f);
                yield return new WaitForSeconds(timeToWait);
                currentHideHealthTime -= timeToWait;
            }
            healthBar.enabled = false;
            coroutine = null;
        }


        // Castables

        public readonly CastableItem[] castableItems = new CastableItem[5];
        public readonly Castable[] castables = new Castable[5];
        public void InitializeCastables()
        {
            if (loadout != null)
            {
                for (int i = 0; i < Mathf.Min(loadout.abilities.Count, 2); i++)
                {
                    SetCastable(i, loadout.abilities[i]);
                }
                for (int i = 0; i < Mathf.Min(loadout.weapons.Count, 2); i++)
                {
                    SetCastable(2 + i, loadout.weapons[i]);
                }
                SetCastable(4, loadout.mobility);
            }
            if (brain != null)
                brain.RegisterCastables(castableItems);
        }

        public void SetCastable(int idx, CastableItem item)
        {
            castables[idx]?.UnEquip();

            if (loadout != null && item != null)
            {
                castableItems[idx] = item;
                castables[idx] = Instantiate(item.prefab, transform);
                castables[idx].Initialize(this, item);
            }
        }

        public void ActivateCastable(ICastable castable)
        {
            if (caster != null && caster.enabled)
            {
                caster.Castable = castable;
                Vector2 castVector = caster.castVector;
                if (controllable)
                    caster.Cast(body, castVector);
                else
                    caster.Cast(castVector);
            }
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
        public void MoveCharacter(Vector2 input) { movement.SetMoveVector(input); caster.SetFallback(movement.moveVector); }
        public void Aim(Vector2 input, bool aim=false) { if (aimActive || aim) caster.SetVector(input); }
        public void ActivateCastable(int idx) { ActivateCastable(castables[idx]); }
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