using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on Composite Design Pattern from Daniel Sidhion's article: https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t

namespace Attributes
{
    public class RawBonus : Bonus
    {
        public RawBonus(int value, float multiplier) : base(value, multiplier) { }
    }
}
