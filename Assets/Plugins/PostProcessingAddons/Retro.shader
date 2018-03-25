Shader "Hidden/Custom/Retro"
{
    HLSLINCLUDE
        #pragma multi_compile __ _USECOLORQUANT_ON
        #pragma multi_compile __ _USECRTMASK_ON

        #include "PostProcessing/Shaders/StdLib.hlsl"
        #include "../ShaderToolkit/Shaders/FastMath.cginc"
        #include "../ShaderToolkit/Shaders/ImageProcessing.cginc"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        float4 _GameResolution;

        float4 _CRTResolution;
        float4 _CRTMaskSizePixels;
        TEXTURE2D_SAMPLER2D(_CRTMask, sampler_CRTMask);
        float4 _CRTMask_TexelSize;
        float4 _CRTMaskWeight;
        float _CRTBarrelDistortion;

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

        float Scanline(float dist, float size, float brightness)
        {
            return ScanlinePow(dist, size, brightness);
        }

        float2 BarrelDistortion(float2 v, float amount) 
        {
            float d = dot(v, v);
            return v * (d + amount * d * d) * amount;
        }

        float Vignette(float2 uv, float aspect, float radius, float slope, float amount)
        {
            float2 uv_c = uv - 0.5;
            uv_c *= float2(1.0/aspect, 1.0/aspect);
            uv_c /= radius;
            float t = dot_sat(uv_c, uv_c);

            return saturate(1.0 + pow(t, slope * 0.5) * amount);
        }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 uv = i.texcoord;
            float2 uv_c = uv - 0.5;

            float2 barrel = BarrelDistortion(uv_c, _CRTBarrelDistortion);
            float2 uv_barrel = uv + barrel;
            float2 uv_min = float2(0.0, 0.0);
            float2 uv_max = float2(1.0, 1.0);

            //todo: pass up precalulated
            float crt_aspect = _CRTResolution.x / _CRTResolution.y;

            //apply vignetting
            float dim = Vignette(uv_barrel, crt_aspect, 0.7, 2.0, -2.0);

            if ( any(step(uv_max, uv_barrel)) )
                return float4(0.0, 0.0, 0.0, 0.0);
            if ( any(step(uv_barrel, uv_min)) )
                return float4(0.0, 0.0, 0.0, 0.0);

            //"game" rendering things
            float2 g_ps = uv_barrel * _MainTex_TexelSize.zw;
            float2 g_pr = uv_barrel * _GameResolution.xy;
            float2 g_puv = floor(g_pr)/_GameResolution.xy; 

            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, g_puv);

            #if defined(_USECOLORQUANT_ON)
                col.rgb = round(col.rgb * (_ColorQuantizationBuckets.rgb - 1)) / (_ColorQuantizationBuckets.rgb - 1);
            #endif

            //CRT things
            float2 c_pr = uv_barrel * _CRTResolution.xy;
            
            #if defined(_USECRTMASK_ON)
                float2 subPixelStep = _CRTMask_TexelSize.zw / _CRTMaskSizePixels.xy;
                float2 maskUV = c_pr/subPixelStep;
                col.rgb *= (saturate(SAMPLE_TEXTURE2D(_CRTMask, sampler_CRTMask, maskUV).rgb) / _CRTMaskWeight.rgb);
            #endif

            col.rgb = Brightness(col.rgb, _Brightness);
            col.rgb = Contrast(col.rgb, _Contrast);
            col.rgb = Saturation(col.rgb, _Saturation);

            //TODO: make part of crt mask?
            float sl_dist = abs(frac(c_pr.y) - 0.5);
            float sl = Scanline(sl_dist, 7.5, 0.1);

            return col * dim * sl;// * dim;
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