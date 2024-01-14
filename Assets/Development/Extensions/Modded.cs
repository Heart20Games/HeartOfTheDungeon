using System;
using System.Collections.Generic;

namespace Modifiers
{
    [Serializable]
    public class ModField<T>
    {
        public ModField(string name, T startValue, T maxValue)
        {
            this.name = name;
            current = new(startValue);
            max = new(maxValue);
        }

        public string name;
        public Modded<T> current;
        public Modded<T> max;

        // Subscribe
        public void Subscribe(Modded<T>.Modify currentModifier = null, Modded<T>.Modify maxModifier = null)
        {
            current.Subscribe(currentModifier);
            max.Subscribe(maxModifier);
        }

        public void Subscribe(Modded<T>.Listen currentListener = null, Modded<T>.Listen maxListener = null)
        {
            current.Subscribe(currentListener);
            max.Subscribe(maxListener);
        }

        // Unsubscribe
        public void UnSubscribe(Modded<T>.Modify currentModifier = null, Modded<T>.Modify maxModifier = null)
        {
            current.UnSubscribe(currentModifier);
            max.UnSubscribe(maxModifier);
        }

        public void UnSubscribe(Modded<T>.Listen currentListener = null, Modded<T>.Listen maxListener = null)
        {
            current.UnSubscribe(currentListener);
            max.UnSubscribe(maxListener);
        }
    }

    [Serializable]
    public class Modded<T>
    {
        protected T baseValue;
        protected T value;
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
        public List<Modify> modifiers = new();

        public delegate void Listen(T finalValue);
        public List<Listen> listeners = new();

        // Constructor
        public Modded(T value)
        {
            this.value = value;
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
}
