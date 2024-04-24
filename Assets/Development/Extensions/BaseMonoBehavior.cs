using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public interface IBaseMonoBehaviour
{
    public Transform Transform { get; }
}

public class BaseMonoBehaviour : MonoBehaviour, IBaseMonoBehaviour
{
    public Transform Transform => transform;

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    protected void Print(object message, bool debug = true, Object context=null)
    {
        if (debug) Debug.Log(message, context == null ? this : context);
    }

    void OnDestroy()
    {
        // Clears any and all lists, dictionaries, and non-primitive fields on the GameObject when it is destroyed.
        // Intention: to minimize the risk of memory leaks resulting from incorrect disposal of such things.
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            Type fieldType = field.FieldType;

            if (typeof(IList).IsAssignableFrom(fieldType))
            {
                if (field.GetValue(this) is IList list)
                {
                    list.Clear();
                }
            }

            if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                if (field.GetValue(this) is IDictionary dictionary)
                {
                    dictionary.Clear();
                }
            }

            if (!fieldType.IsPrimitive)
            {
                field.SetValue(this, null);
            }
        }
    }
}
