using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Body;

[CreateAssetMenu(fileName = "NewDamageStatus", menuName = "Statuses/DamageStatus", order = 1)]
public class DamageEffect : StatusEffect
{
    public int damageOnProc = 0;
    public int damageOnTick = 0;

    public override void Proc(int strength, Character character)
    {
        base.Proc(strength, character);
        character.TakeDamage(damageOnProc * strength);
    }

    public override void Tick(int strength, Character character)
    {
        base.Tick(strength, character);
        character.TakeDamage(damageOnTick * strength);
    }
}
