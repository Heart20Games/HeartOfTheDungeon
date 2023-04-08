using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldStatus", menuName = "Statuses/ShieldStatus", order = 1)]
public class ShieldStatus : StatusEffect
{
    public float maxStrength = 1.0f;
    private int strength;

    public override void Apply(Character character, int strength)
    {
        base.Apply(character, strength);
        this.strength = strength;
        character.currentHealth.Subscribe(Shield);
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        character.currentHealth.UnSubscribe(Shield);
    }

    public int Shield(int oldHealth, int newHealth)
    {
        int difference = newHealth - oldHealth;
        if (difference < 0)
        {
            float percentShielded = Mathf.Min(strength / maxStrength, 1.0f);
            int healthShielded = Mathf.CeilToInt(difference * percentShielded);
            return oldHealth + difference - healthShielded;
        }
        else
        {
            return newHealth;
        }
    }
}
