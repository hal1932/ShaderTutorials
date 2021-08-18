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
SAMPLER(point_clamp_sampler);

TEXTURE2D(ToneCurveLut);

float LevelsShadow;
float LevelsHighlight;
float LevelsBlackPoint;
float LevelsWhitePoint;
