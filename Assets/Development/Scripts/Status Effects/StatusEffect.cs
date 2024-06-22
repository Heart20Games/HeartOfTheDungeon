using System;
using UnityEngine;
using UnityEngine.Events;
using Body;

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

public abstract class StatusEffect: ScriptableObject
{
    /* Statuses will likely be expected to:
     * 1. Apply modifiers or effects to characters
     * 2. Control how long they last
     * 3. Position any models or effects they manage
     * 4. Clean up after themselves
     */

    public new string name;
    [SerializeField] private GameObject prefab;
    private UnityEvent onProc = new();
    private UnityEvent onTick = new();

    public virtual void Apply(Character character, int strength)
    {
        foreach (Status status in character.statuses)
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
        character.statuses.Add(new Status(this, strength, instance));
    }

    public virtual void Proc(int strength, Character character)
    {
        GameObject instance = null;
        if (prefab != null)
        {
            instance = Instantiate(prefab, character.transform);
        }
        character.statuses.Add(new Status(this, strength, instance));
        onProc.Invoke();
    }
    
    public virtual void Tick(int strength, Character character)
    {
        onTick.Invoke();
    }
    
    public virtual void Remove(Character character)
    {
        for (int i = 0; i < character.statuses.Count;)
        {
            Status status = character.statuses[i];
            if (status.effect == this)
            {
                GameObject instance = character.statuses[i].instance;
                if (instance != null)
                {
                    Destroy(instance);
                }
                character.statuses.RemoveAt(i);
            }
            else i++;
        }
    }
}
