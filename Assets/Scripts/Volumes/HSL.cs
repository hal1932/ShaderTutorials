using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class HSL : VolumeComponent, IPostProcessComponent
    {
        public ClampedIntParameter Hue = new ClampedIntParameter(0, -180, 180);
        public ClampedIntParameter Saturation = new ClampedIntParameter(0, -100, 100);
        public ClampedIntParameter Lightness = new ClampedIntParameter(0, -100, 100);

        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}
