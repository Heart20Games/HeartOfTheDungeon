using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public abstract class BaseAttribute
    {
        [Serializable] public struct Weighted<T>
        {
            public Weighted(T value, float weight, string name = "[Weighted]")
            {
                this.name = name;
                this.value = value;
                this.weight = weight;
            }
            [HideInInspector] public string name;
            public T value;
            public float weight;
        }

        public enum Part { BaseValue, BaseMultiplier }

        public string name;

        [ReadOnly][SerializeField] private float finalPreview;

        [SerializeField] private int baseValue;
        [SerializeField] private float baseMultiplier;
        [SerializeField] private Vector2 baseValueRange = new(0, 5);
        [SerializeField] private Vector2 baseMultiplierRange = new(0, 5);

        [SerializeField] protected bool debug = false;

        [HideInInspector] public UnityEvent updated = new();
        [HideInInspector] public UnityEvent<float> updatedFinalFloat = new();
        [HideInInspector] public UnityEvent<int> updatedFinalInt = new();
        public void Updated()
        {
            if (debug) Debug.Log($"Updated {name}");
            updated.Invoke();
            float finalValue = FinalValue;
            updatedFinalFloat.Invoke(finalValue);
            updatedFinalInt.Invoke((int)finalValue);
        }

        public BaseAttribute(int value, string name = "[New Base Attribute]", float multiplier = 0)
        {
            baseValue = value;
            this.name = name;
            baseMultiplier = multiplier;
        }

        public abstract float FinalValue { get; }

        public int BaseValue { get => baseValue; set { baseValue = (int)Mathf.Clamp(value, baseValueRange.x, baseValueRange.y); Updated(); } }
        public float BaseMultiplier { get => baseMultiplier; set { baseMultiplier = (int)Mathf.Clamp(value, baseMultiplierRange.x, baseMultiplierRange.y); Updated(); } }
    }
}
