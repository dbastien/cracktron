#ifndef GAMMA_CORRECTION
#define GAMMA_CORRECTION

#include "FastMath.cginc"

//https://www.wolframalpha.com/input/?i=Plot%5B%7B(1.055+*+x+%5E+0.416666667+-+0.055),+(x*(1%2Fsqrt(x))),+(1.055*(x+%5E+0.416666667)-0.055)%7D,%7Bx,0,1%7D%5D
inline float3 LinearToSRGBTaylor(float3 color)
{
    return color * TaylorRsqrt(color);
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

#endif //GAMMA_CORRECTION