#ifndef FAST_MATH
#define FAST_MATH

// macros used in lieu of functions often for multiple type support - float, float2, etc.

// constants
// 32 bit float is 6 digits of safe precision, 64 is 15

//irrationals
#define TAU          6.283185307179586
#define TAU_DIV2     3.141592653589793
#define TAU_DIV4     1.570796326794897
#define TAU_DIV8     0.785398163397448
#define TAU_RCP      0.159154943091895
#define TAU_DIV2_RCP 0.318309886183791
#define GOLDENRATIO  1.618033988749895
#define SQRTPI       1.772453850905516
#define SQRT2        1.414213562373095
#define SQRT3        1.732050807568877

//reciprocals
#define ONE_DIV3     0.333333333333333
#define ONE_DIV6     0.166666666666667
#define ONE_DIV7     0.142857142857143
#define ONE_DIV9     0.111111111111111

//limits
#define U64_MAX 18446744073709551615
#define S64_MIN -9223372036854775808
#define S64_MAX 9223372036854775807

#define U32_MAX 4294967295u
#define S32_MIN -2147483648
#define S32_MAX 2147483647

#define U16_MAX 65535u
#define S16_MIN -32768
#define S16_MAX 32767

#define U8_MAX 255
#define S8_MIN –128
#define S8_MAX 127

#define F64_EPSILON 2.2204460492503131e–016
#define F64_MIN     2.2250738585072014e–308
#define F64_MAX     1.7976931348623158e+308
#define F64_DIG     15

#define F32_MAX     3.402823466e+38F
#define F32_MIN     1.175494351e–38F
#define F32_EPSILON 1.192092896e–07F
#define F32_DIG     6 


// TODO: defines for fullp and minp so they can be set per platform
//       half is float on desktop, etc.
//       min16float always behaves as expected,
//       but not all desktop GPUs support and perf is crippled to 1/64th rate on others
//       half would be safe, however for mobile ogl es3+ targets half will be float as well

// shorthand
#define sat(x)         ( saturate((x)) )
#define mad_sat(m,a,v) ( saturate( mad((m), (a), (v)) ) )
#define dot_sat(x,y)   ( saturate( dot((x), (y)) ) )

#define min3(x,y,z) ( min( min((x), (y)) , (z) )  )

#define pow2(x) ( (x)*(x) )
#define pow3(x) ( (x)*(x)*(x) )
#define pow4(x) ( (x)*(x)*(x)*(x) )
#define pow5(x) ( (x)*(x)*(x)*(x)*(x) )

// taylor inverse square root in mad() form
// mileage varies, but half the latency of rsqrt on some platforms
// use with extreme care outside of input domain of [0,1]
// https://www.wolframalpha.com/input/?i=Plot%5B%7B(1%2Fsqrt(x)),+(-0.96+*+x+%2B+1.92)%7D,+-1..2%5D
#define Taylor01Rsqrt(x) ( mad(-0.85373472095314, (x), 1.79284291400159) )
#define Fast01Rsqrt(x) ( mad(-0.96, (x), 1.92) )

#define RSQRT01_FUNC(x) ( Fast01Rsqrt((x)) )

#define Remap01(x, minX, maxX) ( ((x) - (minX)) / ((maxX) - (minX)) )
#define Remap(x, minIn, maxIn, minOut, maxOut) ( (minOut) + ((maxOut) - (minOut)) * Remap01((x), (minIn), maxIn)) )

#define RemapNormF32ToU32(x) ( uint((x)*U32_MAX) )
#define RemapU32ToNormF32(x) ( float((x))/U32_MAX ) 

// maps from [0,1] to [-1,1]
#define Remap01Center(x) ( ((x) - 0.5) * 2.0 )

// maps from [-1,1] to [0,1]
#define RemapCenter01(x) ( ((x) + 1.0) * 0.5 )

// maps from [0,1] to [-5,.5]
#define Remap01CenterHalf(x) ( (x) - 0.5 )

// https://www.wolframalpha.com/input/?i=Plot%5Bt*t*(3-2*t),+0..1%5D
#define SmoothStep01(x) ( (x) * (x) * (3.0 - 2.0 * (x)) )

#define SmootherStep01(x) ( ((x) * (x) * (x)) * ((x) * ((x) * 6 - 15) + 10) )

// https://www.wolframalpha.com/input/?i=Plot%5Bt+*+t,+0..1%5D
#define QuadInEase01(x) ( pow2((x)) )

// https://www.wolframalpha.com/input/?i=Plot%5B-(t+*+(t+-+2)),+0..1%5D
#define QuadOutEase01(x) ( -( (x) * ((x)-2.0) ) )

// https://www.wolframalpha.com/input/?i=Plot%5Bt+*+t+*+t,+0..1%5D
#define CubicInEase01(x) ( pow3((x)) )

#define CubicOutEase01(x) ( pow3((x)-1.0) + 1.0 )

inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
}

// Interleaved gradient function from CoD AW.
// http://www.iryoku.com/next-generation-post-processing-in-call-of-duty-advanced-warfare
inline float InterleavedGradient(float2 uv)
{
	const float3 magic = float3(0.06711056, 0.00583715, 52.9829189);
	return frac(magic.z * frac(dot(uv, magic.xy)));
}

inline float OrderedDither(float2 uv, float2 texelSize)
{
	return InterleavedGradient(uv / texelSize) / 255;
}
 
#endif //FAST_MATH