inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
}

float remap01(float x, float minIn, float maxIn)
{
    return (x - minIn) / (maxIn - minIn);
}

float remap(float x, float minIn, float maxIn, float minOut, float maxOut)
{
    return minOut + (maxOut - minOut) * remap01(x, minIn, maxIn);
}

float4 remap01(float4 x, float4 minIn, float4 maxIn)
{
    return (x - minIn) / (maxIn - minIn);
}

float4 remap(float4 x, float4 minIn, float4 maxIn, float4 minOut, float4 maxOut)
{
    return minOut + (maxOut - minOut) * remap01(x, minIn, maxIn);
}

float smootherstep01(float x)
{
    return x*x*x*(x*(x * 6 - 15) + 10);
}

float Random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}