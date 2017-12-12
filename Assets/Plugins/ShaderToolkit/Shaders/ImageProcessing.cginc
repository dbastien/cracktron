
#ifndef IMAGE_PROCESSING
#define IMAGE_PROCESSING

#include "./FastMath.cginc"

float4 LinearSamplingGaussian3x3(Texture2D tex, SamplerState samp, float2 uv, float2 texelSize)
{
    //does a 9 tap gaussian with only 4 samples by taking advantage of hardware filtering

    //filter weighting:
    //1 2 1
    //2 4 2
    //1 2 1

    float2 offset = texelSize * 0.5;

    float2 ul = float2(-offset.x, -offset.y);
    float2 ur = float2( offset.x, -offset.y);
    float2 bl = float2(-offset.x,  offset.y);
    float2 br = float2( offset.x,  offset.y);

    float4 col;
    col  = tex.Sample(samp, uv + ul);
    col += tex.Sample(samp, uv + ur);
    col += tex.Sample(samp, uv + bl);
    col += tex.Sample(samp, uv + br);
    col *= 0.25;

    return col;
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