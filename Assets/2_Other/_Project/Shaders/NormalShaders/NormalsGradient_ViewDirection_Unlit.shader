Shader "MyShaders/NormalsGradient_ViewDirection_Unlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _MainTex ("Texture", 2D) = "white" {}
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
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

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
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (VertexOutput o) : SV_Target
            {
                float2 uv = o.uv0;

                float tex = tex2D(_MainTex, uv);

                // return float4 (o.worldPos, 1);

                // Lighting

                // Direct Light
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float lightFalloff = max(0, dot(lightDir, o.normal));
                float3 directDiffuseLight = lightColor * lightFalloff;

                // Ambient Light
                float3 ambientLight = float3(0.1, 0.1, 0.1);

                // Direct Specular Light
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - o.worldPos;
                float3 viewDir = normalize(fragToCam) * 0.5 + 0.5;

                float3 composite = viewDir * tex;

                return float4 (composite, 1);

                // Composite
                float diffuseLight = ambientLight + directDiffuseLight;
                float3 finalSurfaceColor = diffuseLight * _Color.rgb;
                
                return float4 (finalSurfaceColor, 0);
            }
            ENDCG
        }
    }
}
