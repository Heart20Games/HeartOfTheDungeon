using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class BaseScriptableObject : ScriptableObject, INamed
{
    public string Name { get => name; }

    [ReadOnly] public bool initialized = false;
    public virtual void Init() { initialized = true; }

#if !UNITY_EDITOR
        private void Awake() => Init();
#endif

    private void OnDestroy()
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
