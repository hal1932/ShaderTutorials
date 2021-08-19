#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half3 ToneCurve(half3 color, TEXTURE2D(lut))
{
    color.r = SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(color.r, 0.0)).r;
    color.g = SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(color.g, 0.0)).r;
    color.b = SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(color.b, 0.0)).r;
    return color;
}

half3 Levels(half3 color, float shadow, float highlight, float blackPoint, float whitePoint)
{
    color = (color - shadow) / (highlight - shadow);
    color = color * (whitePoint - blackPoint) + blackPoint;
    return color;
}

half3 BrightnessContrast(half3 color, float brightness, float contrast)
{
    float C = (1.0 + contrast) / (1.0 - contrast);
    color =  (color - 0.5) * C + 0.5;
    color = color * (1.0 + brightness);
    return color;
}

half3 Exposure(half3 color, float amount, float gammaCorrection)
{
    color = color * pow(2.0, amount);
    color = pow(color, 1.0 + gammaCorrection);
    return color;
}

half3 Lut(half3 color, TEXTURE2D(lut))
{
    color.r = SAMPLE_TEXTURE2D(lut, point_clamp_sampler, float2(color.r, 0.875)).r;
    color.g = SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(color.g, 0.625)).g;
    color.b = SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(color.b, 0.375)).b;
    return color;
}

half4 Fragment(Varyings input) : SV_Target
{
    //return half4(input.uv, 0, 1);
    half4 color = SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv);

    #if ENABLE_TONE_CURVE
        color.rgb = ToneCurve(color.rgb, ToneCurveLut);
    #endif

    #if ENABLE_LEVELS
        color.rgb = Levels(color.rgb, LevelsShadow, LevelsHighlight, LevelsBlackPoint, LevelsWhitePoint);
    #endif

    #if ENABLE_BRIGHTNESS_CONTRAST
        color.rgb = BrightnessContrast(color.rgb, Brightness, Contrast);
    #endif

    #if ENABLE_EXPOSURE
        color.rgb = Exposure(color.rgb, ExposureAmount, GammaCorrection);
    #endif

    #if ENABLE_LUT
        color.rgb = Lut(color.rgb, LutTexture);
    #endif

    return color;
}
