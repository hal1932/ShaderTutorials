struct Attributes {
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
};

struct Varyings {
    half4 positionCS    : SV_POSITION;
    half2 uv            : TEXCOORD0;
};
