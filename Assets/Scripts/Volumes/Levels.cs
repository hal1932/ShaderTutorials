﻿using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    [Serializable]
    public class Levels : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter Shadow = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter Highlight = new ClampedFloatParameter(1, 0, 1);
        public ClampedFloatParameter BlackPoint = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter WhitePoint = new ClampedFloatParameter(1, 0, 1);

        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}