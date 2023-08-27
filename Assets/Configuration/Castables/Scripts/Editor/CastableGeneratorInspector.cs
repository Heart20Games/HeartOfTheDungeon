using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CastableGenerator))]
public class CastableGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CastableGenerator generator = (CastableGenerator)target;

        EditorGUILayout.Separator();

        string buttonText = generator.prefab != null ? "Regenerate Castable" : "Generate New Castable";

        if (GUILayout.Button(buttonText))
        {
            generator.GenerateCastable();
        }
    }
}
