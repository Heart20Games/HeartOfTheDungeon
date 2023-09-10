using Attributes;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

[CreateAssetMenu(fileName = "NewCastableStatAttributes", menuName = "Loadouts/Castable Stat Attributes", order = 1)]
public class CastableStatAttributes : ScriptableObject
{
    public StatAttribute[] damage;
    public StatAttribute[] cooldown;
    public StatAttribute[] knockback;
    public StatAttribute[] range;
    public StatAttribute[] castStatusPower;
    public StatAttribute[] hitStatusPower;
}

[CreateAssetMenu(fileName = "NewCastableStats", menuName = "Loadouts/Castable Stats", order = 1)]
public class CastableStats : ScriptableObject
{
    public enum CastableType { Melee, Ranged, Magic }
    public string usabilityTag = "None";
    public CastableType type = CastableType.Melee;
    public Identity targetIdentity = Identity.Neutral;
    public CastableStatAttributes attributes;

    [Header("Damage")]
    public bool dealDamage = false;
    [ConditionalField("dealDamage", false, true)]
    public DependentAttribute damage = new(1);
    public int Damage { get => (int)damage.FinalValue; }

    [Header("Cooldown")]
    public bool useCooldown = false;
    [ConditionalField("useCooldown", false, true)]
    public DependentAttribute cooldown = new(1);
    public float Cooldown { get => cooldown.FinalValue; }

    [Header("Knockback")]
    public DependentAttribute knockback = new(1);
    public float Knockback { get => knockback.FinalValue; }

    [Header("Range")]
    public DependentAttribute range = new(1);
    public float Range { get => range.FinalValue; }

    [Header("Statuses")]
    public DependentAttribute castStatusPower;
    public int CastStatusPower { get => (int)castStatusPower.FinalValue; }
    public List<Status> castStatuses;
    public DependentAttribute hitStatusPower;
    public int HitStatusPower { get => (int)hitStatusPower.FinalValue; }
    public List<Status> hitStatuses;

    [Header("Bonuses")]
    public List<StatBonus> bonuses;

    public void Equip(StatBlock statBlock)
    {
        AssignBonuses(damage, attributes.damage, statBlock);
        AssignBonuses(cooldown, attributes.cooldown, statBlock);
        AssignBonuses(knockback, attributes.knockback, statBlock);
        AssignBonuses(range, attributes.range, statBlock);
        AssignBonuses(castStatusPower, attributes.castStatusPower, statBlock);
        AssignBonuses(hitStatusPower, attributes.hitStatusPower, statBlock);
    }

    public void AssignBonuses(DependentAttribute dependent, StatAttribute[] attributes, StatBlock statBlock)
    {
        foreach (var attribute in attributes)
        {
            dependent.AddAttribute(statBlock.GetStat(attribute.stat), attribute.weight);
        }
    }
}
