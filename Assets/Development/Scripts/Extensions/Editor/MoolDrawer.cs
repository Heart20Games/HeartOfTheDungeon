using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Mool))]
public class MoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProperty = property.FindPropertyRelative("value");

        Rect pos = EditorGUI.PrefixLabel(position, new($"[M]  {property.displayName}"));

        Rect valPos = pos;

        EditorGUI.PropertyField(valPos, valueProperty, GUIContent.none);
    }
}
