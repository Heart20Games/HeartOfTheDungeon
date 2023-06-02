using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RampEditor
{
    static int previewWidth = 64;
    static int width = 128;
    static string texturePath;

    private Texture2D m_cachedTexture;
    private Texture2D m_cachedTexturePreview;

    private MaterialProperty rampMap = null;
    private MaterialProperty rampMapLoaded = null;
    MaterialEditor m_MaterialEditor = null;

    public static Gradient gradient;// = new Gradient
    //{
    //    mode = GradientMode.Fixed,
    //    colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0.5f), new GradientColorKey(Color.white, 1) },
    //    alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
    //};


    public void Setup(MaterialProperty rampTexture, MaterialProperty rampTextureLoaded, MaterialEditor materialEditor)
    {
        rampMap = rampTexture;
        rampMapLoaded = rampTextureLoaded;
        m_MaterialEditor = materialEditor;
    }

    public void DrawRamp()
    {
        if (gradient == null) gradient = new Gradient();

        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        m_MaterialEditor.ShaderProperty(rampMap, rampMap.displayName);
        if (EditorGUI.EndChangeCheck())
        {
            rampMapLoaded.floatValue = 1;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.GradientField(gradient);
        if (EditorGUI.EndChangeCheck())
        {
            m_cachedTexturePreview = UpdateTex(previewWidth);
            rampMap.textureValue = m_cachedTexturePreview;
            rampMapLoaded.floatValue = 0;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        using (var disableScope = new EditorGUI.DisabledScope(rampMapLoaded.floatValue == 1))
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set path for texture PNG"))
            {
                SetTexturePath();
            }


            if (GUILayout.Button("Export as PNG"))
            {
                m_cachedTexture = UpdateTex(width);

                ExportToPNG(m_cachedTexture);

                rampMapLoaded.floatValue = 0;
                rampMap.textureValue = null;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.TextField(texturePath);

        }


        EditorGUILayout.EndVertical();

    }


    private Texture2D UpdateTex(int width)
    {
        Texture2D tex = null;

        if (tex == null)
        {
            tex = new Texture2D(width, 1, TextureFormat.RGBA32, 0, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.name = "RampTexture";
        }

        var colors = new Color[tex.width * tex.height];
        for (int x = 0; x < width; ++x)
        {
            colors[x] = gradient.Evaluate(1.0f * x / (width - 1));
        }
        tex.SetPixels(colors);
        tex.Apply();


        return tex;
    }

    private void ExportToPNG(Texture2D rampTex)
    {
        var savePath = texturePath;

        if (string.IsNullOrEmpty(savePath))
        {
            SetTexturePath();

            savePath = texturePath;

            var bytes = rampTex.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
        }

        if (!string.IsNullOrEmpty(savePath))
        {
            var bytes = rampTex.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
        }

        //rampMap.textureValue = (Texture)AssetDatabase.LoadMainAssetAtPath(savePath);

    }

    private void SetTexturePath()
    {
        var currentRampObjPath = AssetDatabase.GetAssetPath(m_MaterialEditor.serializedObject.targetObject);
        var defaultDirectory = System.IO.Path.GetDirectoryName(currentRampObjPath);
        texturePath = EditorUtility.SaveFilePanelInProject("Export as PNG file", "Ramp", "png", "Set file path for the PNG file", defaultDirectory);
    }
}
