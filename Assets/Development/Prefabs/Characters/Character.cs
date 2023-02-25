using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    // Character Parts
    public Transform body;
    public Transform pivot;
    public Animator animator;
    public Transform weaponHand;
    public Transform moveReticle;
    public HealthbarUI healthBarUI;
    [HideInInspector] public Movement movement;
    [HideInInspector] public Interactor interactor;
    [HideInInspector] public PlayerAttack attacker;

    // Castables
    public Loadout loadout;
    public ICastable primaryCastable;
    public ICastable secondaryCastable;
    public Ability ability;
    public Weapon weapon;
    private int abilityIdx = -1;
    private int weaponIdx = -1;

    // Health
    public float startingHealth = 25f;
    public float currentHealth;
    public UnityEvent onDeath;

    // State
    public bool controllable = true;

    // Initialization
    private void Awake()
    {
        movement = GetComponent<Movement>();
        interactor = GetComponent<Interactor>();
        attacker = GetComponent<PlayerAttack>();
        SetControllable(false);
    }

    private void Start()
    {
        currentHealth = startingHealth;
        InitializeCastables();
        UpdateHealthUI();
    }


    // State

    public void SetControllable(bool _controllable)
    {
        controllable = _controllable;
        movement.canMove = controllable;
        attacker.active = controllable;
        if (moveReticle != null)
        {
            moveReticle.gameObject.SetActive(_controllable);
        }
    }


    // Health

    public void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateFill(currentHealth, startingHealth);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthUI();
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }


    // Castables

    public void InitializeCastables()
    {
        if (loadout != null)
        {
            if (ability == null)
            {
                ChangeAbility();
            }
            if (weapon == null)
            {
                ChangeWeapon();
            }
        }
    }

    public void ChangeCastable(bool primary)
    {
        ICastable castable = primary ? weapon as ICastable : ability as ICastable;
        if (castable != null)
        {
            castable.UnEquip();
        }

        if (primary)
        {
            weaponIdx = (weaponIdx + 1) % loadout.weapons.Count;
            weapon = Instantiate(loadout.weapons[weaponIdx], transform);
            weapon.Initialize(this);
        }
        else
        {
            abilityIdx = (abilityIdx + 1) % loadout.abilities.Count;
            ability = Instantiate(loadout.abilities[abilityIdx], transform);
            ability.Initialize(this);
        }
    }

    public void ActivateCastable(ICastable castable)
    {
        if (attacker != null && attacker.active)
        {
            attacker.Castable = castable;
            attacker.Slashie(movement.getAttackVector());
        }
    }


    // Actions
    public void MoveCharacter(Vector2 input) { movement.SetInputVector(input); }
    public void ChangeAbility() { ChangeCastable(false); }
    public void ChangeWeapon() { ChangeCastable(true); }
    public void ActivateWeapon() { ActivateCastable(weapon); }
    public void ActivateAbility() { ActivateCastable(ability); }
    public void Interact() { interactor.Talk(); }


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
