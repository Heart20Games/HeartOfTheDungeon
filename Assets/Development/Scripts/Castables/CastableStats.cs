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
    public enum CastableStat { Damage, Cooldown, ChargeRate, ChargeLimit, Knockback, Range, CastStatusPower, HitStatusPower, ComboCooldown}
    public string usabilityTag = "None"; // Currently Does Absolutely Nothing
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

    [Header("Combos")]
    public bool useComboCooldown = false;
    [ConditionalField("useComboCooldown", false, true)]
    public bool useComboCooldownTime = false;
    [ConditionalField(new string[] { "useComboCooldown", "useComboCooldownTime" })]
    public DependentAttribute comboCooldown = new(1);
    public float ComboCooldown { get => comboCooldown.FinalValue; }

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

    private void OnEnable()
    {
        damage.name = "Damage";
        cooldown.name = "Cooldown";
        range.name = "Range";
        chargeRate.name = "Charge Rate";
        chargeLimit.name = "Charge Limit";
        comboCooldown.name = "Combo Cooldown";
        knockback.name = "Knockback";
        castStatusPower.name = "Cast Status Power";
        hitStatusPower.name = "Hit Status Power";
    }

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
            CastableStat.ComboCooldown => comboCooldown,
            CastableStat.Knockback => knockback,
            CastableStat.CastStatusPower => castStatusPower,
            CastableStat.HitStatusPower => hitStatusPower,
            _ => null
        };
    }

    // Updating

    public void SendUpdateOnAll()
    {
        damage.Updated();
        cooldown.Updated();
        range.Updated();
        chargeRate.Updated();
        chargeLimit.Updated();
        comboCooldown.Updated();
        knockback.Updated();
        castStatusPower.Updated();
        hitStatusPower.Updated();
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
        if (attributes != null)
        {
            AssignBonuses(damage, attributes.damage, statBlock);
            AssignBonuses(cooldown, attributes.cooldown, statBlock);
            AssignBonuses(chargeRate, attributes.chargeRate, statBlock);
            AssignBonuses(chargeLimit, attributes.chargeLimit, statBlock);
            AssignBonuses(comboCooldown, attributes.comboCooldown, statBlock);
            AssignBonuses(knockback, attributes.knockback, statBlock);
            AssignBonuses(range, attributes.range, statBlock);
            AssignBonuses(castStatusPower, attributes.castStatusPower, statBlock);
            AssignBonuses(hitStatusPower, attributes.hitStatusPower, statBlock);
        }
        SendUpdateOnAll();
    }

    public void UnEquip()
    {
        damage.Clear();
        cooldown.Clear();
        chargeRate.Clear();
        chargeLimit.Clear();
        comboCooldown.Clear();
        knockback.Clear();
        range.Clear();
        castStatusPower.Clear();
        hitStatusPower.Clear();
    }
}
