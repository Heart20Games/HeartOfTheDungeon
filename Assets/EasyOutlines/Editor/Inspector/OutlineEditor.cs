using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OutlineEditor : BasicShaderEditor
{
    private RampEditor m_RampEditor = new RampEditor();

    MaterialProperty rampTexture;
    MaterialProperty rampTextureLoaded;

    MaterialProperty useGradient;
    MaterialProperty intensity;
    MaterialProperty noiseScale;
    MaterialProperty flowSpeed;
    MaterialProperty rotation;
    
    MaterialProperty color;
    MaterialProperty adaptive;
    MaterialProperty thickness;
    MaterialProperty type;

    MaterialProperty outlineSwitch;

    public override void FindProperties(MaterialProperty[] props) 
    {
        base.FindProperties(props);

        rampTexture = FindProperty("_RampMap", props);
        rampTextureLoaded = FindProperty("_RampMapLoaded", props);

        useGradient = FindProperty("_UseGradient", props);
        intensity = FindProperty("_Intensity", props);
        noiseScale = FindProperty("_NoiseScale", props);
        flowSpeed = FindProperty("_FlowSpeed", props);
        rotation = FindProperty("_Rotation", props);

        color = FindProperty("_OutlineColor", props);
        adaptive = FindProperty("_AdaptiveThickness", props);
        thickness = FindProperty("_Thickness", props);
        type = FindProperty("_OutlineType", props);

        outlineSwitch = FindProperty("_Enabled", props);

    }

    public override void Setup()
    {
        base.Setup();

        if (m_RampEditor == null) m_RampEditor = new RampEditor();

        m_RampEditor.Setup(rampTexture, rampTextureLoaded, m_MaterialEditor);
    }


    public override void ShaderPropertiesGUI(Material material, MaterialEditor materialEditor)
    {
        base.ShaderPropertiesGUI(material, materialEditor);

        DrawOutline();
        DrawGradient();
    }

    private void DrawOutline()
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);
        DrawProperty(color);
        DrawProperty(thickness);
        DrawProperty(adaptive);
        DrawProperty(type);
        DrawProperty(outlineSwitch);
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
            DrawProperty(flowSpeed);
            DrawProperty(rotation);

            m_RampEditor.DrawRamp();

            EditorGUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }


        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();


    }
}
