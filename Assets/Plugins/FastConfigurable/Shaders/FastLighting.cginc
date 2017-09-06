#ifndef FAST_LIGHTING
#define FAST_LIGHTING

// normal should be normalized, w=1.0
// linear + constant terms
inline float3 FastSHEvalLinearL0L1(float4 normal)
{
    // TODO: pass up sh data in srgb as well and switch on UNITY_COLORSPACE_GAMMA for perf
    return float3(dot(unity_SHAr, normal), dot(unity_SHAg, normal), dot(unity_SHAb, normal));
}

float3 FastSHEvalLinearL2(float3 normal)
{
    // 4 of the quadratic (L2) polynomials
    float4 vB = normal.xyzz * normal.yzzx;

    // TODO: normal guaranteed to be normalized so some extraneous work here, maybe switch to dp3 and add or something more clever?
    // TODO: pass up sh data in srgb as well and ifdef on UNITY_COLORSPACE_GAMMA to decide which to use for perf
    float3 x1 = float3(dot(unity_SHBr, vB), dot(unity_SHBg, vB), dot(unity_SHBb, vB));

    // Final (5th) quadratic (L2) polynomial
    float vC = (normal.x*normal.x) - (normal.y*normal.y);

    return mad(unity_SHC.rgb, vC, x1);
}

// http://www.ppsloan.org/publications/StupidSH36.pdf
inline float3 FastShadeSH9(float4 normal)
{
    float3 res = sat(FastSHEvalLinearL0L1(normal) + FastSHEvalLinearL2(normal));

    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGBTaylor(res);
    #endif

    return res;
}

// lambertian lighting for point and spot lights
// note that spot lights will behave as point lights, LIGHT_ATTENUATION() is key
float3 FastShade4PointLights(float3 pos, float3 normal)
{
    // to light vectors
    float4 toLightX = unity_4LightPosX0 - pos.x;
    float4 toLightY = unity_4LightPosY0 - pos.y;
    float4 toLightZ = unity_4LightPosZ0 - pos.z;

    //TODO: verify generates mad instructions
    float4 ndotl = (toLightX * normal.x) + (toLightY * normal.y) + (toLightZ * normal.z);
    float4 lengthSq = (toLightX * toLightX) + (toLightY * toLightY) + (toLightZ * toLightZ);

    // correct NdotL
    // use taylor multiplicative inverse square root approximation
    ndotl = sat(ndotl * taylorrsqrt(lengthSq));
    //ndotl = sat(ndotl * rsqrt(lengthSq));

    // attenuation
    float4 atten = rcp(mad(lengthSq, unity_4LightAtten0, 1.0));
    float4 diff = ndotl * atten;

    // final color
    return (unity_LightColor[0].rgb * diff.x) + (unity_LightColor[1].rgb * diff.y) + (unity_LightColor[2].rgb * diff.z) + (unity_LightColor[3].rgb * diff.w);
}

inline float3 FastLightingDiffuseLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    return lightCol * dot_sat(normal, lightDir);
}

inline float3 FastLightingSpecularBlinnPhong(float3 normal, float3 viewDir,
                                             float3 lightDir, float3 lightCol,
                                             float specularPower, float specularScale, float3 specularColor)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = dot_sat(normal, h);
    float blinnTerm = pow(nh, specularPower);

    return (lightCol * specularColor) * blinnTerm * specularScale;
}

//http://page.mi.fu-berlin.de/block/htw-lehre/wise2012_2013/bel_und_rend/skripte/schlick1994.pdf
inline float3 FastLightingSpecularSchlick(float3 normal, float3 viewDir,
                                          float3 lightDir, float3 lightCol,
                                          float specularPower, float specularScale, float3 specularColor)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = dot_sat(normal, h);
    float schlickTerm = nh / (specularPower - (specularPower * nh) + nh);

    return (lightCol * specularColor) * schlickTerm * specularScale;
}

#endif //FAST_LIGHTING