#include "Assets/Shaders/Utils/FullScreenVert.hlsl"
#include "Assets/Shaders/Blurs/GaussianBlurHelper.hlsl"

half4 FragmentBlurUpScale(Varyings input) : SV_Target
{
    float2 screenSize = _ScreenParams.xy;
    float2 uvOffset = UvOffsetSize / screenSize;

    half4 col = half4(0, 0, 0, 0);
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(-1, -1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(1, -1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(-1,  1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(1,  1));
    col *= 0.25;

    return col;
}

half4 FragmentGaussianHorizontal(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float2 offsetUnit = float2(UvOffsetSize / width, 0.0);
    return CalcGaussianFromPascalsTriangle(TEXTURE2D_ARGS(_MainTex, linear_clamp_sampler), input.uv, offsetUnit);
}

half4 FragmentGaussianVertical(Varyings input) : SV_Target
{
    float height = _ScreenParams.y;
    float2 offsetUnit = float2(0.0, UvOffsetSize / height);
    return CalcGaussianFromPascalsTriangle(TEXTURE2D_ARGS(_MainTex, linear_clamp_sampler), input.uv, offsetUnit);
}
