using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public class Attribute : BaseAttribute
    {
        protected List<Weighted<Bonus>> rawBonuses = new();
        protected List<Weighted<Bonus>> finalBonuses = new();

        protected float finalValue;

        public Attribute(int startingValue) : base(startingValue)
        {
            finalValue = BaseValue;
        }

        public float FinalValue { get => CalculateValue(); }

        public void AddRawBonus(RawBonus bonus, float weight = 1)
        {
            rawBonuses.Add(new(bonus, weight));
        }

        public void AddFinalBonus(FinalBonus bonus, float weight = 1)
        {
            finalBonuses.Add(new(bonus, weight));
        }

        public void RemoveRawBonus(RawBonus bonus, float weight = 1)
        {
            rawBonuses.Remove(new(bonus, weight));
        }

        public void RemoveFinalBonus(FinalBonus bonus, float weight = 1)
        {
            finalBonuses.Remove(new(bonus, weight));
        }

        public virtual void Clear()
        {
            rawBonuses.Clear();
            finalBonuses.Clear();
        }


        protected void ApplyBonuses<T>(List<Weighted<T>> bonuses) where T : BaseAttribute
        {
            // Adding value from bonuses
            float bonusValue = 0;
            float bonusMultiplier = 0;

            foreach (Weighted<T> bonus in bonuses)
            {
                bonusValue += bonus.value.BaseValue * bonus.weight;
                bonusMultiplier += bonus.value.BaseMultiplier;
            }

            finalValue += bonusValue;
            finalValue *= (1 + bonusMultiplier);
        }

        public virtual float CalculateValue()
        {
            finalValue = BaseValue;

            ApplyBonuses(rawBonuses);
            ApplyBonuses(finalBonuses);

            return finalValue;
        }
    }
}
