#pragma shader_feature_local_fragment _OBSTRUCTION_CURVE
#pragma shader_feature_local_fragment _DISSOLVEMASK
#pragma shader_feature_local_fragment _ZONING
#pragma shader_feature_local_fragment _REPLACEMENT
#pragma shader_feature_local_fragment _PLAYERINDEPENDENT

float _SyncCullMode;

float _IsReplacementShader;
float _TriggerMode;
float _RaycastMode;
float _IsExempt;
float _isReferenceMaterial;
float _InteractionMode;
int _ArrayLength = 0;
#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) 
        float4 _PlayersPosVectorArray[20];
        float _PlayersDataFloatArray[150];     
#else
float4 _PlayersPosVectorArray[100];
float _PlayersDataFloatArray[500];
#endif
float _tDirection = 0;
float _numOfPlayersInside = 0;
float _tValue = 0;
float _id = 0;
#if _ZONING
#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) 
            float _ZDFA[500];
#else
            float _ZDFA[1000];
#endif
        float _ZonesDataCount;
#endif

float _STSCustomTime = 0;

#if _REPLACEMENT        
        float4 _DissolveColorGlobal;
        float _DissolveColorSaturationGlobal;
        float _DissolveEmissionGlobal;
        float _DissolveEmissionBoosterGlobal;
        float _TextureVisibilityGlobal;
        float _ObstructionGlobal;
        float _AngleStrengthGlobal;
        float _ConeStrengthGlobal;
        float _ConeObstructionDestroyRadiusGlobal;
        float _CylinderStrengthGlobal;
        float _CylinderObstructionDestroyRadiusGlobal;
        float _CircleStrengthGlobal;
        float _CircleObstructionDestroyRadiusGlobal;
        float _CurveStrengthGlobal;
        float _CurveObstructionDestroyRadiusGlobal;
        float _DissolveFallOffGlobal;
        float _AffectedAreaPlayerBasedObstructionGlobal;
        float _IntrinsicDissolveStrengthGlobal;
        float _PreviewModeGlobal;
        float _UVsGlobal;
        float _hasClippedShadowsGlobal;
        float _FloorGlobal;
        float _FloorModeGlobal;
        float _FloorYGlobal;
        float _PlayerPosYOffsetGlobal;
        float _FloorYTextureGradientLengthGlobal;
        float _AffectedAreaFloorGlobal;
        float _AnimationEnabledGlobal;
        float _AnimationSpeedGlobal;
        float _DefaultEffectRadiusGlobal;
        float _EnableDefaultEffectRadiusGlobal;
        float _TransitionDurationGlobal;        
        float _TexturedEmissionEdgeGlobal;
        float _TexturedEmissionEdgeStrengthGlobal;
        float _IsometricExclusionGlobal;
        float _IsometricExclusionDistanceGlobal;
        float _IsometricExclusionGradientLengthGlobal;
        float _CeilingGlobal;
        float _CeilingModeGlobal;
        float _CeilingBlendModeGlobal;
        float _CeilingYGlobal;
        float _CeilingPlayerYOffsetGlobal;
        float _CeilingYGradientLengthGlobal;
        float _ZoningGlobal;
        float _ZoningModeGlobal;
        float _ZoningEdgeGradientLengthGlobal;
        float _IsZoningRevealableGlobal;
        float _SyncZonesWithFloorYGlobal;
        float _SyncZonesFloorYOffsetGlobal;
        float4 _ObstructionCurveGlobal_TexelSize;
        float4 _DissolveMaskGlobal_TexelSize;
        float _DissolveMaskEnabledGlobal;
        float _PreviewIndicatorLineThicknessGlobal;
        float _UseCustomTimeGlobal;

        half _DissolveMethodGlobal;
        half _DissolveTexSpaceGlobal;

#else
        half _TextureVisibility;
        half _AngleStrength;
        float _Obstruction;
        float _UVs;
        float4 _ObstructionCurve_TexelSize;
        float _DissolveMaskEnabled;
        float4 _DissolveMask_TexelSize;
        float4 _DissolveColor;
        float _DissolveColorSaturation;
        float _DissolveEmission;
        float _DissolveEmissionBooster;
        float _hasClippedShadows;
        float _ConeStrength;
        float _ConeObstructionDestroyRadius;
        float _CylinderStrength;
        float _CylinderObstructionDestroyRadius;
        float _CircleStrength;
        float _CircleObstructionDestroyRadius;
        float _CurveStrength;
        float _CurveObstructionDestroyRadius;
        float _IntrinsicDissolveStrength;
        float _DissolveFallOff;
        float _AffectedAreaPlayerBasedObstruction;
        float _PreviewMode;
        float _PreviewIndicatorLineThickness;
        float _AnimationEnabled;
        float _AnimationSpeed;
        float _DefaultEffectRadius;
        float _EnableDefaultEffectRadius;
        float _TransitionDuration;
        float _TexturedEmissionEdge;
        float _TexturedEmissionEdgeStrength;
        float _IsometricExclusion;
        float _IsometricExclusionDistance;
        float _IsometricExclusionGradientLength;
        float _Floor;
        float _FloorMode;
        float _FloorY;
        float _FloorYTextureGradientLength;
        float _PlayerPosYOffset;
        float _AffectedAreaFloor;
        float _Ceiling;
        float _CeilingMode;
        float _CeilingBlendMode;
        float _CeilingY;
        float _CeilingPlayerYOffset;
        float _CeilingYGradientLength;
        float _Zoning;
        float _ZoningMode;
        float _ZoningEdgeGradientLength;
        float _IsZoningRevealable;
        float _SyncZonesWithFloorY;
        float _SyncZonesFloorYOffset;
        float _UseCustomTime;

        half _DissolveMethod;
        half _DissolveTexSpace;
#endif



         

         


#if _REPLACEMENT
        sampler2D _DissolveTexGlobal;
#else
sampler2D _DissolveTex;
#endif

    //#if _DISSOLVEMASK
#if _REPLACEMENT
        sampler2D _DissolveMaskGlobal;
#else
sampler2D _DissolveMask;
#endif
    //#endif

#if _REPLACEMENT
        sampler2D _ObstructionCurveGlobal;
#else
sampler2D _ObstructionCurve;
#endif





#ifdef USE_UNITY_TEXTURE_2D_TYPE
#undef USE_UNITY_TEXTURE_2D_TYPE
#endif


struct ShaderData
{
    float3 worldSpaceNormal;
    float3 worldSpacePosition;
};

struct Surface
{
    float3 Normal;
    half3 Emission;
    half3 Albedo;
};

#include "Packages/com.shadercrew.seethroughshader.core/Scripts/Shaders/xxSharedSTSDependencies/SeeThroughShaderFunction.hlsl"


void AddSeeThroughShaderToShader(inout float3 albedo, inout float3 emission, inout float alphaForClipping, float3 worldPos, float3 worldNormal, float4 screenPos)
{
    
    float3 normal = float3(0, 0, 1);

#ifdef USE_UNITY_TEXTURE_2D_TYPE
#undef USE_UNITY_TEXTURE_2D_TYPE
#endif
#if _REPLACEMENT
    DoSeeThroughShading( albedo, normal, worldPos, worldNormal, screenPos,

                        _numOfPlayersInside, _tDirection, _tValue, _id,
                        _TriggerMode, _RaycastMode,
                        _IsExempt,
    
                        _DissolveMethodGlobal, _DissolveTexSpaceGlobal,

                        _DissolveColorGlobal, _DissolveColorSaturationGlobal, _UVsGlobal,
                        _DissolveEmissionGlobal, _DissolveEmissionBoosterGlobal, _TexturedEmissionEdgeGlobal, _TexturedEmissionEdgeStrengthGlobal,
                        _hasClippedShadowsGlobal,

                        _ObstructionGlobal,
                        _AngleStrengthGlobal,
                        _ConeStrengthGlobal, _ConeObstructionDestroyRadiusGlobal,
                        _CylinderStrengthGlobal, _CylinderObstructionDestroyRadiusGlobal,
                        _CircleStrengthGlobal, _CircleObstructionDestroyRadiusGlobal,
                        _CurveStrengthGlobal, _CurveObstructionDestroyRadiusGlobal,
                        _DissolveFallOffGlobal,
                        _DissolveMaskEnabledGlobal,
                        _AffectedAreaPlayerBasedObstructionGlobal,
                        _IntrinsicDissolveStrengthGlobal,
                        _DefaultEffectRadiusGlobal, _EnableDefaultEffectRadiusGlobal,

                        _IsometricExclusionGlobal, _IsometricExclusionDistanceGlobal, _IsometricExclusionGradientLengthGlobal,
                        _CeilingGlobal, _CeilingModeGlobal, _CeilingBlendModeGlobal, _CeilingYGlobal, _CeilingPlayerYOffsetGlobal, _CeilingYGradientLengthGlobal,
                        _FloorGlobal, _FloorModeGlobal, _FloorYGlobal, _PlayerPosYOffsetGlobal, _FloorYTextureGradientLengthGlobal, _AffectedAreaFloorGlobal,

                        _AnimationEnabledGlobal, _AnimationSpeedGlobal,
                        _TransitionDurationGlobal,
    
                        _UseCustomTimeGlobal,

                        _ZoningGlobal, _ZoningModeGlobal, _IsZoningRevealableGlobal, _ZoningEdgeGradientLengthGlobal,
                        _SyncZonesWithFloorYGlobal, _SyncZonesFloorYOffsetGlobal,

                        _PreviewModeGlobal,
                        _PreviewIndicatorLineThicknessGlobal,

                        _DissolveTexGlobal,
                        _DissolveMaskGlobal,
                        _ObstructionCurveGlobal,

                        _DissolveMaskGlobal_TexelSize,
                        _ObstructionCurveGlobal_TexelSize,

                        albedo, emission, alphaForClipping);
#else 
    
    DoSeeThroughShading(albedo, normal, worldPos, worldNormal, screenPos,

                        _numOfPlayersInside, _tDirection, _tValue, _id,
                        _TriggerMode, _RaycastMode,
                        _IsExempt,
    
                        _DissolveMethod, _DissolveTexSpace,

                        _DissolveColor, _DissolveColorSaturation, _UVs,
                        _DissolveEmission, _DissolveEmissionBooster, _TexturedEmissionEdge, _TexturedEmissionEdgeStrength,
                        _hasClippedShadows,

                        _Obstruction,
                        _AngleStrength,
                        _ConeStrength, _ConeObstructionDestroyRadius,
                        _CylinderStrength, _CylinderObstructionDestroyRadius,
                        _CircleStrength, _CircleObstructionDestroyRadius,
                        _CurveStrength, _CurveObstructionDestroyRadius,
                        _DissolveFallOff,
                        _DissolveMaskEnabled,
                        _AffectedAreaPlayerBasedObstruction,
                        _IntrinsicDissolveStrength,
                        _DefaultEffectRadius, _EnableDefaultEffectRadius,

                        _IsometricExclusion, _IsometricExclusionDistance, _IsometricExclusionGradientLength,
                        _Ceiling, _CeilingMode, _CeilingBlendMode, _CeilingY, _CeilingPlayerYOffset, _CeilingYGradientLength,
                        _Floor, _FloorMode, _FloorY, _PlayerPosYOffset, _FloorYTextureGradientLength, _AffectedAreaFloor,

                        _AnimationEnabled, _AnimationSpeed,
                        _TransitionDuration,
    
                        _UseCustomTime,

                        _Zoning, _ZoningMode, _IsZoningRevealable, _ZoningEdgeGradientLength,
                        _SyncZonesWithFloorY, _SyncZonesFloorYOffset,

                        _PreviewMode,
                        _PreviewIndicatorLineThickness,

                        _DissolveTex,
                        _DissolveMask,
                        _ObstructionCurve,

                        _DissolveMask_TexelSize,
                        _ObstructionCurve_TexelSize,

                        albedo, emission, alphaForClipping);
#endif 
}

