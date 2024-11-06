using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    [CustomEditor(typeof(ManualTriggerByParent))]
    public class ManualTriggerByParentEditor : Editor
    {
        private bool showDescription;

        public override void OnInspectorGUI()
        {


            serializedObject.Update();

            showDescription = EditorUtils.usualStartWithDescription(Strings.MANUAL_TRIGGER_BY_PARENT_TITLE,
                                                                            Strings.MANUAL_TRIGGER_BY_PARENT_DESCRIPTION,
                                                                            showDescription);



            EditorUtils.usualEnd();
            serializedObject.ApplyModifiedProperties();
        }
    }
}