using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    [Serializable]
    public abstract class Bonus : BaseAttribute
    {
        public Bonus(int value, float multiplier=0, string name = "[New Bonus]") : base(value, name, multiplier) { }
        public override abstract float FinalValue { get; }
    }
}
