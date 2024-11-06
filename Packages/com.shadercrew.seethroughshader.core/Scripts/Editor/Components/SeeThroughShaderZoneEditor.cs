using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    [CustomEditor(typeof(SeeThroughShaderZone))]
    public class SeeThroughShaderZoneEditor : Editor
    {
        private SeeThroughShaderZone seeThroughShaderZone;
        private bool showDescription;



        private SerializedProperty showZoneOnlyWhenSelected;
        private SerializedProperty transitionDuration;
        //private SerializedProperty type;

        //private SerializedProperty layerMask;

        //private float tempLocalScaleX; // for keeping x and z scale synced
        //private float tempLocalScaleZ;
        private void OnEnable()
        {
            seeThroughShaderZone = target as SeeThroughShaderZone;
            showZoneOnlyWhenSelected = serializedObject.FindProperty(nameof(SeeThroughShaderZone.showZoneOnlyWhenSelected));
            transitionDuration = serializedObject.FindProperty(nameof(SeeThroughShaderZone.transitionDuration));
            //layerMask = serializedObject.FindProperty(nameof(SeeThroughShaderZone.layerMask));

            //type = serializedObject.FindProperty(nameof(SeeThroughShaderZone.type));

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            //if (type.enumValueIndex != (int)ZoneType.Box) // for keeping x and z scale synced
            //{
            //    Vector3 localScale = seeThroughShaderZone.transform.localScale;
            //    if (tempLocalScaleX != localScale.x)
            //    {
            //        //localScale.z = localScale.x;
            //    }

            //    //    if (tempLocalScaleZ != localScale.z)
            //    //    {
            //    //        localScale.x = localScale.z;
            //    //    }

            //    //    if (type.enumValueIndex != (int)ZoneType.Box) // for keeping x and z scale synced
            //    //    {
            //    //        //localScale.y = localScale.x;
            //    //    }
            //    tempLocalScaleX = localScale.x;
            //    //    tempLocalScaleZ = localScale.z;
            //    //    seeThroughShaderZone.transform.localScale = localScale;
            //}


            //SeeThroughShaderEditorUtils.usualStart("Trigger Box");
            showDescription = EditorUtils.usualStartWithDescription(Strings.STS_ZONE,
                                                    Strings.STS_ZONE_DESCRIPTION,
                                                    showDescription);
            var oriCol = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.white;
            //base.DrawDefaultInspector();

            EditorGUILayout.PropertyField(showZoneOnlyWhenSelected);

            //// Start a code block to check for GUI changes
            //EditorGUI.BeginChangeCheck();
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(transitionDuration);
                //EditorGUILayout.PropertyField(layerMask);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(transitionDuration);
                //EditorGUILayout.PropertyField(layerMask);
            }

            //// End the code block and update the label if a change occurred
            //if (EditorGUI.EndChangeCheck())
            //{
            //    Debug.Log("HEY!");
            //}


            if (seeThroughShaderZone != null)
            {
                string buttonText;

                if (!seeThroughShaderZone.isActivated)
                {
                    buttonText = "Activate Zone";
                }
                else
                {
                    buttonText = "Deactivate Zone";
                }
                if (GUILayout.Button(buttonText))
                {
                    if (seeThroughShaderZone.isActivated)
                    {
                        seeThroughShaderZone.toggleZoneActivation();
                    }
                    else
                    {
                        seeThroughShaderZone.toggleZoneActivation();
                    }
                }
            }

            EditorUtils.usualEnd();
            EditorStyles.label.normal.textColor = oriCol;
            serializedObject.ApplyModifiedProperties();
        }

    }
}