#ifndef FAST_MATH
#define FAST_MATH

//macros are used in lieu of functions often for multiple type support - float, float2, etc.

#define sat(x) ( saturate((x)) )
#define mad_sat(m,a,v) ( saturate( mad((m), (a), (v)) ) )
#define dot_sat(x,y) ( saturate( dot((x), (y)) ) )

#define pow2(x) ( (x)*(x) )
#define pow3(x) ( (x)*(x)*(x) )

//todo: verify compiler optimizes to putting x*x into a temp register then further multiplies using that
#define pow4(x) ( (x)*(x)*(x)*(x) )
#define pow5(x) ( (x)*(x)*(x)*(x)*(x) )

//taylor inverse square root in mad() form
//mileage varies, but half the latency of rsqrt on some platforms
//https://www.wolframalpha.com/input/?i=Plot%5B+%7B(1.055+*+x+%5E+0.416666667+-+0.055),+(x*(1%2Fsqrt(x)))%7D,+%7Bx,0,1%7D+%5D
#define taylorrsqrt(x) ( mad(-0.85373472095314, (x), 1.79284291400159) )

#define remap01(x, minX, maxX) ( ((x) - (minX)) / ((maxX) - (minX)) )
#define remap(x, minIn, maxIn, minOut, maxOut) ( (minOut) + ((maxOut) - (minOut)) * remap01((x), (minIn), maxIn)) )
#define smootherstep01(x) ( ((x) * (x) * (x)) * ((x) * ((x) * 6 - 15) + 10) )

// noise
#define mod289(x) ( (x) - floor((x) * (1.0 / 289.0)) * 289.0 )
#define permute(x) ( mod289((((x) * 34.0) + 1.0) * (x)) )

inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
}

float random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

//https://www.wolframalpha.com/input/?i=Plot%5B%7B(1.055+*+x+%5E+0.416666667+-+0.055),+(x*(1%2Fsqrt(x))),+(1.055*(x+%5E+0.416666667)-0.055)%7D,%7Bx,0,1%7D%5D
inline float3 LinearToSRGBTaylor(float3 color)
{
    return color * taylorrsqrt(color);
}

inline float3 LinearToSRGBChilliant(float3 color)
{
    // http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html
    // linear to srgb
    // this is the method unity uses (although not explicitly in mad() form)
    // essentially the official conversion but non-piecewise - omitting the small linear component
    return mad_sat(1.055, pow(color, 1.0 / 2.4), -0.055);
}

inline float3 LinearToSRGBOfficial(float3 color)
{
    //sometimes incorrectly implemented as col < instead of <=
	return (color <= 0.0031308) ? 12.92 * color : 1.055 * pow(color, 1.0 / 2.4) - 0.055;
}

//http://entropymine.com/imageworsener/srgbformula/
inline float3 LinearToSRGBImproved(float3 color)
{
	return (color <= 0.00313066844250063) ? 12.92 * color : 1.055 * pow(color, 1.0 / 2.4) - 0.055;
}

#endif //FAST_MATH