using System;
using UnityEngine.Rendering;

namespace Volumes.Colors
{
    [Serializable]
    public class BrightnessContrast : VolumeComponentBase
    {
        public BoolParameter IsEnabled = new BoolParameter(false);
        public ClampedFloatParameter Brightness = new ClampedFloatParameter(0, -0.5f, 1.0f);
        public ClampedFloatParameter Contrast = new ClampedFloatParameter(0, -0.5f, 0.5f);

        public override bool IsActive() => IsEnabled.value;
    }
}
