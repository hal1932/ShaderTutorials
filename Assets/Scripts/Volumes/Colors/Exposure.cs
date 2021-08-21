using System;
using UnityEngine.Rendering;

namespace Volumes.Colors
{
    [Serializable]
    public class Exposure : VolumeComponentBase
    {
        public BoolParameter IsEnabled = new BoolParameter(false);
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, -1.0f, 1.0f);
        public ClampedFloatParameter Offset = new ClampedFloatParameter(0, -0.5f, 0.5f);
        public ClampedFloatParameter GammaCorrection = new ClampedFloatParameter(1, 0.01f, 2.0f);

        public override bool IsActive() => IsEnabled.value;
    }
}
