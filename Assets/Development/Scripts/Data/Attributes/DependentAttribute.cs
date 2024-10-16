using System;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public class DependentAttribute : Attribute
    {
        [SerializeField] protected List<Weighted<Attribute>> otherAttributes = new();
        public List<Weighted<Attribute>> OtherAttributes { get { return otherAttributes ??= new(); } }

        public DependentAttribute(int startingValue, string name = "[New Dependent Attribute]", string owner = "[Unknown]") : base(startingValue, name, owner) { }

        public override float FinalValue { get => CalculateValue(); }

        public void AddAttribute(Attribute attribute, float weight=1)
        {
            attribute.updated.AddListener(Updated);
            OtherAttributes.Add(new(attribute, weight, attribute.name));
        }

        public void RemoveAttribute(Attribute attribute, float weight=1)
        {
            attribute.updated.RemoveListener(Updated);
            OtherAttributes.Remove(new(attribute, weight, attribute.name));
        }

        public bool HasAttribute(Attribute attribute, float weight=0)
        {
            foreach (var otherAttribute in OtherAttributes)
            {
                if (debug) Debug.Log($"{otherAttribute.value.name} vs {attribute.name}");
                if (otherAttribute.value == attribute)
                    if (weight == 0 || otherAttribute.weight == weight)
                        return true;
            }
            return false;
        }

        public override void Clear()
        {
            ClearAttributes(OtherAttributes);
            base.Clear();
        }

        public virtual void ClearAttributes(List<Weighted<Attribute>> attributeList)
        {
            foreach (Weighted<Attribute> attribute in attributeList)
            {
                attribute.value.updated.RemoveListener(Updated);
            }
            attributeList.Clear();
        }

        public override float CalculateValue()
        {
            finalValue = BaseValue;

            ApplyBonuses(OtherAttributes);
            ApplyBonuses(RawBonuses);
            ApplyBonuses(FinalBonuses);

            return finalValue;
        }
    }
}
