using System;
using Renderers.Blurs;
using UnityEngine.Rendering;
using Volumes.Blurs;

namespace Volumes.Opticals
{
    public enum BlurMethod
    {
        Gaussian,
        Kawase,
        Hexa,
    }

    public enum BokehOrientation
    {
        Back,
        Front,
        BackAndFront,
    }

    [Serializable]
    public class BlurMethodParameter : VolumeParameter<BlurMethod>
    {
        public BlurMethodParameter(BlurMethod method)
            : base(method, false)
        { }
    }

    [Serializable]
    public class BokehOrientationParameter : VolumeParameter<BokehOrientation>
    {
        public BokehOrientationParameter(BokehOrientation orientation)
            : base(orientation, false)
        { }
    }

    [Serializable]
    public class DepthOfField : VolumeComponentBase
    {
        public BlurMethodParameter Method = new BlurMethodParameter(BlurMethod.Gaussian);
        public BokehOrientationParameter Orientation = new BokehOrientationParameter(BokehOrientation.Back);

        public GaussianMethodParameter GaussianMethod = new GaussianMethodParameter(GaussianBlurMethod.PascalsTriangle);
        public ClampedIntParameter GaussianRadius = new ClampedIntParameter(1, 1, 9);
        public ClampedFloatParameter GaussianSigma = new ClampedFloatParameter(0.01f, 0.01f, 5.0f);

        public ClampedIntParameter KawaseRadius = new ClampedIntParameter(3, 1, 5);

        public ClampedFloatParameter Aperture = new ClampedFloatParameter(0.5f, 0, 1);
        public ClampedFloatParameter FocalLength = new ClampedFloatParameter(0.5f, 0, 1);
        public ClampedFloatParameter Distance = new ClampedFloatParameter(0.5f, 0, 1);

        public override bool IsActive() => Method.overrideState;
    }
}
