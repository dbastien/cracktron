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

inline float4 FastPreMultiplyAlphaWithReflectivity(float4 color, float oneMinusReflectivity)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return float4(color.rgb * color.a, 1 - oneMinusReflectivity + color.a * oneMinusReflectivity);
#endif
    return color;
}

#endif //SHADER_COMMON