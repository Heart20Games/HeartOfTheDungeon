using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoadout", menuName = "Loadouts/Loadout", order = 1)]
public class Loadout : ScriptableObject
{
    public List<CastableItem> abilities;
    public List<CastableItem> weapons;

    public CastableItem mobility;
}