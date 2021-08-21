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
TEXTURE2D(Texture1);
TEXTURE2D(Texture3);
TEXTURE2D(Texture5);
TEXTURE2D(Texture7);
TEXTURE2D(Texture9);
SAMPLER(linear_clamp_sampler);
SAMPLER(point_clamp_sampler);
