using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modifiers
{
    [Serializable]
    public class ModField<T> where T : IComparable
    {
        public ModField(string name, T startValue)
        {
            this.name = name;
            current = new(startValue, $"{name} Current");
        }

        [HideInInspector] public string name;
        public Modded<T> current;

        // Subscribe
        public void Subscribe(Modded<T>.Modify currentModifier = null) { current.Subscribe(currentModifier); }
        public void Subscribe(Modded<T>.Listen currentListener = null) { current.Subscribe(currentListener); }

        // Unsubscribe
        public void UnSubscribe(Modded<T>.Modify currentModifier = null) { current.UnSubscribe(currentModifier); }
        public void UnSubscribe(Modded<T>.Listen currentListener = null) { current.UnSubscribe(currentListener); }
    }


    [Serializable]
    public class MaxModField<T> : ModField<T> where T : IComparable
    {
        public MaxModField(string name, T startValue, T maxValue, Modded<T>.Constraint constraint = null, Modded<T>.Constraint constrainer = null) : base(name, startValue)
        {
            max = new(maxValue, $"{name} Max");

            current.constraint = constraint ?? BeConstrained;
            max.constraint = constrainer ?? Constrain;
        }

        public Modded<T> max;

        // Constraint
        public T BeConstrained(T finalValue)
        {
            return finalValue.CompareTo(max.Value) < 0 ? finalValue : max.Value;
        }

        public T Constrain(T finalValue)
        {
            current.Value = finalValue.CompareTo(current.Value) < 0 ? max.Value : current.Value;
            return finalValue;
        }

        // Subscribe
        public void Subscribe(Modded<T>.Modify currentModifier = null, Modded<T>.Modify maxModifier = null)
        {
            base.Subscribe(currentModifier);
            max.Subscribe(maxModifier);
        }

        public void Subscribe(Modded<T>.Listen currentListener = null, Modded<T>.Listen maxListener = null)
        {
            base.Subscribe(currentListener);
            max.Subscribe(maxListener);
        }

        // Unsubscribe
        public void UnSubscribe(Modded<T>.Modify currentModifier = null, Modded<T>.Modify maxModifier = null)
        {
            base.UnSubscribe(currentModifier);
            max.UnSubscribe(maxModifier);
        }

        public void UnSubscribe(Modded<T>.Listen currentListener = null, Modded<T>.Listen maxListener = null)
        {
            base.UnSubscribe(currentListener);
            max.UnSubscribe(maxListener);
        }
    }

    [Serializable]
    public class Modded<T>
    {
        [SerializeField] protected bool debug = false;

        [HideInInspector] public string name;
        [SerializeField] protected T baseValue;
        [SerializeField] protected T value;
        public T Value
        {
            get { return value; }
            set { SetValue(value); }
        }
        public T BaseValue
        {
            get { return baseValue; }
        }

        public delegate T Modify(T oldValue, T newValue);
        [HideInInspector] public List<Modify> modifiers = new();

        public delegate void Listen(T finalValue);
        [HideInInspector] public List<Listen> listeners = new();

        public delegate T Constraint(T finalValue);
        [HideInInspector] public Constraint constraint;

        // Constructor
        public Modded(T value, string name = "[Modded]")
        {
            this.value = value;
            this.name = name;
        }

        // Modifiers
        public void Subscribe(Modify modify)
        {
            if (modify != null)
                if (!modifiers.Contains(modify))
                    modifiers.Add(modify);
        }

        public void UnSubscribe(Modify modify)
        {
            if (modify != null)
                if (modifiers.Contains(modify))
                    modifiers.Remove(modify);
        }

        // Listeners
        public void Subscribe(Listen listen)
        {
            if (listen != null)
                if (!listeners.Contains(listen))
                    listeners.Add(listen);
        }

        public void UnSubscribe(Listen listen)
        {
            if (listen != null)
                if (listeners.Contains(listen))
                    listeners.Remove(listen);
        }

        // Setter
        public void SetValue(T value)
        {
            if (debug) Debug.Log($"Trying to set {name} value to {value} from {this.value}.");
            foreach (Modify modifier in modifiers)
            {
                value = modifier.Invoke(this.value, value);
            }
            if (debug) Debug.Log($"Modifiers applied. New {name} value now {value}.");
            if (constraint != null)
            {
                value = constraint.Invoke(value);
            }
            if (debug) Debug.Log($"Constraint applied. New {name} value now {value}.");
            foreach (Listen listener in listeners)
            {
                listener.Invoke(value);
            }
            if (debug) Debug.Log($"{name} value set to {value} from {this.value}.");
            this.value = value;
        }
    }
}
