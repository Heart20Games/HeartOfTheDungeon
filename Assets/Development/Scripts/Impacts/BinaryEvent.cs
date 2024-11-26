using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomUnityEvents
{
    [Serializable]
    public class BinaryEvent
    {
        public BinaryEvent(UnityEvent _enter, UnityEvent _exit)
        {
            enter = _enter != null ? _enter : new();
            exit = _exit != null ? _exit : new();
        }
        public UnityEvent enter;
        public UnityEvent exit;
    }

    [Serializable]
    public class BinaryEvent<T>
    {
        public BinaryEvent(UnityEvent<T> _enter, UnityEvent<T> _exit)
        {
            enter = _enter;
            exit = _exit;
        }
        public UnityEvent<T> enter;
        public UnityEvent<T> exit;
    }

    [Serializable]
    public class BinaryEvents<T>
    {
        public BinaryEvent trigger;
        public BinaryEvent<T> value;
        public void InvokeEnter(T value)
        {
            trigger.enter.Invoke();
            this.value.enter.Invoke(value);
        }
        public void InvokeExit(T value)
        {
            trigger.exit.Invoke();
            this.value.exit.Invoke(value);
        }
    }

    [Serializable]
    public class UnityEvents<T>
    {
        public UnityEvent trigger;
        public UnityEvent<T> value;
        public void Invoke(T value)
        {
            this.trigger.Invoke();
            this.value.Invoke(value);
        }
    }

    [Serializable]
    public class UnityEvents<TS, TV>
    {
        public UnityEvent trigger;
        public UnityEvent<TS> source;
        public UnityEvent<TV> value;
        public void Invoke(TS source, TV value)
        {
            trigger.Invoke();
            this.source.Invoke(source);
            this.value.Invoke(value);
        }
    }
}
