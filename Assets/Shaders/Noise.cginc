#ifndef NOISE
#define NOISE

#define Mod289(x) ( mad(-floor((x) * (1.0 / 289.0)), 289.0, (x)) )

//TODO: consider switching more functionss to macros
//      to easily perfomantly support half, min16float, etc.
inline float Permute(float v) 
{
    return Mod289(mad(pow2(v), 34.0, v));
}

inline float Permute2D(float2 v)
{
    return Permute(v.y + Permute(v.x));
}

inline float Permute3D(float3 v)
{
    return Permute(v.z + Permute2D(v.xy));
}

inline float Permute4D(float4 v)
{
    return Permute(v.w + Permute3D(v.xyz));
}

//TODO: rand functions relying on trig may run into hardware implementation variability

//range [0,1)
inline float Rand(float seed)
{
    return frac(sin(seed) * 43758.5453123);
}

//range [0,1)
//everyone's favorite psuedo-random number generator
//http://www.wolframalpha.com/input/?i=plot(+mod(+sin(x*12.9898+%2B+y*78.233)+*+43758.5453123,1)x%3D0..1,+y%3D0..1)
inline float Rand(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453123);
}

//range [0,1)
inline float2 Rand2D(float2 seed)
{
    return float2(Rand(seed), Rand(seed*2));
}

float4 QuadraticGrad(float2 v)
{
    //gather 4 samples - 1 at each point of the quad   
    return float4
    (
        Rand(v),
        Rand(v + float2(1.0, 0.0)),
        Rand(v + float2(0.0, 1.0)),
        Rand(v + float2(1.0, 1.0))
    );
}

// low complexity, decent perf, OK quality
float QuadraticNoise(float2 v)
{
    float2 v_floor = floor(v);
    float2 v_frac = v - v_floor;

    float4 c = QuadraticGrad(v_floor);

    //smooth the interpolation value
    float2 t = SmootherStep01(v_frac);

    return Bilinear(c, t);
}

//TODO: consider using hack-y interfaces/function pointers
//      http://code4k.blogspot.com/2011/11/advanced-hlsl-using-closures-and.html
#define NOISE2D_FUNC(x) QuadraticNoise((x))

inline float FBMNoiseStep
(
    in float2 v,
    inout float freq,
    inout float amplitude,
    in float gain,
    in float lacunarity,
    inout float range
)
{
    float stepResult = NOISE2D_FUNC(v*freq) * amplitude;    
    freq *= lacunarity;
    range += amplitude;
    amplitude *= gain;
    return stepResult;
}

float FBMNoise(float2 v, float freq, float gain, float lacunarity, float octaves)
{
    float amplitude = 1.0;
    float result = 0.0f;
    float range = 0.0f;
    
    [unroll]
    for (float i = 0; i < octaves; ++i)
    {
        result += FBMNoiseStep(v, freq, amplitude, gain, lacunarity, range);
    }

    return result/range;
}

float Turbulence(float2 v,
                 float freq, float gain, float lacunarity, float whirl,
                 float octaves)
{
    float amplitude = 1.0;
    float result = 0.0f;
    float range = 0.0f;
    
    [unroll]
    for (float i = 0; i < octaves; ++i)
    {
        float n = FBMNoiseStep(v, freq, amplitude, gain, lacunarity, range);

        //move in a predictable way to add some patterning
        //create a cheap normal-esque direction for a given n
        //then inversely scale by current frequency
        float2 offset = float2(n, 1-n) * rcp(freq*whirl);

        v += offset;
        result += n;
    }

    //normalize result
    return result/range;
}

#endif //NOISE