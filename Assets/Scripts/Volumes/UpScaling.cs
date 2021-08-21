using System;
using UnityEngine.Rendering;

namespace Volumes
{
    public enum UpScalingMethod
    {
        Blinear,
        BlurredBilinear,
        Gaussian,
    }

    [Serializable]
    public class UpScalingMethodParameter : VolumeParameter<UpScalingMethod>
    {
        public UpScalingMethodParameter(UpScalingMethod method)
            : base(method, false)
        { }
    }

    [Serializable]
    public class UpScaling : VolumeComponentBase
    {
        public UpScalingMethodParameter Method = new UpScalingMethodParameter(UpScalingMethod.Blinear);
        public ClampedFloatParameter UvOffsetUnit = new ClampedFloatParameter(1.0f, 0.0f, 2.0f);
        public ClampedIntParameter GaussianRadius = new ClampedIntParameter(2, 1, 3);

        public static readonly int[] GaussianKernelSizeCandidates = new[] { 3, 5, 7 };
        public int GaussianKernelSize => GaussianKernelSizeCandidates[GaussianRadius.value - 1];

        public override bool IsActive() => Method.overrideState;
    }
}
