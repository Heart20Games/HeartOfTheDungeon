using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ShaderCrew.SeeThroughShader
{
    [CustomEditor(typeof(PlayerToCameraRaycastTriggerManager))]
    public class PlayerToCameraRaycastTriggerManagerEditor : Editor
    {

        private bool showDescription;

        private PlayerToCameraRaycastTriggerManager playerToCameraRaycastTriggerManager;
        private SerializedProperty playableCharactersProperty;

        private ReorderableList playableCharactersList;

        private SerializedProperty showDebugRays;
        private SerializedProperty timeUntilExit;


        private void OnEnable()
        {

            playerToCameraRaycastTriggerManager = (PlayerToCameraRaycastTriggerManager)target;


            showDebugRays = serializedObject.FindProperty(nameof(PlayerToCameraRaycastTriggerManager.ShowDebugRays));
            timeUntilExit = serializedObject.FindProperty(nameof(PlayerToCameraRaycastTriggerManager.timeUntilExit));

            playableCharactersProperty = serializedObject.FindProperty(nameof(PlayerToCameraRaycastTriggerManager.playerList));

            playableCharactersList = new ReorderableList(serializedObject, playableCharactersProperty)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = false,
                drawHeaderCallback = rect =>
                {
                    //GUI.color = Color.cyan;
                    //GUI.backgroundColor = Color.red;
                    EditorGUI.LabelField(rect, playableCharactersProperty.displayName, EditorStyles.boldLabel);

                },
                drawElementCallback = (rect, index, focused, active) =>
                {

                    var element = playableCharactersProperty.GetArrayElementAtIndex(index);

                    var backgroundColor = GUI.backgroundColor;
                    int count = 0;
                    for (int i = 0; i < playableCharactersProperty.arraySize; i++)
                    {
                        if (playableCharactersProperty.GetArrayElementAtIndex(i).objectReferenceValue == element.objectReferenceValue)
                        {
                            count++;
                        }
                    }
                    if (element.objectReferenceValue == null || count > 1)
                    {
                        GUI.backgroundColor = Color.red;
                    }
                    //else if (
                    //        ((GameObject)element.objectReferenceValue).GetComponent<Collider>() == null ||
                    //        ((GameObject)element.objectReferenceValue).GetComponent<Collider>().enabled == false)
                    //{
                    //    GUI.backgroundColor = Color.magenta;
                    //}
                    else
                    {
                        GUI.backgroundColor = backgroundColor;
                    }




                    //GUI.color = Color.cyan;
                    GUIStyle insertVarNameHere = new GUIStyle();
                    insertVarNameHere.fontStyle = FontStyle.Bold;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element)), element, new GUIContent("Character " + (index + 1) + ": "));

                    GUI.backgroundColor = backgroundColor;


                    if (element.objectReferenceValue == null)
                    {
                        rect.y += EditorGUI.GetPropertyHeight(element);
                        EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "Character  " + (index + 1) + "  may not be empty!", MessageType.Error);
                    }
                    else if (count > 1)
                    {
                        rect.y += EditorGUI.GetPropertyHeight(element);
                        EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "Duplicate! Character  " + (index + 1) + "  has to be unique!", MessageType.Error);
                    }
                    //else if (((GameObject)element.objectReferenceValue).GetComponent<Rigidbody>() == null)
                    //{
                    //    rect.y += EditorGUI.GetPropertyHeight(element);
                    //    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "Character " + (index + 1) + " has no Rigidbody attached to itself. Without it, Triggers won't work!", MessageType.Info);
                    //}
                    //else if (((GameObject)element.objectReferenceValue).GetComponent<Collider>() == null)
                    //{
                    //    rect.y += EditorGUI.GetPropertyHeight(element);
                    //    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "Character " + (index + 1) + "  has no Collider. Without it, Triggers won't work!", MessageType.Info);
                    //}
                    //else if (((GameObject)element.objectReferenceValue).GetComponent<Collider>().enabled == false)
                    //{
                    //    rect.y += EditorGUI.GetPropertyHeight(element);
                    //    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "Character " + (index + 1) + "  has a Collider, but it is disabled. Triggers won't work!", MessageType.Info);
                    //}
                },

                elementHeightCallback = index =>
                {
                    var element = playableCharactersProperty.GetArrayElementAtIndex(index);
                    int count = 0;
                    for (int i = 0; i < playableCharactersProperty.arraySize; i++)
                    {
                        if (playableCharactersProperty.GetArrayElementAtIndex(i).objectReferenceValue == element.objectReferenceValue)
                        {
                            count++;
                        }
                    }
                    var height = EditorGUI.GetPropertyHeight(element);
                    if (element.objectReferenceValue == null || count > 1)
                    {
                        height += EditorGUIUtility.singleLineHeight * 2;
                    }
                    //else if (
                    //        ((GameObject)element.objectReferenceValue).GetComponent<Collider>() == null ||
                    //        ((GameObject)element.objectReferenceValue).GetComponent<Collider>().enabled == false
                    //)
                    //{
                    //    height += EditorGUIUtility.singleLineHeight * 2;
                    //}
                    return height;
                },

                onAddCallback = list =>
                {
                    list.serializedProperty.arraySize++;

                    var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                    newElement.objectReferenceValue = null;
                },
                onRemoveCallback = list =>
                {
                    List<GameObject> temp = new List<GameObject>();
                    for (int i = 0; i < list.serializedProperty.arraySize; i++)
                    {
                        temp.Add((GameObject)list.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    }
                    list.serializedProperty.arraySize--;
                    temp.RemoveAt(list.index);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var d = list.serializedProperty.GetArrayElementAtIndex(i);
                        d.objectReferenceValue = temp[i];
                    }
                    //ReorderableList.defaultBehaviours.DoRemoveButton(list);

                }

            };

        }

        public override void OnInspectorGUI()
        {
            //base.DrawDefaultInspector();

            serializedObject.Update();
            showDescription = EditorUtils.usualStartWithDescription(Strings.PLAYER_TO_CAMERA_RAYCAST_TRIGGER_MANAGER_TITLE,
                                                                            Strings.PLAYER_TO_CAMERA_RAYCAST_TRIGGER_MANAGER_DESCRIPTION,
                                                                            showDescription);
            //Color c = new Color(1, 1, 1);
            //GUI.backgroundColor = c;

            playableCharactersList.DoLayoutList();
            EditorGUILayout.PropertyField(showDebugRays);
            EditorGUILayout.PropertyField(timeUntilExit);
            EditorUtils.usualEnd();
            serializedObject.ApplyModifiedProperties();
        }
    }
}