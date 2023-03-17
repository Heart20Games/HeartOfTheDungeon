using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using static Yarn.Compiler.BasicBlock;

public static class Awarn
{
    // Lambdas
    private static Action<bool,string> AssertIsFalse = (c, m) => { if (m == null) Assert.IsFalse(c); else Assert.IsFalse(c, m); };
    private static Action<bool,string> AssertIsTrue = (c, m) => { if (m == null) Assert.IsTrue(c); else Assert.IsTrue(c, m); };
    private static Action<object,string> AssertIsNotNull = (o, m) => { if (m == null) Assert.IsNotNull(o); else Assert.IsNotNull(o, m); };
    private static Action<object,string> AssertIsNull = (o, m) => { if (m == null) Assert.IsNull(o); else Assert.IsNull(o, m); };

    // Bool Awarn
    private static void BoolAwarn(Action<bool,string> action, bool condition, string message = null)
    {
        try
        {
            action.Invoke(condition, message);
        }
        catch (AssertionException exception)
        {
            Debug.LogWarning(exception.Message);
        }
    }

    public static void IsFalse(bool condition, string message = null)
    {
        BoolAwarn(AssertIsFalse, condition, message);
    }

    public static void IsTrue(bool condition, string message = null)
    {
        BoolAwarn(AssertIsTrue, condition, message);
    }

    // Object Awarn
    private static void ObjectAwarn(Action<object,string> action, object value, string message = null)
    {
        try
        {
            action.Invoke(value, message);
        }
        catch (AssertionException exception)
        {
            Debug.LogWarning(exception.Message);
        }
    }

    public static void IsNotNull(object obj, string msg)
    {
        ObjectAwarn(AssertIsNotNull, obj, msg);
    }

    public static void IsNull(object obj, string msg)
    {
        ObjectAwarn(AssertIsNull, obj, msg);
    }
}
