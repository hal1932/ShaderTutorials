Shader "Hidden/KawaseBlur"
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
            #pragma fragment FragmentBlur

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "KawaseBlurInput.hlsl"
            #include "KawaseBlurPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Composite"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment FragmentComposite

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "KawaseBlurInput.hlsl"
            #include "KawaseBlurPass.hlsl"
            ENDHLSL
        }
    }
}
//meyupo


