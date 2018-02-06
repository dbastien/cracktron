#ifndef GAMMA_CORRECTION
#define GAMMA_CORRECTION

#include "FastMath.cginc"

//linear to srgb space intersection points
//of the linear and curved components of the function
//http://entropymine.com/imageworsener/srgbformula/
#define LTSI_OFFICIAL 0.00031308
#define LTSI_IMPROVED 0.00313066844250063

#define STLI_OFFICIAL 0.04045
#define STLI_IMPROVED 0.0404482362771082

#define LTSI LTSI_IMPROVED
#define STLI STLI_IMPROVED

//https://www.wolframalpha.com/input/?i=Plot%5B%7B(1.055+*+x+%5E+0.416666667+-+0.055),+(x*(1%2Fsqrt(x))),+(1.055*(x+%5E+0.416666667)-0.055)%7D,%7Bx,0,1%7D%5D
inline float3 LinearToSRGBTaylor(float3 color)
{
    return color * sat(Taylor01Rsqrt(color));
}

// same number of operations as TaylorRsqrt but ~33% more accurate for the domain [0,1]
inline float3 LinearToSRGBMAD(float3 color)
{
    return color * sat(Fast01Rsqrt(color));
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
	return (color <= LTSI) ? 12.92 * color : 1.055 * pow(color, 1.0 / 2.4) - 0.055;
}

#endif //GAMMA_CORRECTION