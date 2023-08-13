using System;
using UnityEditor;
using UnityEngine;


// Credited to James Lafritz via Dev Genius: https://blog.devgenius.io/making-the-inspector-look-better-175baf39ada0


// Tag Drawer
[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagAttributePropertyDrawer : PropertyDrawer
{
    #region Overrides of PropertyDrawer

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null) return;

        if (property.propertyType != SerializedPropertyType.String)
        {
            Debug.LogError($"{nameof(TagAttribute)} supports only string fields (found {property.propertyType.ToString()})");
            return;
        }

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            using (EditorGUI.ChangeCheckScope changeCheck = new())
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                if (changeCheck.changed)
                {
                    property.serializedObject?.ApplyModifiedProperties();
                }
            }
        }
    }
    #endregion
}

// Layer Drawer
[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributePropertyDrawer : PropertyDrawer
{
    #region Overrides of PropertyDrawer

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null) return;

        if (property.propertyType != SerializedPropertyType.Integer)
        {
            Debug.LogError($"{nameof(LayerAttribute)} supports only integer fields (found {property.propertyType.ToString()})");
            return;
        }

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            using (EditorGUI.ChangeCheckScope changeCheck = new())
            {
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                if (changeCheck.changed)
                {
                    property.serializedObject?.ApplyModifiedProperties();
                }
            }
        }
    }
    #endregion
}