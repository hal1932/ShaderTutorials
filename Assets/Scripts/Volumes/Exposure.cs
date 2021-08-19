using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class Exposure : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter IsEnabled = new BoolParameter(false);
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0, -1.0f, 1.0f);
        public ClampedFloatParameter Offset = new ClampedFloatParameter(0, -0.5f, 0.5f);
        public ClampedFloatParameter GammaCorrection = new ClampedFloatParameter(1, 0.01f, 2.0f);

        public bool IsActive() => IsEnabled.value;
        public bool IsTileCompatible() => false;
    }
}
