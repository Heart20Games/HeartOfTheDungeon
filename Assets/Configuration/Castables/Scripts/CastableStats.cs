using Attributes;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

[CreateAssetMenu(fileName = "NewCastableStats", menuName = "Loadouts/Castable Stats", order = 1)]
public class CastableStats : ScriptableObject
{
    // Details
    public enum CastableType { Melee, Ranged, Magic }
    public enum CastableStat { Damage, Cooldown, ChargeRate, ChargeLimit, Knockback, Range, CastStatusPower, HitStatusPower}
    public string usabilityTag = "None";
    public CastableType type = CastableType.Melee;
    public Identity targetIdentity = Identity.Neutral;

    // Attributes
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

    [Header("Charge-Up")]
    public bool useChargeUp = false;
    [ConditionalField("useChargeUp", false, true)]
    public DependentAttribute chargeRate = new(1);
    [ConditionalField("useChargeUp", false, true)]
    public DependentAttribute chargeLimit = new(1);
    public float ChargeRate { get => chargeRate.FinalValue; }
    public float ChargeLimit { get => chargeLimit.FinalValue; }

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

    // Accessors
    
    public DependentAttribute GetAttribute(CastableStat stat)
    {
        return stat switch
        {
            CastableStat.Damage => damage,
            CastableStat.Cooldown => cooldown,
            CastableStat.Range => range,
            CastableStat.ChargeRate => chargeRate,
            CastableStat.ChargeLimit => chargeLimit,
            CastableStat.Knockback => knockback,
            CastableStat.CastStatusPower => castStatusPower,
            CastableStat.HitStatusPower => hitStatusPower,
            _ => null
        };
    }

    // Equipping

    public void AssignBonuses(DependentAttribute dependent, StatAttribute[] attributes, StatBlock statBlock)
    {
        foreach (var attribute in attributes)
        {
            dependent.AddAttribute(statBlock.GetAttribute(attribute.stat), attribute.weight);
        }
    }

    public void Equip(StatBlock statBlock)
    {
        UnEquip();
        AssignBonuses(damage, attributes.damage, statBlock);
        AssignBonuses(cooldown, attributes.cooldown, statBlock);
        AssignBonuses(chargeRate, attributes.chargeRate, statBlock);
        AssignBonuses(chargeLimit, attributes.chargeLimit, statBlock);
        AssignBonuses(knockback, attributes.knockback, statBlock);
        AssignBonuses(range, attributes.range, statBlock);
        AssignBonuses(castStatusPower, attributes.castStatusPower, statBlock);
        AssignBonuses(hitStatusPower, attributes.hitStatusPower, statBlock);
    }

    public void UnEquip()
    {
        damage.Clear();
        cooldown.Clear();
        chargeRate.Clear();
        chargeLimit.Clear();
        knockback.Clear();
        range.Clear();
        castStatusPower.Clear();
        hitStatusPower.Clear();
    }
}
