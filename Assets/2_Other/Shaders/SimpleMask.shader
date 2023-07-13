Shader "Portal/SimpleMask"
{
    SubShader
    {
        // Will only write to the depth buffer
        Tags { "Queue"="Transparent-1" }
        ZWrite On
        ColorMask 0
        
        Pass {}
    }
}
