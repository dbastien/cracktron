Shader "Cracktron/Debug/Normals"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
            
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            #pragma target 5.0			
            
            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 color : COLOR0;
            };

            v2f vert (a2v v)
            {
                v2f o;		
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.normal * 0.5 + 0.5;
                
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                return float4(i.color, 1);
            }
            ENDCG
        }
    }
}
