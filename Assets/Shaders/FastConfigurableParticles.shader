Shader "Cracktron/Fast Configurable Particles"
{
    //Existing mobile alpha blended
    // Stats for Vertex shader:
    //         d3d9: 12 math
    //        d3d11: 9 math
    // Stats for Fragment shader:
    //         d3d9: 2 math, 1 texture
    //        d3d11: 1 math, 1 texture

    //Existing mobile add
    // Stats for Vertex shader:
    //         d3d9: 12 math
    //        d3d11: 9 math
    // Stats for Fragment shader:
    //         d3d9: 2 math, 1 texture
    //        d3d11: 1 math, 1 texture

    Properties
    {
        _Mode("Rendering Mode", Float) = 0.0

        _Color("Main Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "red" {}

        _TextureScaleOffset("Texture Scale (XY) and Offset (ZW)", Vector) = (1, 1, 0, 0)

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"

        [Toggle] _AlphaTest("Alpha test enabled?", Float) = 0
        _Cutoff("Alpha Cutoff", Range(-0.1, 1.0)) = -0.1

        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
        LOD 100
        Blend[_SrcBlend][_DstBlend]
        BlendOp[_BlendOp]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Cull[_Cull]
        ColorMask[_ColorWriteMask]

        Pass
        {
            //Base forward pass (directional light, emission, lightmaps, ...)
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            CGPROGRAM
                #include "TextureMacro.cginc"

                #pragma vertex vert
                #pragma fragment frag

                #pragma multi_compile_fog

                #pragma target 5.0
                #pragma only_renderers d3d11
                #pragma enable_d3d11_debug_symbols

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _ALPHATEST_ON

                //scale and offset will apply to all
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                //may be set from script so generate both paths
                #pragma multi_compile __ _NEAR_PLANE_FADE_ON
                
                float4 _Color;
                UNITY_DECLARE_TEX2D(_MainTex);
                float4 _TextureScaleOffset;
                float _Cutoff;   

                struct a2v
                {
                    float4 vertex : POSITION;
                    float2 mainUV : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;

                    #if defined(_NEAR_PLANE_FADE_ON)
                        float3 tex : TEXCOORD0;
                    #else
                        float2 tex : TEXCOORD0;
                    #endif
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    UNITY_INITIALIZE_OUTPUT(v2f, o);

                    o.tex.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);

                    //fade away objects closer to the camera
                    #if defined(_NEAR_PLANE_FADE_ON)
                        o.tex.z = ComputeNearPlaneFadeLinear(v.vertex);
                    #endif

                    o.pos = UnityObjectToClipPos(v.vertex);

                    return o;
                }
                
                fixed4 frag(v2f IN) : SV_Target
                {
                    float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.tex.xy);

                    #if defined(_ALPHATEST_ON)
                        //note execution continues but all writes are disabled
                        clip(color.a - _Cutoff);
                    #endif

                    #if defined(_NEAR_PLANE_FADE_ON)
                        color.rgb *= IN.tex.z;
                    #endif

                    return color;
                }
            ENDCG
        }
    }

    CustomEditor "FastConfigurableParticlesGUI"
}