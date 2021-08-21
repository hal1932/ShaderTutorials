using System;
using UnityEngine.Rendering;

namespace Volumes
{
    public enum GaussianMethod
    {
        PascalsTriangle,
        Weights,
    }

    [Serializable]
    public class GaussianMethodParameter : VolumeParameter<GaussianMethod>
    {
        public GaussianMethodParameter(GaussianMethod method)
            : base(method, false)
        { }
    }

    [Serializable]
    public class GaussianBlur : VolumeComponentBase
    {
        public GaussianMethodParameter Method = new GaussianMethodParameter(GaussianMethod.PascalsTriangle);
        public ClampedIntParameter Radius = new ClampedIntParameter(1, 1, 9);
        public ClampedFloatParameter Sigma = new ClampedFloatParameter(0.01f, 0.01f, 5.0f);

        public static readonly int[] PascalsRadiusCandidates = new[] { 3, 5, 7 };
        public int PascalsRadius => PascalsRadiusCandidates[Math.Min(Radius.value - 1, PascalsRadiusCandidates.Length - 1)];

        public bool CalcWeights(float[] weights)
        {
            if (_lastRadius == Radius.value && _lastSigma == Sigma.value)
            {
                return false;
            }
            _lastRadius = Radius.value;
            _lastSigma = Sigma.value;

            var sigma2 = Sigma.value * Sigma.value;
            var total = 0.0f;
            for (var i = 0; i < Radius.value; ++i)
            {
                weights[i] = (float)Math.Exp(-(i * i) / sigma2);
                total += weights[i] * (i == 0 ? 1 : 2);
            }
            for (var i = 0; i < Radius.value; ++i)
            {
                weights[i] = weights[i] / total;
            }
            //for (var i = Radius.value; i < weights.Length; ++i)
            //{
            //    weights[i] = 0.0f;
            //}

            return true;
        }

        public override bool IsActive() => Method.overrideState;

        private int _lastRadius;
        private float _lastSigma;
    }
}
