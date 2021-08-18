Shader "Hidden/GaussianBlur"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Horizontal"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentHorizontal

            #pragma shader_feature_local GAUSSIAN_METHOD_PASCALS GAUSSIAN_METHOD_WEIGHTS
            #pragma shader_feature_local GAUSSIAN_KERNEL_3 GAUSSIAN_KERNEL_5 GAUSSIAN_KERNEL_7

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "GaussianBlurInput.hlsl"
            #include "GaussianBlurPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Vertical"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentVertical

            #pragma shader_feature_local GAUSSIAN_METHOD_PASCALS GAUSSIAN_METHOD_WEIGHTS
            #pragma shader_feature_local GAUSSIAN_KERNEL_3 GAUSSIAN_KERNEL_5 GAUSSIAN_KERNEL_7

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "GaussianBlurInput.hlsl"
            #include "GaussianBlurPass.hlsl"
            ENDHLSL
        }
    }
}
//meyupo


