using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Body;

[Serializable]
public struct Status
{
    public Status(StatusEffect _effect, int _strength)
    {
        effect = _effect;
        strength = _strength;
    }
    public StatusEffect effect;
    public int strength;
}

public abstract class StatusEffect: ScriptableObject
{
    /* Statuses will likely be expected to:
     * 1. Apply modifiers or effects to characters
     * 2. Control how long they last
     * 3. Position any models or effects they manage
     * 4. Clean up after themselves
     */

    public new string name;
    private UnityEvent onProc;
    private UnityEvent onTick;

    public virtual void Apply(Body.Character character, int strength)
    {
        foreach (Status status in character.statuses)
        {
            if (status.effect == this)
            {
                return;
            }
        }
        character.statuses.Add(new Status(this, strength));
    }

    public virtual void Proc(int strength, Body.Character character)
    {
        character.statuses.Add(new Status(this, strength));
        onProc.Invoke();
    }
    
    public virtual void Tick(int strength, Body.Character character)
    {
        onTick.Invoke();
    }
    
    public virtual void Remove(Body.Character character)
    {
        for (int i = 0; i < character.statuses.Count;)
        {
            Status status = character.statuses[i];
            if (status.effect == this)
            {
                character.statuses.RemoveAt(i);
            }
            else i++;
        }
    }
}
