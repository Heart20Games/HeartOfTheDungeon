using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public interface IBaseMonoBehaviour
{
    public Transform Transform { get; }
}

public class BaseMonoBehaviour : MonoBehaviour, IBaseMonoBehaviour
{
    public Transform Transform => transform;

    void OnDestroy()
    {
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
