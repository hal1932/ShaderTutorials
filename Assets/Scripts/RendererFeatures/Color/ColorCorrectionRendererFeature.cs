using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes;

namespace RendererFeatures.Color
{
    public class ColorCorrectionRenderPass : RenderPassBase
    {
        public ColorCorrectionRenderPass()
        {
            _tone = VolumeManager.instance.stack.GetComponent<ToneCurve>();
            _levels = VolumeManager.instance.stack.GetComponent<Levels>();
            _brightnessContrast = VolumeManager.instance.stack.GetComponent<BrightnessContrast>();
            _exposure = VolumeManager.instance.stack.GetComponent<Exposure>();
            _lut = VolumeManager.instance.stack.GetComponent<Lut>();

            var shader = Shader.Find("Hidden/ColorCorrection");
            _material = CoreUtils.CreateEngineMaterial(shader);

            _toneCurveLut = new Texture2D(256, 1, TextureFormat.R16, false);

            _t = Resources.Load<Texture2D>("BaseLut");
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
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
                        _tone.CalcCurveLut(_toneCurveLut);
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
                    //_material.SetKeyword("ENABLE_LUT", isLutEnabled);
                    _material.SetKeyword("ENABLE_LUT", true);
                    //if (isExposureEnabled)
                    {
                        //_material.SetTexture("LutTexture", _lut.Texture.value);
                        _material.SetTexture("LutTexture", _t);
                    }

                    cmd.Blit(target, temp, _material);
                    cmd.Blit(temp, target);
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private ToneCurve _tone;
        private Levels _levels;
        private BrightnessContrast _brightnessContrast;
        private Exposure _exposure;
        private Lut _lut;

        private Material _material;
        private Texture2D _toneCurveLut;
        private Texture2D _t;
    }

    public class ColorCorrectionRendererFeature : RendererFeatureBase<ColorCorrectionRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
