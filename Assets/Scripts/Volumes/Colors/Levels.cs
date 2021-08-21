using System;
using UnityEngine.Rendering;

namespace Volumes.Colors
{
    [Serializable]
    public class Levels : VolumeComponentBase
    {
        public BoolParameter IsEnabled = new BoolParameter(false);
        public ClampedFloatParameter Shadow = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Highlight = new ClampedFloatParameter(1, 0, 1);
        public ClampedFloatParameter BlackPoint = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter WhitePoint = new ClampedFloatParameter(1, 0, 1);

        public override bool IsActive() => IsEnabled.value;
    }
}
