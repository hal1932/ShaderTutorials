using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class Lut : VolumeComponent, IPostProcessComponent
    {
        public TextureParameter Texture = new TextureParameter(null);

        public bool IsActive() => Texture.value != null;
        public bool IsTileCompatible() => false;
    }
}
