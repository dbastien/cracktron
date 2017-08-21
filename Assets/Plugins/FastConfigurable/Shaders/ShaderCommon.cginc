// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#ifndef SHADER_COMMON
#define SHADER_COMMON

float4 _NearPlaneFadeDistance;

// Helper function for focal plane fading
// Could instead be done non-linear in projected space for speed
inline float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -UnityObjectToViewPos(vertex).z;
    return saturate(mad(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x));
}

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

    return (lightCol * specularColor) * pow(nh, specularPower) * specularScale;
}

inline float4 FastConfigurablePreMultiplyAlpha(float4 color)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return float4(color.rgb * color.a, color.a);
#endif
    return color;
}

inline float3 FastConfigurablePreMultiplyAlphaWithReflectivity(float3 diffColor, float alpha, float oneMinusReflectivity, out float outModifiedAlpha)
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

#endif //SHADER_COMMON