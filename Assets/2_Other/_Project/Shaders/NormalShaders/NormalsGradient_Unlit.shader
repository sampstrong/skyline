Shader "MyShaders/NormalsGradient"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 clipSpacePos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (VertexOutput o) : SV_Target
            {
                float2 uv = o.uv0;
                float3 normal = o.normal * 0.5 + 0.5;
                return float4 (normal, 0);
            }
            ENDCG
        }
    }
}
