using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modified <T>
{
    public T value;
    public T Value {
        get { return value; }
        set { SetValue(value); }
    }
    public delegate T Modify(T oldValue, T newValue);
    public List<Modify> modifiers = new();

    public Modified (T value)
    {
        this.value = value;
    }

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

    private void SetValue(T value)
    {
        foreach (Modify modifier in modifiers)
        {
            value = modifier.Invoke(this.value, value);
        }
        this.value = value;
    }
}
