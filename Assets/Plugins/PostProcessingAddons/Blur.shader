Shader "Hidden/Custom/Blur"
{
    HLSLINCLUDE
        #include "../PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        float4 GaussianLinear8(TEXTURE2D_ARGS(tex, samp), float2 uv, float2 d)
        {
            static const int taps = 8;
            float weight[taps] = {0.04462541, 0.08833525, 0.08476666, 0.07870257, 0.070701, 0.06145185, 0.05167937, 0.04205059};
            float offset[taps] = {0, 1.496901, 3.492769, 5.488638, 7.484509, 9.480382, 11.47626, 13.47214};

		    float4 col = SAMPLE_TEXTURE2D(tex, samp, uv) * weight[0];		
            UNITY_UNROLL
			for (int i = 1; i < taps; ++i)
			{
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d * offset[i]) * weight[i];
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d * offset[i]) * weight[i];
			}
            return col;
        }

        float4 FragH(VaryingsDefault i) : SV_Target
        {		
            return GaussianLinear8(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i.texcoord, float2(_MainTex_TexelSize.x, 0.0));
        }
        float4 FragV(VaryingsDefault i) : SV_Target
        {		
            return GaussianLinear8(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i.texcoord, float2(0.0, _MainTex_TexelSize.y));
        }        
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragH
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragV
            ENDHLSL
        }        
    }
}