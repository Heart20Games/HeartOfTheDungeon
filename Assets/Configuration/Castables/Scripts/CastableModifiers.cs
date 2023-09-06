using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableModifiers", menuName = "Loadouts/CastableModifiers", order = 1)]
public class CastableModifiers : ScriptableObject
{
    public List<StatMod> damage = new();
    public List<StatMod> cooldown = new();
    public List<StatMod> knockback = new();
    public List<StatMod> range = new();
}
