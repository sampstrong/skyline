Shader "MyShaders/ScanningShader02"
{
    Properties
    {
        _WaveTex ("Wave Texture", 2D) = "black" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _InnerAlphaTex ("Inner Alpha Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            struct VertexOutput
            {
                float4 clipSpacePos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
            };

            sampler2D _WaveTex;
            sampler2D _AlphaTex;
            float4 _AlphaTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            sampler2D _InnerAlphaTex;
            float4 _Color;

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.uv1 = TRANSFORM_TEX(v.uv1, _AlphaTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (VertexOutput o) : SV_Target
            {
                // Wave Parameters
                float waveMap = tex2D(_WaveTex, o.uv0).x;
                float waveSize = 0.3;
                float shape = waveMap;
                float speed = 0.25;

                // Wave Creation and Animation
                float waveAmp = float(1 - frac(shape / waveSize + _Time.y * speed));
                waveAmp *= waveMap;

                // Grid Alpha
                float4 alphaMap = tex2D(_AlphaTex, o.uv1);

                // Inner Alpha
                float4 innerAlpha = tex2D(_InnerAlphaTex, o.uv0);

                // Noise Parameters
                float noiseTexture = tex2D(_NoiseTex, o.uv2);
                float noiseSize = 0.05;
                float noiseSpeed = 5;

                // Noise Animation
                float noise = float(sin(noiseTexture / noiseSize + _Time.y * noiseSpeed) + 1) * 0.5;
                
                // Multiplying Alphas
                float alpha = waveAmp * alphaMap * noise * innerAlpha;

                // Output
                return float4(_Color.rgb, alpha);
                
                return waveAmp;
                
                return waveMap;

                return frac(_Time.y);
            }
            ENDCG
        }
    }
}
