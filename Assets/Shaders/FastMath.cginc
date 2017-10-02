#ifndef FAST_MATH
#define FAST_MATH

// macros used in lieu of functions often for multiple type support - float, float2, etc.

// constants
// 32 bit float is 6 digits of safe precision
#define TAU_F32 6.28319
#define TAU_DIV2_F32 3.14159
#define TAU_DIV4_F32 1.57080

// TODO: defines for fullp and minp so they can be set per platform
//       half is float on desktop, etc.
//       min16float always behaves as expected,
//       but not all desktop GPUs support and perf is crippled to 1/64th rate on others
//       half would be save, however for mobile ogl es3+ targets half will be float as well

// shorthand
#define sat(x) ( saturate((x)) )
#define mad_sat(m,a,v) ( saturate( mad((m), (a), (v)) ) )
#define dot_sat(x,y) ( saturate( dot((x), (y)) ) )

#define pow2(x) ( (x)*(x) )
#define pow3(x) ( (x)*(x)*(x) )
#define pow4(x) ( (x)*(x)*(x)*(x) )
#define pow5(x) ( (x)*(x)*(x)*(x)*(x) )

// taylor inverse square root in mad() form
// mileage varies, but half the latency of rsqrt on some platforms
// depending on domain, better approximations in the same form can be found
// https://www.wolframalpha.com/input/?i=Plot%5B+%7B(1.055+*+x+%5E+0.416666667+-+0.055),+(x*(1%2Fsqrt(x)))%7D,+%7Bx,0,1%7D+%5D
#define TaylorRsqrt(x) ( mad(-0.85373472095314, (x), 1.79284291400159) )

#define Remap01(x, minX, maxX) ( ((x) - (minX)) / ((maxX) - (minX)) )
#define Remap(x, minIn, maxIn, minOut, maxOut) ( (minOut) + ((maxOut) - (minOut)) * Remap01((x), (minIn), maxIn)) )

// maps from [0,1] to [-1,1]
#define Remap01Center(x) ( ((x) - 0.5) * 2.0 )

// https://www.wolframalpha.com/input/?i=Plot%5B+t*t*(3-2*t),+0..1+%5D
#define SmoothStep01(x) ( (x) * (x) * (3.0 - 2.0 * (x)) )

// https://www.wolframalpha.com/input/?i=Plot%5Bt*t*t+*+(t+*+(t+*+6+-+15)+%2B+10),+0..1+%5D
#define SmootherStep01(x) ( ((x) * (x) * (x)) * ((x) * ((x) * 6 - 15) + 10) )

// https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+*+t+%7D,+%7Bt,0,1%7D+%5D
#define QuadEase01(x) ( pow2((x)) )

// https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D-(t+*+(t+-+2))+%7D,+%7Bt,0,1%7D+%5D
#define QuadOutEase01(x) ( -( (x) * ((x)-2.0) ) )

// https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+*+t+*+t+%7D,+%7Bt,0,1%7D+%5D
#define CubicEase01(x) ( pow3((x)) )

// https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D(t-1)%5E3+%7D,+%7Bt,0,1%7D+%5D
#define CubicOutEase01(x) ( pow3((x)-1.0) + 1.0 )

inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
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

#endif //FAST_MATH