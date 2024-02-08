using System;
using UnityEngine.Events;

public interface IEventAdaptor<T>
{
    public T Value { set; }
}

[Serializable]
public struct EventAdaptor<T> : IEventAdaptor<T>
{
    public T Value { get => value; set { this.value = value; this.oldValue = value; onSetValue.Invoke(value); } }
    public UnityEvent<T> onSetValue;
    public T value;
    public T oldValue;
    public EventAdaptor(T initialValue)
    {
        onSetValue = new();
        value = initialValue;
        oldValue = initialValue;
    }
    public void SendChanges()
    {
        if (!value.Equals(oldValue))
        {
            Value = value;
        }
    }
}

public abstract class Adaptor : BaseMonoBehaviour {}
