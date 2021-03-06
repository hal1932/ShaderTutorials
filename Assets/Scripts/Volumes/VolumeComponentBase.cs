using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    public abstract class VolumeComponentBase : VolumeComponent, IPostProcessComponent
    {
        public abstract bool IsActive();
        public virtual bool IsTileCompatible() => false;
    }
}
