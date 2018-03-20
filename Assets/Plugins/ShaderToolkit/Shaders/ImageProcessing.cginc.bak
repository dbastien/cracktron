#ifndef IMAGE_PROCESSING
#define IMAGE_PROCESSING

#include "./FastMath.cginc"

//https://www.wikiwand.com/en/Blend_modes
inline float3 BlendSoftLight(float3 a, float3 b) { return (1-2*b)*a*a + 2*b*a; }

//darken modes
inline float3 BlendMultiply(float3 a, float3 b) { return a * b; }
inline float3 BlendColorBurn(float3 a, float3 b) { return 1 - (1-b)/a; }
inline float3 BlendLinearBurn(float3 a, float3 b) { return a + b - 1; }
inline float3 BlendDarkenOnly(float3 a, float3 b) { return min(a,b); }

//lighten modes
inline float3 BlendScreen(float3 a, float3 b) { return 1 - ((1-a)*(1-b)); }
inline float3 BlendColorDodge(float3 a, float3 b) { return b / (1-a); }
inline float3 BlendLinearDodge(float3 a, float3 b) { return a + b; }
inline float3 BlendLightenOnly(float3 a, float3 b) { return max(a,b); }

//cancellation modes
inline float3 BlendSubtract(float3 a, float3 b) { return b - a; }
inline float3 BlendDivide(float3 a, float3 b) { return b / a; }

inline float3 Brightness(float3 col, float amount)
{
    return col + amount;
}

inline float3 Contrast(float3 col, float amount)
{
    return col - amount * (col - 1.0) * col * (col - 0.5);
}

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