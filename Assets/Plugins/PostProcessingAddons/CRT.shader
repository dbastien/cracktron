Shader "Hidden/Custom/CRT"
{
    HLSLINCLUDE
        #include "../PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        float ScanlineSmoothStep(float dist, float size)
        {
            return smoothstep(0.0, 1.0, 1.0 - dist*size);
        }
        
        float ScanlinePow(float dist, float size, float brightness)
        {
            return max(1.0-dist*dist*size, brightness);
        }

        float ScanLineHard(float dist, float size, float brightness)
        {
            float s = step(size, dist);
            return saturate(s + (1.0 - s) * brightness);
        }
        
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            static const int ColorPerChannelBucketCount = 6;

            float2 MonitorRes = float2(160.0, 120.0);
            float2 p = i.texcoord * MonitorRes;

            float2 dist = abs(p - (floor(p) + 0.5));

            float2 dx = float2(_MainTex_TexelSize.x, 0.0);
            float2 dy = float2(0.0, _MainTex_TexelSize.y);

            float4 c_00 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ((int2)p)/MonitorRes);

            c_00 = round(c_00 * (ColorPerChannelBucketCount - 1)) / (ColorPerChannelBucketCount - 1);

            float sl = ScanlinePow(dist.y, 3, 0.5);
            return c_00*sl;
        }    
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}