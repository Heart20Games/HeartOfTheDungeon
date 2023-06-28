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
        // Character Parts
        public Transform body;
        public Transform pivot;
        public ArtRenderer artRenderer;
        public Transform weaponHand;
        public Pivot moveReticle;
        public Health healthBar;
        public CinemachineVirtualCamera virtualCamera;
        public CharacterUIElements characterUIElements;
        [HideInInspector] public Brain brain;
        [HideInInspector] public Movement movement;
        [HideInInspector] public Talker talker;
        [HideInInspector] public Attack attacker;
        [HideInInspector] public float baseOffset;

        // Castables
        public Loadout loadout;
        public ICastable primaryCastable;
        public ICastable secondaryCastable;
        public Castable primary;
        public Castable secondary;
        public Vector3 weaponOffset = Vector3.up;
        private int abilityIdx = -1;
        private int weaponIdx = -1;
        public enum CastSlot { PRIMARY, SECONDARY };

        // Identity
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
        public List<Status> statuses;

        // Health
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
        public UnityEvent onDeath;
        public UnityEvent onDmg;

        // State
        public bool controllable = true;
        public bool aimActive = false;
        public bool alive = false;

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
            MaxHealth = MaxHealth;
            CurrentHealth = CurrentHealth;
            SetControllable(false);
        }

        private void Start()
        {
            InitializeCastables();
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
        }

        public void SetComponentActive(Component component, bool _active)
        {
            if (component != null)
            {
                component.gameObject.SetActive(_active);
            }
        }


        // Health

        public void SetAlive(bool alive)
        {
            body.gameObject.SetActive(alive);
            brain.Alive = alive;
        }

        public void SetMaxHealth(int amount)
        {
            maxHealth.value = amount;
            if (healthBar != null)
            {
                healthBar.SetHealthTotal(MaxHealth);
            }
            //CurrentHealth = CurrentHealth;
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

        public void InitializeCastables()
        {
            if (loadout != null)
            {
                if (secondary == null)
                {
                    ChangeCastable(CastSlot.SECONDARY);
                }
                if (primary == null)
                {
                    ChangeCastable(CastSlot.PRIMARY);
                }
            }
        }

        public void ChangeCastable(CastSlot slot)
        {
            ICastable castable = slot == CastSlot.PRIMARY ? primary as ICastable : secondary as ICastable;
            if (castable != null)
            {
                castable.UnEquip();
            }

            if (loadout != null)
            {
                switch (slot)
                {
                    case CastSlot.PRIMARY:
                        if (loadout.weapons.Count > 0)
                        {
                            weaponIdx = (weaponIdx + 1) % loadout.weapons.Count;
                            primary = Instantiate(loadout.weapons[weaponIdx].prefab, transform);
                            primary.Initialize(this);
                        }
                        break;
                    case CastSlot.SECONDARY:
                        if (loadout.abilities.Count > 0)
                        {
                            abilityIdx = (abilityIdx + 1) % loadout.abilities.Count;
                            secondary = Instantiate(loadout.abilities[abilityIdx].prefab, transform);
                            secondary.Initialize(this);
                        }
                        break;
                }
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
        public void AimCharacter(Vector2 input) { if (aimActive) movement.SetAimVector(input); }
        public void ChangeAbility() { ChangeCastable(CastSlot.SECONDARY); }
        public void ChangeWeapon() { ChangeCastable(CastSlot.PRIMARY); }
        public void ActivateWeapon() { ActivateCastable(primary); }
        public void ActivateAbility() { ActivateCastable(secondary); }
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