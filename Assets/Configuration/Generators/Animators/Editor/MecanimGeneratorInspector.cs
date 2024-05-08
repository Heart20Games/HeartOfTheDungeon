using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MecanimGenerator))]
public class MecanimGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        MecanimGenerator generator = (MecanimGenerator)target;
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        string buttonText = generator.mecanim != null ? "Regenerate Mecanim" : "Generate New Mecanim";

        if (GUILayout.Button(buttonText))
        {
            generator.GenerateMecanim();
        }
    }
}
