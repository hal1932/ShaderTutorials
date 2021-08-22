#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half4 Fragment(float2 uv, float2 uvOffsetUnit)
{
    uvOffsetUnit *= 3.0;
    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit);

    for (int i = 0; i < Radius; ++i) {
        float step = i + 2;
        col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + uvOffsetUnit * step);
    }
    return col / (Radius + 1);
}

half4 FragmentUp(Varyings input) : SV_Target
{
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(0.0, 1.0 / height) * StepScale);
}

half4 FragmentLeft(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(-0.86602 / width, -0.5 / height) * StepScale);
}

half4 FragmentRight(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float height = _ScreenParams.y;
    return Fragment(input.uv, float2(0.86602 / width, -0.5 / height) * StepScale);
}
