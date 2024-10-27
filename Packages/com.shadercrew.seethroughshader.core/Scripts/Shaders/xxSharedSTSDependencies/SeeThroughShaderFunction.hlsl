#ifndef SEETHROUGHSHADER_FUNCTION
#define SEETHROUGHSHADER_FUNCTION


#ifndef UNITY_MATRIX_I_M
        #define UNITY_MATRIX_I_M   unity_WorldToObject
#endif






void DoSeeThroughShading(
                                    float3 l0,
                                    float3 ll0, float3 lll0,
                                    float3 llll0, float4 lllll0,
                                    float llllll0, float lllllll0, float llllllll0, float lllllllll0,
                                    float llllllllll0, float lllllllllll0,
                                    float llllllllllll0,
                                    half lllllllllllll0, half llllllllllllll0,
                                    float4 lllllllllllllll0, float llllllllllllllll0, float lllllllllllllllll0,
                                    float llllllllllllllllll0, float lllllllllllllllllll0, float llllllllllllllllllll0, float lllllllllllllllllllll0,
                                    bool llllllllllllllllllllll0,
                                    float lllllllllllllllllllllll0,
                                    float llllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllll0, float llllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllll0, float llllllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllllll0, float llllllllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllllllll0, float l1,
                                    float ll1,
                                    float lll1,
                                    float llll1,
                                    float lllll1,
                                    float llllll1, float lllllll1,
                                    float llllllll1, float lllllllll1, float llllllllll1,
                                    float lllllllllll1, float llllllllllll1, float lllllllllllll1, float llllllllllllll1, float lllllllllllllll1, float llllllllllllllll1,
                                    float lllllllllllllllll1, float llllllllllllllllll1, float lllllllllllllllllll1, float llllllllllllllllllll1, float lllllllllllllllllllll1, float llllllllllllllllllllll1,
                                    float lllllllllllllllllllllll1, float llllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllll1,
                                    bool llllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllll1, float llllllllllllllllllllllllllll1, float lllllllllllllllllllllllllllll1, float llllllllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllllllll1, float l2,
                                    float ll2,
                                    float lll2,
#ifdef USE_UNITY_TEXTURE_2D_TYPE
                                    UnityTexture2D llll2,
                                    UnityTexture2D lllll2,
                                    UnityTexture2D llllll2,
#else
                                    sampler2D llll2,
                                    sampler2D lllll2,
                                    sampler2D llllll2,
                                    float4 lllllll2,
                                    float4 llllllll2,
#endif
                                    out half3 lllllllll2,
                                    out half3 llllllllll2,
                                    out float lllllllllll2
)
{
    ShaderData d;
    d.worldSpaceNormal = llll0;
    d.worldSpacePosition = lll0;
    float3 llllllllllll2 = float3(0, 0, 0);
#ifdef _HDRP
        llllllllllll2 = mul(UNITY_MATRIX_I_M, float4(GetCameraRelativePositionWS(d.worldSpacePosition), 1)).xyz;
#else
    llllllllllll2 = mul(UNITY_MATRIX_I_M, float4(d.worldSpacePosition, 1)).xyz;
#endif
    Surface o;
    o.Normal = ll0;
    o.Albedo = half3(0, 0, 0) + l0;
    o.Emission = half3(0, 0, 0);
    lllllllll2 = half3(0, 0, 0);
    llllllllll2 = half3(0, 0, 0);
    lllllllllll2 = 1;
    float lllllllllllll2 = _Time.y;
    if (llllllllllllllllllllllllll1)
    {
        lllllllllllll2 = _STSCustomTime;
    }
    bool llllllllllllll2 = (llllll0 > 0 || lllllll0 == -1 && lllllllllllll2 - llllllll0 < lllllllllllllllllllllllll1) || (llllll0 >= 0 && lllllll0 == 1);
    bool lllllllllllllll2 = !llllllllll0 && !lllllllllll0;
    float llllllllllllllll2 = 0;
    half4 lllllllllllllllll2 = half4(0, 0, 0, 0);
    if (!llllllllllll0 && (llllllllllllll2 || lllllllllllllll2))
    {
        float4 llllllllllllllllll2 = float4(0, 0, 0, 0);
        float4 lllllllllllllllllll2 = float4(0, 0, 0, 0);
#ifdef USE_UNITY_TEXTURE_2D_TYPE
        lllllllllllllllllll2 = llllll2.texelSize;
        llllllllllllllllll2 = lllll2.texelSize;
#else
        lllllllllllllllllll2 = llllllll2;
        llllllllllllllllll2 = lllllll2;
#endif
        if (lllll1 < 0)
        {
            lllll1 = 0;
        }
        half llllllllllllllllllll2 = 0;
        if (lllllllllllll0 == 0) 
        {
            float3 lllllllllllllllllllll2 = float3(0, 0, 0);
            if (llllllllllllll0 == 0) 
            {
                lllllllllllllllllllll2 = llllllllllll2 / (-1.0 * abs(lllllllllllllllll0));
            }
            else 
            {
                lllllllllllllllllllll2 = d.worldSpacePosition / (-1.0 * abs(lllllllllllllllll0));
            }
            if (lllllllllllllllllllllll1)
            {
                lllllllllllllllllllll2 = lllllllllllllllllllll2 + abs(((lllllllllllll2) * llllllllllllllllllllllll1));
            }
            float3 llllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllll2.yz).rgb;
            float3 lllllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllll2.xz).rgb;
            float3 llllllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllll2.xy).rgb;
            float lllllllllllllllllllllllll2 = abs(d.worldSpaceNormal.x);
            float llllllllllllllllllllllllll2 = abs(d.worldSpaceNormal.z);
            float3 lllllllllllllllllllllllllll2 = lerp(lllllllllllllllllllllll2, llllllllllllllllllllll2, lllllllllllllllllllllllll2).rgb;
            float3 llllllllllllllllllllllllllll2 = lerp(lllllllllllllllllllllllllll2, llllllllllllllllllllllll2, llllllllllllllllllllllllll2).rgb;
            llllllllllllllllllll2 = llllllllllllllllllllllllllll2.r;
        }
        else if (lllllllllllll0 == 1) 
        {
            float4 lllllllllllllllllllllllllllll2 = float4(lllll0.xy / max(0.01, lllll0.w), 0, 0);
            float2 llllllllllllllllllllllllllllll2 = lllllllllllllllllllllllllllll2 * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
            };
            uint llllllllllll7 = (uint(llllllllllllllllllllllllllllll2.x) % 4) * 4 + uint(llllllllllllllllllllllllllllll2.y) % 4;
            llllllllllllllllllll2 = DITHER_THRESHOLDS[llllllllllll7];
        }
        else 
        {
            llllllllllllllllllll2 = 0.5; 
        }
        float3 l3 = UNITY_MATRIX_V[2].xyz;
#ifdef _HDRP
                l3 =  mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2]).xyz;
#else
        l3 = mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V))[2]).xyz;
#endif
        float ll3 = 0;
        float lll3 = 0;
        float llll3 = 1;
        bool lllll3 = false;
        float llllll3 = 0;
        float lllllll3 = 0;
        float llllllll3 = 0;
        float lllllllll3 = 0;
        float llllllllll3 = 0;
        float lllllllllll3 = 0;
#if defined(_ZONING)  
                if(lllllllllllllllllllllllllll1) {
                    float llllllllllll3 = 0;
                    for (int z = 0; z < _ZonesDataCount; z++){
                        bool lllllllllllll3 = false;
                        float llllllllllllll3 = llllllllllll3;
                        if (_ZDFA[llllllllllll3 + 1] == 0) { 
#if !_EXCLUDE_ZONEBOXES
                            float lllllllllllllll3 = llllllllllll3 + 2; 
                            float3 llllllllllllllll3 = d.worldSpacePosition - float3(_ZDFA[lllllllllllllll3],_ZDFA[lllllllllllllll3+1], _ZDFA[lllllllllllllll3+2]);
                            float3 lllllllllllllllll3 =     float3(_ZDFA[lllllllllllllll3+ 3],_ZDFA[lllllllllllllll3+ 4], _ZDFA[lllllllllllllll3+ 5]);
                            float3 llllllllllllllllll3 =     float3(_ZDFA[lllllllllllllll3+ 6],_ZDFA[lllllllllllllll3+ 7], _ZDFA[lllllllllllllll3+ 8]);
                            float3 lllllllllllllllllll3 =     float3(_ZDFA[lllllllllllllll3+ 9],_ZDFA[lllllllllllllll3+10], _ZDFA[lllllllllllllll3+11]);
                            float3 llllllllllllllllllll3 = float3(_ZDFA[lllllllllllllll3+12],_ZDFA[lllllllllllllll3+13], _ZDFA[lllllllllllllll3+14]);
                            float lllllllllllllllllllll3 = abs(dot(llllllllllllllll3, lllllllllllllllll3));
                            float llllllllllllllllllllll3 = abs(dot(llllllllllllllll3, llllllllllllllllll3));
                            float lllllllllllllllllllllll3 = abs(dot(llllllllllllllll3, lllllllllllllllllll3));
                            lllllllllllll3 =    lllllllllllllllllllll3 <= llllllllllllllllllll3.x &&
                                        llllllllllllllllllllll3 <= llllllllllllllllllll3.y &&
                                        lllllllllllllllllllllll3 <= llllllllllllllllllll3.z;
                            if(lllllllllllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1) {
                                llllllll3 = _ZDFA[lllllllllllllll3+1] - _ZDFA[lllllllllllllll3+13];  
                                if(llllllllllllllllll1 == 0) {                                    
                                    bool llllllllllllllllllllllll3 = ((llllllll3 - l2)  <= lllllllllllllllllll1); 
                                    if(!llllllllllllllllllllllll3) {
                                        lllllllllllll3 = false;
                                    }
                                }
                            }
                            if(lllllllllllll3) {
                                float lllllllllllllllllllllllll3 = llllllllllllllllllll3.x - lllllllllllllllllllll3;
                                float llllllllllllllllllllllllll3 = llllllllllllllllllll3.y - llllllllllllllllllllll3;
                                float lllllllllllllllllllllllllll3 = llllllllllllllllllll3.z - lllllllllllllllllllllll3;
                                float llllllllllllllllllllllllllll3 = min(llllllllllllllllllllllllll3,lllllllllllllllllllllllll3);
                                llllllllllllllllllllllllllll3 = min(llllllllllllllllllllllllllll3,lllllllllllllllllllllllllll3);
                                lllllll3 = max(llllllllllllllllllllllllllll3,lllllll3);
                                if(llllllllllllllllllllllllllll3<0) {
                                    lllllll3 = 0;
                                }
                            }
                            if (lllllllllllll3)
                            {
                                if (lllll3 == false)
                                {
                                    llllll3 = _ZDFA[llllllllllllll3];
                                    lllll3 = true;
                                    lllllllll3 = _ZDFA[llllllllllllll3 + 17];
                                    lllllllllll3 = _ZDFA[llllllllllllll3 + 18];
                                    llllllllll3 = _ZDFA[llllllllllllll3 + 19];
                                }                     
                            }
#endif        
                            llllllllllll3 = llllllllllll3 + 17 + 3;
                        } else if (_ZDFA[llllllllllll3 + 1] == 1) { 
#if !_EXCLUDE_ZONESPHERES
                            float lllllllllllllllllllllllllllll3 = llllllllllll3 + 2; 
                            float3 llllllllllllllllllllllllllllll3 = float3(_ZDFA[lllllllllllllllllllllllllllll3], _ZDFA[lllllllllllllllllllllllllllll3 + 1], _ZDFA[lllllllllllllllllllllllllllll3 + 2]);
                            float lllllllllllllllllllllllllllllll3 = _ZDFA[lllllllllllllllllllllllllllll3 + 3];
                            float l4 = distance(d.worldSpacePosition, llllllllllllllllllllllllllllll3);
                            lllllllllllll3 = l4 < lllllllllllllllllllllllllllllll3;
                            if (lllllllllllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1)
                            {
                                llllllll3 = _ZDFA[lllllllllllllllllllllllllllll3 + 1] - _ZDFA[lllllllllllllllllllllllllllll3 + 3];
                                if (llllllllllllllllll1 == 0)
                                {
                                    bool llllllllllllllllllllllll3 = ((llllllll3 - l2) <= lllllllllllllllllll1);
                                    if (!llllllllllllllllllllllll3)
                                    {
                                        lllllllllllll3 = false;
                                    }
                                }
                            }
                            if (lllllllllllll3)
                            {
                                if (lllll3 == false)
                                {
                                    llllll3 = _ZDFA[llllllllllllll3];
                                    lllll3 = true;
                                    lllllllll3 = _ZDFA[llllllllllllll3 + 6];
                                    lllllllllll3 = _ZDFA[llllllllllllll3 + 7];
                                    llllllllll3 = _ZDFA[llllllllllllll3 + 8];
                                }
                            }
                            if (lllllllllllll3)
                             {
                                float llllllllllllllllllllllllllll3 = max(0, (lllllllllllllllllllllllllllllll3 - l4));
                                lllllll3 = max(llllllllllllllllllllllllllll3, lllllll3);
                            }
#endif
                            llllllllllll3 = llllllllllll3 + 6 + 3;
                        } else if (_ZDFA[llllllllllll3 + 1] == 2) { 
#if !_EXCLUDE_ZONECYLINDERS
                            float llll4 = llllllllllll3 + 2;
                            float3 lllll4 = float3(_ZDFA[llll4], _ZDFA[llll4 + 1], _ZDFA[llll4 + 2]);
                            float3 llllll4 = float3(_ZDFA[llll4 + 3], _ZDFA[llll4 + 4], _ZDFA[llll4 + 5]);
                            float lllllll4 = dot(d.worldSpacePosition.xyz - lllll4, llllll4);
                            float llllllll4 = _ZDFA[llll4 + 6];
                            float lllllllll4 = _ZDFA[llll4 + 7];
                            float llllllllll4 = length((d.worldSpacePosition.xyz - lllll4) - lllllll4 * llllll4);
                            lllllllllllll3 = (abs(lllllll4) < lllllllll4/2) && (llllllllll4 < llllllll4);
                            if (lllllllllllll3)
                            {
                                if (lllll3 == false)
                                {
                                    llllll3 = _ZDFA[llllllllllllll3];
                                    lllll3 = true;
                                    lllllllll3 = _ZDFA[llllllllllllll3 + 10];
                                    lllllllllll3 = _ZDFA[llllllllllllll3 + 11];
                                    llllllllll3 = _ZDFA[llllllllllllll3 + 12];
                                }
                            }
                            if (lllllllllllll3)
                            {
                                float llllllllllllllllllllllllllll3 = max(0, (llllllll4 - llllllllll4));
                                llllllllllllllllllllllllllll3 = min(llllllllllllllllllllllllllll3, (lllllllll4/2 - abs(lllllll4)));
                                lllllll3 = max(llllllllllllllllllllllllllll3, lllllll3);
                            }
#endif
                            llllllllllll3 = llllllllllll3 + 10 + 3;
                        }
                        else if (_ZDFA[llllllllllll3 + 1] == 3) { 
#if !_EXCLUDE_ZONECONES
                            float llll4 = llllllllllll3 + 2;
                            float3 lllll4 = float3(_ZDFA[llll4], _ZDFA[llll4 + 1], _ZDFA[llll4 + 2]);
                            float3 llllll4 = float3(_ZDFA[llll4 + 3], _ZDFA[llll4 + 4], _ZDFA[llll4 + 5]);
                            float lllllll4 = dot(d.worldSpacePosition.xyz - lllll4, llllll4);
                            float llllllllllllllll4 = _ZDFA[llll4 + 6];
                            float lllllllllllllllll4 = _ZDFA[llll4 + 7];
                            float3 llllllllllllllllll4 = lllll4 + (llllll4 * lllllllllllllllll4/2); 
                            float lllllllllllllllllll4 = dot(llllllllllllllllll4 - d.worldSpacePosition.xyz, llllll4);
                            float llllllllllllllllllll4 = (lllllllllllllllllll4 / lllllllllllllllll4) * llllllllllllllll4;
                            float llllllllll4 = length((llllllllllllllllll4 - d.worldSpacePosition.xyz) - lllllllllllllllllll4 * llllll4);        
                            lllllllllllll3 = (abs(lllllll4) < lllllllllllllllll4/2) && (llllllllll4 < llllllllllllllllllll4);
                            if (lllllllllllll3)
                            {
                                if (lllll3 == false)
                                {
                                    llllll3 = _ZDFA[llllllllllllll3];
                                    lllll3 = true;
                                    lllllllll3 = _ZDFA[llllllllllllll3 + 10];
                                    lllllllllll3 = _ZDFA[llllllllllllll3 + 11];
                                    llllllllll3 = _ZDFA[llllllllllllll3 + 12];
                                }
                            }
                            if (lllllllllllll3)
                            {
                                float llllllllllllllllllllllllllll3 = max(0, (llllllllllllllllllll4 - llllllllll4));
                                llllllllllllllllllllllllllll3 = min(llllllllllllllllllllllllllll3, (lllllllllllllllll4 - lllllllllllllllllll4));
                                lllllll3 = max(llllllllllllllllllllllllllll3, lllllll3);
                            }
#endif
                            llllllllllll3 = llllllllllll3 + 10 + 3;
                        }
                        else if (_ZDFA[llllllllllll3 + 1] == 4) { 
#if !_EXCLUDE_ZONEPLANES
                            float lllllllllllllllllllllll4 = llllllllllll3 + 2;
                            float3 llllllllllllllllllllllll4 = float3(_ZDFA[lllllllllllllllllllllll4], _ZDFA[lllllllllllllllllllllll4 + 1], _ZDFA[lllllllllllllllllllllll4 + 2]);
                            float lllllllllllllllllllllllll4 = _ZDFA[lllllllllllllllllllllll4 + 3];       
                            float llllllllllllllllllllllllll4 = dot(d.worldSpacePosition.xyz, llllllllllllllllllllllll4.xyz) + lllllllllllllllllllllllll4;
                            lllllllllllll3 = llllllllllllllllllllllllll4 < 0;
                            if (lllllllllllll3)
                            {
                                if (lllll3 == false)
                                {
                                    llllll3 = _ZDFA[llllllllllllll3];
                                    lllll3 = true;
                                    lllllllll3 = _ZDFA[llllllllllllll3 + 6];
                                    lllllllllll3 = _ZDFA[llllllllllllll3 + 7];
                                    llllllllll3 = _ZDFA[llllllllllllll3 + 8];
                                }
                            }
                            if (lllllllllllll3)
                            {
                                float llllllllllllllllllllllllllll3 = max(0, 0 - llllllllllllllllllllllllll4);
                                lllllll3 = max(llllllllllllllllllllllllllll3, lllllll3);
                            }
#endif
                            llllllllllll3 = llllllllllll3 + 6 + 3;
                        }
                }
            }
#endif
        float llllllllllllllllllllllllllll4 = 0;
        float lllllllllllllllllllllllllllll4 = lllll3;
#if !defined(_PLAYERINDEPENDENT)
#if defined(_ZONING)
                    if(lllll3 && lllllllllllllllll1 == 1 && llllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1) {
                        float llllllllllllllllllllllllllllll4 = 0;
                        bool lllllllllllllllllllllllllllllll4 = false;
                        for (int i = 0; i < _ArrayLength; i++){
                            float l5 = _PlayersDataFloatArray[llllllllllllllllllllllllllllll4+1]; 
                            float3 ll5 = _PlayersPosVectorArray[l5].xyz - _WorldSpaceCameraPos;               
                            if(dot(l3,ll5) <= 0) {       
                                if(!lllllllllllllll2) {
                                    float lll5 = llllllllllllllllllllllllllllll4 + 3;
                                    float llll5 = 4;
                                    for (int llllll8 = 0; llllll8 < _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 2]; llllll8++){
                                        float lllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 2];
                                        if (lllll5 != 0 && lllll5 == lllllllll0) {
                                            float llllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 ];
                                            float lllllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 1];
                                            if ((lllllll5 == -1 && lllllllllllll2 - llllll5 < lllllllllllllllllllllllll1 )|| (lllllll5 == 1) ) {
                                                float llllllll5 = _PlayersPosVectorArray[l5].y+ llllllllllllllllllll1;
                                                if(ll2) {
                                                    if(i==0) {
                                                        llllllllllllllllllllllllllll4 = llllllll5;
                                                    } else {
                                                        llllllllllllllllllllllllllll4 = max(llllllllllllllllllllllllllll4,llllllll5);
                                                    }
                                                }
                                                bool lllllllll5 = llllllll3 >= llllllll5 + l2; 
                                                if(!lllllllll5) {
                                                    lllllllllllllllllllllllllllllll4 = true;
                                                } 
                                            }                        
                                        }
                                    }
                                } else if (lllllll1 == 0 || distance(_PlayersPosVectorArray[l5].xyz, d.worldSpacePosition.xyz) < llllll1) {
                                    float llllllll5 = _PlayersPosVectorArray[l5].y+ llllllllllllllllllll1;
                                    if(ll2) {
                                        if(i==0) {
                                            llllllllllllllllllllllllllll4 = llllllll5;
                                        } else {
                                            llllllllllllllllllllllllllll4 = max(llllllllllllllllllllllllllll4,llllllll5);
                                        }
                                    }
                                    bool lllllllll5 = llllllll3 >= llllllll5 + l2; 
                                    if(!lllllllll5) {
                                        lllllllllllllllllllllllllllllll4 = true;
                                    } 
                                }
                                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 2]*4 + 3; 
                                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4]*4 + 1; 
                            }
                        }
                        if(!lllllllllllllllllllllllllllllll4) {
                            lllll3 = false;
                        }
                    }
#endif
        float llllllllllllllllllllllllllllll4 = 0;
        for (int i = 0; i < _ArrayLength; i++)
        {
            float l5 = _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 1];
            if (sign(_PlayersPosVectorArray[l5].w) != -1) 
            {
                float3 ll5 = _PlayersPosVectorArray[l5].xyz - _WorldSpaceCameraPos;
                float lllllllllllllll5 = 0;
                float llll5 = 4;
                if (!lllllllllllllll2)
                {
                    float lll5 = llllllllllllllllllllllllllllll4 + 3;
                    for (int llllll8 = 0; llllll8 < _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 2]; llllll8++)
                    {
                        float lllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 2];
                        if (lllll5 != 0 && lllll5 == lllllllll0)
                        {
                            float llllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5];
                            float lllllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 1];
                            lllllllllllllll5 = 1;
                            if (lllllll5 != 0 && llllll5 != 0 && lllllllllllll2 - llllll5 < lllllllllllllllllllllllll1)
                            {
                                if (lllllll5 == 1)
                                {
                                    lllllllllllllll5 = ((lllllllllllllllllllllllll1 - (lllllllllllll2 - llllll5)) / lllllllllllllllllllllllll1);
                                }
                                else
                                {
                                    lllllllllllllll5 = ((lllllllllllll2 - llllll5) / lllllllllllllllllllllllll1);
                                }
                            }
                            else if (lllllll5 == -1)
                            {
                                lllllllllllllll5 = 1;
                            }
                            else if (lllllll5 == 1)
                            {
                                lllllllllllllll5 = 0;
                            }
                            else
                            {
                                lllllllllllllll5 = 1;
                            }
                            lllllllllllllll5 = 1 - lllllllllllllll5;
                        }
                    }
                }
                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 2] * 4 + 3;
                float lllllllllllllllllllll5 = 0;
                float llllllllllllllllllllll5 = 0;
                float lllllllllllllllllllllll5 = 0;
                float llllllllllllllllllllllll5 = llllllllllllllllllllll5;
                bool lllllllllllllllllllllllll5 = distance(_PlayersPosVectorArray[l5].xyz, d.worldSpacePosition) > llllll1;
                if ((lllllllllllllll5 != 0) || ((!llllllllll0 && !lllllllllll0) && (lllllll1 == 0 || !lllllllllllllllllllllllll5)))
                {
#if defined(_ZONING)
                            if(lllllllllllllllllllllllllll1) {
                                if(lllll3) 
                                {
                                    if(lllllllllllllllllllllllllllll1) {
                                        float lll5 = llllllllllllllllllllllllllllll4 + 1;
                                        for (int llllll8 = 0; llllll8 < _PlayersDataFloatArray[llllllllllllllllllllllllllllll4]; llllll8++){
                                            float lllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 2];
                                            if (lllll5 != 0 && lllll5 == llllll3) {
                                                float llllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 ];
                                                float lllllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 1];
                                                lllllllllllllllllllll5 = 1;
                                                float llllllllllllllllllllllllllllll5 = _PlayersDataFloatArray[lll5 + llllll8 * llll5 + 3];
                                                if( lllllll5!= 0 && llllll5 != 0 && lllllllllllll2-llllll5 < llllllllllllllllllllllllllllll5) {
                                                    if(lllllll5 == 1) {
                                                        lllllllllllllllllllll5 = ((llllllllllllllllllllllllllllll5-(lllllllllllll2-llllll5))/llllllllllllllllllllllllllllll5);
                                                    } else {
                                                        lllllllllllllllllllll5 = ((lllllllllllll2-llllll5)/llllllllllllllllllllllllllllll5);
                                                    }
                                                } else if(lllllll5 ==-1) {
                                                    lllllllllllllllllllll5 = 1;
                                                } else if(lllllll5 == 1) {
                                                    lllllllllllllllllllll5 = 0;
                                                } else {
                                                    lllllllllllllllllllll5 = 1;
                                                }
                                                lllllllllllllllllllll5 = 1 - lllllllllllllllllllll5;
                                            }
                                            if(llllllllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllllllll1) {
                                                float lllllllllllllllllllllllllllllll5 = 1 / llllllllllllllllllllllllllllll1;
                                                if (lllllll3 < llllllllllllllllllllllllllllll1)  {
                                                    float l6 = ((llllllllllllllllllllllllllllll1-lllllll3) * lllllllllllllllllllllllllllllll5);
                                                    lllllllllllllllllllll5 =  max(lllllllllllllllllllll5,l6);
                                                }
                                            }
                                        }
                                    } else { 
                                    }
                                } else {
                                }
                            }
#endif
                    if (dot(l3, ll5) <= 0)
                    {
                        if (lllllllllllllllllllllll0 == 2 || lllllllllllllllllllllll0 == 3 || lllllllllllllllllllllll0 == 4 || lllllllllllllllllllllll0 == 5 || lllllllllllllllllllllll0 == 6 || lllllllllllllllllllllll0 == 7)
                        {
                            float4 ll6 = float4(0, 0, 0, 0);
                            float4 lll6 = float4(0, 0, 0, 0);
                            float llll6 = 0;
                            if (lll1 || lllllllllllllllllllllll0 == 6)
                            {
                                float lllll6 = _ScreenParams.x / _ScreenParams.y;
#ifdef _HDRP
                                        float4 llllll6 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(_PlayersPosVectorArray[l5].xyz), 1.0));
                                        lll6 = ComputeScreenPos(llllll6 , _ProjectionParams.x);
#else
                                float4 llllll6 = mul(UNITY_MATRIX_VP, float4(_PlayersPosVectorArray[l5].xyz, 1.0));
                                lll6 = ComputeScreenPos(llllll6);
#endif
                                lll6.xy /= lll6.w;
                                lll6.x *= lllll6;
#ifdef _HDRP
                                        float4 llllllll6 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(d.worldSpacePosition.xyz), 1.0));
                                        ll6 = ComputeScreenPos(llllllll6 , _ProjectionParams.x);
#else
                                float4 llllllll6 = mul(UNITY_MATRIX_VP, float4(d.worldSpacePosition.xyz, 1.0));
                                ll6 = ComputeScreenPos(llllllll6);
#endif
                                ll6.xy /= ll6.w;
                                ll6.x *= lllll6;
#if defined(_DISSOLVEMASK)
                                        if(lll1) {
                                                llll6 = max(llllllllllllllllll2.z,llllllllllllllllll2.w);
                                        }
#endif
                            }
                            float3 llllllllll6 = _WorldSpaceCameraPos - _PlayersPosVectorArray[l5].xyz;
                            float3 lllllllllll6 = normalize(llllllllll6);
                            float lllllll4 = dot(d.worldSpacePosition.xyz - _PlayersPosVectorArray[l5].xyz, lllllllllll6);
                            float lllllllllllll6 = 0;
                            float llllllllllllll6 = 0;
                            float2 lllllllllllllll6 = float2(0, 0);
                            if (lllllllllllllllllllllll0 == 2 || lllllllllllllllllllllll0 == 3)
                            {
                                lllllllllllll6 = lllllllllllllllllllllllll0;
                                float llllllllll4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[l5].xyz) - lllllll4 * lllllllllll6);
                                float lllllllllllllllll4 = length(llllllllll6);
                                float llllllllllllllll4 = llllllllllllllllllllllllll0;
                                float llllllllllllllllllll4 = (lllllll4 / lllllllllllllllll4) * llllllllllllllll4;
#if _DISSOLVEMASK
                                        float llllllllllllllllllll6 = (2*llllllllllllllllllll4) / llll6;
                                        float2 lllllllllllllllllllll6 = ll6.xy - lll6.xy;
                                        lllllllllllllllllllll6 =  normalize(lllllllllllllllllllll6)*llllllllll4;
                                        lllllllllllllll6 = lllllllllllllllllllll6 /llllllllllllllllllll6;
#else
                                float llllllllllllllllllllll6 = llllllllll4 < llllllllllllllllllll4;
                                if (llllllllllllllllllllll6)
                                {
                                    float lllllllllllllllllllllll6 = llllllllll4 / llllllllllllllllllll4;
                                    llllllllllllll6 = lllllllllllllllllllllll6;
                                }
                                else
                                {
                                    llllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 4 || lllllllllllllllllllllll0 == 5)
                            {
                                lllllllllllll6 = lllllllllllllllllllllllllll0;
                                float llllllllll4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[l5].xyz) - lllllll4 * lllllllllll6);
                                float llllllll4 = llllllllllllllllllllllllllll0;
                                float llllllllllllllllllllllllll6 = (llllllllll4 < llllllll4) && lllllll4 > 0;
#if _DISSOLVEMASK
                                        float llllllllllllllllllll6 = (2*llllllll4) / llll6;
                                        float2 lllllllllllllllllllll6 = ll6.xy - lll6.xy;
                                        lllllllllllllllllllll6 =  normalize(lllllllllllllllllllll6)*llllllllll4;
                                        lllllllllllllll6 = lllllllllllllllllllll6 /llllllllllllllllllll6;
#else
                                if (llllllllllllllllllllllllll6)
                                {
                                    float lllllllllllllllllllllll6 = llllllllll4 / llllllll4;
                                    llllllllllllll6 = lllllllllllllllllllllll6;
                                }
                                else
                                {
                                    llllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 6)
                            {
                                lllllllllllll6 = lllllllllllllllllllllllllllll0;
                                float llllllllllllllllllllllllllllll6 = length(llllllllll6);
                                float lllll6 = _ScreenParams.x / _ScreenParams.y;
                                float l7 = min(1, lllll6);
                                float ll7 = distance(ll6.xy, lll6.xy) < llllllllllllllllllllllllllllll0 / llllllllllllllllllllllllllllll6 * l7;
                                float lll7 = (ll7) && lllllll4 > 0;
#if _DISSOLVEMASK
                                        float llll7 = llllllllllllllllllllllllllllll0/llllllllllllllllllllllllllllll6*l7;
                                        float llllllllllllllllllll6 = (2*llll7) / llll6;
                                        float2 lllllllllllllllllllll6 = ll6.xy - lll6.xy;
                                        lllllllllllllll6 = lllllllllllllllllllll6 /llllllllllllllllllll6;
#else
                                if (lll7)
                                {
                                    float lllllll7 = (distance(ll6.xy, lll6.xy) / (llllllllllllllllllllllllllllll0 / llllllllllllllllllllllllllllll6 * l7));
                                    llllllllllllll6 = lllllll7;
                                }
                                else
                                {
                                    llllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 7)
                            {
#if _OBSTRUCTION_CURVE
                                        lllllllllllll6 = lllllllllllllllllllllllllllllll0;
                                        float llllllllll4 = length((d.worldSpacePosition.xyz  - _PlayersPosVectorArray[l5].xyz) - lllllll4 * lllllllllll6);
                                        float llllllllllllllllllllllllllllll6 = length(llllllllll6);
                                        float4 llllllllll7 = float4(0,0,0,0);
                                        float lllllllllll7 = lllllllllllllllllll2.z;
                                        float llllllllllll7 = (lllllll4/llllllllllllllllllllllllllllll6) * lllllllllll7;
                                        float4 lllllllllllll7 = float4(0,0,0,0);
                                        lllllllllllll7 = lllllllllllllllllll2;
                                        float2 llllllllllllll7 = (llllllllllll7+0.5) * lllllllllllll7.xy;
                                            llllllllll7 = tex2D(llllll2, llllllllllllll7);
                                        float lllllllllllllll7 = llllllllll7.r * l1;
                                        float llllllllllllllll7 = (llllllllll4 < lllllllllllllll7) && lllllll4 > 0 ;
#if _DISSOLVEMASK
                                            float llllllllllllllllllll6 = (2*lllllllllllllll7) / llll6;
                                            float2 lllllllllllllllllllll6 = ll6.xy - lll6.xy;
                                            lllllllllllllllllllll6 =  normalize(lllllllllllllllllllll6)*llllllllll4;
                                            lllllllllllllll6 = lllllllllllllllllllll6 /llllllllllllllllllll6;
#else
                                            if(llllllllllllllll7){
                                                float lllllllllllllllllllllll6 = llllllllll4/lllllllllllllll7;
                                                llllllllllllll6 = lllllllllllllllllllllll6;
                                            } else {
                                                llllllllllllll6 = -1;
                                            }
#endif
#endif
                            }
#if defined(_DISSOLVEMASK)
                                    if(lll1) {
                                        float4 llllllllllllllllllll7 = float4(0,0,0,0);
                                        llllllllllllllllllll7 = llllllllllllllllll2;
                                        float2 lllllllllllllllllllll7 = float2(llllllllllllllllllll7.z/2,llllllllllllllllllll7.w/2);
                                        float2 llllllllllllllllllllll7 = lllllllllllllllllllll7 + lllllllllllllll6;
                                        float2 lllllllllllllllllllllll7 = (llllllllllllllllllllll7+0.5) * llllllllllllllllllll7.xy;
                                        float4 llllllllllllllllllllllll7 = float4(0,0,0,0);
                                            llllllllllllllllllllllll7 = tex2D(lllll2, lllllllllllllllllllllll7);
                                        float lllllllllllllllllllllllll7 = -1;
                                        if(llllllllllllllllllllll7.x <= llllllllllllllllllll7.z && llllllllllllllllllllll7.x >= 0 && llllllllllllllllllllll7.y <= llllllllllllllllllll7.w && llllllllllllllllllllll7.y >= 0 && llllllllllllllllllllllll7.x <= 0 && lllllll4 > 0 ){
                                            float llllllllllllllllllllllllll7 = sqrt(pow(llllllllllllllllllll7.z,2)+pow(llllllllllllllllllll7.w,2))/2;
                                            float lllllllllllllllllllllllllll7 = 40;
                                            float llllllllllllllllllllllllllll7 = llllllllllllllllllllllllll7/lllllllllllllllllllllllllll7;
                                            float lllllllllllllllllllllllllllll7 = 0;
                                            lllllllllllllllllllllllll7 = 0;     
                                                for (int i = 0; i < lllllllllllllllllllllllllll7; i++){
                                                    float2 llllllllllllllllllllllllllllll7 = lllllllllllllllllllll7 + (lllllllllllllll6 + ( normalize(lllllllllllllll6)*llllllllllllllllllllllllllll7*i));
                                                    float2 lllllllllllllllllllllllllllllll7 = (llllllllllllllllllllllllllllll7+0.5) * llllllllllllllllllll7.xy;
                                                    float4 l8 = tex2Dlod(lllll2, float4(lllllllllllllllllllllllllllllll7, 0.0, 0.0)); 
                                                    float2 ll8 = step(float2(0,0), llllllllllllllllllllllllllllll7) - step(float2(llllllllllllllllllll7.z,llllllllllllllllllll7.w), llllllllllllllllllllllllllllll7);
                                                    if(l8.x <= 0) {
                                                        lllllllllllllllllllllllllllll7 +=  (1/lllllllllllllllllllllllllll7) * (ll8.x * ll8.y);
                                                    }                                            
                                                }   
                                            lllllllllllllllllllllllll7 = 1-lllllllllllllllllllllllllllll7;  
                                        }         
                                        llllllllllllll6 = lllllllllllllllllllllllll7;
                                    }
#endif
                            if (ll1 <= 1)
                            {
                                if (llllllllllllll6 != -1)
                                {
                                    float lll8 = max(ll1, 0.00001);
                                    float llll8 = 1 - lllllllllllll6;
                                    float lllll8 = exp(lll8 * 6);
                                    float llllll8 = llllllllllllll6;
                                    float lllllll8 = llll8 / (lll8 / (lll8 * llll8 - 0.15 * (lll8 - llll8)));
                                    float llllllll8 = ((llllll8 - lllllll8) / (lllll8 * (1 - llllll8) + llllll8)) + lllllll8;
                                    llllllll8 = 1 - llllllll8;
                                    llllllllllllllllllllll5 = llllllll8 * sign(lllllllllllll6);
                                }
                            }
                            else
                            {
                                llllllllllllllllllllll5 = llllllllllllll6;
                            }
                        }
                        if (lllllllllllllllllllllll0 == 1 || lllllllllllllllllllllll0 == 3 || lllllllllllllllllllllll0 == 5)
                        {
                            float lllllllll8 = distance(_WorldSpaceCameraPos, _PlayersPosVectorArray[l5].xyz);
                            float llllllllll8 = distance(_WorldSpaceCameraPos, d.worldSpacePosition.xyz);
                            float3 lllllllllll8 = d.worldSpacePosition.xyz - _PlayersPosVectorArray[l5].xyz;
                            float3 llllllllllll8 = d.worldSpaceNormal;
                            float lllllllllllll8 = acos(dot(lllllllllll8, llllllllllll8) / (length(lllllllllll8) * length(llllllllllll8)));
                            if (lllllllllllll8 <= 1.5 && lllllllll8 > llllllllll8)
                            {
                                float llllllllllllll8 = (sqrt((lllllllll8 - llllllllll8)) * 25 / lllllllllllll8) * llllllllllllllllllllllll0;
                                llllllllllllllllllllll5 += max(0, log(llllllllllllll8 * 0.2));
                            }
                        }
                    }
                    float lllllllllllllll8 = llllllllllllllllllllll5;
                    float llllllllllllllll8 = 0;
                    float lllllllllllllllll8 = 0;
                    if (llll1 == 1 && llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                    {
                        llllllllllllllllllllll5 = min((1 * lllll1), 1);
                        lllllllllllllllllllllll5 = llllllllllllllllllllll5;
                    }
                    else
                    {
                        llllllllllllllllllllll5 = min(llllllllllllllllllllll5 + (1 * lllll1), 1);
                        lllllllllllllllllllllll5 = min((1 * lllll1), 1);
                    }
                    if (lllll3)
                    {
                        if (llllllllllllllllllllllllllll1 == 1)
                        {
                            float lllllllllllllllllllllllllllllll5 = 1 / llllllllllllllllllllllllllllll1;
                            if (lllllll3 < llllllllllllllllllllllllllllll1)
                            {
                                float lllllllllllllllllll8 = 1 - ((llllllllllllllllllllllllllllll1 - lllllll3) * lllllllllllllllllllllllllllllll5);
                                llllllllllllllllllllll5 = min(llllllllllllllllllllll5, lllllllllllllllllll8);
                                lllllllllllllllllllllll5 = min(lllllllllllllllllllllll5, lllllllllllllllllll8);
                            }
                        }
                        else if (llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                        {
                            if (llll1 == 1)
                            {
                                float llllllllllllllllllll8 = ((lllllllllllllll8) / llllllllllllllllllllllllllllll1);
                                if (lllllll3 < llllllllllllllllllllllllllllll1 && lllllllllllllll8 > 0 && saturate(lllllllllllllll8) > lllll1)
                                {
                                    float lllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllll3) * (llllllllllllllllllll8));
                                    lllllllllllllll8 = lllllllllllllll8 - (lllllllllllllllllll8);
                                }
                                else
                                {
                                }
                            }
                            if (lllllll3 < llllllllllllllllllllllllllllll1)
                            {
                                float lllllllllllllllllllllllllllllll5 = llllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                                float lllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllll3) * lllllllllllllllllllllllllllllll5);
                                llllllllllllllllllllll5 = max(0, lllllllllllllllllll8);
                                float llllllllllllllllllllllll8 = lllllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                                float lllllllllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllll3) * llllllllllllllllllllllll8);
                                lllllllllllllllllllllll5 = max(0, lllllllllllllllllllllllll8);
                                llllllllllllllll8 = llllllllllllllllllllll5;
                                lllllllllllllllll8 = lllllllllllllllllllllll5;
                                if (llll1 == 0 || llll1 == 1)
                                {
                                    llllllllllllllllllllll5 = max(lllllllllllllll8, lllllllllllllllllll8);
                                }
                            }
                            else
                            {
                                llllllllllllllllllllll5 = 0;
                                lllllllllllllllllllllll5 = 0;
                                llllllllllllllll8 = llllllllllllllllllllll5;
                                lllllllllllllllll8 = lllllllllllllllllllllll5;
                                if (llll1 == 0 || llll1 == 1)
                                {
                                    llllllllllllllllllllll5 = max(lllllllllllllll8, llllllllllllllllllllll5);
                                }
                            }
                        }
                    }
                    if (llllllll1)
                    {
                        float llllllllllllllllllllllllll8 = llllllllllllllllllllll5 / llllllllll1;
                        float lllllllllllllllllllllllllll8 = lllllllllllllllllllllll5 / llllllllll1;
                        float3 ll5 = _PlayersPosVectorArray[l5].xyz - _WorldSpaceCameraPos;
                        float3 lllllllllllllllllllllllllllll8 = d.worldSpacePosition.xyz - _WorldSpaceCameraPos;
                        float llllllllllllllllllllllllllllll8 = dot(lllllllllllllllllllllllllllll8, normalize(ll5));
                        if (llllllllllllllllllllllllllllll8 - lllllllll1 >= length(ll5))
                        {
                            float lllllllllllllllllllllllllllllll8 = llllllllllllllllllllllllllllll8 - lllllllll1 - length(ll5);
                            if (lllllllllllllllllllllllllllllll8 < 0)
                            {
                                lllllllllllllllllllllllllllllll8 = 0;
                            }
                            if (lllllllllllllllllllllllllllllll8 < llllllllll1)
                            {
                                llllllllllllllllllllll5 = (llllllllll1 - lllllllllllllllllllllllllllllll8) * llllllllllllllllllllllllll8;
                                lllllllllllllllllllllll5 = (llllllllll1 - lllllllllllllllllllllllllllllll8) * lllllllllllllllllllllllllll8;
                            }
                            else
                            {
                                llllllllllllllllllllll5 = 0;
                                lllllllllllllllllllllll5 = 0;
                            }
                        }
                    }
                    if (lllllllllllllllllllllllllll1 && !lllll3)
                    {
                        if (llllllllllllllllllllllllllll1 == 1)
                        {
                            llllllllllllllllllllll5 = 0;
                            lllllllllllllllllllllll5 = 0;
                        }
                    }
                    if (lllllllllll1 == 1)
                    {
                        float l9 = 0;
                        float ll9 = 0;
                        if (lllllllllllll1 == 0)
                        {
                            l9 = llllllllllllllllllllll5 / llllllllllllllll1;
                            ll9 = lllllllllllllllllllllll5 / llllllllllllllll1;
                        }
                        else if (lllllllllllll1 == 1)
                        {
                            float lll9 = 1 - llllllllllllllllllllll5;
                            float llll9 = 1 - lllllllllllllllllllllll5;
                            if (lllllllllllllllllllllllllll1 && lllll3 && lllllllllllllllllllllllllllll1)
                            {
                                lll9 = max(1 - llllllllllllllllllllll5, 1 - (llllllllllllllllllllll5 * lllllllllllllllllllll5));
                                llll9 = max(1 - lllllllllllllllllllllll5, 1 - (lllllllllllllllllllllll5 * lllllllllllllllllllll5));
                            }
                            l9 = lll9 / llllllllllllllll1;
                            ll9 = llll9 / llllllllllllllll1;
                        }
                        if (llllllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y > (_PlayersPosVectorArray[l5].y + lllllllllllllll1))
                            {
                                float lllllllllllllllllllllllllllllll8 = d.worldSpacePosition.y - (_PlayersPosVectorArray[l5].y + lllllllllllllll1);
                                if (lllllllllllllllllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllllllllllllllllll8 = 0;
                                }
                                if (lllllllllllll1 == 0)
                                {
                                    if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllll5 = ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                                        lllllllllllllllllllllll5 = ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * ll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllll5 = 0;
                                        lllllllllllllllllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                                        lllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * ll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllll5 = 1;
                                        lllllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllll5 = 1;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y > llllllllllllll1)
                            {
                                float lllllllllllllllllllllllllllllll8 = d.worldSpacePosition.y - llllllllllllll1;
                                if (lllllllllllllllllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllllllllllllllllll8 = 0;
                                }
                                if (lllllllllllll1 == 0)
                                {
                                    if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllll5 = ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                                        lllllllllllllllllllllll5 = ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * ll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllll5 = 0;
                                        lllllllllllllllllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                                        lllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * ll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllll5 = 1;
                                        lllllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllll5 = 1;
                                }
                            }
                        }
                    }
                    float lllllll9 = llllllllllllllllllllll5;
                    float llllllll9 = lllllllllllllllllllllll5;
                    if (lllllllllllllllll1 == 1)
                    {
                        float lllllllll9 = llllllllllllllllllllll5 / lllllllllllllllllllll1;
                        float llllllllll9 = lllllllllllllllllllllll5 / lllllllllllllllllllll1;
                        if (llllllllllllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y < (_PlayersPosVectorArray[l5].y + llllllllllllllllllll1))
                            {
                                float lllllllllllllllllllllllllllllll8 = (_PlayersPosVectorArray[l5].y + llllllllllllllllllll1) - d.worldSpacePosition.y;
                                if (lllllllllllllllllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllllllllllllllllll8 = 0;
                                }
                                if (lllllllllllllllllllllllllllllll8 < lllllllllllllllllllll1)
                                {
                                    llllllllllllllllllllll5 = (lllllllllllllllllllll1 - lllllllllllllllllllllllllllllll8) * lllllllll9;
                                    lllllllllllllllllllllll5 = (lllllllllllllllllllll1 - lllllllllllllllllllllllllllllll8) * llllllllll9;
                                }
                                else
                                {
                                    llllllllllllllllllllll5 = 0;
                                    lllllllllllllllllllllll5 = 0;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y < lllllllllllllllllll1)
                            {
                                float lllllllllllllllllllllllllllllll8 = lllllllllllllllllll1 - d.worldSpacePosition.y;
                                if (lllllllllllllllllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllllllllllllllllll8 = 0;
                                }
                                if (lllllllllllllllllllllllllllllll8 < lllllllllllllllllllll1)
                                {
                                    llllllllllllllllllllll5 = (lllllllllllllllllllll1 - lllllllllllllllllllllllllllllll8) * lllllllll9;
                                    lllllllllllllllllllllll5 = (lllllllllllllllllllll1 - lllllllllllllllllllllllllllllll8) * llllllllll9;
                                }
                                else
                                {
                                    llllllllllllllllllllll5 = 0;
                                    lllllllllllllllllllllll5 = 0;
                                }
                            }
                        }
                        if (llllllllllllllllllllll1 == 0) 
                        {
                        }
                        else if (llllllllllllllllllllll1 == 1) 
                        {
                            if (lllll3)
                            {
                                llllllllllllllllllllll5 = max(llllllllllllllll8, llllllllllllllllllllll5);
                                lllllllllllllllllllllll5 = max(lllllllllllllllll8, lllllllllllllllllllllll5);
                            }
                            else
                            {
                                llllllllllllllllllllll5 = lllllll9;
                                lllllllllllllllllllllll5 = llllllll9;
                            }
                        }
                        else if (llllllllllllllllllllll1 == 2) 
                        {
                            if (lllll3)
                            {
                                llllllllllllllllllllll5 = min(llllllllllllllll8, llllllllllllllllllllll5);
                                llllllllllllllllllllll5 = max(lllllllllllllll8, llllllllllllllllllllll5);
                                lllllllllllllllllllllll5 = min(lllllllllllllllll8, lllllllllllllllllllllll5);
                            }
                        }
                    }
                    if (!llllllllll0 && !lllllllllll0)
                    {
                        if (lllllll1 == 1 && distance(_PlayersPosVectorArray[l5].xyz, d.worldSpacePosition) > llllll1)
                        {
                            llllllllllllllllllllll5 = 0;
                            lllllllllllllllllllllll5 = 0;
                        }
                    }
                }
                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4] * 4 + 1;
                if (lllllllllllllllllllllllllll1 && lllll3 && lllllllllllllllllllllllllllll1)
                {
                    lllllllllllllll5 = lllllllllllllll5 * lllllllllllllllllllll5;
                }
                if (llllllllll0 || lllllllllll0)
                {
                    llllllllllllllllllllll5 = lllllllllllllll5 * llllllllllllllllllllll5;
                    lllllllllllllllllllllll5 = lllllllllllllll5 * lllllllllllllllllllllll5;
                }
                else
                {
                    if (lllllllllllllllllllllllllll1)
                    {
                        if (lllll3)
                        {
                            if (lllllllllllllllllllllllllllll1)
                            {
                                llllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllll5;
                                lllllllllllllllllllllll5 = lllllllllllllllllllll5 * lllllllllllllllllllllll5;
                            }
                        }
                        else
                        {
                            if (llllllllllllllllllllllllllll1 == 1)
                            {
                                llllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllll5;
                                lllllllllllllllllllllll5 = lllllllllllllllllllll5 * lllllllllllllllllllllll5;
                            }
                        }
                    }
                }
                ll3 = max(ll3, llllllllllllllllllllll5);
                lll3 = max(lll3, lllllllllllllllllllllll5);
            }
            else
            {
                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4 + 2] * 4 + 3;
                llllllllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllllllllllllllllll4] * 4 + 1;
            }
        }
#else
        float lllllllllllllll5 = 0;
        if (!lllllllllllllll2)
        {
            lllllllllllllll5 = 1;
            if (lllllll0 != 0 && llllllll0 != 0 && lllllllllllll2 - llllllll0 < lllllllllllllllllllllllll1)
            {
                if (lllllll0 == 1)
                {
                    lllllllllllllll5 = ((lllllllllllllllllllllllll1 - (lllllllllllll2 - llllllll0)) / lllllllllllllllllllllllll1);
                }
                else
                {
                    lllllllllllllll5 = ((lllllllllllll2 - llllllll0) / lllllllllllllllllllllllll1);
                }
            }
            else if (lllllll0 == -1)
            {
                lllllllllllllll5 = 1;
            }
            else if (lllllll0 == 1)
            {
                lllllllllllllll5 = 0;
            }
            else
            {
                lllllllllllllll5 = 1;
            }
            lllllllllllllll5 = 1 - lllllllllllllll5;
        }
        float llllllllllllllllllllll5 = 0;
        float lllllllllllllllllllll5 = 0;
        bool lllllllllllllllllllllllll5 = distance(_WorldSpaceCameraPos, d.worldSpacePosition) > llllll1;
        if ((lllllllllllllll5 != 0) || ((!llllllllll0 && !lllllllllll0) && (lllllll1 == 0 || !lllllllllllllllllllllllll5) ))
        {
#if defined(_ZONING)
                        if(lllllllllllllllllllllllllll1) {
                            if(lllll3) 
                            {
                                if(lllllllllllllllllllllllllllll1) {
                                    float llllll5 = lllllllll3;
                                    float lllllll5 = lllllllllll3;
                                    lllllllllllllllllllll5 = 1;
                                    float llllllllllllllllllllllllllllll5 = llllllllll3;
                                    if( lllllll5!= 0 && llllll5 != 0 && lllllllllllll2-llllll5 < llllllllllllllllllllllllllllll5) {
                                        if(lllllll5 == 1) {
                                            lllllllllllllllllllll5 = ((llllllllllllllllllllllllllllll5-(lllllllllllll2-llllll5))/llllllllllllllllllllllllllllll5);
                                        } else {
                                            lllllllllllllllllllll5 = ((lllllllllllll2-llllll5)/llllllllllllllllllllllllllllll5);
                                        }
                                    } else if(lllllll5 ==-1) {
                                        lllllllllllllllllllll5 = 1;
                                    } else if(lllllll5 == 1) {
                                        lllllllllllllllllllll5 = 0;
                                    } else {
                                        lllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllll5 = 1 - lllllllllllllllllllll5;
                                    if(llllllllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllllllll1) {
                                        float lllllllllllllllllllllllllllllll5 = 1 / llllllllllllllllllllllllllllll1;
                                        if (lllllll3 < llllllllllllllllllllllllllllll1)  {
                                            float l6 = ((llllllllllllllllllllllllllllll1-lllllll3) * lllllllllllllllllllllllllllllll5);
                                            lllllllllllllllllllll5 =  max(lllllllllllllllllllll5,l6);
                                        }
                                    }
                                } else { 
                                }
                            } else {
                            }
                        }
#endif
            llllllllllllllllllllll5 = min(llllllllllllllllllllll5 + (1 * lllll1), 1);
            if (lllll3)
            {
                if (llllllllllllllllllllllllllll1 == 1)
                {
                    float lllllllllllllllllllllllllllllll5 = 1 / llllllllllllllllllllllllllllll1;
                    if (lllllll3 < llllllllllllllllllllllllllllll1)
                    {
                        float lllllllllllllllllllllll9 = 1 - ((llllllllllllllllllllllllllllll1 - lllllll3) * lllllllllllllllllllllllllllllll5);
                        llllllllllllllllllllll5 = min(llllllllllllllllllllll5, lllllllllllllllllllllll9);
                    }
                }
                else if (llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                {
                    float lllllllllllllllllllllllllllllll5 = llllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                    if (lllllll3 < llllllllllllllllllllllllllllll1)
                    {
                        float lllllllllllllllllllllll9 = ((llllllllllllllllllllllllllllll1 - lllllll3) * lllllllllllllllllllllllllllllll5);
                        llllllllllllllllllllll5 = max(0, lllllllllllllllllllllll9);
                    }
                    else
                    {
                        llllllllllllllllllllll5 = 0;
                    }
                }
            }
            if (lllllllllllllllllllllllllll1 && !lllll3)
            {
                if (llllllllllllllllllllllllllll1 == 1)
                {
                    llllllllllllllllllllll5 = 0;
                }
            }
            if (lllllllllll1 == 1 && llllllllllll1 == 0)
            {
                float l9 = 0;
                if (lllllllllllll1 == 0)
                {
                    l9 = (llllllllllllllllllllll5) / llllllllllllllll1;
                }
                else if (lllllllllllll1 == 1)
                {
                    float lll9 = 1 - llllllllllllllllllllll5;
                    if (lllllllllllllllllllllllllll1 && lllll3 && lllllllllllllllllllllllllllll1)
                    {
                        lll9 = max(1 - llllllllllllllllllllll5, 1 - (llllllllllllllllllllll5 * lllllllllllllllllllll5));
                    }
                    l9 = lll9 / llllllllllllllll1;
                }
                if (d.worldSpacePosition.y > llllllllllllll1)
                {
                    float lllllllllllllllllllllllllllllll8 = d.worldSpacePosition.y - llllllllllllll1;
                    if (lllllllllllllllllllllllllllllll8 < 0)
                    {
                        lllllllllllllllllllllllllllllll8 = 0;
                    }
                    if (lllllllllllll1 == 0)
                    {
                        if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                        {
                            llllllllllllllllllllll5 = ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                        }
                        else
                        {
                            llllllllllllllllllllll5 = 0;
                        }
                    }
                    else
                    {
                        if (lllllllllllllllllllllllllllllll8 < llllllllllllllll1)
                        {
                            llllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - lllllllllllllllllllllllllllllll8) * l9);
                        }
                        else
                        {
                            llllllllllllllllllllll5 = 1;
                        }
                        lllllllllllllllllllll5 = 1;
                    }
                }
            }
            if (lllllllllllllllll1 == 1 && llllllllllllllllll1 == 0)
            {
                float lllllllll9 = llllllllllllllllllllll5 / lllllllllllllllllllll1;
                if (d.worldSpacePosition.y < lllllllllllllllllll1)
                {
                    float lllllllllllllllllllllllllllllll8 = lllllllllllllllllll1 - d.worldSpacePosition.y;
                    if (lllllllllllllllllllllllllllllll8 < 0)
                    {
                        lllllllllllllllllllllllllllllll8 = 0;
                    }
                    if (lllllllllllllllllllllllllllllll8 < lllllllllllllllllllll1)
                    {
                        llllllllllllllllllllll5 = (lllllllllllllllllllll1 - lllllllllllllllllllllllllllllll8) * lllllllll9;
                    }
                    else
                    {
                        llllllllllllllllllllll5 = 0;
                    }
                }
            }
        }
        if (lllllllllllllllllllllllllll1 && lllll3 && lllllllllllllllllllllllllllll1)
        {
            lllllllllllllll5 = lllllllllllllll5 * lllllllllllllllllllll5;
        }
        if (llllllllll0 || lllllllllll0)
        {
            llllllllllllllllllllll5 = lllllllllllllll5 * llllllllllllllllllllll5;
        }
        else
        {
            llllllllllllllllllllll5 = llllllllllllllllllllll5;
            if (lllllllllllllllllllllllllll1)
            {
                if (lllll3)
                {
                    if (lllllllllllllllllllllllllllll1)
                    {
                        llllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllll5;
                    }
                }
                else
                {
                    if (llllllllllllllllllllllllllll1 == 1)
                    {
                        llllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllll5;
                    }
                }
            }
        }
        ll3 = max(ll3, llllllllllllllllllllll5);
#endif
        float llllllllllllllllllllllll5 = ll3;
        if (!ll2)
        {
            if (llllllllllllllllllllllll5 == 1)
            {
                llllllllllllllllllllllll5 = 10;
            }
            if (!llllllllllllllllllllll0) 
            {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                if (!any(unity_LightShadowBias))
                {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllll2 - llllllllllllllllllllllll5);
#endif
                    lllllllllll2 = llllllllllllllllllll2 - llllllllllllllllllllllll5;
                }
                else
                {
                    if(llllllllllllllllllllll0) 
                    {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllll2 - llllllllllllllllllllllll5);
#endif
                        lllllllllll2 = llllllllllllllllllll2 - llllllllllllllllllllllll5;
                    }
                }
#endif
#else
#if !defined(NO_STS_CLIPPING)
                clip(llllllllllllllllllll2 - llllllllllllllllllllllll5);
#endif
                lllllllllll2 = llllllllllllllllllll2 - llllllllllllllllllllllll5;
#endif
            }
            else
            {
                if (lllllllllllllllllllllll0 == 6 && llllllllllllllllllllll0)
                {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                    if (!any(unity_LightShadowBias))
                    {
                    }
                    else
                    {
                        llllllllllllllllllllllll5 = lll3;
                        if (llllllllllllllllllllllll5 == 1)
                        {
                            llllllllllllllllllllllll5 = 10;
                        }                    
                    }                
#endif
#endif
                }
#if !defined(NO_STS_CLIPPING)
                clip(llllllllllllllllllll2 - llllllllllllllllllllllll5);
#endif
                lllllllllll2 = llllllllllllllllllll2 - llllllllllllllllllllllll5;
            }
            if (llllllllllllllllllll2 - llllllllllllllllllllllll5 < 0)
            {
                lllllllllll2 = 0;
            }
            else
            {
                lllllllllll2 = 1;
            }
        }
        if (ll2)
        {
            llllllllllllllll2 = 1;
            if ((llllllllllllllllllll2 - llllllllllllllllllllllll5) < 0)
            {
                lllllllllllllllll2 = half4(1, 1, 1, 1);
                o.Emission = 1;
            }
            else
            {
                lllllllllllllllll2 = half4(0, 0, 0, 1);
            }
            if (lllllllllllllllllllllllllllll4)
            {
                if ((llllllllllllllllllll2 - llllllllllllllllllllllll5) < 0)
                {
                    lllllllllllllllll2 = half4(0.5, 1, 0.5, 1);
                    o.Emission = 0;
                }
                else
                {
                    lllllllllllllllll2 = half4(0, 0.1, 0, 1);
                }
            }
            if (lllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1)
            {
                float l10 = 0;
                if (llllllllllllllllll1 == 1)
                {
                    llllllllllllllllllllllllllll4 = llllllllllllllllllllllllllll4 + l2;
                    l10 = llllllllllllllllllllllllllll4;
                }
                else
                {
                    l10 = lllllllllllllllllll1 + l2;
                }
                if (d.worldSpacePosition.y > (l10 - lll2) && d.worldSpacePosition.y < (l10 + lll2))
                {
                    lllllllllllllllll2 = half4(1, 0, 0, 1);
                }
            }
        }
        else
        {
            half3 ll10 = lerp(1, lllllllllllllll0, llllllllllllllll0).rgb;
            if (llllllllllllllllllll0)
            {
                lllllllllllllllllllll0 = 0.2 + (lllllllllllllllllllll0 * (0.8 - 0.2));
                o.Emission = o.Emission + min(clamp(ll10 * clamp(((llllllllllllllllllllllll5 / lllllllllllllllllllll0) - llllllllllllllllllll2), 0, 1), 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0), clamp(ll10 * llllllllllllllllllllllll5, 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0));
            }
            else
            {
                o.Emission = o.Emission + clamp(ll10 * llllllllllllllllllllllll5, 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0);
            }
        }
    }
    if (llllllllllllllll2)
    {
        o.Albedo = lllllllllllllllll2.rgb;
    }
    lllllllll2 = o.Albedo;
    llllllllll2 = o.Emission;
    #ifdef _HDRP  
        float lll10 = 0;
        float llll10 = 0;
    #if SHADEROPTIONS_PRE_EXPOSITION
            llll10 =  LOAD_TEXTURE2D(_ExposureTexture, int2(0, 0)).x * _ProbeExposureScale;
    #else
            llll10 = _ProbeExposureScale;
    #endif
            float lllll10 = 0;
            float llllll10 = llll10;
            lllll10 = rcp(llllll10 + (llllll10 == 0.0));
            float3 lllllll10 = o.Emission * lllll10;
            o.Emission = lerp(lllllll10, o.Emission, lll10);
        llllllllll2 = o.Emission;
    #endif
}
void DoCrossSection(
                    half llllllll10,
                    half4 lllllllll10,
                    half llllllllll10,
                    sampler2D lllllllllll10,
                    float llllllllllll10,
                    half lllllllllllll10,
                    bool llllllllllllll10,
                    float4 lllll0,
                    inout half4 llllllllllllllll10
                    )
{
    if (llllllll10 == 1)
    {
        if (llllllllllllll10 == false)
        {
            if (llllllllll10 == 1)
            {
                float2 lllllllllllllllllllllllllllllll7 = lllll0.xy / lllll0.w;
                if (lllllllllllll10 == 1)
                {
                    float4 llllllllllllllllll10 = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1));
                    lllllllllllllllllllllllllllllll7.xy *= distance(_WorldSpaceCameraPos, llllllllllllllllll10);
                }
                float lllll6 = _ScreenParams.x / _ScreenParams.y;
                lllllllllllllllllllllllllllllll7.x *= lllll6;
                half3 llllllllllllllllllll10 = tex2D(lllllllllll10, lllllllllllllllllllllllllllllll7 * llllllllllll10).rgb;
                llllllllllllllll10 = half4(llllllllllllllllllll10, 1) * lllllllll10;
            }
            else
            {
                llllllllllllllll10 = lllllllll10;
            }
        }
    }
}


#endif
