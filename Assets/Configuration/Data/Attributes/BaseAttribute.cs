using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    public abstract class BaseAttribute
    {
        public readonly struct Weighted<T>
        {
            public Weighted(T value, float weight)
            {
                this.value = value;
                this.weight = weight;
            }
            public readonly T value;
            public readonly float weight;
        }

        public enum Part { BaseValue, BaseMultiplier }
        [SerializeField] private int baseValue;
        [SerializeField] private float baseMultiplier;
        [SerializeField] private Vector2 baseValueRange = new(0, 5);
        [SerializeField] private Vector2 baseMultiplierRange = new(0, 5);

        [HideInInspector] public UnityEvent updated = new();
        [HideInInspector] public UnityEvent<float> updatedFinal = new();
        public void Updated()
        {
            updated.Invoke();
            updatedFinal.Invoke(FinalValue);
        }

        public BaseAttribute(int value, float multiplier = 0)
        {
            baseValue = value;
            baseMultiplier = multiplier;
        }

        public abstract float FinalValue { get; }

        public int BaseValue { get => baseValue; set { baseValue = (int)Mathf.Clamp(value, baseValueRange.x, baseValueRange.y); Updated(); } }
        public float BaseMultiplier { get => baseMultiplier; set { baseMultiplier = (int)Mathf.Clamp(value, baseMultiplierRange.x, baseMultiplierRange.y); Updated(); } }
    }
}
