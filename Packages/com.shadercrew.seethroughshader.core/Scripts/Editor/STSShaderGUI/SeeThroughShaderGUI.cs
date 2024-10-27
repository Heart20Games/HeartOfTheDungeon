using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using static ShaderCrew.SeeThroughShader.GeneralUtils;
using static ShaderCrew.SeeThroughShader.SeeThroughShaderGUI;

namespace ShaderCrew.SeeThroughShader
{

    public enum DissolveMethod
    {
        TextureBased = 0,
        Dither = 1,
        None = 2,
    }

    public enum DissolveTexSpace
    {
        Local = 0,
        World = 1
    }

    public class SeeThroughShaderGUI
    {

        public SeeThroughShaderGUI()
        {

        }

        public SeeThroughShaderGUI(Dictionary<string, bool> _STSKeywordBools)
        {
            STSKeywordBools = _STSKeywordBools;
        }
        Dictionary<string, bool> STSKeywordBools;

        public MaterialEditor m_MaterialEditor;

        Color oriCol;



        Color textColor;
        Color originalColor;



        // See-through Shader
        public MaterialProperty dissolveMethod = null;
        public MaterialProperty dissolveTexSpace = null;

        public MaterialProperty dissolveMap = null;
        public MaterialProperty dissolveColor = null;
        public MaterialProperty dissolveSize = null;
        public MaterialProperty dissolveColorSaturation = null;

        public MaterialProperty dissolveEmmission = null;
        public MaterialProperty dissolveEmmissionBooster = null;
        public MaterialProperty dissolveTexturedEmissionEdge = null;
        AnimBool dissolveTexturedEmissionEdgeAnimBool;
        public MaterialProperty dissolveTexturedEmissionEdgeStrength = null;

        public MaterialProperty dissolveClippedShadowsEnabled = null;


        public MaterialProperty crossSectionEnabled = null;
        public MaterialProperty crossSectionColor = null;
        public MaterialProperty crossSectionTextureEnabled = null;
        public MaterialProperty crossSectionTexture = null;
        public MaterialProperty crossSectionTextureScale = null;
        public MaterialProperty crossSectionUVScaledByDistance = null;




        public MaterialProperty dissolveTextureAnimationEnabled = null;
        AnimBool dissolveTextureAnimationEnabledAnimBool;
        public MaterialProperty dissolveTextureAnimationSpeed = null;
        public MaterialProperty dissolveTransitionDuration = null;

        public MaterialProperty interactionMode = null;
        //MaterialProperty centerPosition = null;

        public MaterialProperty obstructionMode = null;
        public MaterialProperty angleStrength = null;
        public MaterialProperty coneStrength = null;
        public MaterialProperty coneObstructionDestroyRadius = null;
        public MaterialProperty cylinderStrength = null;
        public MaterialProperty cylinderObstructionDestroyRadius = null;
        public MaterialProperty circleStrength = null;
        public MaterialProperty circleObstructionDestroyRadius = null;

        public MaterialProperty curveStrength = null;
        public MaterialProperty curveObstructionDestroyRadius = null;

        public MaterialProperty dissolveFallOff = null;
        public MaterialProperty dissolveMask = null;
        public MaterialProperty dissolveMaskEnabled = null;
        AnimBool dissolveMaskEnabledAnimBool;

        public MaterialProperty affectedAreaPlayerBasedObstruction = null;

        public MaterialProperty intrinsicDissolveStrength = null;


        AnimBool ceilingEnabledAnimBool;
        public MaterialProperty ceilingEnabled = null;
        public MaterialProperty ceilingMode = null;
        public MaterialProperty ceilingBlendMode = null;
        public MaterialProperty ceilingY = null;
        public MaterialProperty ceilingPlayerYOffset = null;
        public MaterialProperty ceilingYGradientLength = null;

        AnimBool isometricExlusionEnabledAnimBool;
        public MaterialProperty isometricExlusionEnabled = null;
        public MaterialProperty isometricExclusionDistance = null;
        public MaterialProperty isometricExclusionGradientLength = null;

        AnimBool floorEnabledAnimBool;
        public MaterialProperty floorEnabled = null;
        public MaterialProperty floorMode = null;
        public MaterialProperty floorY = null;
        public MaterialProperty playerPosYOffset = null;
        public MaterialProperty floorYTextureGradientLength = null;
        public MaterialProperty affectedAreaFloor = null;
        

        AnimBool zoningEnabledAnimBool;
        public MaterialProperty zoningEnabled = null;
        public MaterialProperty zoningMode = null;
        public MaterialProperty zoningEdgeGradientLength = null;
        public MaterialProperty zoningIsRevealable = null;
        AnimBool zoningSyncZonesWithFloorYAnimBool = null;
        public MaterialProperty zoningSyncZonesWithFloorY = null;
        public MaterialProperty zoningSyncZonesFloorYOffset = null;

        AnimBool debugModeEnabledAnimBool;
        public MaterialProperty debugModeEnabled = null;
        public MaterialProperty debugModeIndicatorLineThickness = null;

        public MaterialProperty isReplacementShader = null;


        public MaterialProperty enableDefaultEffectRadius = null;
        AnimBool enableDefaultEffectRadiusAnimBool;
        public MaterialProperty defaultEffectRadius = null;


        public MaterialProperty isReferenceMaterialMat = null;

        public bool isReferenceMaterial;

        AnimationCurve curveY;
        public AnimationCurveSO curveSO;
        Texture2D curveTexture;

        public MaterialProperty dissolveObstructionCurve = null;
        readonly int curveTextureResolution = 512;



        public MaterialProperty useCustomTime = null;



        public MaterialProperty showContentDissolveArea = null;
        public MaterialProperty showContentInteractionOptionsArea = null;
        public MaterialProperty showContentObstructionOptionsArea = null;
        public MaterialProperty showContentAnimationArea = null;
        public MaterialProperty showContentZoningArea = null;
        public MaterialProperty showContentReplacementOptionsArea = null;
        public MaterialProperty showContentDebugArea = null;

        public MaterialProperty syncCullMode = null;

        public MaterialProperty cull = null;


        //// TODO: move inside shader so it can get serialized
        //private bool showContentDissolveArea = true;
        //private bool showContentInteractionOptionsArea = true;
        //private bool showContentObstructionOptionsArea = true;
        //private bool showContentAnimationArea = true;
        //private bool showContentZoningArea = true;
        //private bool showContentDebugArea = true;

        public enum STSInteractionMode
        {
            PlayerBased = 0,
            Independent = 1
        }
        public enum ObstructionMode
        {
            None,
            AngleOnly,
            ConeOnly,
            AngleAndCone,
            CylinderOnly,
            AngleAndCylinder,
            Circle,
            Curve
        }
        public enum FloorMode
        {
            Manual,
            PlayerPosition
        }
        public enum CeilingMode
        {
            Manual,
            PlayerPosition
        }
        public enum CeilingBlendMode
        {
            Additive,
            Subtractive
        }
        public enum ZoningMode
        {
            Additive,
            Subtractive
        }

        public enum AffectedArea
        {
            Everywhere,
            InsideAdditiveZones,
            OutsideAdditiveZones
        }
        


        public static class SeethroughShaderStyles
        {
            public static GUIContent dissolveText = EditorGUIUtility.TrTextContent("Dissolve Effect Texture", "Dissolve Effect Texture");
            public static GUIContent crossSectionTextureText = EditorGUIUtility.TrTextContent("Screen-Space Texture", "Screen-Space Texture");
            public static GUIContent dissolveSizeText = EditorGUIUtility.TrTextContent("Texture Scale", "Dissolve Texture Scale");
            public static GUIContent dissolveEmissionText = EditorGUIUtility.TrTextContent("Strength", "Dissolve Emission Strength");
            public static GUIContent dissolveEmissionBoosterText = EditorGUIUtility.TrTextContent("Dissolve Emission Booster", "Dissolve Emission Booster");
            public static GUIContent dissolveColorSaturationText = EditorGUIUtility.TrTextContent("Saturation", "Dissolve Color Saturation");

            public static GUIContent dissolveMaskText = EditorGUIUtility.TrTextContent("Dissolve Mask", "Dissolve Mask (R)");

            public static string stsShaderText = "See-through Shader : <i>Properties</i>";
        }




        public string GetSTSMaterialType()
        {
            if (isReferenceMaterialMat.floatValue == 1)
            {
                isReferenceMaterial = true;
            }
            else
            {
                isReferenceMaterial = false;
            }

            return isReferenceMaterial ? "Reference Shader" : "The Shader";
        }


        RenderPipeline rp = RenderPipeline.NONE;
        public void DoSetup(MaterialEditor materialEditor)
        {
            if(rp == RenderPipeline.NONE)
            {
                rp = GeneralUtils.getCurrentRenderPipeline();
            }

            m_MaterialEditor = materialEditor;

            dissolveTexturedEmissionEdgeAnimBool = new AnimBool(false);
            dissolveTexturedEmissionEdgeAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            dissolveTextureAnimationEnabledAnimBool = new AnimBool(false);
            dissolveTextureAnimationEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            dissolveMaskEnabledAnimBool = new AnimBool(false);
            dissolveMaskEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            ceilingEnabledAnimBool = new AnimBool(false);
            ceilingEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            isometricExlusionEnabledAnimBool = new AnimBool(false);
            isometricExlusionEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            floorEnabledAnimBool = new AnimBool(false);
            floorEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            enableDefaultEffectRadiusAnimBool = new AnimBool(false);
            enableDefaultEffectRadiusAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            zoningEnabledAnimBool = new AnimBool(false);
            zoningEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            zoningSyncZonesWithFloorYAnimBool = new AnimBool(false);
            zoningSyncZonesWithFloorYAnimBool.valueChanged.AddListener(materialEditor.Repaint);

            debugModeEnabledAnimBool = new AnimBool(false);
            debugModeEnabledAnimBool.valueChanged.AddListener(materialEditor.Repaint);


            if (dissolveObstructionCurve == null || dissolveObstructionCurve.textureValue == null)
            {
                curveTexture = new Texture2D(curveTextureResolution, 1, TextureFormat.R8, false, true);
            }
            else
            {
                curveTexture = (Texture2D)dissolveObstructionCurve.textureValue;
            }


            if (curveSO == null)
            {
                string name = (materialEditor.target as Material).name;
                name = name.Replace("/","_");
                //Debug.Log("name " + name);
                if (!name.Contains(SeeThroughShaderConstants.STS_INSTANCE_PREFIX) &&
                    !name.Contains(SeeThroughShaderConstants.STS_TRIGGER_PREFIX))
                {
                    curveSO = Resources.Load<AnimationCurveSO>("AnimationCurveScriptableObjects/" + name);
                }


                if (curveSO == null)
                {
                    curveSO = ScriptableObject.CreateInstance<AnimationCurveSO>();
                    curveSO.curve = AnimationCurve.Linear(0, 0, 1, 1);
                    if (!name.Contains(SeeThroughShaderConstants.STS_INSTANCE_PREFIX) &&
                        !name.Contains(SeeThroughShaderConstants.STS_TRIGGER_PREFIX))
                    {
                        var dirPath = "Packages/com.shadercrew.seethroughshader.core/Scripts/Editor/Resources/AnimationCurveScriptableObjects/";
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        //string directoryNameMaterial = Path.GetDirectoryName(dirPath + name);
                        //if (!Directory.Exists(directoryNameMaterial))
                        //{
                        //    Directory.CreateDirectory(directoryNameMaterial);
                        //}
                        //Debug.Log("dirpath " + directoryNameMaterial);

                        AssetDatabase.CreateAsset(curveSO, dirPath + name + ".asset");


                    }
                }
                else
                {
                    if (!curveSO.isBakedToTexture)
                    {
                        SaveTexture(materialEditor.target as Material);
                        //name = name.Replace(" (Instance)", "");
                        Texture2D texture = Resources.Load("Curves/" + name, typeof(Texture2D)) as Texture2D;
                        SetTextureImporterFormat(texture, true);
                        dissolveObstructionCurve.textureValue = texture;
                        //material.SetTexture("_ObstructionCurve", texture);
                        curveSO.isBakedToTexture = true;
                        EditorUtility.SetDirty(curveSO);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }

            if (obstructionMode.floatValue == (float)ObstructionMode.Curve)
            {
                CreateInitialTexture(materialEditor.target as Material);
            }

            originalColor = EditorStyles.label.normal.textColor;


            if (EditorGUIUtility.isProSkin)
            {
                textColor = Color.white;
                oriCol = EditorStyles.label.normal.textColor;
            }
            else
            {
                //textColor = EditorStyles.label.normal.textColor;
                textColor = Color.black;
                oriCol = new Color(0.9f, 0.9f, 0.9f, 1);
            }


            MaterialChanged(materialEditor.target as Material);

        }


        public void STSShaderPropertiesGUI(Material material)
        {

            EditorGUILayout.Space();

            //EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
            GUIStyle replacementStyle = new GUIStyle();

            replacementStyle.normal.textColor = textColor;
            replacementStyle.alignment = TextAnchor.MiddleCenter;
            replacementStyle.fontStyle = FontStyle.Bold;
            replacementStyle.fontSize = 15;
            replacementStyle.richText = true;

            GUILayout.Label(SeethroughShaderStyles.stsShaderText, replacementStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (!material.name.Contains(SeeThroughShaderConstants.STS_INSTANCE_PREFIX) &&
                    !material.name.Contains(SeeThroughShaderConstants.STS_TRIGGER_PREFIX))
            {
                float originalLabelWidht = EditorGUIUtility.labelWidth;
                if (!isReferenceMaterial)
                {
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 94;
                }
                else
                {
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 94; //80
                }
                DoDissolveArea(material);
                DoInteractionOptionsArea(material);
                DoObstructionOptionsArea(material);
                DoAnimationArea(material);
                DoZoningArea(material);
                if (!material.HasProperty("_IsMicroSplatSTS"))
                {
                    DoReplacementArea(material);
                }

                DoDebugArea(material);
                EditorGUIUtility.labelWidth = originalLabelWidht;
                EditorGUILayout.Space();
            }
            else
            {
                if (material.name.Contains(SeeThroughShaderConstants.STS_INSTANCE_PREFIX))
                {
                    EditorGUILayout.HelpBox("This is just an Instanced Material! If you want to change the 'See-through Shader' settings, you have to do that in the associated Reference Material.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("This is a Trigger enabled STS Material! If you want to change the 'See-through Shader' settings, you have to do that in the associated Material, before you press play.", MessageType.Error);
                }
            }

            }

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

        }



        void DoDissolveArea(Material material)
        {



            //MakeSTSSectionHeader("Dissolve Effect Texture and Styling");
            EditorGUI.indentLevel += 1;
            //showContentDissolveArea = MakeSTSSectionHeaderWithFoldout("Dissolve Effect Texture and Styling", showContentDissolveArea);
            showContentDissolveArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Dissolve Effect Texture and Styling", Convert.ToBoolean(showContentDissolveArea.floatValue)));

            EditorGUI.indentLevel -= 1;
            if (showContentDissolveArea.floatValue == 1)
            {

                EditorGUI.indentLevel += 1;
                EditorGUILayout.Space();
                float oriLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2f;
                dissolveMethod.floatValue = (int)(DissolveMethod)EditorGUILayout.EnumPopup("Dissolve Method", (DissolveMethod)dissolveMethod.floatValue);
                EditorGUIUtility.labelWidth = oriLabelWidth;
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 1;
                if (dissolveMethod.floatValue == 0)
                {
                    EditorGUI.indentLevel += 1;
                    m_MaterialEditor.TexturePropertySingleLine(SeethroughShaderStyles.dissolveText, dissolveMap, dissolveColor);
                    EditorGUI.indentLevel -= 1;
                    if (dissolveMap.textureValue == null)
                    {
                        EditorGUILayout.HelpBox("You didn't select any dissolve texture! The 'See-through Shader' effect won't work without it!", MessageType.Error);
                    } else
                    {
                        EditorGUI.indentLevel += 2;

                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2f;
                        dissolveTexSpace.floatValue = (int)(DissolveTexSpace)EditorGUILayout.EnumPopup("Texture Space", (DissolveTexSpace)dissolveTexSpace.floatValue);
                        EditorGUIUtility.labelWidth = oriLabelWidth;

                        m_MaterialEditor.ShaderProperty(dissolveSize, SeethroughShaderStyles.dissolveSizeText);

                        EditorGUI.indentLevel -= 2;

                    }
                } 
                else
                {
                    EditorGUI.indentLevel += 2;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 100;
                    m_MaterialEditor.ShaderProperty(dissolveColor, "Color");
                    EditorGUIUtility.labelWidth = oriLabelWidth;
                    EditorGUI.indentLevel -= 2;
                }


                if (dissolveMap.textureValue != null || dissolveMethod.floatValue != 0)
                {
                    EditorGUI.indentLevel += 2;
                    m_MaterialEditor.ShaderProperty(dissolveColorSaturation, SeethroughShaderStyles.dissolveColorSaturationText);
                    EditorGUILayout.Space();

                    EditorUtils.DrawUILineSubMenu();

                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Emission");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    m_MaterialEditor.ShaderProperty(dissolveEmmission, SeethroughShaderStyles.dissolveEmissionText);

                    if (dissolveEmmission.floatValue > 0)
                    {
                        m_MaterialEditor.ShaderProperty(dissolveEmmissionBooster, SeethroughShaderStyles.dissolveEmissionBoosterText);
                        EditorGUILayout.Space();
                        dissolveTexturedEmissionEdge.floatValue = Convert.ToSingle(EditorGUILayout.Toggle("Textured Emission Edge", Convert.ToBoolean(dissolveTexturedEmissionEdge.floatValue)));
                        dissolveTexturedEmissionEdgeAnimBool.target = dissolveTexturedEmissionEdge.floatValue == 1;
                        if (EditorGUILayout.BeginFadeGroup(dissolveTexturedEmissionEdgeAnimBool.faded))
                        {
                            //EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(dissolveTexturedEmissionEdgeStrength, "Strength");
                            //EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndFadeGroup();

                    }

                    //if (obstructionMode.floatValue != 6)
                    //{
                        EditorGUILayout.Space();

                        EditorUtils.DrawUILineSubMenu();

                        EditorGUI.indentLevel -= 1;
                        EditorStyles.label.normal.textColor = oriCol;
                        EditorGUILayout.LabelField("Shadows");
                        EditorStyles.label.normal.textColor = textColor;
                        EditorGUI.indentLevel += 1;

                        m_MaterialEditor.ShaderProperty(dissolveClippedShadowsEnabled, "Enable Clipped Shadows");
                    if (obstructionMode.floatValue == 6 && dissolveClippedShadowsEnabled.floatValue == 1)
                    {
                        EditorGUILayout.HelpBox("In Obstruction Mode \"Circle\" Clipped Shadows act like in \"None\" Mode!", MessageType.Info);

                    }

#if USING_HDRP
                    if (material.HasProperty("_CrossSectionEnabled") )
                    {
                        GUI.enabled = false;
                        crossSectionEnabled.floatValue = 0;
                    }
#elif !USING_URP
                    bool containsNonForwardCam = false;
                    bool containsForwardCam = false;
                    if(Camera.allCamerasCount > 0 )
                    {
                        foreach (Camera cam in Camera.allCameras)
                        {
                            if(cam != null)
                            {
                                if (cam.actualRenderingPath != RenderingPath.Forward)
                                {
                                    containsNonForwardCam = true;
                                } 
                                else if (cam.actualRenderingPath == RenderingPath.Forward)
                                {
                                    containsForwardCam = true;
                                }
                            }
   
                        }
                    } else
                    {
                        containsForwardCam = true;
                    }

#endif
                    if (material.HasProperty("_CrossSectionEnabled") && crossSectionEnabled != null && crossSectionColor != null)
                    {

                        EditorGUILayout.Space();

                        EditorUtils.DrawUILineSubMenu();


                        EditorGUI.indentLevel -= 1;
                        EditorStyles.label.normal.textColor = oriCol;
                        crossSectionEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Cross-Section",
                                                                  Convert.ToBoolean(crossSectionEnabled.floatValue)));
                        EditorStyles.label.normal.textColor = textColor;
                        EditorGUI.indentLevel += 1;

                        //enableDefaultEffectRadiusAnimBool.target = enableDefaultEffectRadius.floatValue == 1;
                        //if (EditorGUILayout.BeginFadeGroup(enableDefaultEffectRadiusAnimBool.faded))
                        if (crossSectionEnabled.floatValue == 1)
                        {
                            m_MaterialEditor.TexturePropertySingleLine(SeethroughShaderStyles.crossSectionTextureText, crossSectionTexture, crossSectionColor);
                            if(crossSectionTexture != null && crossSectionTexture.textureValue != null)
                            {
                                m_MaterialEditor.ShaderProperty(crossSectionTextureScale, "Texture Scale");
                                m_MaterialEditor.ShaderProperty(crossSectionUVScaledByDistance, "Scale UV by Camera Distance");
                                crossSectionTextureEnabled.floatValue = 1.0f;
                            }
                            else
                            {
                                crossSectionTextureEnabled.floatValue = 0.0f;
                            }

                            if (cull != null && cull.floatValue != 0) 
                            {
                                EditorGUILayout.HelpBox("You have to set culling to OFF/BOTH, for the Cross-Section effect to work", MessageType.Warning);
                            } 
                            else if (cull == null)
                            {
                                EditorGUILayout.HelpBox("Don't forget, you have to set culling to off, for the Cross-Section effect to work!", MessageType.Info);
                            }

                            //#if USING_URP
                            //                                EditorGUILayout.HelpBox("The CrossSection feature doesn't work with the Deferred Rendering Path!", MessageType.Info);
                            //#endif
                        }
                        //EditorGUILayout.EndFadeGroup();

                    }

                    GUI.enabled = true;

#if USING_HDRP
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("The CrossSection feature doesn't work with HDRP!", MessageType.Warning);
#elif !USING_URP
                    if(material.HasProperty("_CrossSectionEnabled"))
                    {
                        if (containsNonForwardCam)
                        {
                            EditorGUILayout.Space();

                            if (containsForwardCam)
                            {
                                EditorGUILayout.HelpBox("Keep in mind that the CrossSection feature only works with the Forward Rendering Path!", MessageType.Info);
                            }
                            else if (Camera.allCamerasCount > 0)
                            {
                                EditorGUILayout.HelpBox("The CrossSection feature only works with the Forward Rendering Path! Currently your scene only has cameras in the Non-Forward Rendering Path!", MessageType.Warning);
                            }
                        }
                        if (Camera.allCamerasCount <= 0)
                        {
                            EditorGUILayout.HelpBox("The CrossSection feature only works with the Forward Rendering Path! Don't forget to add cameras in the Forward Rendering Path for this feature to work!", MessageType.Info);
                        }

                    }

#endif




                    //}


                    EditorGUI.indentLevel -= 2;

                }

                EditorGUILayout.Space();

            }
            else
            {

            }
        }


        void DrawBox(Rect position, Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            GUI.Box(position, GUIContent.none);
            GUI.color = oldColor;
        }



        void MakeSTSSectionHeader(string name)
        {
            Rect rect2 = EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (EditorGUIUtility.isProSkin)
            {
                //GUI.Box(rect2, GUIContent.none);
                DrawBox(rect2, new Color(0.5f, 1.5f, 3f, 1f));
            }
            else
            {
                DrawBox(rect2, new Color(0.8f, 0.8f, 0.8f, 1));
            }

            GUILayout.Label(name, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }


        bool MakeSTSSectionHeaderWithFoldout(string name, bool showContent)
        {

            //showDescription = EditorGUILayout.Foldout(showDescription, name, EditorStyles.boldLabel); //, EditorStyles.boldLabel);

            Rect rect2 = EditorGUILayout.BeginVertical();

            if (EditorGUIUtility.isProSkin)
            {
                Color lightBlue = new Color(0.5f, 0.6f, 0.7f, 1);
                //EditorUtils.DrawUILine(new Color(0.2f, 0.2f, 0.2f, 1f));
                EditorUtils.DrawUILine(lightBlue, 1, 0);

            }
            else
            {
                Color lightBlue = new Color(0.6f, 0.8f, 2f, 1);
                EditorUtils.DrawUILine(rect2, new Color(0.1f, 0.1f, 0.1f, 1f), 2, -8);
                EditorUtils.DrawUILine(rect2, lightBlue, 1, -5);
            }

            //EditorGUILayout.Space();
            if (EditorGUIUtility.isProSkin)
            {
                //GUI.Box(rect2, GUIContent.none);
                DrawBox(rect2, new Color(0.5f, 1.5f, 2.5f, 1f));
            }
            else
            {

                DrawBox(rect2, new Color(0.5f, 0.7f, 0.9f, 1f));
                //DrawBox(rect2, new Color(0.8f, 0.8f, 0.8f, 1));
            }
            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = EditorStyles.foldout.fontSize + 1;
            Color myStyleColor = new Color(1f, 1f, 1f, 1f);

            style.normal.textColor = myStyleColor;
            style.onNormal.textColor = myStyleColor;
            style.hover.textColor = myStyleColor;
            style.onHover.textColor = myStyleColor;
            style.focused.textColor = myStyleColor;
            style.onFocused.textColor = myStyleColor;
            style.active.textColor = myStyleColor;
            style.onActive.textColor = myStyleColor;
            showContent = EditorGUILayout.Foldout(showContent, name, style); //, EditorStyles.boldLabel);
            style.fontStyle = previousStyle;
            //GUILayout.Label(name, EditorStyles.boldLabel);

            //EditorGUILayout.Space();



            if (EditorGUIUtility.isProSkin)
            {
                EditorUtils.DrawUILine(new Color(0.25f, 0.25f, 0.25f, 1f));
            }
            else
            {
                EditorUtils.DrawUILine(rect2, new Color(0.1f, 0.1f, 0.1f, 1f), 2, -8);
            }

            EditorGUILayout.EndVertical();


            return showContent;
        }

        void DoAnimationArea(Material material)
        {

            //MakeSTSSectionHeader("Dissolve Effect Animations");
            EditorGUI.indentLevel += 1;
            //showContentAnimationArea = MakeSTSSectionHeaderWithFoldout("Dissolve Effect Animations", showContentAnimationArea);
            showContentAnimationArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Dissolve Effect Animations", Convert.ToBoolean(showContentAnimationArea.floatValue)));

            EditorGUI.indentLevel -= 1;
            //showDescription = EditorGUILayout.Foldout(showDescription, title);
            if (showContentAnimationArea.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;

                if((DissolveMethod)dissolveMethod.floatValue == DissolveMethod.TextureBased)
                {
                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Dissolve Texture");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    GUIStyle optionStyle = new GUIStyle();
                    optionStyle.normal.textColor = textColor;
                    //optionStyle.fontSize = 15;
                    optionStyle.fontStyle = FontStyle.Bold;
                    dissolveTextureAnimationEnabled.floatValue = Convert.ToSingle(EditorGUILayout.Toggle("Animation Enabled", Convert.ToBoolean(dissolveTextureAnimationEnabled.floatValue)));
                    //m_MaterialEditor.ShaderProperty(dissolveTextureAnimationEnabled,dissolveTextureAnimationEnabled.displayName);
                    dissolveTextureAnimationEnabledAnimBool.target = dissolveTextureAnimationEnabled.floatValue == 1;
                    if (EditorGUILayout.BeginFadeGroup(dissolveTextureAnimationEnabledAnimBool.faded))
                    {
                        //EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(dissolveTextureAnimationSpeed, "Speed");
                        //EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndFadeGroup();

                    EditorGUILayout.Space();

                    EditorUtils.DrawUILineSubMenu();
                }


                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                EditorGUILayout.LabelField("Enter/Exit Transition");
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;


                if (Application.isPlaying)
                {
                    GUI.enabled = false;
                    m_MaterialEditor.ShaderProperty(dissolveTransitionDuration, "Transition Duration In Seconds");
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("You can't change the transition duration while being in 'Play Mode'", MessageType.Info);

                }
                else
                {
                    m_MaterialEditor.ShaderProperty(dissolveTransitionDuration, "Transition Duration In Seconds");
                }
                EditorGUILayout.Space();

                EditorUtils.DrawUILineSubMenu();
                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                EditorGUILayout.LabelField("Use Custom Time (Global)");
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;

                m_MaterialEditor.ShaderProperty(useCustomTime, "Use Custom Time");
                if(useCustomTime.floatValue == 1)
                {
                    EditorGUILayout.LabelField("Globally Set \"_STSCustomTime\": " + Shader.GetGlobalFloat("_STSCustomTime"));
                    EditorGUILayout.HelpBox("You can use \"STSCustomTimeSetter.DirectlySetSTSCustomTime(float customTime)\" or \"Shader.SetGlobalFloat(\"_STSCustomTime\", customTime)\" to set the custom time.", MessageType.Info);

                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 2;
            }


        }


        void DoZoningArea(Material material)
        {
            float oriLabelWidth = EditorGUIUtility.labelWidth;
            //MakeSTSSectionHeader("Zoning");

            EditorGUI.indentLevel += 1;
            //showContentZoningArea = MakeSTSSectionHeaderWithFoldout("Zoning", showContentZoningArea);
            showContentZoningArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Zoning", Convert.ToBoolean(showContentZoningArea.floatValue)));
            EditorGUI.indentLevel -= 1;
            if (showContentZoningArea.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;

                //SeeThroughShaderEditorUtils.DrawUILineSubMenu();

                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                zoningEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Zoning",
                                                          Convert.ToBoolean(zoningEnabled.floatValue)));
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;

                if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null && STSKeywordBools.ContainsKey("_ZONING"))
                {
                    if(STSKeywordBools["_ZONING"])
                    {
                        zoningEnabled.floatValue = 1;
                        EditorGUILayout.HelpBox("To deactivate Zoning you have to do that in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);

                    }
                    else
                    {
                        zoningEnabled.floatValue = 0;
                        EditorGUILayout.HelpBox("To activate Zoning you have to do that in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);

                    }

                }


                EditorGUILayout.Space();

                zoningEnabledAnimBool.target = zoningEnabled.floatValue == 1;
                if (EditorGUILayout.BeginFadeGroup(zoningEnabledAnimBool.faded))
                {
                    //EditorGUI.indentLevel++;

                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                    //m_MaterialEditor.ShaderProperty(zoningMode, "Zoning Mode");
                    zoningMode.floatValue = (int)(ZoningMode)EditorGUILayout.EnumPopup("Zoning Mode", (ZoningMode) zoningMode.floatValue);
                    EditorGUIUtility.labelWidth = oriLabelWidth;

                    m_MaterialEditor.ShaderProperty(zoningIsRevealable, "Is Zoning Revealable");

                    //if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                    //{
                    //}
                    //else
                    //{
                    //    zoningIsRevealable.floatValue = 0;
                    //}

                    m_MaterialEditor.ShaderProperty(zoningEdgeGradientLength, "Zone Edge Gradient Length");
                    makeAlwaysPositiv(zoningEdgeGradientLength);

                    //m_MaterialEditor.ShaderProperty(isometricExclusionGradientLength, "Dissolve Texture Gradient Length");
                    //makeAlwaysPositiv(isometricExclusionGradientLength);
                    //EditorGUILayout.LabelField("Zoning stuff..");
                    //EditorGUI.indentLevel--;

                    if (floorEnabled.floatValue == 1)
                    {
                        EditorGUILayout.Space();
                        EditorUtils.DrawUILineSubMenu();


                        EditorGUI.indentLevel -= 1;
                        EditorStyles.label.normal.textColor = oriCol;
                        EditorGUILayout.LabelField("Synchronization of Zones and FloorY");
                        EditorStyles.label.normal.textColor = textColor;
                        EditorGUI.indentLevel += 1;

                        zoningSyncZonesWithFloorY.floatValue = Convert.ToSingle(EditorGUILayout.Toggle("Sync Zones With FloorY",
                                                                  Convert.ToBoolean(zoningSyncZonesWithFloorY.floatValue)));


                        zoningSyncZonesWithFloorYAnimBool.target = zoningSyncZonesWithFloorY.floatValue == 1;
                        if (EditorGUILayout.BeginFadeGroup(zoningSyncZonesWithFloorYAnimBool.faded))
                        {
                            m_MaterialEditor.ShaderProperty(zoningSyncZonesFloorYOffset, "Sync FloorY Offset");
                        }
                        EditorGUILayout.EndFadeGroup();
                    }

                    if(obstructionMode.floatValue == 0 && intrinsicDissolveStrength.floatValue == 0)
                    {
                        EditorGUILayout.HelpBox("In \"None\" Mode you won't see any effect of Zoning if you don't increase Intrinsic Dissolve Obstruction Strength beyond 0", MessageType.Info);
                    }



                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 2;
            }


        }


        void DoDebugArea(Material material)
        {
            float oriLabelWidth = EditorGUIUtility.labelWidth;

            //MakeSTSSectionHeader("Debug");

            EditorGUI.indentLevel += 1;
            //showContentDebugArea = MakeSTSSectionHeaderWithFoldout("Debug", showContentDebugArea);
            showContentDebugArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Debug", Convert.ToBoolean(showContentDebugArea.floatValue)));
            EditorGUI.indentLevel -= 1;
            if (showContentDebugArea.floatValue == 1)
            {


                //makeBetweenRange(previewIndicatorLineThickness,0.1f,0.5f);


                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;

                //SeeThroughShaderEditorUtils.DrawUILineSubMenu();

                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                debugModeEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Preview Mode",
                                                          Convert.ToBoolean(debugModeEnabled.floatValue)));
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;


                EditorGUILayout.Space();

                debugModeEnabledAnimBool.target = debugModeEnabled.floatValue == 1;
                if (EditorGUILayout.BeginFadeGroup(debugModeEnabledAnimBool.faded))
                {
                    //EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(debugModeIndicatorLineThickness, "Line Thickness");
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel -= 2;
            }

        }

        // Ensure that shader variant for global replacement exists
        void DoReplacementArea(Material material)
        {
            float oriLabelWidth = EditorGUIUtility.labelWidth;

            //MakeSTSSectionHeader("Debug");

            EditorGUI.indentLevel += 1;
            //showContentDebugArea = MakeSTSSectionHeaderWithFoldout("Replacement Options", showContentDebugArea);
            showContentReplacementOptionsArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Culling Options", Convert.ToBoolean(showContentReplacementOptionsArea.floatValue)));

            EditorGUI.indentLevel -= 1;
            if (showContentReplacementOptionsArea.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;


                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                EditorGUILayout.LabelField("Culling Mode");
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;

                m_MaterialEditor.ShaderProperty(syncCullMode, "Sync Culling Mode");
                if(syncCullMode.floatValue == 1)
                {
                    EditorGUILayout.HelpBox("You can change culling at the top!", MessageType.Info);
                }


                // Not needed?
                //EditorGUI.indentLevel -= 1;
                //EditorStyles.label.normal.textColor = oriCol;
                //EditorGUILayout.LabelField("Replacement Type");
                //EditorStyles.label.normal.textColor = textColor;
                //EditorGUI.indentLevel += 1;

                //m_MaterialEditor.ShaderProperty(isReplacementShader, "Global Replacement");
                //if (isReplacementShader.floatValue == 1)
                //{
                //    EditorGUILayout.HelpBox("Use this material only in conjunction with the \"Global Shader Replacement\" script! For every other use case, please disable this option!", MessageType.Info);
                //}
                EditorGUI.indentLevel -= 2;
                //makeBetweenRange(previewIndicatorLineThickness,0.1f,0.5f);


                EditorGUILayout.Space();

            }
            //if(isReplacementShader.floatValue == 1)
            //{
            //    isReferenceMaterial = true;
            //    isReferenceMaterialMat.floatValue = 1;
            //}

        }

        void DoInteractionOptionsArea(Material material)
        {
            if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null 
                && STSKeywordBools.ContainsKey("_PLAYERINDEPENDENT") )
            {
                interactionMode.floatValue = Convert.ToSingle(STSKeywordBools["_PLAYERINDEPENDENT"]);
            }

            if (interactionMode.floatValue == (float)STSInteractionMode.Independent)
            {
                obstructionMode.floatValue = (float)ObstructionMode.None;
            }

            //MakeSTSSectionHeader("Interaction Options");
            EditorGUI.indentLevel += 1;
            //showContentInteractionOptionsArea = MakeSTSSectionHeaderWithFoldout("Interaction Options", showContentInteractionOptionsArea);
            showContentInteractionOptionsArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Interaction Options", Convert.ToBoolean(showContentInteractionOptionsArea.floatValue)));

            EditorGUI.indentLevel -= 1;
            //showDescription = EditorGUILayout.Foldout(showDescription, title);
            if (showContentInteractionOptionsArea.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;

                float oriLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                //m_MaterialEditor.ShaderProperty(interactionMode, "Interaction Mode"); 
                interactionMode.floatValue = (int)(STSInteractionMode)EditorGUILayout.EnumPopup("Interaction Mode" , (STSInteractionMode)interactionMode.floatValue);
                EditorGUIUtility.labelWidth = oriLabelWidth;
                if (material.HasProperty("_IsMicroSplatSTS"))
                {
                    EditorGUILayout.HelpBox("To change the Interaction Mode you have to do that in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);
                }



                //if (interactionMode.floatValue == (float)STSInteractionMode.Independent)
                //{
                //    m_MaterialEditor.ShaderProperty(centerPosition, "Center Position");
                //}
                if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                {
                    EditorGUILayout.Space();

                    EditorUtils.DrawUILineSubMenu();

                    //EditorGUI.indentLevel -= 1;
                    //EditorStyles.label.normal.textColor = oriCol;
                    //EditorGUILayout.LabelField("Effect Radius Only");
                    //EditorStyles.label.normal.textColor = textColor;
                    //EditorGUI.indentLevel += 1;


                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    enableDefaultEffectRadius.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Default Effect Radius",
                                                              Convert.ToBoolean(enableDefaultEffectRadius.floatValue)));
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;
                    enableDefaultEffectRadiusAnimBool.target = enableDefaultEffectRadius.floatValue == 1;
                    if (EditorGUILayout.BeginFadeGroup(enableDefaultEffectRadiusAnimBool.faded))
                    {
                        m_MaterialEditor.ShaderProperty(defaultEffectRadius, "Default Effect Radius");
                        makeAlwaysPositiv(defaultEffectRadius);
                    }
                    EditorGUILayout.EndFadeGroup();


                }
                else
                {

                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 2;
            }




        }

        void DoObstructionOptionsArea(Material material)
        {

            //MakeSTSSectionHeader("Obstruction Options");
            EditorGUI.indentLevel += 1;
            //showContentObstructionOptionsArea = MakeSTSSectionHeaderWithFoldout("Obstruction Options", showContentObstructionOptionsArea);
            showContentObstructionOptionsArea.floatValue = Convert.ToSingle(MakeSTSSectionHeaderWithFoldout("Obstruction Options", Convert.ToBoolean(showContentObstructionOptionsArea.floatValue)));
            EditorGUI.indentLevel -= 1;
            //showDescription = EditorGUILayout.Foldout(showDescription, title);
            if (showContentObstructionOptionsArea.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel += 2;

                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                EditorGUILayout.LabelField("Obstruction Settings");
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;

                float oriLabelWidth = EditorGUIUtility.labelWidth;
                if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                {

                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                    //m_MaterialEditor.ShaderProperty(obstructionMode, "Obstruction Mode");
                    obstructionMode.floatValue = (int)(ObstructionMode)EditorGUILayout.EnumPopup("Obstruction Mode", (ObstructionMode)obstructionMode.floatValue);
                    EditorGUIUtility.labelWidth = oriLabelWidth;
                }
                else
                {
                    obstructionMode.floatValue = (float)ObstructionMode.None;
                }
                EditorGUI.indentLevel += 1;
                EditorGUI.indentLevel += 1;



                if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null && STSKeywordBools.ContainsKey("_OBSTRUCTION_CURVE") && STSKeywordBools["_OBSTRUCTION_CURVE"])
                {
                    obstructionMode.floatValue = (float)ObstructionMode.Curve;
                }



                if (obstructionMode.floatValue != (float)ObstructionMode.None)
                {
                    Rect rect = EditorGUILayout.BeginVertical();
                    rect.width -= 40;
                    rect.x += 40;
                    //GUI.Box(rect, GUIContent.none);

                    if (EditorGUIUtility.isProSkin)
                    {
                        //GUI.Box(rect, GUIContent.none);
                        DrawBox(rect, new Color(1f, 1.5f, 2f, 1));

                    }
                    else
                    {
                        DrawBox(rect, new Color(0.8f, 0.8f, 0.8f, 1));
                    }
                }


                if (obstructionMode.floatValue == (float)ObstructionMode.AngleOnly ||
                    obstructionMode.floatValue == (float)ObstructionMode.AngleAndCone ||
                    obstructionMode.floatValue == (float)ObstructionMode.AngleAndCylinder)
                {
                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Angle Obstruction");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    m_MaterialEditor.ShaderProperty(angleStrength, "Strength");

                }

                if (obstructionMode.floatValue == (float)ObstructionMode.ConeOnly ||
                    obstructionMode.floatValue == (float)ObstructionMode.AngleAndCone)
                {
                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Cone Obstruction");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    m_MaterialEditor.ShaderProperty(coneStrength, "Strength");
                    m_MaterialEditor.ShaderProperty(coneObstructionDestroyRadius, "Obstruction Destroy Radius");
                    makeAlwaysPositiv(coneObstructionDestroyRadius);

                }



                if (obstructionMode.floatValue == (float)ObstructionMode.CylinderOnly ||
                    obstructionMode.floatValue == (float)ObstructionMode.AngleAndCylinder)
                {
                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Cylinder Obstruction");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    m_MaterialEditor.ShaderProperty(cylinderStrength, "Strength");
                    m_MaterialEditor.ShaderProperty(cylinderObstructionDestroyRadius, "Obstruction Destroy Radius");
                    makeAlwaysPositiv(cylinderObstructionDestroyRadius);

                }


                if (obstructionMode.floatValue == (float)ObstructionMode.Circle)
                {
                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    EditorGUILayout.LabelField("Circle Obstruction");
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;

                    m_MaterialEditor.ShaderProperty(circleStrength, "Strength");
                    m_MaterialEditor.ShaderProperty(circleObstructionDestroyRadius, "Obstruction Destroy Radius");
                    makeAlwaysPositiv(circleObstructionDestroyRadius);

                }


                if (obstructionMode.floatValue == (float)ObstructionMode.Curve)
                {
                    if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null && STSKeywordBools.ContainsKey("_OBSTRUCTION_CURVE") && !STSKeywordBools["_OBSTRUCTION_CURVE"])
                    {
                        EditorGUILayout.HelpBox("You need to activate the Obstruction Curve feature in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Error);
                    }
                    else
                    {
                        if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null && STSKeywordBools.ContainsKey("_OBSTRUCTION_CURVE") && STSKeywordBools["_OBSTRUCTION_CURVE"])
                        {
                            EditorGUILayout.HelpBox("To change the Obstruction Mode you have to disable the Obstruction Curve feature in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);
                        }


                        EditorGUI.indentLevel -= 1;
                        EditorStyles.label.normal.textColor = oriCol;
                        EditorGUILayout.LabelField("Curve Obstruction");
                        EditorStyles.label.normal.textColor = textColor;
                        EditorGUI.indentLevel += 1;
                        EditorGUI.BeginChangeCheck();
                        curveSO.curve = EditorGUILayout.CurveField(curveSO.curve, Color.red, new Rect(0, 0, 1, 1));
                        if (EditorGUI.EndChangeCheck())
                        {
                            UpdateCurveTexture(material);
                            //EditorUtility.SetDirty(curveSO);
                            //AssetDatabase.SaveAssets();
                        }
                        string name = material.name;
                        if (!name.Contains(SeeThroughShaderConstants.STS_INSTANCE_PREFIX) &&
                        !material.name.Contains(SeeThroughShaderConstants.STS_TRIGGER_PREFIX))
                        {
                            if (!curveSO.isBakedToTexture)
                            {
                                EditorGUILayout.HelpBox("THIS IS ONLY A PREVIEW! TO USE THE CURVE IN BUILDS, YOU HAVE TO BAKE THE CURVE!", MessageType.Warning);
                                var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(new GUILayoutOption[] { }));
                                if (GUI.Button(rect, "Bake Curve"))
                                {
                                    SaveTexture(material);
                                    //name = name.Replace(" (Instance)", "");
                                    Texture2D texture = Resources.Load("Curves/" + name, typeof(Texture2D)) as Texture2D;
                                    SetTextureImporterFormat(texture, true);
                                    dissolveObstructionCurve.textureValue = texture;
                                    //material.SetTexture("_ObstructionCurve", texture);
                                    curveSO.isBakedToTexture = true;
                                    EditorUtility.SetDirty(curveSO);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                        }


                        m_MaterialEditor.ShaderProperty(curveStrength, "Strength");
                        m_MaterialEditor.ShaderProperty(curveObstructionDestroyRadius, "Obstruction Destroy Radius");
                        makeAlwaysPositiv(curveObstructionDestroyRadius);
                        //CurveToTexture();
                        //if (curveTexture != null)
                        //{
                        //    EditorGUI.PrefixLabel(new Rect(25, 45, 100, 15), 0, new GUIContent("Preview:"));
                        //    EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), curveTexture);
                        //} else
                        //{
                        //    Debug.Log("CURVE TEX IS NULL");
                        //}
                    }
                }



                //EditorGUILayout.Space();
                if (obstructionMode.floatValue != (float)ObstructionMode.None && obstructionMode.floatValue != (float)ObstructionMode.AngleOnly)
                {
                    m_MaterialEditor.ShaderProperty(dissolveFallOff, "FallOff");
                }

                EditorGUILayout.Space();

                if (obstructionMode.floatValue > 1)
                {

                    dissolveMaskEnabled.floatValue = Convert.ToSingle(EditorGUILayout.Toggle("Use Dissolve Mask", Convert.ToBoolean(dissolveMaskEnabled.floatValue)));

                    if (material.HasProperty("_IsMicroSplatSTS") && STSKeywordBools != null && STSKeywordBools.ContainsKey("_DISSOLVEMASK"))
                    {
                        if (!STSKeywordBools["_DISSOLVEMASK"])
                        {
                            dissolveMaskEnabled.floatValue = 0;
                            EditorGUILayout.HelpBox("To use a Dissolve Mask you have to enable the Dissolve Mask feature in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);
                        }
                        else
                        {
                            dissolveMaskEnabled.floatValue = 1;
                            EditorGUILayout.HelpBox("To disable the Dissolve Mask you have to disable the Dissolve Mask feature in the MicroSplat feature tap under the See-through Shader feature!", MessageType.Info);
                        }
                    }

                    dissolveMaskEnabledAnimBool.target = dissolveMaskEnabled.floatValue == 1;
                    if (EditorGUILayout.BeginFadeGroup(dissolveMaskEnabledAnimBool.faded))
                    {
                        EditorGUILayout.Space();
                        //EditorGUI.indentLevel++;
                        m_MaterialEditor.TexturePropertySingleLine(SeethroughShaderStyles.dissolveMaskText, dissolveMask);
                        //EditorGUI.indentLevel--;
                        if (dissolveMask.textureValue == null)
                        {
                            EditorGUILayout.HelpBox("You didn't select any dissolve mask! The 'See-through Shader' effect will only work if you select one OR disable 'Use Dissolve Mask'!", MessageType.Error);
                        }
                    }
                    EditorGUILayout.EndFadeGroup();


                }

                if (obstructionMode.floatValue != (float)ObstructionMode.None)
                {

                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                    if (zoningEnabled.floatValue == 1 && zoningMode.floatValue == 0)
                    {
                        affectedAreaPlayerBasedObstruction.floatValue = (int)(AffectedArea)EditorGUILayout.EnumPopup("Affected Area", (AffectedArea)affectedAreaPlayerBasedObstruction.floatValue);
                    }
                    else
                    {
                        //GUI.enabled = false;
                        affectedAreaPlayerBasedObstruction.floatValue = 0;
                        //affectedArea.floatValue = (int)(AffectedArea)EditorGUILayout.EnumPopup("Affected Area", (AffectedArea)affectedArea.floatValue);
                        //GUI.enabled = true;
                    }
                    EditorGUIUtility.labelWidth = oriLabelWidth;


                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel -= 1;

                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                EditorGUILayout.LabelField("Intrinsic Dissolve Obstruction");
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;

                m_MaterialEditor.ShaderProperty(intrinsicDissolveStrength, "Strength");

                EditorGUI.indentLevel -= 1;


                if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                {
                    EditorGUILayout.Space();

                    EditorUtils.DrawUILineSubMenu();

                    EditorGUI.indentLevel -= 1;
                    EditorStyles.label.normal.textColor = oriCol;
                    isometricExlusionEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Isometric Exclusion",
                                                              Convert.ToBoolean(isometricExlusionEnabled.floatValue)));
                    EditorStyles.label.normal.textColor = textColor;
                    EditorGUI.indentLevel += 1;


                    EditorGUILayout.Space();

                    isometricExlusionEnabledAnimBool.target = isometricExlusionEnabled.floatValue == 1;
                    if (EditorGUILayout.BeginFadeGroup(isometricExlusionEnabledAnimBool.faded))
                    {
                        //EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(isometricExclusionDistance, "Isometric Plane Distance");
                        m_MaterialEditor.ShaderProperty(isometricExclusionGradientLength, "Dissolve Texture Gradient Length");
                        makeAlwaysPositiv(isometricExclusionGradientLength);
                        //EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndFadeGroup();

                }
                else
                {
                    isometricExlusionEnabled.floatValue = 0;
                }


                EditorGUILayout.Space();
                EditorUtils.DrawUILineSubMenu();
                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                ceilingEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Ceiling",
                                                          Convert.ToBoolean(ceilingEnabled.floatValue)));
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;
                EditorGUILayout.Space();


                ceilingEnabledAnimBool.target = ceilingEnabled.floatValue == 1;
                if (EditorGUILayout.BeginFadeGroup(ceilingEnabledAnimBool.faded))
                {
                    //EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                    if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                    {

                        //m_MaterialEditor.ShaderProperty(ceilingMode, "Ceiling Mode");
                        ceilingMode.floatValue = (int)(CeilingMode)EditorGUILayout.EnumPopup("Ceiling Mode", (CeilingMode)ceilingMode.floatValue);

                    }
                    else
                    {
                        ceilingMode.floatValue = (float)CeilingMode.Manual;
                    }

                    //m_MaterialEditor.ShaderProperty(ceilingBlendMode, "Blend Mode");
                    ceilingBlendMode.floatValue = (int)(CeilingBlendMode)EditorGUILayout.EnumPopup("Blend Mode", (CeilingBlendMode)ceilingBlendMode.floatValue);

                    EditorGUIUtility.labelWidth = oriLabelWidth;
                    if (ceilingMode.floatValue == (float)CeilingMode.Manual)
                    {
                        m_MaterialEditor.ShaderProperty(ceilingY, "CeilingY");
                    }
                    else if (ceilingMode.floatValue == (float)CeilingMode.PlayerPosition)
                    {
                        m_MaterialEditor.ShaderProperty(ceilingPlayerYOffset, "PlayerPos Y Offset");
                    }
                    m_MaterialEditor.ShaderProperty(ceilingYGradientLength, "Dissolve Texture Gradient Length");
                    makeAlwaysPositiv(ceilingYGradientLength);
                    //EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();


                EditorGUILayout.Space();
                EditorUtils.DrawUILineSubMenu();

                //EditorGUI.indentLevel -= 1;
                //EditorStyles.label.normal.textColor = oriCol;
                //EditorGUILayout.LabelField("Floor Exclusion Settings");
                //EditorStyles.label.normal.textColor = textColor;
                //EditorGUI.indentLevel += 1;
                EditorGUI.indentLevel -= 1;
                EditorStyles.label.normal.textColor = oriCol;
                floorEnabled.floatValue = Convert.ToSingle(EditorGUILayout.ToggleLeft("Floor Exclusion",
                                                          Convert.ToBoolean(floorEnabled.floatValue)));
                EditorStyles.label.normal.textColor = textColor;
                EditorGUI.indentLevel += 1;
                floorEnabledAnimBool.target = floorEnabled.floatValue == 1;
                if (EditorGUILayout.BeginFadeGroup(floorEnabledAnimBool.faded))
                {
                    if (interactionMode.floatValue == (float)STSInteractionMode.PlayerBased)
                    {
                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                        //m_MaterialEditor.ShaderProperty(floorMode, "Floor Mode");
                        floorMode.floatValue = (int)(FloorMode)EditorGUILayout.EnumPopup("Floor Mode", (FloorMode)floorMode.floatValue);

                        EditorGUIUtility.labelWidth = oriLabelWidth;
                    }
                    else
                    {
                        floorMode.floatValue = (float)FloorMode.Manual;
                    }


                    if (floorMode.floatValue == (float)FloorMode.Manual)
                    {
                        m_MaterialEditor.ShaderProperty(floorY, "FloorY");
                    }
                    else if (floorMode.floatValue == (float)FloorMode.PlayerPosition)
                    {
                        m_MaterialEditor.ShaderProperty(playerPosYOffset, "PlayerPos Y Offset");
                    }

                    m_MaterialEditor.ShaderProperty(floorYTextureGradientLength, "Floor Gradient Length");
                    makeAlwaysPositiv(floorYTextureGradientLength);


                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2;
                    if (zoningEnabled.floatValue == 1 && zoningMode.floatValue == 0 && interactionMode.floatValue == 0)
                    {
                        affectedAreaFloor.floatValue = (int)(AffectedArea)EditorGUILayout.EnumPopup("Affected Area", (AffectedArea)affectedAreaFloor.floatValue);
                    }
                    else
                    {
                        //GUI.enabled = false;
                        affectedAreaFloor.floatValue = 0;
                        //affectedArea.floatValue = (int)(AffectedArea)EditorGUILayout.EnumPopup("Affected Area", (AffectedArea)affectedArea.floatValue);
                        //GUI.enabled = true;
                    }
                    EditorGUIUtility.labelWidth = oriLabelWidth;

                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 2;

            }

        }



        public void CurveToTexture()
        {
            if (curveTexture != null)
            {
                if (curveTexture.width != curveTextureResolution)
                {
#if !UNITY_2021_1_OR_NEWER
                    curveTexture.Resize(curveTextureResolution, 1);
#else
                    curveTexture.Reinitialize(curveTextureResolution, 1);
#endif
                }

                curveTexture.wrapMode = TextureWrapMode.Clamp;
                curveTexture.filterMode = FilterMode.Bilinear;

                Color[] colors = new Color[curveTextureResolution];
                for (int i = 0; i < curveTextureResolution; ++i)
                {
                    var t = (float)i / curveTextureResolution;

                    colors[i].r = curveSO.curve.Evaluate(t);
                }
                curveTexture.SetPixels(colors);
                curveTexture.Apply(false);
            }
        }

        public void SaveTexture(Material material)
        {
            if (curveTexture != null)
            {
                byte[] bytes = curveTexture.EncodeToPNG();

                string dataPathWithoutAssets = Application.dataPath;
                if (dataPathWithoutAssets.EndsWith("/Assets"))
                {
                    dataPathWithoutAssets = dataPathWithoutAssets.Substring(0, dataPathWithoutAssets.LastIndexOf("/Assets"));
                }
                var dirPath = dataPathWithoutAssets + "/Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Curves/";

  

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                string name = material.name;
                //name = name.Replace(" (Instance)", "");
                File.WriteAllBytes(dirPath + name + ".png", bytes);
                //Debug.Log(name + " saved");
            }
        }


        public void CreateInitialTexture(Material material)
        {
            if (curveTexture != null)
            {
                string dataPathWithoutAssets = Application.dataPath;
                if (dataPathWithoutAssets.EndsWith("/Assets"))
                {
                    dataPathWithoutAssets = dataPathWithoutAssets.Substring(0, dataPathWithoutAssets.LastIndexOf("/Assets"));
                }
                var dirPath = dataPathWithoutAssets + "/Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Curves/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //else
                //{
                string name = material.name;

                if (!File.Exists(dirPath + name + ".png"))
                {
                    //CurveToTexture();
                    Debug.Log(name + " doesn't exist");
                    SaveTexture(material);
                    AssetDatabase.Refresh();
                    Texture2D texture = Resources.Load("Curves/" + name, typeof(Texture2D)) as Texture2D;
                    SetTextureImporterFormat(texture, true);
                    dissolveObstructionCurve.textureValue = texture;
                }
                //}

            }
        }

        public void UpdateCurveTexture(Material material)
        {
            CurveToTexture();
            //SaveTexture(material);
            if (curveTexture != null)
            {
                //Texture2D texture = Resources.Load("Curves/" + "CurveFor" + material.name, typeof(Texture2D)) as Texture2D;
                //SetTextureImporterFormat(texture, true);
                //dissolveObstructionCurve.textureValue = null;
                dissolveObstructionCurve.textureValue = curveTexture;
                //material.SetTexture("_ObstructionCurve", curveTexture);
                curveSO.isBakedToTexture = false;
            }
        }


        public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
        {
            if (null == texture) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.isReadable = isReadable;

                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh();
            }
        }

        void MaterialChanged(Material material)
        {
            SetMaterialKeywords(material);

        }

        void SetMaterialKeywords(Material material)
        {
            SetKeyword(material, "_DISSOLVEMASK", material.GetFloat("_DissolveMaskEnabled") == 1);
            SetKeyword(material, "_ZONING", material.GetFloat("_Zoning") == 1);
            SetKeyword(material, "_OBSTRUCTION_CURVE", material.GetFloat("_Obstruction") == (float)ObstructionMode.Curve);
            SetKeyword(material, "_PLAYERINDEPENDENT", material.GetFloat("_InteractionMode") == (float)STSInteractionMode.Independent);
            SetKeyword(material, "_REPLACEMENT", material.GetFloat("_IsReplacementShader") == 1);
        }

        void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }

            //Work around for ShaderGraph as ShaderGraph also needs to set floats of associated keywords
            if (material.HasProperty(keyword))
            {
                material.SetFloat(keyword, Convert.ToSingle(state));
            }
        }

        bool IsKeywordEnabled(Material m, string keyword)
        {
            return m.IsKeywordEnabled(keyword);
        }

        void makeAlwaysPositiv(MaterialProperty materialProperty)
        {
            materialProperty.floatValue = Mathf.Max(materialProperty.floatValue, 0);
        }


        void makeBetweenRange(MaterialProperty materialProperty, float min, float max)
        {
            materialProperty.floatValue = Mathf.Max(materialProperty.floatValue, min);
            materialProperty.floatValue = Mathf.Min(materialProperty.floatValue, max);
        }

    }

}