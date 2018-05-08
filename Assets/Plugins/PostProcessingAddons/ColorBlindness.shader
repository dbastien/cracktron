Shader "Hidden/Custom/ColorBlindness"
{
    HLSLINCLUDE
        #include "ColorBlindness.cginc"
        #include "PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        float4 frag(VaryingsDefault i) : SV_Target
        {	
            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            
            col.rgb = LinearRGBToLMS(col.rgb);
            col.rgb = Protanopia(col.rgb);

            return col;
        }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment frag
            ENDHLSL
        }
    }
}