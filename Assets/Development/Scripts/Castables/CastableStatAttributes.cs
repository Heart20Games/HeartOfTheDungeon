using Attributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewCastableStatAttributes", menuName = "Loadouts/Castable Stat Attributes", order = 1)]
public class CastableStatAttributes : ScriptableObject
{
    public StatAttribute[] damage;
    public StatAttribute[] cooldown;
    public StatAttribute[] chargeRate;
    public StatAttribute[] chargeLimit;
    public StatAttribute[] comboCooldown;
    public StatAttribute[] knockback;
    public StatAttribute[] range;
    public StatAttribute[] activationStatusPower;
    [FormerlySerializedAs("castStatusPower")]
    public StatAttribute[] executionStatusPower;
    [FormerlySerializedAs("hitStatusPower")]
    public StatAttribute[] hitStatusPower;
}