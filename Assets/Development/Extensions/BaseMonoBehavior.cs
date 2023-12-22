using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public interface IBaseMonoBehavior
{
    public Transform GetTransform();
}

public class BaseMonoBehaviour : MonoBehaviour, IBaseMonoBehavior
{
    public Transform GetTransform() { return transform; }

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
