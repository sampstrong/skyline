Shader "Tutorials/BasicSpecular"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _Gloss ("Gloss", Float) = 1
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
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;

            float4 _Color;
            float _Gloss;

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (VertexOutput o) : SV_Target
            {
                float2 uv = o.uv0;
                float3 normal = normalize(o.normal); // Interpolated

                // return float4 (o.worldPos, 1);

                // Lighting

                // Direct Light
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float lightFalloff = max(0, dot(lightDir, normal));
                float3 directDiffuseLight = lightColor * lightFalloff;

                // Ambient Light
                float3 ambientLight = float3(0.1, 0.1, 0.1);

                //Direct Specular Light
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - o.worldPos;
                float3 viewDir = normalize(fragToCam);

                float3 viewReflect = reflect(-viewDir, normal);
                

                float specularFalloff = max(0, dot(viewReflect, lightDir));

                specularFalloff = pow(specularFalloff, _Gloss);

                float3 directSpecular = specularFalloff * lightColor;

                //return float4(specularFalloff.xxx, 0);
                
                // Phong
                // Blinn Phong

                

                // Composite
                float diffuseLight = ambientLight + directDiffuseLight;
                float3 finalSurfaceColor = diffuseLight * _Color.rgb + directSpecular;
                
                return float4 (finalSurfaceColor, 0);
            }
            ENDCG
        }
    }
}
