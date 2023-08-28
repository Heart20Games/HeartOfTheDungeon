using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CastableStats
{
    public enum CastableType { Melee, Ranged, Magic }
    public string usabilityTag = "None";
    public CastableType type = CastableType.Melee;

    [Header("Damage")]
    public bool dealDamage = false;
    [ConditionalField("dealDamage", false, true)]
    [Range(0, 10)] public int baseDamage = 1;

    [Header("Cooldown")]
    public bool useCooldown = false;
    [ConditionalField("useCooldown", false, true)]
    [Range(0, 10)] public float baseCooldown = 1;

    [Header("Knockback")]
    [Range(0, 10)] public float baseKnockback = 1;

    [Header("Range")]
    [Range(0, 10)] public float baseRange = 1;

    [Header("Statuses")]
    [Range(0, 3)] public int baseCastStatusPower;
    public List<Status> castStatuses;
    [Range(0, 3)] public int baseHitStatusPower;
    public List<Status> hitStatuses;

    [Header("Bonuses")]
    public List<StatBonus> statBonuses;
}
