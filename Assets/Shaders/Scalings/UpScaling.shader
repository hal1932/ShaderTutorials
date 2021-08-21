Shader "Hidden/UpScaling"
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
            Name "Blur"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentBlurUpScale

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "UpScalingInput.hlsl"
            #include "UpScalingPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "GaussianHorizontal"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentGaussianHorizontal

            #pragma shader_feature_local GAUSSIAN_KERNEL_3 GAUSSIAN_KERNEL_5 GAUSSIAN_KERNEL_7

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "UpScalingInput.hlsl"
            #include "UpScalingPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "GaussianVertical"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentGaussianVertical

            #pragma shader_feature_local GAUSSIAN_KERNEL_3 GAUSSIAN_KERNEL_5 GAUSSIAN_KERNEL_7

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "UpScalingInput.hlsl"
            #include "UpScalingPass.hlsl"
            ENDHLSL
        }
    }
}
//meyupo


