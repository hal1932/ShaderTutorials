#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half ToneCurveByChannel(half value, TEXTURE2D(lut))
{
    return SAMPLE_TEXTURE2D(lut, linear_clamp_sampler, float2(value, 0.0)).r;
}

half4 ToneCurve(half4 color, TEXTURE2D(lut))
{
    color.r = ToneCurveByChannel(color.r, lut);
    color.g = ToneCurveByChannel(color.g, lut);
    color.b = ToneCurveByChannel(color.b, lut);
    return color;
}

half LevelsByChannel(half value, float shadow, float highlight, float blackPoint, float whitePoint)
{
    value = (value - shadow) / (highlight - shadow);
    value = value * (whitePoint - blackPoint) + blackPoint;
    return value;
}

half4 Levels(half4 color, float shadow, float highlight, float blackPoint, float whitePoint)
{
    color.r = LevelsByChannel(color.r, shadow, highlight, blackPoint, whitePoint);
    color.g = LevelsByChannel(color.g, shadow, highlight, blackPoint, whitePoint);
    color.b = LevelsByChannel(color.b, shadow, highlight, blackPoint, whitePoint);
    return color;
}

half4 Fragment(Varyings input) : SV_Target
{
    half4 color = SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv);

    #if ENABLE_TONE_CURVE
        color = ToneCurve(color, ToneCurveLut);
    #endif

    #if ENABLE_LEVELS
        color = Levels(color, LevelsShadow, LevelsHighlight, LevelsBlackPoint, LevelsWhitePoint);
    #endif

    #if ENABLE_BRIGHTNESS_CONTRAST
    #endif

    #if ENABLE_EXPOSURE
    #endif

    #if ENABLE_HSL
    #endif



    return color;
}
