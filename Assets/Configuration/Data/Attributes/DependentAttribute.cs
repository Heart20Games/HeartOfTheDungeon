using System;
using System.Collections.Generic;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public class DependentAttribute : Attribute
    {
        protected List<Weighted<Attribute>> otherAttributes = new();
        public List<Weighted<Attribute>> OtherAttributes { get { return otherAttributes ??= new(); } }

        public DependentAttribute(int startingValue) : base(startingValue) { }

        public override float FinalValue { get => CalculateValue(); }

        public void AddAttribute(Attribute attribute, float weight=1)
        {
            OtherAttributes.Add(new(attribute, weight));
        }

        public void RemoveAttribute(Attribute attribute, float weight=1)
        {
            OtherAttributes.Remove(new(attribute, weight));
        }

        public override void Clear()
        {
            OtherAttributes.Clear();
            base.Clear();
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
