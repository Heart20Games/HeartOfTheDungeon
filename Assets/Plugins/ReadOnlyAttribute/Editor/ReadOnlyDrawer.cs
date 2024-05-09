using UnityEditor;
using UnityEngine;

/* Custom ReadOnly Attribute Drawer
 * 
 * Used in conjunction with ReadOnlyAttribute.
 * As seen at: https://forum.unity.com/threads/read-only-fields.68976/
 */
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
