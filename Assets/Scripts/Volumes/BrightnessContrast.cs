using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class BrightnessContrast : VolumeComponent, IPostProcessComponent
    {
        public ClampedIntParameter Brightness = new ClampedIntParameter(0, -150, 150);
        public ClampedIntParameter Contrast = new ClampedIntParameter(0, -50, 100);

        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}
