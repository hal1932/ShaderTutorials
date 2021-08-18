Shader "Hidden/ColorCorrection"
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
            Name "ColorCorrection"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullScreenVert
            #pragma fragment Fragment
   
            #pragma shader_feature_local ENABLE_TONE_CURVE
            #pragma shader_feature_local ENABLE_LEVELS
            #pragma shader_feature_local ENABLE_BRIGHTNESS_CONTRAST
            #pragma shader_feature_local ENABLE_EXPOSURE
            #pragma shader_feature_local ENABLE_HSL

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "ColorCorrectionInput.hlsl"
            #include "ColorCorrectionPass.hlsl"
            ENDHLSL
        }
    }
}
//meyupo


