using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoadout", menuName = "ScriptableObjects/Loadout", order = 1)]
public class Loadout : ScriptableObject
{
    public List<Castable> abilities;
    public List<Castable> weapons;
}
