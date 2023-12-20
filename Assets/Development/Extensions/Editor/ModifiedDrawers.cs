using UnityEditor;
using UnityEngine;

namespace Modifiers
{
    [CustomPropertyDrawer(typeof(Modded<double>))]
    [CustomPropertyDrawer(typeof(Modded<string>))]
    [CustomPropertyDrawer(typeof(Modded<int>))]
    [CustomPropertyDrawer(typeof(Modded<float>))]
    public class SimpleModifiedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            EditorGUI.PropertyField(position, valueProperty, new GUIContent("[M]  " + property.displayName));
        }
    }
}