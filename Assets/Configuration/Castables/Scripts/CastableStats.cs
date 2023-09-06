using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

[CreateAssetMenu(fileName = "NewCastableStats", menuName = "Loadouts/CastableStats", order = 1)]
public class CastableStats : ScriptableObject
{
    public enum CastableType { Melee, Ranged, Magic }
    public string usabilityTag = "None";
    public CastableType type = CastableType.Melee;
    public Identity targetIdentity = Identity.Neutral;
    public CastableModifiers modifiers;

    [Header("Damage")]
    public bool dealDamage = false;
    [ConditionalField("dealDamage", false, true)]
    [Range(0, 10)] public int baseDamage = 1;
    public int Damage(StatBlock statBlock)
    {
        return Modify(baseDamage, modifiers.damage, statBlock);
    }

    [Header("Cooldown")]
    public bool useCooldown = false;
    [ConditionalField("useCooldown", false, true)]
    [Range(0, 10)] public float baseCooldown = 1;
    public float Cooldown(StatBlock statBlock)
    {
        return Modify((int)(baseCooldown*10), modifiers.cooldown, statBlock) / 10;
    }

    [Header("Knockback")]
    [Range(0, 10)] public float baseKnockback = 1;
    public float Knockback(StatBlock statBlock)
    {
        return Modify((int)(baseKnockback * 10), modifiers.knockback, statBlock) / 10;
    }

    [Header("Range")]
    [Range(0, 10)] public float baseRange = 1;
    public float Range(StatBlock statBlock)
    {
        return Modify((int)(baseRange * 10), modifiers.range, statBlock) / 10;
    }

    [Header("Statuses")]
    [Range(0, 3)] public int baseCastStatusPower;
    public List<Status> castStatuses;
    [Range(0, 3)] public int baseHitStatusPower;
    public List<Status> hitStatuses;

    [Header("Bonuses")]
    public List<StatBonus> statBonuses;

    public int Modify(int value, List<StatMod> modifiers, StatBlock block)
    {
        int modified = value;
        foreach (StatMod mod in modifiers)
        {
            modified = block.ModifyStat(value, mod);
        }
        return modified;
    }
}
