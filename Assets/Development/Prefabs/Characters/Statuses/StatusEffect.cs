using System;
using UnityEngine;
using UnityEngine.Events;
using Body;
using HotD.Body;
using System.Collections.Generic;

[Serializable]
public struct Status
{
    public Status(StatusEffect _effect, int _strength)
    {
        name = _effect.name;
        effect = _effect;
        strength = _strength;
    }
    public string name;
    public StatusEffect effect;
    public int strength;
}

public interface IStatusEffectable
{
    public List<Status> Statuses { get; }
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
    private UnityEvent onProc = new();
    private UnityEvent onTick = new();

    public virtual void Apply(Character character, int strength)
    {
        foreach (Status status in character.Statuses)
        {
            if (status.effect == this)
            {
                return;
            }
        }
        character.Statuses.Add(new Status(this, strength));
    }

    public virtual void Proc(int strength, Character character)
    {
        character.Statuses.Add(new Status(this, strength));
        onProc.Invoke();
    }
    
    public virtual void Tick(int strength, Character character)
    {
        onTick.Invoke();
    }
    
    public virtual void Remove(Character character)
    {
        for (int i = 0; i < character.Statuses.Count;)
        {
            Status status = character.Statuses[i];
            if (status.effect == this)
            {
                character.Statuses.RemoveAt(i);
            }
            else i++;
        }
    }
}
