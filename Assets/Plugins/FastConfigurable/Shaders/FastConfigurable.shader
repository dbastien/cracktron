Shader "HoloToolkit/Fast Configurable"
{
    Properties
    {
        _Mode("Rendering Mode", Float) = 0

        [Toggle] _UseVertexColor("Vertex Color Enabled?", Float) = 0
        [Toggle] _UseMainColor("Main Color Enabled?", Float) = 0
        _Color("Main Color", Color) = (1,1,1,1)		
        [Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "red" {}

        [Toggle] _UseOcclusionMap("Occlusion/Detail Texture Enabled?", Float) = 0
        [NoScaleOffset]_OcclusionMap("Occlusion/Detail Texture", 2D) = "blue" {}
       
        [Toggle] _UseAmbient("Ambient Lighting Enabled?", Float) = 1
        [Toggle] _UseDiffuse("Diffuse Lighting Enabled?", Float) = 1

        [Toggle] _SpecularHighlights("Specular Lighting Enabled?", Float) = 0
        [Toggle] _Shade4("Use additional lighting data? (Expensive!)", Float) = 0

        [Toggle] _ForcePerPixel("Light per-pixel (always on if a map is set)", Float) = 0
        
        _SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        [PowerSlider(5.0)]_Specular("Specular (Specular Power)", Range(0.01, 1000.0)) = 50.0
        [Toggle] _UseSpecularMap("Use Specular Map? (per-pixel)", Float) = 0
        [NoScaleOffset]_SpecularMap("Specular Map", 2D) = "white" {}

        _Gloss("Gloss (Specular Scale)", Range(0.1, 10.0)) = 1.0
        [Toggle] _UseGlossMap("Use Gloss Map? (per-pixel)", Float) = 0
        [NoScaleOffset]_GlossMap("Gloss Map", 2D) = "white" {}

        [Toggle] _UseBumpMap("Normal Map Enabled? (per-pixel)", Float) = 0
        [NoScaleOffset][Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        [Toggle] _UseReflections("Reflections Enabled?", Float) = 0
        [Toggle] _UseCustomCubeMap("Use Custom Cube Map?", Float) = 1
        [NoScaleOffset]_CubeMap("CubeMap", Cube) = "" {}
        _ReflectionScale("Reflection Scale", Range(0.01, 1.5)) = 1.0

        [Toggle] _UseRimLighting("Rim Lighting Enabled?", Float) = 0
        [PowerSlider(.6)]_RimPower("Power", Range(0.1, 1.0)) = 0.7
        _RimColor("Color", Color) = (1,1,1,1)
    
        [Toggle] _UseEmissionColor("Emission Color Enabled?", Float) = 0
        _EmissionColor("Emission Color", Color) = (1,1,1,1)
        [Toggle] _UseEmissionMap("Emission Map Enabled?", Float) = 0
        [NoScaleOffset] _EmissionMap("Emission Map", 2D) = "blue" {}

        _TextureScaleOffset("Texture Scale (XY) and Offset (ZW)", Vector) = (1, 1, 0, 0)

        [Toggle] _AlphaTest("Alpha test enabled?", Float) = 0
        _Cutoff("Alpha Cutoff", Range(-0.1, 1.0)) = -0.1
        [Toggle] _AlphaPremultiply("Pre-multiply alpha?", Float) = 0

        //shadows
        [Toggle] _UseNormalOffsetShadows("Normal offset shadows enabled?", Float) = 0
        [Toggle] _UseSemiTransparentShadows("Semi-transparent shadows enabled?", Float) = 0        

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"

        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
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

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                //compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
                #pragma multi_compile_fwdbase

                //expands to several variants to handle different fog types
                #pragma multi_compile_fog

                #pragma target 5.0
                #pragma only_renderers d3d11
                #pragma enable_d3d11_debug_symbols

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _USEOCCLUSIONMAP_ON
                #pragma shader_feature _USEBUMPMAP_ON
                #pragma shader_feature _USEAMBIENT_ON
                #pragma shader_feature _USEDIFFUSE_ON
                #pragma shader_feature _SPECULARHIGHLIGHTS_ON
                #pragma shader_feature _FORCEPERPIXEL_ON
                #pragma shader_feature _USEGLOSSMAP_ON
                #pragma shader_feature _SHADE4_ON
                #pragma shader_feature _USEREFLECTIONS_ON
                #pragma shader_feature _USECUSTOMCUBEMAP_ON               
                #pragma shader_feature _USERIMLIGHTING_ON
                #pragma shader_feature _USEEMISSIONCOLOR_ON
                #pragma shader_feature _USEEMISSIONMAP_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON               

                //scale and offset will apply to all
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                //may be set from script so generate both paths
                #pragma multi_compile __ _NEAR_PLANE_FADE_ON

                #include "FastConfigurable.cginc"
            ENDCG
        }

        Pass
        {
            Name "FORWARD_DELTA"
            Tags{ "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                #pragma multi_compile_fwdadd_fullshadows

                //expands to several variants to handle different fog types
                #pragma multi_compile_fog

                #pragma target 5.0
                #pragma only_renderers d3d11
                #pragma enable_d3d11_debug_symbols

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _USEOCCLUSIONMAP_ON
                #pragma shader_feature _USEBUMPMAP_ON
                #pragma shader_feature _USEDIFFUSE_ON
                #pragma shader_feature _SPECULARHIGHLIGHTS_ON
                #pragma shader_feature _FORCEPERPIXEL_ON
                #pragma shader_feature _USEGLOSSMAP_ON
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON               

                //scale and offset will apply to all
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                //may be set from script so generate both paths
                #pragma multi_compile __ _NEAR_PLANE_FADE_ON

                #include "FastConfigurable.cginc"
            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }
            Blend [_SrcBlend] One

            CGPROGRAM
                #pragma vertex shadowcast_vert
                #pragma fragment shadowcast_frag

                #pragma multi_compile_shadowcaster

                #pragma target 5.0
                #pragma only_renderers d3d11
                #pragma enable_d3d11_debug_symbols

                //shader features are only compiled if a material uses them
                #pragma shader_feature _USEVERTEXCOLOR_ON
                #pragma shader_feature _USEMAINCOLOR_ON
                #pragma shader_feature _USEMAINTEX_ON
                #pragma shader_feature _ALPHATEST_ON

                //scale and offset will apply to all
                #pragma shader_feature _MainTex_SCALE_ON
                #pragma shader_feature _MainTex_OFFSET_ON

                #pragma shader_feature _USENORMALOFFSETSHADOWS_ON
                #pragma shader_feature _USESEMITRANSPARENTSHADOWS_ON

                //may be set from script so generate both paths
                #pragma multi_compile __ _NEAR_PLANE_FADE_ON

                #include "FastConfigurableShadowCast.cginc" 
            ENDCG
        }
    } 
    
    CustomEditor "HoloToolkit.Unity.FastConfigurableGUI"
}