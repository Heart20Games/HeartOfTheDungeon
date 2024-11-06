using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class STSDialogSamplesImport : EditorWindow
{
    private Texture2D stslogo;
    private Texture2D samplespic;
    private GUIStyle replacementStyle = new GUIStyle();
    private GUIStyle replacementStyle2 = new GUIStyle();
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        GetWindow<STSDialogSamplesImport>("See-through Shader Samples");
    }

    void OnGUI()
    {
       // stslogo = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/samplesimport.PNG") as Texture2D;
        samplespic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/samplesimport.PNG") as Texture2D;
        replacementStyle.normal.textColor = Color.white;
        replacementStyle.alignment = TextAnchor.MiddleCenter;
        replacementStyle.fontStyle = FontStyle.Bold;
        replacementStyle2.fixedWidth = 1500;
        replacementStyle2.fixedHeight = 900;
        replacementStyle2.imagePosition = ImagePosition.ImageOnly;


        //DrawHeaderArea();        
        EditorUtils.Header("Import See-through Shader Samples", replacementStyle);
        Rect rectt = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox); 
        GUI.Box(rectt, GUIContent.none);
        //GUILayout.FlexibleSpace();
        
        //GUILayout.FlexibleSpace();
        GUILayout.Label(samplespic, replacementStyle2);
        //GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();






    }
}
