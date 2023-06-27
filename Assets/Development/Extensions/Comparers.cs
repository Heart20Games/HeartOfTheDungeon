using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumComparer<TEnum> where TEnum : struct, IEqualityComparer<TEnum>
{
    public bool Equals(TEnum x, TEnum y)
    {
        return EqualityComparer<TEnum>.Default.Equals(x, y);
    }

    public int GetHashCode(TEnum x)
    {
        return EqualityComparer<TEnum>.Default.GetHashCode(x);
    }
}
