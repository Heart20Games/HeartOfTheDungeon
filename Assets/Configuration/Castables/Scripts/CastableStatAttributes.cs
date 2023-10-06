using Attributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableStatAttributes", menuName = "Loadouts/Castable Stat Attributes", order = 1)]
public class CastableStatAttributes : ScriptableObject
{
    public StatAttribute[] damage;
    public StatAttribute[] cooldown;
    public StatAttribute[] chargeRate;
    public StatAttribute[] chargeLimit;
    public StatAttribute[] knockback;
    public StatAttribute[] range;
    public StatAttribute[] castStatusPower;
    public StatAttribute[] hitStatusPower;
}