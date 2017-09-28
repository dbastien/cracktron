#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "ShaderCommon.cginc"
#include "FastMath.cginc"
#include "FastLighting.cginc"
#include "TextureMacro.cginc"

//todo: reassess these
#define PIXEL_SHADER_USES_WORLDPOS  (_SPECULARHIGHLIGHTS_ON || (_FORCEPERPIXEL_ON && (_SHADE4_ON || _USERIMLIGHTING_ON)))
#define PIXEL_SHADER_USES_NORMAL (!_USEBUMPMAP_ON && (_FORCEPERPIXEL_ON || _SPECULARHIGHLIGHTS_ON))
#define USES_TEX_XY (_USEMAINTEX_ON || _USEOCCLUSIONMAP_ON || _USEEMISSIONMAP_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON || _USESPECULARMAP_ON)

//todo: share world view dir

float4 _Color;
UNITY_DECLARE_TEX2D(_MainTex);
UNITY_DECLARE_TEX2D(_OcclusionMap);

UNITY_DECLARE_TEX2D(_BumpMap);

float _Specular;
UNITY_DECLARE_TEX2D(_SpecularMap);

float _Gloss;
UNITY_DECLARE_TEX2D(_GlossMap);

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

    #if defined(_USEVERTEXCOLOR_ON)
        float4 color : COLOR;
    #endif

    #if USES_TEX_XY
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

    #if PIXEL_SHADER_USES_NORMAL
        float3 worldNormal : NORMAL;
    #endif

    #if USES_TEX_XY || defined(_NEAR_PLANE_FADE_ON)
        float3 texXYFadeZ : TEXCOORD0;
    #endif

    #if LIGHTMAP_ON
        float2 lmap : TEXCOORD1;
    #else
        float3 vertexLighting : TEXCOORD1;
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        float3 worldPos: TEXCOORD2;
    #endif

    #if defined(_USEREFLECTIONS_ON)
        float3 worldReflection : TEXCOORD3;
    #endif

    LIGHTING_COORDS(4, 5)
    UNITY_FOG_COORDS(6)
    UNITY_VERTEX_OUTPUT_STEREO
};

#if defined(_USERIMLIGHTING_ON)
    inline float3 RimLight(float3 worldNormal, float3 worldPos)
    {
        //TODO: optimize whole function
        float rim = 1.0 - dot_sat(worldNormal, UnityWorldSpaceViewDir(worldPos));

        //boost to the dot product to allow for a greater max effect - .25 is the boost factor
        return smoothstep(0.25, 1.0, mad(rim, _RimPower, 0.25)) * _RimColor.rgb;
    }
#endif

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

    #if defined(_SPECULARHIGHLIGHTS_ON) || defined(_SHADE4_ON) || defined(_USEREFLECTIONS_ON) || defined(_USERIMLIGHTING_ON)
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
    #endif

    #if PIXEL_SHADER_USES_WORLDPOS
        o.worldPos = worldPos;
    #endif

    #if USES_TEX_XY
        o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
    #endif

    //todo: not always needed
    float3 worldNormal = UnityObjectToWorldNormal(v.normal);

    #if defined(LIGHTMAP_ON)
        o.lmap.xy = mad(v.lightMapUV.xy, unity_LightmapST.xy, unity_LightmapST.zw);
    #else
        #if !defined(_USEBUMPMAP_ON) && defined(_USEAMBIENT_ON) && UNITY_SHOULD_SAMPLE_SH
            //only do spherical harmonics in pixel shader if there's a bump map even if per-pixel is set
            //not much quality benefit for the cost
            o.vertexLighting += FastShadeSH9(float4(worldNormal, 1.0));
        #endif

        #if !defined(_FORCEPERPIXEL_ON)
            #if defined(_USEDIFFUSE_ON)
                o.vertexLighting += FastLightingDiffuseLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif

            #if defined(_SHADE4_ON)
                o.vertexLighting += FastShade4PointLights(worldPos, worldNormal);
            #endif

            #if defined(_USERIMLIGHTING_ON)
                o.vertexLighting += RimLight(worldNormal, worldPos);
            #endif
            //todo: if no pixel work being performed, could premul alpha here
        #endif
    #endif

    #if PIXEL_SHADER_USES_NORMAL
        o.worldNormal = worldNormal;
    #endif
    
    #if defined(_USEREFLECTIONS_ON)
        float3 worldViewDir = _WorldSpaceCameraPos - worldPos;
        //incident vector need not be normalized
        o.worldReflection = reflect(-worldViewDir, worldNormal);
   #endif

    //fade away objects closer to the camera
    #if defined(_NEAR_PLANE_FADE_ON)
        o.texXYFadeZ.z = ComputeNearPlaneFadeLinear(v.vertex);
    #endif

    //transfer shadow and lightmap info
    TRANSFER_VERTEX_TO_FRAGMENT(o);

    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

fixed4 frag(v2f IN) : SV_Target
{
    #if defined(_USEMAINTEX_ON)
        float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
    #else
        float4 color = 1.0;
    #endif

    #if defined(_USEOCCLUSIONMAP_ON)
        color *= UNITY_SAMPLE_TEX2D(_OcclusionMap, IN.texXYFadeZ.xy);
    #endif

    #if defined(_USEVERTEXCOLOR_ON)
        //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
        color *= IN.color;        
    #elif defined(_USEMAINCOLOR_ON)
        color *= _Color;
    #endif

    #if defined(_ALPHATEST_ON)
        //note execution continues but all writes are disabled
        clip(color.a - _Cutoff);
    #endif
        
    float lightAttenuation = LIGHT_ATTENUATION(IN);
    
    //TODO: consider UnityComputeForwardShadows
    #if defined(LIGHTMAP_ON)
        float3 diffuseContrib = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));

        #if defined(SHADOWS_SCREEN)
            diffuseContrib = min(lightColor, lightAttenuation * 2.0);
        #endif
    #else
        float3 diffuseContrib = IN.vertexLighting;
        float3 specContrib = 0;

        #if defined(_USEBUMPMAP_ON)
            //unpack can be expensive if normal map is dxt5
            float3 worldNormal = UnityObjectToWorldNormal(UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.texXYFadeZ.xy)));
        #elif PIXEL_SHADER_USES_NORMAL
            //linearly interpolating normals will not produce a normal
            float3 worldNormal = normalize(IN.worldNormal);
        #endif 

        #if defined(_FORCEPERPIXEL_ON)               
            #if defined(_USEBUMPMAP_ON) && defined(_USEAMBIENT_ON) && UNITY_SHOULD_SAMPLE_SH
                //only do spherical harmonics in pixel shader if there's a bump map even if per-pixel is set
                //not much quality benefit for the cost
                diffuseContrib += FastShadeSH9(float4(worldNormal, 1.0));
            #endif
        
            #if defined(_USEDIFFUSE_ON)
                diffuseContrib += FastLightingDiffuseLambertian(worldNormal, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
            #endif

            #if defined(_SHADE4_ON)
                diffuseContrib += FastShade4PointLights(IN.worldPos, worldNormal);
            #endif

            #if defined(_USERIMLIGHTING_ON)
                diffuseContrib += RimLight(worldNormal, IN.worldPos);
            #endif         
        #endif

        #if defined(_SPECULARHIGHLIGHTS_ON)
            float gloss = _Gloss;
            #if defined(_USEGLOSSMAP_ON)
                gloss *= UNITY_SAMPLE_TEX2D(_GlossMap, IN.texXYFadeZ.xy).r;
            #endif

            float specular = _Specular;
            #if defined(_USESPECULARMAP_ON)
                specular *= UNITY_SAMPLE_TEX2D(_SpecularMap, IN.texXYFadeZ.xy).r;
            #endif

            #if defined(_SPECULARHIGHLIGHTS_ON)
                specContrib += FastLightingSpecularSchlick(worldNormal, UnityWorldSpaceViewDir(IN.worldPos), _WorldSpaceLightPos0.xyz, _LightColor0.rgb, specular, gloss, _SpecColor);
            #endif
        #endif   

        color = FastPreMultiplyAlpha(color * float4(diffuseContrib,1));
        color.rgb += specContrib;
        color.rgb *= lightAttenuation;
    #endif
   
    #if defined(_USEREFLECTIONS_ON)
        #if defined(_USECUSTOMCUBEMAP_ON)
            color.rgb += UNITY_SAMPLE_TEXCUBE(_CubeMap, IN.worldReflection) * _ReflectionScale;
        #else
            color.rgb += UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldReflection) * _ReflectionScale;
        #endif
    #endif

    #if defined(_USEEMISSIONMAP_ON)
        color.rgb += UNITY_SAMPLE_TEX2D(_EmissionMap, IN.texXYFadeZ.xy);
    #endif

    #if defined(_USEEMISSIONCOLOR_ON)
        color.rgb += _EmissionColor;
    #endif

    #if defined(_NEAR_PLANE_FADE_ON)
        color.rgb *= IN.texXYFadeZ.z;
    #endif

    UNITY_APPLY_FOG(IN.fogCoord, color);
    
    return color;
}