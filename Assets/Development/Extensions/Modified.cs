using System;
using System.Collections.Generic;
using UnityEngine;
using static EffectSpawner;

[Serializable]
public class Modified<T>
{
    public T value;
    public T Value {
        get { return value; }
        set { SetValue(value); }
    }

    public delegate T Modify(T oldValue, T newValue);
    public List<Modify> modifiers = new();

    public delegate void Listen(T finalValue);
    public List<Listen> listeners = new();

    // Constructor
    public Modified(T value)
    {
        this.value = value;
    }

    // Modifiers
    public void Subscribe(Modify modify)
    {
        if (!modifiers.Contains(modify))
            modifiers.Add(modify);
    }

    public void UnSubscribe(Modify modify)
    {
        if (modifiers.Contains(modify))
            modifiers.Remove(modify);
    }

    // Listeners
    public void Subscribe(Listen listen)
    {
        if (!listeners.Contains(listen))
            listeners.Add(listen);
    }

    public void UnSubscribe(Listen listen)
    {
        if (listeners.Contains(listen))
            listeners.Remove(listen);
    }

    // Setter
    private void SetValue(T value)
    {
        foreach (Modify modifier in modifiers)
        {
            value = modifier.Invoke(this.value, value);
        }
        foreach (Listen listener in listeners)
        {
            listener.Invoke(value);
        }
        this.value = value;
    }
}
