#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half4 Fragment(float2 uv, float2 uvOffsetUnit)
{
    uvOffsetUnit *= 3.0;
    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit);
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit * 2);
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit * 3);
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit * 4);
    return col * 0.25;
}

half4 FragmentUp(Varyings input) : SV_Target
{
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(0.0, 1.0 / height));
}

half4 FragmentLeft(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(-0.86602 / width, -0.5 / height));
}

half4 FragmentRight(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(0.86602 / width, -0.5 / height));
}
