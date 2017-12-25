Shader "Hidden/Custom/Retro"
{
    HLSLINCLUDE
        #pragma multi_compile __ _USECOLORQUANT_ON
        #pragma multi_compile __ _USECRTMASK_ON

        #include "../PostProcessing/Shaders/StdLib.hlsl"
        #include "../ShaderToolkit/Shaders/ImageProcessing.cginc"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        TEXTURE2D_SAMPLER2D(_CrtMask, sampler_CrtMask);
        float4 _CrtMask_TexelSize;

        float4 _Resolution;
        float4 _ColorQuantizationBuckets;

        float _Brightness;
        float _Contrast;
        float _Saturation;

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
            float2 pr = i.texcoord * _Resolution.xy;
            float2 puv = pr/_Resolution.xy;
            float2 ps = i.texcoord * _MainTex_TexelSize.zw;

            float2 dist = abs(pr - (floor(pr) + 0.5));

            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, floor(pr)/_Resolution.xy);

            #if defined(_USECRTMASK_ON)
                col.rgb *= SAMPLE_TEXTURE2D(_CrtMask, sampler_CrtMask, i.texcoord*_MainTex_TexelSize.zw/_CrtMask_TexelSize.zw);
            #endif

            #if defined(_USECOLORQUANT_ON)
                col.rgb = round(col.rgb * (_ColorQuantizationBuckets.rgb - 1)) / (_ColorQuantizationBuckets.rgb - 1);
            #endif

            col.rgb = Brightness(col.rgb, _Brightness);
            col.rgb = Contrast(col.rgb, _Contrast);
            col.rgb = Saturation(col.rgb, _Saturation);

            float sl = ScanlinePow(dist.y, 3, 0.5);
            return col * sl;
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