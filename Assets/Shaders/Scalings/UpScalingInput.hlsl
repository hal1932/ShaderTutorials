#include "Assets/Shaders/Utils/FullScreenInput.hlsl"
#include "Assets/Shaders/Blurs/GaussianBlurConstant.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(linear_clamp_sampler);
SAMPLER(point_clamp_sampler);

float UvOffsetSize;
