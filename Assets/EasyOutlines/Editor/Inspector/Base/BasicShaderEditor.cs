using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class BasicShaderEditor : ShaderGUI
{
    MaterialProperty albedoMap = null;
    MaterialProperty albedoColor = null;
    MaterialProperty alphaClipThreshold = null;
    MaterialProperty normalMap = null;
    MaterialProperty smoothness = null;
    MaterialProperty smoothnessSource = null;
    MaterialProperty specularMap = null;
    MaterialProperty specularValue = null;
    MaterialProperty metallicMap = null;
    MaterialProperty metallicValue = null;
    MaterialProperty occlusionMap = null;

    MaterialProperty emissionUse = null;
    MaterialProperty emissionMap = null;
    MaterialProperty emissionColor = null;

    MaterialProperty useSpecular = null;

    protected MaterialEditor m_MaterialEditor;
    protected Material m_Material;

    /// <summary>
    /// Use this function to find material properties. Called first inside OnGui method.
    /// </summary>
    /// <param name="props"></param>
    public virtual void FindProperties(MaterialProperty[] props)
    {
        useSpecular = FindProperty("_UseSpecular", props);

        

        albedoMap = FindProperty("_MainTex", props);
        albedoColor = FindProperty("_Color", props);
        alphaClipThreshold = FindProperty("_Cutoff", props);
        normalMap = FindProperty("_BumpMap", props);
        smoothness = FindProperty("_Glossiness", props);
        smoothnessSource = FindProperty("_GlossSource", props);

        if(useSpecular.displayName == "yes")
        {
            specularMap = FindProperty("_SpecGlossMap", props);
            specularValue = FindProperty("_SpecColor", props);
        }
        else
        {
            metallicMap = FindProperty("_MetallicGlossMap", props);
            metallicValue = FindProperty("_Metallic", props);
        }
        
        occlusionMap = FindProperty("_OcclusionMap", props);

        emissionUse = FindProperty("_UseEmission", props);
        emissionMap = FindProperty("_EmissionMap", props);
        emissionColor = FindProperty("_EmissionColor", props);

    }


    /// <summary>
    /// Function called before ShaderPropertiesGUI inside OnGui method. Use to initialize things.
    /// </summary>
    public virtual void Setup()
    {

    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props);
        m_MaterialEditor = materialEditor;
        m_Material = materialEditor.target as Material;

        Setup();

        ShaderPropertiesGUI(m_Material, materialEditor);
    }


    /// <summary>
    /// Method to draw your custom editor. Everything will be drawn after basic editor. Called last inside OnGui method.
    /// </summary>
    /// <param name="material"></param>
    /// <param name="materialEditor"></param>
    public virtual void ShaderPropertiesGUI(Material material, MaterialEditor materialEditor)
    {
        MainEditor(useSpecular.displayName == "yes");
        AdvancedEditor();
    }

    protected void DrawBoxSpace(string header, List<MaterialProperty> props)
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        GUILayout.Label(header, ShaderEditorUtility.LabelStyle);

        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        foreach (var prop in props)
        {
            DrawProperty(prop);
        }

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
    }

    protected void DrawToggleBoxScope(MaterialProperty header,List<MaterialProperty> props)
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        DrawToggleHeader(header);


        bool isParamPropEnabled = !Mathf.Approximately(header.floatValue, 0f);
        if(isParamPropEnabled)
        {
            EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
            EditorGUILayout.Space(2);

            foreach (var prop in props)
            {
                DrawProperty(prop);
            }

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
        if(string.IsNullOrEmpty(name))
        {
            name = prop.displayName.Replace("Use", "");
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(name, ShaderEditorUtility.LabelStyle);
        m_MaterialEditor.ShaderProperty(prop, string.Empty);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

    }

    private void MainEditor(bool useSpecular = false)
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        GUILayout.Label("Main Maps", ShaderEditorUtility.LabelStyle);

        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), albedoMap, albedoColor);
        DrawProperty(alphaClipThreshold);

        if(useSpecular)
        {
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Specular"), specularMap, specularValue);
        }
        else
        {
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Metallic"), metallicMap, metallicValue);
        }

        DrawProperty(smoothness);
        DrawProperty(smoothnessSource);
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), normalMap);
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Occlusion Map"), occlusionMap);



        bool emission = m_MaterialEditor.EmissionEnabledProperty();
        if (emission) emissionUse.floatValue = 1;
        else emissionUse.floatValue = 0;

        using (var disableScope = new EditorGUI.DisabledScope(!emission))
        {
            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission Map"), emissionMap, emissionColor, false);
            EditorGUILayout.EndHorizontal();
        }

        m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);


        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
    }

    private void AdvancedEditor()
    {
        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        GUILayout.Label("Advanced", ShaderEditorUtility.LabelStyle);

        EditorGUILayout.BeginVertical(ShaderEditorUtility.BoxScopeStyle);
        EditorGUILayout.Space(2);

        m_MaterialEditor.RenderQueueField();
        m_MaterialEditor.EnableInstancingField();
        m_MaterialEditor.DoubleSidedGIField();
        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
    }
}
