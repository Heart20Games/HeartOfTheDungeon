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
            SerializedProperty debugProperty = property.FindPropertyRelative("debug");

            Rect pos = EditorGUI.PrefixLabel(position, new($"[M]  {property.displayName}"));

            pos.width *= 0.25f;
            pos.x *= 0.75f;
            Rect p1 = pos, p2 = pos;
            p2.x += pos.width;

            EditorGUI.PropertyField(p1, valueProperty, GUIContent.none);

            Rect pos2 = EditorGUI.PrefixLabel(p2, new GUIContent("(Debug?)"));
            pos2.x = p2.x + ((pos2.x - p2.x) * 0.5f);
            EditorGUI.PropertyField(pos2, debugProperty, GUIContent.none);
        }
    }
}