using System;
using UnityEngine.Rendering;

namespace Volumes.Blurs
{
    [Serializable]
    public class MotionBlur : VolumeComponentBase
    {
        public BoolParameter IsEnabled = new BoolParameter(false);

        public override bool IsActive() => IsEnabled.value;
    }
}
