using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Modified<double>))]
[CustomPropertyDrawer(typeof(Modified<string>))]
[CustomPropertyDrawer(typeof(Modified<int>))]
[CustomPropertyDrawer(typeof(Modified<float>))]
public class SimpleModifiedDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        EditorGUI.PropertyField(position, valueProperty, new GUIContent("[M]  " + property.displayName));
    }
}