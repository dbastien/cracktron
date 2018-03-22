Shader "Cracktron/Fast Traditional"
{
    Properties
    { 
        _Mode("Rendering Mode", Float) = 0
        [Toggle] _ShowAdvanced("Advanced Settings", Float) = 0

        [Toggle] _UseVertexColor("Model Vertex Color", Float) = 0

        _Color("Main Color", Color) = (1,1,1,1)		
        [Toggle] _UseMainColor("Main Color Enabled?", Float) = 0
        
        [NoScaleOffset]_MainTex("Albedo", 2D) = "red" {}
        [Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0

        [Toggle] _UseAmbient("Ambient", Float) = 1
        [Toggle] _UseDiffuse("Diffuse", Float) = 1
        [Toggle] _Shade4("Vertex", Float) = 0

        [Toggle] _SpecularHighlights("Specular Lighting", Float) = 0
        _Specular("Specular", Range(0, 2.0)) = 1.0
        _Gloss("Gloss", Range(0, 2.0)) = 1.0          

        [NoScaleOffset]_SpecularMap("Specular Map", 2D) = "white" {}
        [Toggle] _UseSpecularMap("Specular Map Enabled?", Float) = 0

        [NoScaleOffset][Normal]_BumpMap("Normal Map", 2D) = "bump" {}
        [Toggle] _UseBumpMap("Normal Map Enabled?", Float) = 0

        [Toggle] _UseReflections("Reflections", Float) = 0

        [NoScaleOffset]_CubeMap("Cube Map", Cube) = "" {}
        [Toggle] _UseCustomCubeMap("Use Custom Cube Map?", Float) = 1

        _ReflectionScale("Scale", Range(0.01, 1.5)) = 1.0
        
        [Toggle] _UseRimLighting("Rim Lighting", Float) = 0
        [PowerSlider(.6)]_RimPower("Power", Range(0.1, 1.0)) = 0.7
        _RimColor("Color", Color) = (1,1,1,1)

        _EmissionColor("Emission Color", Color) = (1,1,1,1)
        [Toggle] _UseEmissionColor("Emission Color Enabled?", Float) = 0

        [NoScaleOffset] _EmissionMap("Emission Map", 2D) = "blue" {}
        [Toggle] _UseEmissionMap("Emission Map Enabled?", Float) = 0

        _TextureScaleOffset("Global UV", Vector) = (1, 1, 0, 0)

        [Toggle] _AlphaTest("Alpha Test", Float) = 0
        _Cutoff("Cutoff Threshold", Range(-0.1, 1.0)) = -0.1
        [Toggle] _AlphaPremultiply("Pre-Multiply", Float) = 0

        //shadows
        [Toggle] _UseNormalOffsetShadows("Normal Offset", Float) = 0
        [Toggle] _UseSemiTransparentShadows("Transparent", Float) = 0        

        //alpha blending
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination", Float) = 0 //"Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Operation", Float) = 0 //"Add"

        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling Mode", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("Z Write", Float) = 1 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("Color Write Mask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend [_SrcBlend] [_DstBlend]
        BlendOp [_BlendOp]
        ZTest [_ZTest]
        ZWrite [_ZWrite]
        Cull [_Cull]
        ColorMask [_ColorWriteMask]

        Pass
        {
            //Base forward pass (directional light, emission, lightmaps, ...)		
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            //AlphaToMask On

            CGPROGRAM
                #pragma target 3.5
                #pragma only_renderers d3d11 glcore gles
                //#pragma enable_d3d11_debug_symbols

                #pragma vertex vert
                #pragma fragment frag

                #pragma multi_compile_fwdbase nolightmap nodynlightmap
			    #pragma multi_compile_instancing
                //#pragma multi_compile_fog

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _USEBUMPMAP_ON
                #pragma shader_feature _USEAMBIENT_ON
                #pragma shader_feature _USEDIFFUSE_ON
                #pragma shader_feature _SPECULARHIGHLIGHTS_ON
                #pragma shader_feature _USESPECULARMAP_ON
                #pragma shader_feature _SHADE4_ON
                #pragma shader_feature _USEREFLECTIONS_ON 
                #pragma shader_feature _USECUSTOMCUBEMAP_ON
                #pragma shader_feature _USERIMLIGHTING_ON                
                #pragma shader_feature _USEEMISSIONCOLOR_ON
                #pragma shader_feature _USEEMISSIONMAP_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                #define FORWARD_BASE_PASS               

                #include "FastTraditional.cginc"
            ENDCG
        }

        Pass
        {
            Name "FORWARD_DELTA"
            Tags{ "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            //AlphaToMask On
            ZWrite Off    
            ZTest LEqual

            CGPROGRAM
                #pragma target 3.5
                #pragma only_renderers d3d11 glcore gles                
                //#pragma enable_d3d11_debug_symbols

                #pragma vertex vert
                #pragma fragment frag
                
                #pragma multi_compile_fwdadd
			    #pragma multi_compile_instancing                
                //#pragma multi_compile_fog

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _USEBUMPMAP_ON
                #pragma shader_feature _USEDIFFUSE_ON
                #pragma shader_feature _SPECULARHIGHLIGHTS_ON
                #pragma shader_feature _USESPECULARMAP_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                #include "FastTraditional.cginc" 
            ENDCG
        }      

        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }
            Blend [_SrcBlend] One
            //AlphaToMask On            
            ZWrite On
            ZTest LEqual

            CGPROGRAM
                #pragma target 3.5
                #pragma only_renderers d3d11 glcore gles
                //#pragma enable_d3d11_debug_symbols

                #pragma vertex shadowcast_vert
                #pragma fragment shadowcast_frag

                #pragma multi_compile_shadowcaster
			                     #pragma multi_compile_instancing

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                #pragma shader_feature _USENORMALOFFSETSHADOWS_ON
                #pragma shader_feature _USESEMITRANSPARENTSHADOWS_ON

                #include "FastShadowCast.cginc" 
            ENDCG
        }
    } 
    CustomEditor "FastTraditionalGUI"
}
