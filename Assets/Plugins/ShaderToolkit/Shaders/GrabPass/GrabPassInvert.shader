Shader "Cracktron\GrabPass\Invert"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        GrabPass { "_BGTex" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BGTex;
            float4 _BGTex_TexelSize;

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
                float4 bgcolor = tex2Dproj(_BGTex, i.grabPos);
                return 1 - bgcolor;
            }
            ENDCG
        }
    }
}