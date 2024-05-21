using Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Impact.Other))]
public class ImpactOtherDrawer : PropertyDrawer
{
    bool show;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty gameObjectProperty = property.FindPropertyRelative("gameObject");
        SerializedProperty colliderProperty = property.FindPropertyRelative("collider");


        EditorGUI.BeginProperty(position, label, property);

        Rect foldPos = position;
        show = EditorGUI.BeginFoldoutHeaderGroup(foldPos, show, "Collision");

        if (show)
        {
            Rect pos1 = position, pos2 = position;
            pos1.height = EditorGUIUtility.singleLineHeight;
            pos2.height = EditorGUIUtility.singleLineHeight;
            pos1.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            pos2.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            EditorGUI.PropertyField(pos1, gameObjectProperty, GUIContent.none);
            EditorGUI.PropertyField(pos2, colliderProperty, GUIContent.none);
        }

        EditorGUI.EndFoldoutHeaderGroup();

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (show)
        {
            return (EditorGUIUtility.singleLineHeight * 3) + (EditorGUIUtility.standardVerticalSpacing * 2);
        }
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}

//[CustomPropertyDrawer(typeof(Modded<double>))]
//[CustomPropertyDrawer(typeof(Modded<string>))]
//[CustomPropertyDrawer(typeof(Modded<int>))]
//[CustomPropertyDrawer(typeof(Modded<float>))]
//public class SimpleModifiedDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        SerializedProperty valueProperty = property.FindPropertyRelative("value");
//        SerializedProperty debugProperty = property.FindPropertyRelative("debug");

//        Rect pos = EditorGUI.PrefixLabel(position, new($"[M]  {property.displayName}"));

//        pos.width *= 0.25f;
//        pos.x *= 0.75f;
//        Rect p1 = pos, p2 = pos;
//        p2.x += pos.width;

//        EditorGUI.PropertyField(p1, valueProperty, GUIContent.none);

//        Rect pos2 = EditorGUI.PrefixLabel(p2, new GUIContent("(Debug?)"));
//        pos2.x = p2.x + ((pos2.x - p2.x) * 0.5f);
//        EditorGUI.PropertyField(pos2, debugProperty, GUIContent.none);
//    }
//}
