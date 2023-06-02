using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaskOutlineEditor : ShaderGUI
{
    private RampEditor m_RampEditor = new RampEditor();

    MaterialProperty rampTexture;
    MaterialProperty rampTextureLoaded;

    MaterialProperty useGradient;
    MaterialProperty intensity;
    MaterialProperty noiseScale;
    MaterialProperty flowSpeed;
    MaterialProperty rotation;
    MaterialProperty screenSpace;

    MaterialProperty color;
    MaterialProperty adaptive;
    MaterialProperty thickness;
    MaterialProperty type;

    MaterialProperty maskRef;
    MaterialProperty outlineSwitch;

    protected MaterialEditor m_MaterialEditor;
    protected Material m_Material;


    public  void FindProperties(MaterialProperty[] props) 
    {
        rampTexture = FindProperty("_RampMap", props);
        rampTextureLoaded = FindProperty("_RampMapLoaded", props);

        useGradient = FindProperty("_UseGradient", props);
        intensity = FindProperty("_Intensity", props);
        noiseScale = FindProperty("_NoiseScale", props);
        flowSpeed = FindProperty("_FlowSpeed", props);
        rotation = FindProperty("_Rotation", props);
        screenSpace = FindProperty("_ScreenSpace", props);

        color = FindProperty("_OutlineColor", props);
        adaptive = FindProperty("_AdaptiveThickness", props);
        thickness = FindProperty("_Thickness", props);
        type = FindProperty("_OutlineType", props);

        maskRef = FindProperty("_MaskRef", props);

        outlineSwitch = FindProperty("_Enabled", props);
    }

    public void Setup()
    {
        if (m_RampEditor == null) m_RampEditor = new RampEditor();

        m_RampEditor.Setup(rampTexture, rampTextureLoaded, m_MaterialEditor);
    }
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props);
        m_MaterialEditor = materialEditor;
        m_Material = materialEditor.target as Material;

        Setup();

        ShaderPropertiesGUI(m_Material, materialEditor);
    }



    public void ShaderPropertiesGUI(Material material, MaterialEditor materialEditor)
    {
        DrawOutline();
        DrawGradient();
    }

    private void DrawOutline()
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        GUILayout.Label("Outline", ShaderEditorUtility.LabelStyle);
        EditorGUILayout.Space(2);

        DrawProperty(color);
        DrawProperty(thickness);
        DrawProperty(adaptive);
        DrawProperty(type);
        DrawProperty(outlineSwitch);
        EditorGUILayout.Space(2);
        DrawProperty(maskRef);
        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
    }

    private void DrawGradient()
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        DrawToggleHeader(useGradient);

        bool isParamPropEnabled = !Mathf.Approximately(useGradient.floatValue, 0f);
        if (isParamPropEnabled)
        {
            EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
            EditorGUILayout.Space(2);

            DrawProperty(intensity);
            DrawProperty(noiseScale);
            DrawProperty(screenSpace);
            DrawProperty(flowSpeed);
            DrawProperty(rotation);

            m_RampEditor.DrawRamp();

            EditorGUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }


        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();


    }

    protected void DrawProperty(MaterialProperty prop)
    {
        m_MaterialEditor.ShaderProperty(prop, prop.displayName);
    }

    protected void DrawToggleHeader(MaterialProperty prop, string name = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = prop.displayName.Replace("Use", "");
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(name, ShaderEditorUtility.LabelStyle);
        m_MaterialEditor.ShaderProperty(prop, string.Empty);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

    }

}
