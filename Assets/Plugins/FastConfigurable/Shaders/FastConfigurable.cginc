// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

//TODO: check/add vs add passes
//TODO: support reflections from unity_SpecCube0 ?

#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "ShaderCommon.cginc"
#include "Macro.cginc"

#define USE_PER_PIXEL (_USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON || _FORCEPERPIXEL_ON)
#define PIXEL_SHADER_USES_WORLDPOS  (USE_PER_PIXEL && (_SPECULARHIGHLIGHTS_ON || _SHADE4_ON || _USERIMLIGHTING_ON))
#define PIXEL_SHADER_USES_NORMAL (_FORCEPERPIXEL_ON || (USE_PER_PIXEL && !_USEBUMPMAP_ON))
#define USES_TEX_XY (_USEMAINTEX_ON || _USEOCCLUSIONMAP_ON || _USEEMISSIONMAP_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON)

float4 _Color;
UNITY_DECLARE_TEX2D(_MainTex);
UNITY_DECLARE_TEX2D(_OcclusionMap);

UNITY_DECLARE_TEX2D(_BumpMap);

float _Specular;
UNITY_DECLARE_TEX2D(_SpecularMap);

float _Gloss;
UNITY_DECLARE_TEX2D(_GlossMap);

samplerCUBE _CubeMap;
float _ReflectionScale;
float4x4 CalibrationSpaceWorldToLocal;

float _RimPower;
float4 _RimColor;

float4 _EmissionColor;
UNITY_DECLARE_TEX2D(_EmissionMap);

float _Cutoff;
float4 _TextureScaleOffset;

struct a2v
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;

    #if defined(_USEVERTEXCOLOR_ON)
        float4 color : COLOR;
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
        float4 color : COLOR;
    #endif

    #if !defined(_USEBUMPMAP_ON) && PIXEL_SHADER_USES_NORMAL
        float3 worldNormal : NORMAL;
    #endif

    #if USES_TEX_XY || defined(_NEAR_PLANE_FADE_ON)
        float3 texXYFadeZ : TEXCOORD0;
    #endif

    #if LIGHTMAP_ON
        float2 lmap : TEXCOORD1;
    #else
        float3 vertexLighting : TEXCOORD1;
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        float3 worldPos: TEXCOORD2;
    #endif

    #if defined(_USEREFLECTIONS_ON)
        float3 worldReflection : TEXCOORD3;
    #endif

    LIGHTING_COORDS(4, 5)
    UNITY_FOG_COORDS(6)
    UNITY_VERTEX_OUTPUT_STEREO
};

inline float3 FastLightingLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    return lightCol * saturate(dot(normal, lightDir));
}

inline float3 FastLightingBlinnPhong(float3 normal, float3 viewDir,
                                     float3 lightDir, float3 lightCol,
                                     
                                     float specularPower, float specularScale, float3 specularColor)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = saturate(dot(normal, h));

    return (lightCol * specularColor) * (pow(nh, specularPower) * specularScale);
}

#if defined(_USERIMLIGHTING_ON)
inline float3 RimLight(float3 worldNormal, float3 worldPos)
{
    //TODO: optimize whole function
    float rim = 1.0 - saturate(dot(worldNormal, UnityWorldSpaceViewDir(worldPos)));

    //boost to the dot product to allow for a greater max effect - .25 is the boost factor
    return smoothstep(0.25, 1.0, mad(rim, _RimPower, 0.25)) * _RimColor.rgb;
}
#endif

// normal should be normalized, w=1.0
inline float3 FastSHEvalLinearL0L1(float4 normal)
{
    return float3(dot(unity_SHAr, normal), dot(unity_SHAg, normal), dot(unity_SHAb, normal));
}

float3 FastSHEvalLinearL2(float3 normal)
{
    // 4 of the quadratic (L2) polynomials
    float4 vB = normal.xyzz * normal.yzzx;

    //TODO: normal guaranteed to be normalized so some extraneous work here, would a dp3 and add be better?
    float3 x1 = float3(dot(unity_SHBr, vB), dot(unity_SHBg, vB), dot(unity_SHBb, vB));

    // Final (5th) quadratic (L2) polynomial
    float vC = (normal.x*normal.x) - (normal.y*normal.y);
    
    return mad(unity_SHC.rgb, vC, x1);
}

inline float3 FastShadeSH9(float4 normal)
{
    float3 res = saturate(FastSHEvalLinearL0L1(normal) + FastSHEvalLinearL2(normal));

    #ifdef UNITY_COLORSPACE_GAMMA
        res = saturate(mad(1.055, pow(res, 0.416666667), -0.055));
    #endif

    return res;
}

//note that spot lights will behave as point lights with this approach
//TODO some vectors could be 3 component perhaps
float3 FastShade4PointLights(float3 pos, float3 normal)
{
    // to light vectors
    float4 toLightX = unity_4LightPosX0 - pos.x;
    float4 toLightY = unity_4LightPosY0 - pos.y;
    float4 toLightZ = unity_4LightPosZ0 - pos.z;

    //TODO: mad possibilities
    float4 ndotl = (toLightX * normal.x) + (toLightY * normal.y) + (toLightZ * normal.z);
    float4 lengthSq = (toLightX * toLightX) + (toLightY * toLightY) + (toLightZ * toLightZ);

    // correct NdotL
    ndotl = saturate(ndotl * rsqrt(lengthSq));

    // attenuation
    float4 atten = rcp(mad(lengthSq, unity_4LightAtten0, 1.0));
    float4 diff = ndotl * atten;

    // final color
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
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        o.worldPos = worldPos;
    #endif

    #if USES_TEX_XY
        o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
    #endif

    #if FLIP_NORMALS
        float3 flipCorrectedNormal = -v.normal;
    #else
        float3 flipCorrectedNormal = v.normal;
    #endif

    float3 worldNormal = UnityObjectToWorldNormal(flipCorrectedNormal);

    #if defined(LIGHTMAP_ON)
        o.lmap.xy = mad(v.lightMapUV.xy, unity_LightmapST.xy, unity_LightmapST.zw);
    #else
        //TODO: use perpixel define instead
        #if defined(_USEAMBIENT_ON) && !defined(_USEBUMPMAP_ON)
            //grab ambient color from Unity's spherical harmonics
            #if UNITY_SHOULD_SAMPLE_SH
                o.vertexLighting += FastShadeSH9(float4(worldNormal, 1.0));
            #endif
        #endif

        #if !(USE_PER_PIXEL)
            #if defined(_USEDIFFUSE_ON)
                o.vertexLighting += FastLightingLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif
            #if defined(_SPECULARHIGHLIGHTS_ON)
                o.vertexLighting += FastLightingBlinnPhong(worldNormal, UnityWorldSpaceViewDir(worldPos), _WorldSpaceLightPos0.xyz, _LightColor0.rgb, _Specular, _Gloss, _SpecColor);
            #endif
            #if defined(_SHADE4_ON)
                //handle point and spot lights
                o.vertexLighting += FastShade4PointLights(worldPos, worldNormal);
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
        float3 vertexToCamera = _WorldSpaceCameraPos - worldPos;	

        #if defined(_CALIBRATIONSPACEREFLECTIONS_ON)
            //todo: skip normalize?
            float3 normalReflection = normalize(mul((float3x3)CalibrationSpaceWorldToLocal, flipCorrectedNormal));
            vertexToCamera = mul((float3x3)CalibrationSpaceWorldToLocal, vertexToCamera);
        #else
            float3 normalReflection = flipCorrectedNormal;
        #endif
        //incident vector need not be normalized
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

fixed4 frag(v2f IN) : SV_Target
{
    #if defined(_USEMAINTEX_ON)
        float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
    #else
        float4 color = 1.0;
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
        //note execution continues but all writes are disabled
        clip(color.a - _Cutoff);
    #endif
        
    //float lightAttenuation = SHADOW_ATTENUATION(IN);
    float lightAttenuation = LIGHT_ATTENUATION(IN);
    
    //TODO: consider UnityComputeForwardShadows
    #if defined(LIGHTMAP_ON)
        float3 lightmapResult = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
        #if defined(SHADOWS_SCREEN)
            float3 lightColorShadowAttenuated = min(lightmapResult, lightAttenuation * 2.0);
        #else
            float3 lightColorShadowAttenuated = lightmapResult;
        #endif	
    #else
        float3 lightColorShadowAttenuated = IN.vertexLighting;
        #if USE_PER_PIXEL
            //if a normal map is on, it makes sense to do most calculations per-pixel
            #if defined(_USEBUMPMAP_ON)
                //unpack can be expensive if normal map is dxt5
                float3 worldNormal = UnityObjectToWorldNormal(UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.texXYFadeZ.xy)));
                //grab ambient color from Unity's spherical harmonics
                #if UNITY_SHOULD_SAMPLE_SH
                    lightColorShadowAttenuated += FastShadeSH9(float4(worldNormal, 1.0));
                #endif
            #else
                float3 worldNormal = IN.worldNormal;
            #endif					
        
            #if defined(_USEDIFFUSE_ON)
                lightColorShadowAttenuated += FastLightingLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif
            #if defined(_SPECULARHIGHLIGHTS_ON)
                float gloss = _Gloss;
                #if defined(_USEGLOSSMAP_ON)
                    gloss *= UNITY_SAMPLE_TEX2D(_GlossMap, IN.texXYFadeZ.xy).r;
                #endif
                float specular = _Specular;
                #if defined(_USESPECULARMAP_ON)
                    specular *= UNITY_SAMPLE_TEX2D(_SpecularMap, IN.texXYFadeZ.xy).r;
                #endif
                lightColorShadowAttenuated += FastLightingBlinnPhong(worldNormal, UnityWorldSpaceViewDir(IN.worldPos), _WorldSpaceLightPos0.xyz, _LightColor0.rgb, specular, gloss, _SpecColor);
            #endif
            #if defined(_SHADE4_ON)
                //handle point and spot lights
                lightColorShadowAttenuated += FastShade4PointLights(IN.worldPos, worldNormal);
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