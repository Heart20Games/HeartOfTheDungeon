using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

public static class Awarn
{
    // Lambdas
    private static readonly Action<bool,string> AssertIsFalse = (c, m) => { if (m == null) Assert.IsFalse(c); else Assert.IsFalse(c, m); };
    private static readonly Action<bool,string> AssertIsTrue = (c, m) => { if (m == null) Assert.IsTrue(c); else Assert.IsTrue(c, m); };
    private static readonly Action<object,string> AssertIsNotNull = (o, m) => { if (m == null) Assert.IsNotNull(o); else Assert.IsNotNull(o, m); };
    private static readonly Action<object,string> AssertIsNull = (o, m) => { if (m == null) Assert.IsNull(o); else Assert.IsNull(o, m); };

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

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsFalse(bool condition, string message = null)
    {
        BoolAwarn(AssertIsFalse, condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsTrue(bool condition, string message = null)
    {
        BoolAwarn(AssertIsTrue, condition, message);
    }

    // Object Awarn
    private static void ObjectAwarn(Action<object,string> action, object value, string message = null, UnityEngine.Object context=null)
    {
        try
        {
            action.Invoke(value, message);
        }
        catch (AssertionException exception)
        {
            Debug.LogWarning(exception.Message, context);
        }
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNotNull(object obj, string msg)
    {
        ObjectAwarn(AssertIsNotNull, obj, msg);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void IsNull(object obj, string msg)
    {
        ObjectAwarn(AssertIsNull, obj, msg);
    }
}
