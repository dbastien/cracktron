#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "./FastMath.cginc"
#include "./FastLighting.cginc"
#include "./TextureMacro.cginc"

#define USES_MAIN_UV (_USEMAINTEX_ON || _USEEMISSIONMAP_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON)

//todo: share world view dir

float4 _Color;
UNITY_DECLARE_TEX2D(_MainTex);

UNITY_DECLARE_TEX2D(_BumpMap);

float _Gloss;
float _Specular;
UNITY_DECLARE_TEX2D(_SpecularMap);

UNITY_DECLARE_TEXCUBE(_CubeMap);
float _ReflectionScale;

float _RimPower;
float4 _RimColor;

float4 _EmissionColor;
UNITY_DECLARE_TEX2D(_EmissionMap);

float _Cutoff;
float4 _TextureScaleOffset;

struct a2v
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;

    #if defined(_USEBUMPMAP_ON)
        float4 tangent : TANGENT;
    #endif

    #if defined(_USEVERTEXCOLOR_ON)
        float4 color : COLOR;
    #endif

    #if USES_MAIN_UV
        float2 mainUV : TEXCOORD0;
    #endif

    #if LIGHTMAP_ON
        float2 lightMapUV : TEXCOORD1;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 pos : SV_POSITION;

    #if defined(_USEVERTEXCOLOR_ON)
        float4 color : COLOR;
    #endif

    float3 normal : NORMAL;

    #if defined(_USEBUMPMAP_ON)
        float3 tangent : TEXCOORD0;
        float3 bitangent : TEXCOORD1;
    #endif

    #if USES_MAIN_UV
        float2 mainUV : TEXCOORD2;
    #endif

    #if LIGHTMAP_ON
        float2 lmap : TEXCOORD3;
    #else
        float3 vertexLighting : TEXCOORD3;
    #endif

    float3 worldPos: TEXCOORD4;

    #if defined(_USEREFLECTIONS_ON)
        float3 worldReflection : TEXCOORD5;
    #endif

    UNITY_LIGHTING_COORDS(7,8)
    UNITY_FOG_COORDS(9)
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert(a2v v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.pos = UnityObjectToClipPos(v.vertex);

    #if defined(_USEVERTEXCOLOR_ON)
        #if defined(_USEMAINCOLOR_ON)
            o.color = v.color * _Color;
        #else
            o.color = v.color;
        #endif
    #endif

    float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.worldPos = worldPos;

    #if USES_MAIN_UV
        o.mainUV.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
    #endif    

    o.normal = UnityObjectToWorldNormal(v.normal);

    #if defined(LIGHTMAP_ON)
        o.lmap.xy = mad(v.lightMapUV.xy, unity_LightmapST.xy, unity_LightmapST.zw);
    #else
        #if !defined(_USEBUMPMAP_ON) && defined(_USEAMBIENT_ON) && UNITY_SHOULD_SAMPLE_SH
            //only do spherical harmonics in pixel shader if there's a bump map even if per-pixel is set
            //not much quality benefit for the cost
            o.vertexLighting += ShadeSH9Fast(float4(o.normal, 1.0));
        #endif
    #endif
   
    #if defined(_SHADE4_ON) && defined(VERTEXLIGHT_ON)
        o.vertexLighting += Shade4PointLightsFast(o.worldPos, o.normal);
    #endif

    #if defined(_USEREFLECTIONS_ON)
        float3 worldViewDir = _WorldSpaceCameraPos - worldPos;
        //incident vector need not be normalized
        o.worldReflection = reflect(-worldViewDir, o.normal);
    #endif

    #if defined(_USEBUMPMAP_ON)
        o.tangent = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
        o.bitangent = normalize(cross(o.normal, o.tangent) * v.tangent.w);
    #endif

    //transfer shadow and lightmap info
    //UNITY_TRANSFER_LIGHTING
    TRANSFER_VERTEX_TO_FRAGMENT(o);

    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    #if defined(_USEMAINTEX_ON)
        float4 color = UNITY_SAMPLE_TEX2D(_MainTex, i.mainUV.xy);
    #else
        float4 color = 1.0;
    #endif

    #if defined(_USEVERTEXCOLOR_ON)
        //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
        color *= i.color;        
    #elif defined(_USEMAINCOLOR_ON)
        color *= _Color;
    #endif

    #if defined(_ALPHATEST_ON)
        //note execution continues but all writes are disabled
        clip(color.a - _Cutoff);
    #endif
        
    //shadows + light attenuation
    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

    //TODO: consider UnityComputeForwardShadows
    #if defined(LIGHTMAP_ON)
        //float3 diffuseContrib = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));

        #if defined(SHADOWS_SCREEN)
            diffuseContrib = min(diffuseContrib, atten * 2.0);
        #endif
    #else
        float3 diffuseContrib = i.vertexLighting;
        float3 specContrib = 0;

        //linearly interpolating normals will not produce a normal
        float3 modelWorldNormal = normalize(i.normal);
        float3 worldNormal = modelWorldNormal;

        float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos); 

        #if defined(_USEBUMPMAP_ON)
            float3x3 tangentToWorld = float3x3(i.tangent, i.bitangent, worldNormal);
            //unpack can be expensive if normal map is dxt5
            float3 objectNormal = UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, i.mainUV.xy));
            worldNormal = normalize(mul(objectNormal, tangentToWorld));
        #endif

        #if defined(_USEBUMPMAP_ON) && defined(_USEAMBIENT_ON) && UNITY_SHOULD_SAMPLE_SH
            diffuseContrib += ShadeSH9Fast(float4(worldNormal, 1.0));
        #endif
    
        #if defined(_USEDIFFUSE_ON)
            diffuseContrib += DiffuseLambertian(worldNormal, worldLightDir, _LightColor0.rgb);
        #endif

        color = PreMultiplyAlphaFast(color * float4(diffuseContrib,1));

        #if defined(_USERIMLIGHTING_ON)
            color.rgb += RimLight(i.worldPos, modelWorldNormal, _RimPower, _RimColor);
        #endif

        #if defined(_SPECULARHIGHLIGHTS_ON)
            float gloss = _Gloss;
            float3 specularColor = float3(1, 1, 1);

            #if defined(_USESPECULARMAP_ON)
                float4 specularMapSample = UNITY_SAMPLE_TEX2D(_SpecularMap, i.mainUV.xy);
                gloss *= specularMapSample.a;
                specularColor *= specularMapSample.rrr;
            #endif
           
            specContrib += SpecularSchlick(worldNormal, normalize(UnityWorldSpaceViewDir(i.worldPos)),
                                           _WorldSpaceLightPos0.xyz, _LightColor0.rgb,
                                           _Specular, gloss, specularColor);
        #endif

        color.rgb += specContrib;
        color.rgb *= atten;
    #endif
   
    #if defined(_USEREFLECTIONS_ON)
        float3 worldReflection = normalize(i.worldReflection);

        #if defined(_USECUSTOMCUBEMAP_ON)
            color.rgb += UNITY_SAMPLE_TEXCUBE(_CubeMap, worldReflection) * _ReflectionScale;
        #else
            color.rgb += UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldReflection) * _ReflectionScale;
        #endif
    #endif

    #if defined(_USEEMISSIONMAP_ON)
        color.rgb += UNITY_SAMPLE_TEX2D(_EmissionMap, i.mainUV.xy);
    #endif

    #if defined(_USEEMISSIONCOLOR_ON)
        color.rgb += _EmissionColor;
    #endif

    UNITY_APPLY_FOG(i.fogCoord, color);
    
    return color;
}