#include "Assets/Shaders/Utils/FullScreenVert.hlsl"

half4 Fragment(Varyings input) : SV_Target
{
    return half4(1, 0, 0, 1);
}
