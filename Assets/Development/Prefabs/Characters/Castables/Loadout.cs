using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoadout", menuName = "ScriptableObjects/Loadout", order = 1)]
public class Loadout : ScriptableObject
{
    public List<Ability> abilities;
    public List<Weapon> weapons;
}
