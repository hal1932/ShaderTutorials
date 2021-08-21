#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half4 FragmentBlur(Varyings input) : SV_Target
{
    float2 screenSize = _ScreenParams.xy;
    float2 uvOffset = 0.5 / screenSize;

    half4 col = half4(0, 0, 0, 0);
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(-1, -1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2( 1, -1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2(-1,  1));
    col += SAMPLE_TEXTURE2D(_MainTex, linear_clamp_sampler, input.uv + uvOffset * float2( 1,  1));
    col *= 0.25;

    return col;
}

half4 FragmentComposite(Varyings input) : SV_Target
{
    half4 col = half4(0, 0, 0, 0);
    col += SAMPLE_TEXTURE2D(Texture1, linear_clamp_sampler, input.uv);
    col += SAMPLE_TEXTURE2D(Texture3, linear_clamp_sampler, input.uv);
    col += SAMPLE_TEXTURE2D(Texture5, linear_clamp_sampler, input.uv);
    col += SAMPLE_TEXTURE2D(Texture7, linear_clamp_sampler, input.uv);
    col += SAMPLE_TEXTURE2D(Texture9, linear_clamp_sampler, input.uv);
    return col * 0.2;
    //return SAMPLE_TEXTURE2D(Source4, linear_clamp_sampler, input.uv);
}
