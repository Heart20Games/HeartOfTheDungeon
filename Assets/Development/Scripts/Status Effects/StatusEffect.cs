using System;
using UnityEngine;
using UnityEngine.Events;
using Body;
using HotD.Body;
using System.Collections.Generic;
using Attributes;
using UnityEngine.Serialization;
using MyBox;

[Serializable]
public struct Status
{
    public Status(StatusEffect effect, int strength, GameObject instance)
    {
        name = effect.name;
        this.effect = effect;
        this.strength = strength;
        this.instance = instance;
    }
    public string name;
    public StatusEffect effect;
    public int strength;
    public GameObject instance;
}

public enum StatusType { None, Activation, Execution, Hit, Lingering }
[Serializable]
public struct StatusClass
{
    public string name;
    public StatusType type;
    public DependentAttribute power;
    public List<Status> statuses;
    public readonly int Power { get => (int)power.FinalValue; }
}

public interface IStatusEffect
{
    public void Apply(Character character, int strength);
    public void Proc(int strength, Character character);
    public void Tick(int strength, Character character);
    public void Remove(Character character);
}

public interface IStatusEffectable
{
    public List<Status> Statuses { get; }
}

public abstract class StatusEffect: BaseScriptableObject, IStatusEffect
{
    /* Statuses will likely be expected to:
     * 1. Apply modifiers or effects to characters
     * 2. Control how long they last
     * 3. Position any models or effects they manage
     * 4. Clean up after themselves
     */

    [SerializeField] private bool overrideName;
    [ConditionalField("overrideName")]
    public new string name;
    [SerializeField] private GameObject prefab;
    private readonly UnityEvent onProc = new();
    private readonly UnityEvent onTick = new();

    public virtual void Apply(Character character, int strength)
    {
        foreach (Status status in character.Statuses)
        {
            if (status.effect == this)
            {
                return;
            }
        }

        GameObject instance = null;
        if (prefab != null)
        {
            instance = Instantiate(prefab, character.transform);
        }
        character.Statuses.Add(new Status(this, strength, instance));
    }

    public virtual void Proc(int strength, Character character)
    {
        GameObject instance = null;
        if (prefab != null)
        {
            instance = Instantiate(prefab, character.transform);
        }
        character.Statuses.Add(new Status(this, strength, instance));
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
                GameObject instance = character.Statuses[i].instance;
                if (instance != null)
                {
                    Destroy(instance);
                }
                character.Statuses.RemoveAt(i);
            }
            else i++;
        }
    }

    protected virtual void OnValidate()
    {
        if (!overrideName) name = Name;
    }
}
