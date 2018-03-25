Shader "Cracktron/Fast Particles"
{
    //TODO: BEWARE - WIP

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
        [Toggle] _ShowAdvanced("Advanced Settings", Float) = 0

        _Color("Main Color", Color) = (1,1,1,1)		
        [Toggle] _UseMainColor("Main Color Enabled?", Float) = 0

        [NoScaleOffset]_MainTex("Albedo", 2D) = "red" {}
        [Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0

        _TextureScaleOffset("Global UV", Vector) = (1, 1, 0, 0)

       [Toggle] _AlphaTest("Alpha Test", Float) = 0
        _Cutoff("Cutoff Threshold", Range(-0.1, 1.0)) = -0.1
        [Toggle] _AlphaPremultiply("Pre-Multiply", Float) = 0        

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination", Float) = 0 //"Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Operation", Float) = 0 //"Add"

        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling Mode", Float) = 2 //"Back"
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
            //AlphaToMask On           

            CGPROGRAM
                #include "TextureMacro.cginc"

                #pragma vertex vert
                #pragma fragment frag

                //#pragma multi_compile_fog

                #pragma only_renderers d3d11 glcore gles
                #pragma target 3.5
                //#pragma enable_d3d11_debug_symbols

                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON
                
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
					float2 tex : TEXCOORD0;
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    UNITY_INITIALIZE_OUTPUT(v2f, o);

                    o.tex.xy = TRANSFORM_TEX_MAINTEX(v.mainUV.xy, _TextureScaleOffset);
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

                    return color;
                }
            ENDCG
        }
    }
    CustomEditor "FastParticlesGUI"    
}