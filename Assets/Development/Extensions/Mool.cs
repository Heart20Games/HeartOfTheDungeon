using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Mool
{
    // Static
    public enum Value { Maybe = -1, No = 0, Yes = 1 }
    public static Mool Maybe { get { return new(Value.Maybe); } }
    public static Mool No { get { return new(Value.No); } }
    public static Mool Yes { get { return new(Value.Yes); } }

    // Local
    public Value value;
    
    public Mool(Value value) { this.value = value; }
    public Mool(int value)
    {
        this.value = value < 0 ? Value.Maybe : value > 0 ? Value.Yes : Value.No; 
    }

    public bool IsMaybe { get { return value == Value.Maybe || value == Value.Yes; } }
    public bool IsNo { get { return value == Value.No; } }
    public bool IsYes { get { return value == Value.Yes; } }
}
