using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class STSDialog2019MatUpgrade : EditorWindow
{
    private bool m_FirstTimeApply;
    private Texture2D stslogo;
    private Texture2D urppic;
    private GUIStyle replacementStyle = new GUIStyle();
    private GUIStyle replacementStyle2 = new GUIStyle();

    // Start is called before the first frame update
    //[MenuItem("Window/SeeThroughShader 2019/2020 Dialog")]
    public static void ShowWindow()
    {
        GetWindow<STSDialog2019MatUpgrade>("SeeThroughShader 2019/2020 Dialog");
    }

    void OnGUI()
    {
        stslogo = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Editor/Resources/logo-with-outline - Kopie.png") as Texture2D;
        urppic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/20192020URP.PNG") as Texture2D;
        replacementStyle.normal.textColor = Color.white;
        replacementStyle.alignment = TextAnchor.MiddleCenter;
        replacementStyle.fontStyle = FontStyle.Bold;               
        
        replacementStyle2.fixedHeight = 800;
        replacementStyle2.fixedWidth = 600;
        replacementStyle2.imagePosition = ImagePosition.ImageOnly;
        
        //DrawHeaderArea();        

        Rect rectt = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.Box(rectt, GUIContent.none);
        EditorUtils.Header("Upgrade URP Materials",replacementStyle);        
        GUILayout.Label(urppic,replacementStyle2);
        EditorGUILayout.EndVertical();

       
      



    }
}
