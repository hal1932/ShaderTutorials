using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class BrightnessContrast : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter IsEnabled = new BoolParameter(false);
        public ClampedFloatParameter Brightness = new ClampedFloatParameter(0, -0.5f, 1.0f);
        public ClampedFloatParameter Contrast = new ClampedFloatParameter(0, -0.5f, 0.5f);

        public bool IsActive() => IsEnabled.value;
        public bool IsTileCompatible() => false;
    }
}
