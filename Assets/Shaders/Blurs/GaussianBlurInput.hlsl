struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
};

struct Varyings
{
    half4 positionCS    : SV_POSITION;
    half2 uv            : TEXCOORD0;
};

TEXTURE2D(_MainTex);
SAMPLER(linear_clamp_sampler);

#include "Assets/Shaders/Utils/GaussianDef.hlsl"
float Weights[GAUSSIAN_WEIGHTS_MAX];
int Radius;
