#include "Assets/Shaders/Utils/FullScreenVert.hlsl"
#include "Assets/Shaders/Utils/Gaussian.hlsl"

half4 Fragment(float2 uv, float2 offsetUnit)
{
    #if defined(GAUSSIAN_METHOD_PASCALS)
        return CalcGaussianFromPascalsTriangle(TEXTURE2D_ARGS(_MainTex, linear_clamp_sampler), uv, offsetUnit);
    #elif defined(GAUSSIAN_METHOD_WEIGHTS)
        return CalcGaussianFromWeights(TEXTURE2D_ARGS(_MainTex, linear_clamp_sampler), uv, offsetUnit, Weights, Radius);
    #endif
}

half4 FragmentHorizontal(Varyings input) : SV_Target
{
    float width = _ScreenParams.x;
    float2 offsetUnit = float2(2.0 / width, 0.0);
    return Fragment(input.uv, offsetUnit);
}

half4 FragmentVertical(Varyings input) : SV_Target
{
    float height = _ScreenParams.x;
    float2 offsetUnit = float2(0.0, 2.0 / height);
    return Fragment(input.uv, offsetUnit);
}
