using System;
using UnityEngine.Rendering;

namespace Volumes.Blurs
{
    [Serializable]
    public class KawaseBlur : VolumeComponentBase
    {
        public BoolParameter IsEnabled = new BoolParameter(false);

        public override bool IsActive() => IsEnabled.value;
    }
}
