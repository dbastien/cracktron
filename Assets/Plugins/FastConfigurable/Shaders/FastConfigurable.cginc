// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "HoloToolkitCommon.cginc"
#include "macro.cginc"

#define USE_PER_PIXEL (_USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON || _FORCEPERPIXEL_ON)
#define PIXEL_SHADER_USES_WORLDPOS  (USE_PER_PIXEL && (_SPECULARHIGHLIGHTS_ON || _SHADE4_ON || _USERIMLIGHTING_ON))
#define PIXEL_SHADER_USES_NORMAL (_FORCEPERPIXEL_ON || (USE_PER_PIXEL && !_USEBUMPMAP_ON))
#define USES_TEX_XY (_USEMAINTEX_ON || _USEOCCLUSIONMAP_ON || _USEEMISSIONMAP_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON)

half4 _Color;
UNITY_DECLARE_TEX2D_HALF(_MainTex);
UNITY_DECLARE_TEX2D_HALF(_OcclusionMap);

UNITY_DECLARE_TEX2D_HALF(_BumpMap);

half _Specular;
UNITY_DECLARE_TEX2D_HALF(_SpecularMap);

half _Gloss;
UNITY_DECLARE_TEX2D_HALF(_GlossMap);

samplerCUBE _CubeMap;
half _ReflectionScale;
half4x4 CalibrationSpaceWorldToLocal;

half _RimPower;
half4 _RimColor;

half4 _EmissionColor;
UNITY_DECLARE_TEX2D_HALF(_EmissionMap);

half _Cutoff;
float4 _TextureScaleOffset;

struct a2v
{
    float4 vertex : POSITION;
    half3 normal : NORMAL;

    #if defined(_USEVERTEXCOLOR_ON)
        half4 color : COLOR;
    #endif

    #if USES_TEX_XY
        float2 mainUV : TEXCOORD0;
    #endif

    #if LIGHTMAP_ON
        float2 lightMapUV : TEXCOORD1;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 pos : SV_POSITION;

    #if defined(_USEVERTEXCOLOR_ON)
        half4 color : COLOR;
    #endif

    #if !defined(_USEBUMPMAP_ON) && PIXEL_SHADER_USES_NORMAL
        half3 worldNormal : NORMAL;
    #endif

    #if USES_TEX_XY || defined(_NEAR_PLANE_FADE_ON)
        float3 texXYFadeZ : TEXCOORD0;
    #endif

    #if LIGHTMAP_ON
        float2 lmap : TEXCOORD1;
    #else
        half3 vertexLighting : TEXCOORD1;
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        float4 worldPos: TEXCOORD2;
    #endif

    #if defined(_USEREFLECTIONS_ON)
        half3 worldReflection : TEXCOORD3;
    #endif

    LIGHTING_COORDS(4, 5)
    UNITY_FOG_COORDS(6)
    UNITY_VERTEX_OUTPUT_STEREO
};

inline half4 FastConfigurablePreMultiplyAlpha(half4 color)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return half4(color.rgb * color.a, color.a);
#endif
    return color;
}

inline half3 FastConfigurablePreMultiplyAlphaWithReflectivity(half3 diffColor, half alpha, half oneMinusReflectivity, out half outModifiedAlpha)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    diffColor *= alpha;

    //reflectivity is removed from the rest of components, including transparency
    outModifiedAlpha = 1 - oneMinusReflectivity + alpha*oneMinusReflectivity;
#else
    outModifiedAlpha = alpha;
#endif
    return diffColor;
}

inline half3 FastConfigurableLightingLambertian(half3 normal, half3 lightDir, half3 lightCol)
{
    return lightCol * saturate(dot(normal, lightDir));
}

inline half3 FastConfigurableLightingBlinnPhong(half3 normal, half3 lightDir, half3 lightCol, half3 viewDir, half specularPower, half specularScale, half3 specularColor)
{
    half3 h = normalize(lightDir + viewDir);
    half nh = saturate(dot(normal, h));

    return (lightCol * specularColor) * (pow(nh, specularPower) * specularScale);
}

#if defined(_USERIMLIGHTING_ON)
half3 RimLight(half3 worldNormal, float4 worldPos)
{
    const half rimBoost = 0.25;

    //boost to the dot product to allow for a greater max effect
    half rim = 1.0 - saturate(dot(worldNormal, UnityWorldSpaceViewDir(worldPos)));

    half smooth = smoothstep(rimBoost, 1.0, rim * _RimPower + rimBoost);

    return smooth * _RimColor.rgb;
}
#endif

//note that spot lights will behave as point lights with this approach
half3 Shade4PointLightsHalf(float3 pos, half3 normal)
{
    // to light vectors
    //unity_4LightPosX0..Z0 are float4s
    //TODO: make new global shader vars that send these up as half4s
    half4 toLightX = unity_4LightPosX0 - pos.x;
    half4 toLightY = unity_4LightPosY0 - pos.y;
    half4 toLightZ = unity_4LightPosZ0 - pos.z;

    half4 ndotl = toLightX * normal.x + toLightY * normal.y + toLightZ * normal.z;
    half4 lengthSq = toLightX * toLightX + toLightY * toLightY + toLightZ * toLightZ;

    // correct NdotL
    ndotl = saturate(ndotl * rsqrt(lengthSq));

    // attenuation
    //unity_4LightAtten0 is a half
    half4 atten = rcp(mad(lengthSq, unity_4LightAtten0, 1));
    half4 diff = ndotl * atten;

    // final color
    //unity_LightColor[n] is half4
    return (unity_LightColor[0].rgb * diff.x) + (unity_LightColor[1].rgb * diff.y) + (unity_LightColor[2].rgb * diff.z) + (unity_LightColor[3].rgb * diff.w);
}

v2f vert(a2v v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.pos = UnityObjectToClipPos(v.vertex);

    #if defined(_USEVERTEXCOLOR_ON)
        #if defined(_USEMAINCOLOR_ON)
            o.color = v.color * _Color;
        #else
            o.color = v.color;
        #endif
    #endif

    #if defined(_SPECULARHIGHLIGHTS_ON) || defined(_SHADE4_ON) || defined(_USEREFLECTIONS_ON) || defined(_USERIMLIGHTING_ON)
        float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        o.worldPos = worldPos;
    #endif

    #if USES_TEX_XY
        o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
    #endif

    #if FLIP_NORMALS
        half3 flipCorrectedNormal = -v.normal;
    #else
        half3 flipCorrectedNormal = v.normal;
    #endif

    half3 worldNormal = UnityObjectToWorldNormal(flipCorrectedNormal);

    #if defined(LIGHTMAP_ON)
        //TODO: explicit MAD?
        o.lmap.xy = v.lightMapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #else
        #if defined(_USEAMBIENT_ON) && !defined(_USEBUMPMAP_ON)
            //grab ambient color from Unity's spherical harmonics
            //ShadeSH9 operates in half precision
            o.vertexLighting += ShadeSH9(half4(worldNormal, 1.0));
        #endif

        #if !(USE_PER_PIXEL)
            #if defined(_USEDIFFUSE_ON)
                o.vertexLighting += FastConfigurableLightingLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif
            #if defined(_SPECULARHIGHLIGHTS_ON)
                o.vertexLighting += FastConfigurableLightingBlinnPhong(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(worldPos), _Specular, _Gloss, _SpecColor);
            #endif
            #if defined(SHADE4_ON)
                //handle point and spot lights
                o.vertexLighting += Shade4PointLightsHalf(worldPos, worldNormal);
            #endif
            #if defined(_USERIMLIGHTING_ON)
                o.vertexLighting += RimLight(worldNormal, worldPos);
            #endif
        #endif
    #endif

    #if !defined(_USEBUMPMAP_ON) && PIXEL_SHADER_USES_NORMAL
        o.worldNormal = worldNormal;
    #endif
    
    #if defined(_USEREFLECTIONS_ON)
        half3 vertexToCamera = _WorldSpaceCameraPos - worldPos;	

        #if defined(_CALIBRATIONSPACEREFLECTIONS_ON)
            half3 normalReflection = normalize(mul((half3x3)CalibrationSpaceWorldToLocal, flipCorrectedNormal);
            vertexToCamera = normalize(mul((half3x3)CalibrationSpaceWorldToLocal, vertexToCamera));
        #else
            half3 normalReflection = flipCorrectedNormal;
        #endif

        o.worldReflection = reflect(vertexToCamera, normalReflection);
    #endif

    //fade away objects closer to the camera
    #if defined(_NEAR_PLANE_FADE_ON)
        o.texXYFadeZ.z = ComputeNearPlaneFadeLinear(v.vertex);
    #endif

    //transfer shadow and lightmap info
    TRANSFER_VERTEX_TO_FRAGMENT(o);

    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

half4 frag(v2f IN) : SV_Target
{
    #if defined(_USEMAINTEX_ON)
        half4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
    #else
        half4 color = 1;
    #endif

    #if defined(_USEOCCLUSIONMAP_ON)
        color *= UNITY_SAMPLE_TEX2D(_OcclusionMap, IN.texXYFadeZ.xy);
    #endif

    #if defined(_USEVERTEXCOLOR_ON)
        //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
        color *= IN.color;        
    #elif defined(_USEMAINCOLOR_ON)
        color *= _Color;
    #endif

    #if defined(_ALPHATEST_ON)
        clip(color.a - _Cutoff);
    #endif

    //light attenuation from shadows cast onto the object
    half lightAttenuation = SHADOW_ATTENUATION(IN);
    
    //TODO: consider UnityComputeForwardShadows
    #if defined(LIGHTMAP_ON)
        half3 lightmapResult = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
        #if defined(SHADOWS_SCREEN)
            half3 lightColorShadowAttenuated = min(lightmapResult, lightAttenuation * 2);
        #else
            half3 lightColorShadowAttenuated = lightmapResult;
        #endif	
    #else
        half3 lightColorShadowAttenuated = IN.vertexLighting;
        #if USE_PER_PIXEL
            //if a normal map is on, it makes sense to do most calculations per-pixel
            #if defined(_USEBUMPMAP_ON)
                //unpack can be expensive if normal map is dxt5
                half3 worldNormal = UnityObjectToWorldNormal(UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.texXYFadeZ.xy)));
                //grab ambient color from Unity's spherical harmonics
                //ShadeSH9 operates in half precision
                lightColorShadowAttenuated += ShadeSH9(half4(worldNormal, 1.0));
            #else
                half3 worldNormal = IN.worldNormal;
            #endif					
        
            #if defined(_USEDIFFUSE_ON)
                lightColorShadowAttenuated += FastConfigurableLightingLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif
            #if defined(_SPECULARHIGHLIGHTS_ON)
                half gloss = _Gloss;
                #if defined(_USEGLOSSMAP_ON)
                    gloss *= UNITY_SAMPLE_TEX2D(_GlossMap, IN.texXYFadeZ.xy).r;
                #endif
                half specular = _Specular;
                #if defined(_USESPECULARMAP_ON)
                    specular *= UNITY_SAMPLE_TEX2D(_SpecularMap, IN.texXYFadeZ.xy).r;
                #endif
                lightColorShadowAttenuated += FastConfigurableLightingBlinnPhong(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(IN.worldPos), specular, gloss, _SpecColor);
            #endif
            #if _SHADE4_ON
                //handle point and spot lights
                lightColorShadowAttenuated += Shade4PointLightsHalf(IN.worldPos, worldNormal);
            #endif
            #if defined(_USERIMLIGHTING_ON)
                lightColorShadowAttenuated += RimLight(worldNormal, IN.worldPos);
            #endif
        #endif
        lightColorShadowAttenuated *= lightAttenuation;
    #endif
    
    color.rgb *= lightColorShadowAttenuated;

    #if defined(_USEREFLECTIONS_ON)
        color.rgb += texCUBE(_CubeMap, IN.worldReflection) * _ReflectionScale;
    #endif

    #if defined(_USEEMISSIONMAP_ON)
        color.rgb += UNITY_SAMPLE_TEX2D(_EmissionMap, IN.texXYFadeZ.xy);
    #endif

    #if defined(_USEEMISSIONCOLOR_ON)
        color.rgb += _EmissionColor;
    #endif

    #if defined(_NEAR_PLANE_FADE_ON)
        color.rgb *= IN.texXYFadeZ.z;
    #endif

    UNITY_APPLY_FOG(IN.fogCoord, color);
    
    return color;
}