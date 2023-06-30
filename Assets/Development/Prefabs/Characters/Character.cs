using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Body
{
    using Behavior;
    using System.Collections;
    using UnityEngine.TextCore.Text;
    using static Body.Behavior.ContextSteering.CSIdentity;

    [RequireComponent(typeof(Brain))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Talker))]
    [RequireComponent(typeof(Attack))]
    public class Character : BaseMonoBehaviour, IDamageable, IControllable
    {
        // Movement and Positioning
        [Header("Movement and Positioning")]
        public Transform body;
        public Transform pivot;
        public Pivot moveReticle;
        [HideInInspector] public Movement movement;
        [HideInInspector] public float baseOffset;

        // State
        [Header("State")]
        public bool controllable = true;
        public bool aimActive = false;
        public bool alive = false;

        // Appearance
        [Header("Appearance")]
        public ArtRenderer artRenderer;
        public CinemachineVirtualCamera virtualCamera;
        public CharacterUIElements characterUIElements;

        // Interaction
        [Header("Interaction")]
        public Interactor interactor;
        [HideInInspector] public Talker talker;

        // Behaviour
        [Header("Behaviour")]
        [HideInInspector] public Brain brain;

        // Castables
        [Header("Casting")]
        public Loadout loadout;
        public Transform weaponOffset;
        [HideInInspector] public Attack attacker;

        // Identity
        [Header("Identity")]
        public Identity identity = Identity.Neutral;
        public Identity Identity
        {
            get => identity;
            set
            {
                identity = value;
                brain.Identity = value;
            }
        }

        // Statuses
        [Header("Status Effects")]
        public List<Status> statuses;

        // Health
        [Header("Health and Damage")]
        public Health healthBar;
        public bool alwaysHideHealth = false;
        public float hideHealthWaitTime = 15f;
        public Modified<int> maxHealth = new(20);
        public Modified<int> currentHealth = new(20);
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

        // Events
        public UnityEvent onDeath;
        public UnityEvent onDmg;
        public UnityEvent<bool> onControl;


        // Initialization
        private void Awake()
        {
            transform.rotation = new(0,0,0,0);
            Awarn.IsNotNull(body, "Character has no Character");
            InitBody();
            brain = GetComponent<Brain>();
            movement = GetComponent<Movement>();
            talker = GetComponent<Talker>();
            attacker = GetComponent<Attack>();
            InitializeCastables();
            MaxHealth = MaxHealth;
            CurrentHealth = CurrentHealth;
            SetControllable(false);
        }

        private void Start()
        {
            SetComponentActive(healthBar, false);
            healthBar.SetHealthBase(CurrentHealth, MaxHealth);
            Identity = Identity;
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
                status.effect.Tick(status.strength, this);
            }
        }


        // Aiming

        public void OnCastVectorChanged()
        {
            //moveReticle.rotationOffset.y = Vector3.Dot(movement.castVector.FullY(), Vector3.forward);//moveReticle.SetRotationWithVector(movement.castVector);
            moveReticle.body.SetLocalRotationWithVector(movement.castVector);
        }


        // State

        public void SetControllable(bool _controllable)
        {
            brain.Enabled = !_controllable;
            controllable = _controllable;
            //movement.canMove = controllable;
            //attacker.enabled = controllable;
            //SetComponentActive(healthBar, !_controllable && !hideHealth);
            SetComponentActive(moveReticle, _controllable);
            SetComponentActive(virtualCamera, _controllable);
            SetBehaviourEnabled(interactor, _controllable);
            onControl.Invoke(_controllable);
        }

        public void SetBehaviourEnabled(Behaviour behaviour, bool _enabled)
        {
            if (behaviour != null) behaviour.enabled = _enabled;
        }

        public void SetComponentActive(Component component, bool _active)
        {
            if (component != null) component.gameObject.SetActive(_active);
        }


        // Health

        public void SetAlive(bool alive)
        {
            movement.SetMoveVector(new());
            brain.Alive = alive;
            artRenderer.Dead = alive;
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
            currentHealth.Value = Mathf.Min(amount, maxHealth.Value);
            if (healthBar != null)
            {
                healthBar.SetHealth(amount);
            }
        }


        // Damagable

        public void Die()
        {
            onDeath.Invoke();
            SetAlive(false);
        }

        private Coroutine coroutine;
        private float currentHideHealthTime;
        public void TakeDamage(int damageAmount, Identity id=Identity.Neutral)
        {
            if (RelativeIdentity(id, Identity) == Identity.Foe)
            {
                CurrentHealth -= damageAmount;
                SetComponentActive(healthBar, !alwaysHideHealth);
                healthBar.SetHealth(CurrentHealth);
                onDmg.Invoke();
                if (CurrentHealth <= 0f) Die();
                if (coroutine == null)
                    coroutine = StartCoroutine(DeactivateHealthbar(hideHealthWaitTime));
                else
                    currentHideHealthTime = hideHealthWaitTime;
            }
        }

        public IEnumerator DeactivateHealthbar(float waitTime)
        {
            currentHideHealthTime = waitTime;
            while (currentHideHealthTime > 0)
            {
                float timeToWait = Mathf.Min(currentHideHealthTime, 1f);
                yield return new WaitForSeconds(timeToWait);
                currentHideHealthTime -= timeToWait;
            }
            SetComponentActive(healthBar, false);
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
                castables[idx].Initialize(this);
            }
        }

        public void ActivateCastable(ICastable castable)
        {
            if (attacker != null && attacker.enabled)
            {
                attacker.Castable = castable;
                Vector2 castVector = movement.castVector;
                if (controllable)
                {
                    Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.blue, 0.5f);
                    Vector3 cameraDirection = body.position - Camera.main.transform.position;
                    castVector = castVector.Orient(cameraDirection.XZVector().normalized).normalized;
                    Debug.DrawRay(body.position, castVector.FullY() * 2f, Color.yellow, 0.5f);
                }
                attacker.Slashie(castVector);
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
        public void MoveCharacter(Vector2 input) { movement.SetMoveVector(input); }
        public void AimCharacter(Vector2 input, bool aim=false) { if (aimActive || aim) movement.SetAimVector(input); }
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