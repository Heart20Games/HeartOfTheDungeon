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

        public int BaseValue { get => baseValue; set { baseValue = value; Updated(); } }
        public float BaseMultiplier { get => baseMultiplier; set { baseMultiplier = value; Updated(); } }
    }
}
