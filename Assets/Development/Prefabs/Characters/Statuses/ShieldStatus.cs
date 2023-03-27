using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldStatus", menuName = "Statuses/ShieldStatus", order = 1)]
public class ShieldStatus : StatusEffect
{
    public override void Apply(Character character, int strength)
    {
        base.Apply(character, strength);
        character.currentHealth.Subscribe(Shield);
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        character.currentHealth.UnSubscribe(Shield);
    }

    public float Shield(float oldHealth, float newHealth)
    {
        Debug.Log("Shield");
        return oldHealth;
    }
}
