Shader "Cracktron/Debug/Noise"
{
    Properties
    {
        [PowerSlider(3.0)] _NoiseFrequency  ("Frequency", Range(5, 200)) = 73.0
        [PowerSlider(3.0)] _NoiseGain       ("Gain", Range(0.1, 3.0)) = .5
        [PowerSlider(2.0)] _NoiseLacunarity ("Lacunarity", Range(1.0, 3.0)) = 1.93485736
        [PowerSlider(4.0)] _NoiseWhirl      ("Whirl", Range(0.01, 2.0)) = .3

        [NoScaleOffset]    _RandomTex("Random Texture", 2D) = "red" {}

        [Header(Alpha Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"
            
        [Header(Misc.)]	
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Blend[_SrcBlend][_DstBlend]
        BlendOp[_BlendOp]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Cull[_Cull]
        ColorMask[_ColorWriteMask]
            
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"            
            #include "AutoLight.cginc"
            #include "../Noise.cginc"

            float _NoiseFrequency;
            float _NoiseGain;
            float _NoiseLacunarity;
            float _NoiseWhirl;

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };     
           
            v2f vert (a2v v)
            { 
                v2f o;		
                UNITY_INITIALIZE_OUTPUT(v2f, o);
 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
               
            fixed4 frag (v2f i) : SV_Target
            {
                float noise; 
                //noise = NOISE2D_FUNC(i.uv * _NoiseFrequency);
                //noise = FBMNoise(i.uv, _NoiseFrequency, _NoiseGain, _NoiseLacunarity, 3.0);
                noise = Turbulence(i.uv, _NoiseFrequency, _NoiseGain, _NoiseLacunarity, _NoiseWhirl, 4.0);

                return float4(noise, noise, noise, 1.0);
            }
            ENDCG
        }
    }
}
