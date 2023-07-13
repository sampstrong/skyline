Shader "MyShaders/Occlusion"
{
    SubShader
    {
        // Will only write to the depth buffer
        Tags { "Queue"="Transparent-1" }
        ZWrite On
        ColorMask 0
        Cull Off
        
        Pass {}
    }
}
