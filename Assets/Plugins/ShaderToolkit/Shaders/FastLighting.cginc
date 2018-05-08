#ifndef FAST_LIGHTING
#define FAST_LIGHTING

#include "./GammaCorrection.cginc"

inline float4 PreMultiplyAlphaFast(float4 color)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return float4(color.rgb * color.a, color.a);
#endif
    return color;
}

inline float4 PreMultiplyAlphaWithReflectivityFast(float4 color, float oneMinusReflectivity)
{
#if defined(_ALPHAPREMULTIPLY_ON)
    //relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    return float4(color.rgb * color.a, 1 - oneMinusReflectivity + color.a * oneMinusReflectivity);
#endif
    return color;
}

// normal should be normalized, w=1.0
// linear + constant terms
inline float3 SHEvalLinearL0L1Fast(float4 normal)
{
    // TODO: pass up sh data in srgb as well and switch on UNITY_COLORSPACE_GAMMA for perf
    return float3(dot(unity_SHAr, normal), dot(unity_SHAg, normal), dot(unity_SHAb, normal));
}

float3 SHEvalLinearL2Fast(float3 normal)
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
inline float3 ShadeSH9Fast(float4 normal)
{
    float3 res = sat(SHEvalLinearL0L1Fast(normal) + SHEvalLinearL2Fast(normal));

    #ifdef UNITY_COLORSPACE_GAMMA
        //res = LinearToSRGBMAD(res);
        res = LinearToSRGBChilliant(res);
    #endif

    return res;
}

float3 Shade4PointLightsOriginal(float3 pos, float3 normal)
{
    return Shade4PointLights
    (
        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
        unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
        unity_4LightAtten0, pos, normal
    );
}

// lambertian lighting for point and spot lights
// note that spot lights will behave as point lights, LIGHT_ATTENUATION() is key
float3 Shade4PointLightsFast(float3 pos, float3 normal)
{
    // to light vectors
    float4 toLightX = unity_4LightPosX0 - pos.x;
    float4 toLightY = unity_4LightPosY0 - pos.y;
    float4 toLightZ = unity_4LightPosZ0 - pos.z;

    //TODO: verify generates mad instructions
    float4 ndotl = (toLightX * normal.x) + (toLightY * normal.y) + (toLightZ * normal.z);
    float4 lengthSq = (toLightX * toLightX) + (toLightY * toLightY) + (toLightZ * toLightZ);

    //don't produce NaNs if some vertex position overlaps with the light   
    lengthSq = max(lengthSq, 0.000001);

    // correct NdotL
    float4 corr = rsqrt(lengthSq);
    //ndotl = max(float4(0,0,0,0), ndotl * corr);
    ndotl = sat(ndotl * corr);

    // attenuation
    float4 atten = rcp(mad(lengthSq, unity_4LightAtten0, 1.0));
    float4 diff = ndotl * atten;

    // final color
    return (unity_LightColor[0].rgb * diff.x) + (unity_LightColor[1].rgb * diff.y) + 
           (unity_LightColor[2].rgb * diff.z) + (unity_LightColor[3].rgb * diff.w);
}

inline float3 RimLight(float3 worldPos, float3 worldNormal, float rimPower, float3 rimColor)
{
    float3 vn = normalize(UnityWorldSpaceViewDir(worldPos));
    float3 nn = worldNormal;

    //TODO: optimize whole function
    float rim = 1.0 - dot_sat(nn, vn);

    //boost to the dot product to allow for a greater max effect - .25 is the boost factor
    return rim * rimPower * rimColor;
} 

inline float3 DiffuseLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    return lightCol * dot_sat(normal, lightDir);
}

//Real world specular can realistically be 0.1 to > 100000
//Do some non-linear range mapping
float SpecularRangeRemapShaderForge(float specular) { return exp2(specular*10+1); }
//For linear mappings, I've seen 16, 256, and 512 used as coefficients
float SpecularRangeRemapUnity(float specular) { return specular*256.0; }

inline float SpecularBlinnPhong(float3 normal, float3 viewDir, float3 lightDir,
                                float specular, float gloss)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = dot_sat(normal, h);
    return pow(nh, specular) * gloss;
}

inline float3 SpecularPhong(float3 normal, float3 viewDir, float3 lightDir,
                            float specular, float gloss)
{
    float p = dot_sat(-viewDir, reflect(lightDir, normal));
    return pow(p, specular) * gloss;
}

//http://page.mi.fu-berlin.de/block/htw-lehre/wise2012_2013/bel_und_rend/skripte/schlick1994.pdf
inline float3 SpecularSchlick(float3 normal, float3 viewDir, float3 lightDir,
                              float specular, float gloss)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = dot_sat(normal, h);
    //float schlickTerm = rcp((specularPower/nh)-specularPower+1);    
    return (nh / (specular - (specular * nh) + nh)) * gloss;
}

#endif //FAST_LIGHTING