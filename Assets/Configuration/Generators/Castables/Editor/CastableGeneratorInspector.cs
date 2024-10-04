using UnityEditor;
using UnityEngine;

namespace HotD.Generators
{
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
}