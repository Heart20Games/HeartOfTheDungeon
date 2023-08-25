using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CastableStats
{
    public string usabilityTag = "None";
    public int baseDamage = 1;
    public float baseCooldown = 1;
    public float baseKnockback = 1;
    public float baseRange = 1;
    public int baseCastStatusPower;
    public List<StatusEffect> castStatuses;
    public int baseHitStatusPower;
    public List<StatusEffect> hitStatuses;
    public List<StatBonus> statBonuses;
}
