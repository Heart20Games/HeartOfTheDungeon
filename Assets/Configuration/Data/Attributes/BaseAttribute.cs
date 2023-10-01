using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Editor;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    public class BaseAttribute
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

        public BaseAttribute(int value, float multiplier = 0)
        {
            baseValue = value;
            baseMultiplier = multiplier;
        }

        public int BaseValue { get => baseValue; set => baseValue = value; }
        public float BaseMultiplier { get => baseMultiplier; set => baseMultiplier = value; }
    }
}
