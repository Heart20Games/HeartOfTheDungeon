using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform body;
    public Transform pivot;
    public Animator animator;
    public Transform weaponHand;

    public Loadout loadout;
    public ICastable primaryCastable;
    public ICastable secondaryCastable;
    public Ability ability;
    public Weapon weapon;
    private int abilityIdx = -1;
    private int weaponIdx = -1;

    private void Start()
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

    public void ChangeAbility()
    {
        PrintAbilities(loadout.abilities);
        
        abilityIdx = (abilityIdx + 1) % loadout.abilities.Count;
        if (ability != null)
        {
            Destroy(ability.gameObject);
            Debug.Log("Ability Destroyed");
        }
        
        Debug.Log("Ability Index:" + abilityIdx.ToString());
        ability = Instantiate(loadout.abilities[abilityIdx], transform);
        ability.Initialize(this);
        Debug.Log("Ability Created");
    }

    public void ChangeWeapon()
    {
        PrintWeapons(loadout.weapons);
        print(weaponIdx);
        
        weaponIdx = (weaponIdx + 1) % loadout.weapons.Count;
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
            Debug.Log("Weapon Destroyed");
        }
        
        Debug.Log("Weapon Index:" + weaponIdx.ToString());
        weapon = Instantiate(loadout.weapons[weaponIdx], transform);
        weapon.Initialize(this);
        Debug.Log("Weapon Created");
    }

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
