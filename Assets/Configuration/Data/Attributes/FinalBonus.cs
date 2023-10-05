using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    public abstract class FinalBonus : Bonus
    {
        public FinalBonus(int value, float multiplier) : base(value, multiplier) { }
        public override abstract float FinalValue { get; }
    }
}
