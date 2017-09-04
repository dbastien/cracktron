// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#ifndef SHADER_COMMON
#define SHADER_COMMON

#define sat(x) ( saturate((x)) )
#define mad_sat(m,a,v) ( saturate( mad((m), (a), (v)) ) )
#define dot_sat(x,y) ( saturate( dot((x), (y)) ) )

#define pow2(x) ( (x)*(x) )
#define pow3(x) ( (x)*(x)*(x) )
#define pow4(x) ( (x)*(x)*(x)*(x) )
#define pow5(x) ( (x)*(x)*(x)*(x)*(x) )

//taylor inverse square root in mad() form
#define taylorrsqrt(x) ( mad(-0.85373472095314, (x), 1.79284291400159) )

#define remap01(x, minX, maxX) ( ((x) - (minX)) / ((maxX) - (minX)) )
#define remap(x, minIn, maxIn, minOut, maxOut) ( (minOut) + ((maxOut) - (minOut)) * remap01((x), (minIn), maxIn)) )
#define smootherstep01(x) ( ((x) * (x) * (x)) * ((x) * ((x) * 6 - 15) + 10) )

float4 _NearPlaneFadeDistance;

// Helper function for focal plane fading
// Could instead be done non-linear in projected space for speed
inline float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -UnityObjectToViewPos(vertex).z;
    return mad_sat(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x);
}

// normal should be normalized, w=1.0
// linear + constant terms
inline float3 FastSHEvalLinearL0L1(float4 normal)
{
    // TODO: pass up sh data in srgb as well and switch on UNITY_COLORSPACE_GAMMA for perf
    return float3(dot(unity_SHAr, normal), dot(unity_SHAg, normal), dot(unity_SHAb, normal));
}

float3 FastSHEvalLinearL2(float3 normal)
{
    // 4 of the quadratic (L2) polynomials
    float4 vB = normal.xyzz * normal.yzzx;

    // TODO: normal guaranteed to be normalized so some extraneous work here, maybe switch to dp3 and add or something more clever?
    // TODO: pass up sh data in srgb as well and switch on UNITY_COLORSPACE_GAMMA for perf
    float3 x1 = float3(dot(unity_SHBr, vB), dot(unity_SHBg, vB), dot(unity_SHBb, vB));

    // Final (5th) quadratic (L2) polynomial
    float vC = (normal.x*normal.x) - (normal.y*normal.y);

    return mad(unity_SHC.rgb, vC, x1);
}

// http://www.ppsloan.org/publications/StupidSH36.pdf
inline float3 FastShadeSH9(float4 normal)
{
    #if defined(_USEAMBIENT_ON) && UNITY_SHOULD_SAMPLE_SH
        float3 res = saturate(FastSHEvalLinearL0L1(normal) + FastSHEvalLinearL2(normal));

        #ifdef UNITY_COLORSPACE_GAMMA
            // http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html
            // linear to srgb    
            res = mad_sat(1.055, pow(res, 0.416666667), -0.055);
        #endif

        return res;
    #else
        return float3(0, 0, 0);
    #endif
}

// note that spot lights will behave as point lights, LIGHT_ATTENUATION() is key
// TODO: some vectors could be 3 component perhaps
float3 FastShade4PointLights(float3 pos, float3 normal)
{
    #if defined(_SHADE4_ON)
        // to light vectors
        float4 toLightX = unity_4LightPosX0 - pos.x;
        float4 toLightY = unity_4LightPosY0 - pos.y;
        float4 toLightZ = unity_4LightPosZ0 - pos.z;

        //TODO: verify generates mad instructions
        float4 ndotl = (toLightX * normal.x) + (toLightY * normal.y) + (toLightZ * normal.z);
        float4 lengthSq = (toLightX * toLightX) + (toLightY * toLightY) + (toLightZ * toLightZ);

        // correct NdotL
        ndotl = saturate(ndotl * rsqrt(lengthSq));

        // attenuation
        float4 atten = rcp(mad(lengthSq, unity_4LightAtten0, 1.0));
        float4 diff = ndotl * atten;

        // final color
        return (unity_LightColor[0].rgb * diff.x) + (unity_LightColor[1].rgb * diff.y) + (unity_LightColor[2].rgb * diff.z) + (unity_LightColor[3].rgb * diff.w);
    #else
        return float3(0, 0, 0);
    #endif
}

inline float3 FastLightingLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    #if defined(_USEDIFFUSE_ON)
        return lightCol * saturate(dot(normal, lightDir));
    #else
        return float3(0, 0, 0);
    #endif        
}

inline float3 FastLightingBlinnPhong(float3 normal, float3 viewDir,
                                     float3 lightDir, float3 lightCol,
                                     float specularPower, float specularScale, float3 specularColor)
{
    #if defined(_SPECULARHIGHLIGHTS_ON)
        float3 h = normalize(lightDir + viewDir);
        float nh = saturate(dot(normal, h));

        return (lightCol * specularColor) * pow(nh, specularPower) * specularScale;
    #else
        return float3(0, 0, 0);
    #endif
}

inline float4 FastPreMultiplyAlpha(float4 color)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return float4(color.rgb * color.a, color.a);
#endif
    return color;
}

inline float3 FastPreMultiplyAlphaWithReflectivity(float3 diffColor, float alpha, float oneMinusReflectivity, out float outModifiedAlpha)
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

inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
}

float random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

#endif //SHADER_COMMON