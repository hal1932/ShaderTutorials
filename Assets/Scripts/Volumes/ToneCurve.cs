using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Volumes
{
    [Serializable]
    public class ToneCurve : VolumeComponentBase
    {
        public AnimationCurveParameter Curve = new AnimationCurveParameter(null);

        public bool CalcCurveLut(Texture2D curve)
        {
            if (!Curve.overrideState)
            {
                return false;
            }
            for (var i = 0; i < curve.width; ++i)
            {
                var value = Curve.value.Evaluate((float)i / curve.width);
                curve.SetPixel(i, 0, new Color(value, 0, 0, 0));
            }
            curve.Apply();
            return true;
        }

        public override bool IsActive() => Curve.value != null;
    }
}
