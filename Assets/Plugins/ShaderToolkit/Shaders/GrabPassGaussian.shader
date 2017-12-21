Shader "Cracktron\GrabPassGaussian"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        GrabPass { "_BGTexH" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Gaussian.cginc"

            sampler2D _BGTexH;
            float4 _BGTexH_TexelSize;

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }
 
            float4 frag(v2f i) : SV_Target
            {
                return gaussian(_BGTexH, _BGTexH_TexelSize.xy, i.grabPos.xy/i.grabPos.w, float2(1.0, 0.0));
            }
            ENDCG
        }
        Tags { "Queue" = "Transparent" }
        GrabPass { "_BGTexV" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Gaussian.cginc"

            sampler2D _BGTexV;
            float4 _BGTexV_TexelSize;

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return gaussian(_BGTexV, _BGTexV_TexelSize.xy, i.grabPos.xy/i.grabPos.w, float2(0.0, 1.0));
            }
            ENDCG
        }
    }
}