// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "HoloToolkit/Fast Configurable Particles"
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
                
                struct a2v
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    UNITY_INITIALIZE_OUTPUT(v2f, o);

                    o.pos = UnityObjectToClipPos(v.vertex);

                    return o;
                }
                
                fixed4 frag(v2f IN) : SV_Target
                {
                    float4 color = float4(0,0,0,0);

                    #if defined(_ALPHATEST_ON)
                        //note execution continues but all writes are disabled
                        clip(color.a - _Cutoff);
                    #endif

                    return color;
                }
            ENDCG
        }
    }

    CustomEditor "HoloToolkit.Unity.FastConfigurableParticlesGUI"
}