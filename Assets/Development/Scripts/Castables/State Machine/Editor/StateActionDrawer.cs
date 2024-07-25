using HotD.Castables;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

[CustomPropertyDrawer(typeof(StateAction))]
public class StateActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var stateField = property.FindPropertyRelative("state");
        var actionField = property.FindPropertyRelative("action");
        Rect labelPosition = new(position);

        position = EditorGUI.PrefixLabel(labelPosition, EditorGUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        float labelWidth = EditorGUIUtility.labelWidth;

        float halfWidth = position.width / 2;
        float spacing = 25;

        Rect statePos = new(position.x, position.y, halfWidth - (spacing / 2) - 5, position.height);
        Rect actionPos = new(position.x + halfWidth, position.y, halfWidth - (spacing / 2), position.height);

        EditorGUIUtility.labelWidth = 40;
        EditorGUI.PropertyField(statePos, stateField, new GUIContent("State"));
        EditorGUIUtility.labelWidth = 45;
        EditorGUI.PropertyField(actionPos, actionField, new GUIContent("Action"));

        EditorGUI.indentLevel = indent;
        EditorGUIUtility.labelWidth = labelWidth;

        EditorGUI.EndProperty();
    }
}

