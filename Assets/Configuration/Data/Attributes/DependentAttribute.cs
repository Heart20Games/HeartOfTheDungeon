using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public class DependentAttribute : Attribute
    {
        protected List<Weighted<Attribute>> otherAttributes = new();

        public DependentAttribute(int startingValue) : base(startingValue) { }

        public void AddAttribute(Attribute attribute, float weight=1)
        {
            otherAttributes.Add(new(attribute, weight));
        }

        public void RemoveAttribute(Attribute attribute, float weight=1)
        {
            otherAttributes.Remove(new(attribute, weight));
        }

        public override float CalculateValue()
        {
            finalValue = BaseValue;

            ApplyBonuses(otherAttributes);
            ApplyBonuses(rawBonuses);
            ApplyBonuses(finalBonuses);

            return finalValue;
        }
    }
}
