using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Loadout loadout;
    public ICastable primaryCastable;
    public ICastable secondaryCastable;
    public Ability ability;
    public Weapon weapon;

    public Transform pivot;
    public Animator animator;

    private void Start()
    {
        if (loadout != null)
        {
            if (ability == null && loadout.abilities.Count > 0)
            {
                ability = Instantiate(loadout.abilities[0], transform);
            }
            if (weapon == null && loadout.weapons.Count > 0)
            {
                weapon = Instantiate(loadout.weapons[0], transform);
            }
        }
    }
}
