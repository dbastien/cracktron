#ifndef SHADER_COMMON
#define SHADER_COMMON

#include "FastMath.cginc"

float4 _NearPlaneFadeDistance;

// Helper function for focal plane fading
// Could instead be done non-linear in projected space for speed
inline float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -UnityObjectToViewPos(vertex).z;
    return mad_sat(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x);
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

#endif //SHADER_COMMON