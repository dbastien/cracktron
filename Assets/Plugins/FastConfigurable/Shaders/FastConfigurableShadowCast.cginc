#include "UnityCG.cginc"
#include "TextureMacro.cginc"

#define USES_TEX_XY ((_ALPHATEST_ON || _USESEMITRANSPARENTSHADOWS_ON) && _USEMAINTEX_ON)
#define USES_VERTEX_COLOR ((_ALPHATEST_ON || _USESEMITRANSPARENTSHADOWS_ON) && _USEVERTEXCOLOR_ON)

float4 _Color;
UNITY_DECLARE_TEX2D(_MainTex);

float _Cutoff;

float4 _TextureScaleOffset;

//sampler2D _DitherMaskLOD2D;
sampler3D _DitherMaskLOD;

struct shadowcast_a2v
{
    float4 vertex : POSITION;

    #if defined(_USENORMALOFFSETSHADOWS_ON)
        float3 normal : NORMAL;
    #endif

    #if USES_VERTEX_COLOR
        float4 color : COLOR;
    #endif

    #if USES_TEX_XY
        float2 mainUV : TEXCOORD0;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct shadowcast_v2f
{
    #if USES_VERTEX_COLOR
        float4 color : COLOR;
    #endif

    #if USES_TEX_XY || defined(_NEAR_PLANE_FADE_ON)
        float3 texXYFadeZ : TEXCOORD1;
    #endif

    //V2F_SHADOW_CASTER_NOPOS + UNITY_POSITION(pos)
    //uses texcoord0
    V2F_SHADOW_CASTER;
    UNITY_VERTEX_OUTPUT_STEREO
};

shadowcast_v2f shadowcast_vert(shadowcast_a2v v)
{
    shadowcast_v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(shadowcast_v2f, o);    
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    #if USES_VERTEX_COLOR
        #if defined(_USEMAINCOLOR_ON)
            o.color = v.color * _Color;
        #else
            o.color = v.color;
        #endif
    #endif

    #if USES_TEX_XY
        o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
    #endif

    //TRANSFER_SHADOW_CASTER_NOPOS
    #if defined(_USENORMALOFFSETSHADOWS_ON)
        //clip space, linear bias, normal offset
        TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
    #else
        //clip space, linear bias
        TRANSFER_SHADOW_CASTER(o)
    #endif

    return o;
}

fixed4 shadowcast_frag(shadowcast_v2f IN) : SV_Target
{
    #if USES_TEX_XY
        float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
    #else
        float4 color = 1.0;
    #endif

    #if USES_VERTEX_COLOR
        //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
        color *= IN.color;        
    #elif defined(_USEMAINCOLOR_ON)
        color *= _Color;
    #endif

    #if defined(_ALPHATEST_ON)
        //note execution continues but all writes are disabled
        clip(color.a - _Cutoff);
    #endif

    //dither
    #if defined(_USESEMITRANSPARENTSHADOWS_ON)
        float3 ditherLookup = float3(IN.pos.xy * 0.25, color.a * 0.9375);
        float alphaDither = tex3D(_DitherMaskLOD, ditherLookup).a;

        //note execution continues but all writes are disabled
        clip (alphaDither - 0.01);
    #endif

    //UnityApplyDitherCrossFade(vpos.xy);

    //ends up in SHADOWS_CUBE for point lights shadows,
    //or V2F_SHADOW_CASTER_NOPOS for directional or spot lights
    SHADOW_CASTER_FRAGMENT(IN)
}