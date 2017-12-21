
#ifndef IMAGE_PROCESSING
#define IMAGE_PROCESSING

#include "./FastMath.cginc"


inline float3 Saturation(float3 col, float amount)
{
    //historical luma weighting
    const float3 lumaScale = float3(0.3, 0.59, 0.11);
 
    float g = dot(lumaScale, col);

    return lerp(g.rrr, col, amount);
}

inline float3 SaturationUnweighted(float3 col, float amount)
{
    float g = (col.r + col.g + col.b) * ONE_DIV3;

    return lerp(g.rrr, col, amount);
}

inline float Bilinear(float v_00, float v_10, float v_01, float v_11, float2 t)
{
    //actually compiles down better than most more confusing forms
    return ((v_00 * (1-t.x) + v_10 * t.x) * (1-t.y)) +
           ((v_01 * (1-t.x) + v_11 * t.x) * t.y);
}

inline float Bilinear(float4 v, float2 t)
{
    return Bilinear(v.x, v.y, v.z, v.w, t);
}

#endif //IMAGE_PROCESSING