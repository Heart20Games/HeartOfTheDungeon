using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using static CastableGenerator;

[CustomEditor(typeof(CastableGenerator))]
public class CastableGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CastableGenerator generator = (CastableGenerator)target;
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        string buttonText = generator.prefab != null ? "Regenerate Castable" : "Generate New Castable";

        if (GUILayout.Button(buttonText))
        {
            generator.GenerateCastable();
        }
    }
}
