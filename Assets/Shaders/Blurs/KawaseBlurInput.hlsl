#include "Assets/Shaders/Utils/FullScreenInput.hlsl"

TEXTURE2D(_MainTex);

// URP��Texture2DArray�T�|�[�g���悭�킩��Ȃ�
//TEXTURE2D_ARRAY(WorkBuffers)
TEXTURE2D(Texture1);
TEXTURE2D(Texture3);
TEXTURE2D(Texture5);
TEXTURE2D(Texture7);
TEXTURE2D(Texture9);
int BlurSize;

SAMPLER(linear_clamp_sampler);
SAMPLER(point_clamp_sampler);
