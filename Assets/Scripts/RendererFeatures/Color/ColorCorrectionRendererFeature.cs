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
            _hsl = VolumeManager.instance.stack.GetComponent<HSL>();

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
                        cmd.SetGlobalTexture("ToneCurveLut", _toneCurveLut);
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
                    }

                    var isExposureEnabled = _exposure?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_EXPOSURE", isExposureEnabled);
                    if (isExposureEnabled)
                    {
                    }

                    var isHslEnabled = _hsl?.IsActive() ?? false;
                    _material.SetKeyword("ENABLE_HSL", isHslEnabled);
                    if (isHslEnabled)
                    {
                    }

                    cmd.Blit(target, temp);
                    cmd.Blit(temp, target, _material);
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private ToneCurve _tone;
        private Levels _levels;
        private BrightnessContrast _brightnessContrast;
        private Exposure _exposure;
        private HSL _hsl;

        private Material _material;
        private Texture2D _toneCurveLut;
    }

    public class ColorCorrectionRendererFeature : RendererFeatureBase<ColorCorrectionRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
