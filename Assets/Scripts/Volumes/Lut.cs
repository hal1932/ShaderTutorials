using System;
using UnityEngine.Rendering;

namespace Volumes
{
    [Serializable]
    public class Lut : VolumeComponentBase
    {
        // TextureParameter.value 経由で正しくデータを取得設定できないことが多いのでAltVolume.csに移動
        //public TextureParameter Texture = new TextureParameter(null);
        public BoolParameter IsEnabled = new BoolParameter(false);

        //public bool IsActive() => Texture.value != null;
        public override bool IsActive() => IsEnabled.value;
    }
}
