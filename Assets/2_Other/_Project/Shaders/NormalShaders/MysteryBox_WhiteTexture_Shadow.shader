Shader "MyShaders/Unlit/Texture"
{
    Properties
    {
        [HDR]_Color("Albedo", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
       
        [Header(Stencil)]
        _Stencil ("Stencil ID [0;255]", Float) = 0
        _ReadMask ("ReadMask [0;255]", Int) = 255
        _WriteMask ("WriteMask [0;255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Int) = 0
       
        [Header(Rendering)]
        _Offset("Offset", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull Mode", Int) = 2
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4
        [Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask("Color Mask", Int) = 15
    }
   
    CGINCLUDE
    #include "UnityCG.cginc"
 
    half4 _Color;
    sampler2D _MainTex;
    float4 _MainTex_ST;
   
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
 
    VertexOutput vert (VertexInput v)
    {
        VertexOutput o;
        o.uv0 = v.uv0;
        o.normal = v.normal;
        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        o.clipSpacePos = UnityObjectToClipPos(v.vertex);
        return o;
    }
   
    half4 frag (VertexOutput o) : SV_Target
    {
        float2 uv = o.uv0;

        float tex = tex2D(_MainTex, uv);

        // Normal To Cam Position
        float3 camPos = _WorldSpaceCameraPos;
        float3 fragToCam = camPos - o.worldPos;
        float3 viewDir = normalize(fragToCam) * 0.5 + 0.5;

        float3 composite = viewDir + tex;

        return float4 (composite, 1);
    }
    
    struct v2fShadow {
        V2F_SHADOW_CASTER;
        UNITY_VERTEX_OUTPUT_STEREO
    };
 
    v2fShadow vertShadow( appdata_base v )
    {
        v2fShadow o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
        return o;
    }
 
    float4 fragShadow( v2fShadow i ) : SV_Target
    {
        SHADOW_CASTER_FRAGMENT(i)
    }
   
    ENDCG
       
    SubShader
    {
        Stencil
        {
            Ref [_Stencil]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }
       
        Pass
        {
            Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
            LOD 100
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
           
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
       
        // Pass to render object as a shadow caster
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            LOD 80
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
           
            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma target 2.0
            #pragma multi_compile_shadowcaster
            ENDCG
        }
    }
}
