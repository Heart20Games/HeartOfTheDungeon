using System;
using UnityEngine;
using UnityEngine.Events;
using Body;
using HotD.Body;

[CreateAssetMenu(fileName = "NewStunStatus", menuName = "Statuses/StunStatus", order = 1)]
public class StunEffect : StatusEffect
{
    public override void Apply(Character character, int strength)
    {
        base.Apply(character, strength);
        character.BrainDead = true;
        character.brainDead.Subscribe(Stun);
    }

    public override void Remove(Character character)
    {
        base.Remove(character);
        character.brainDead.UnSubscribe(Stun);
        character.BrainDead = false;
    }

    public bool Stun(bool oldBrainDead, bool newBrainDead)
    {
        return true;
    }
}