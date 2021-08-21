using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes;
using Volumes.Colors;

namespace RendererFeatures.Colors
{
    public class ColorCorrectionRenderPass : RenderPassBase
    {
        public ColorCorrectionRenderPass()
        {
            TVolume GetVolume<TVolume>()
                where TVolume : VolumeComponentBase
            {
                var volume = VolumeManager.instance.stack.GetComponent<TVolume>();
                _volumes.Add(volume);
                return volume;
            }

            _tone = GetVolume<ToneCurve>();
            _levels = GetVolume<Levels>();
            _brightnessContrast = GetVolume<BrightnessContrast>();
            _exposure = GetVolume<Exposure>();
            _lut = GetVolume<Lut>();

            _altVolume = GameObject.FindObjectOfType<AltVolume>();

            var shader = Shader.Find("Hidden/ColorCorrection");
            _material = CoreUtils.CreateEngineMaterial(shader);

            _toneCurveLut = new Texture2D(256, 1, TextureFormat.R16, false);
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_volumes.All(v => !v.IsActive()))
            {
                return;
            }

            var cmd = CommandBufferPool.Get("ColorCorrection");
            using (new CommandBufferFromPoolScope(cmd))
            {
                var target = RenderTargets[0];
                var targetDesc = RenderTargetDescs[0];

                using (var temp = new TempRenderTarget(cmd, targetDesc, "Temp"))
                {
                    var isToneCurveEnabled = _tone?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_TONE_CURVE", isToneCurveEnabled);
                    if (isToneCurveEnabled)
                    {
                        _tone.CalcCurveLut(_toneCurveLut);// 重い
                        _material.SetTexture("ToneCurveLut", _toneCurveLut);
                    }

                    var isLevelEnabled = _levels?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_LEVELS", isLevelEnabled);
                    if (isLevelEnabled)
                    {
                        _material.SetFloat("LevelsShadow", _levels.Shadow.value);
                        _material.SetFloat("LevelsHighlight", _levels.Highlight.value);
                        _material.SetFloat("LevelsBlackPoint", _levels.BlackPoint.value);
                        _material.SetFloat("LevelsWhitePoint", _levels.WhitePoint.value);
                    }

                    var isBrightnessContrastEnabled = _brightnessContrast?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_BRIGHTNESS_CONTRAST", isBrightnessContrastEnabled);
                    if (isBrightnessContrastEnabled)
                    {
                        _material.SetFloat("Brightness", _brightnessContrast.Brightness.value);
                        _material.SetFloat("Contrast", _brightnessContrast.Contrast.value);
                    }

                    var isExposureEnabled = _exposure?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_EXPOSURE", isExposureEnabled);
                    if (isExposureEnabled)
                    {
                        _material.SetFloat("ExposureAmount", _exposure.Amount.value);
                        _material.SetFloat("GammaCorrection", _exposure.GammaCorrection.value);
                    }

                    var isLutEnabled = _lut?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_LUT", isLutEnabled);
                    if (isLutEnabled)
                    {
                        _material.SetTexture("LutTexture", _altVolume.LutTexture);
                    }

                    cmd.Blit(target, temp, _material);
                    cmd.Blit(temp, target);
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private List<VolumeComponentBase> _volumes = new List<VolumeComponentBase>();

        private ToneCurve _tone;
        private Levels _levels;
        private BrightnessContrast _brightnessContrast;
        private Exposure _exposure;
        private Lut _lut;
        private AltVolume _altVolume;

        private Material _material;
        private Texture2D _toneCurveLut;
    }

    public class ColorCorrectionRendererFeature : RendererFeatureBase<ColorCorrectionRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
