Shader "Hidden/DepthOfField"
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
            Name ""
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment Fragment

            #pragma shader_feature_local DOF_METHOD_GAUSSIAN DOF_METHOD_KAWASE DOF_METHOD_HEXA
            #pragma shader_feature_local DOF_BOKEH_BACK DOF_BOKEH_FRONT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "DepthOfFieldInput.hlsl"
            #include "DepthOfFieldPass.hlsl"
            ENDHLSL
        }
    }
}
//meyupo


