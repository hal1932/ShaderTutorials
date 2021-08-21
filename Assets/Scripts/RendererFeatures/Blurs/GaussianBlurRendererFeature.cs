using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes.Blurs;

namespace RendererFeatures.Blurs
{
    public class GanssianBlurRenderPass : RenderPassBase<GaussianBlur>
    {
        public GanssianBlurRenderPass()
        {
            var shader = Shader.Find("Hidden/GaussianBlur");
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            UpdateParameters();

            var cmd = CommandBufferPool.Get("GaussianBlur");
            using (new CommandBufferFromPoolScope(cmd))
            {
                var target = RenderTargets[0];
                var targetDesc = RenderTargetDescs[0];

                using (var temp = new TempRenderTarget(cmd, targetDesc, "Temp"))
                {
                    cmd.Blit(target, temp, _material, 0);
                    cmd.Blit(temp, target, _material, 1);
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private void UpdateParameters()
        {
            _currentMethod = Volume.Method.value;

            switch (_currentMethod)
            {
                case GaussianMethod.PascalsTriangle:
                    _material.SetKeyword("GAUSSIAN_METHOD_PASCALS", true);
                    _material.SetKeyword("GAUSSIAN_METHOD_WEIGHTS", false);
                    Array.ForEach(
                        GaussianBlur.PascalsRadiusCandidates,
                        size => _material.SetKeyword($"GAUSSIAN_KERNEL_{size}", size == Volume.PascalsRadius));
                    break;

                case GaussianMethod.Weights:
                    _material.SetKeyword("GAUSSIAN_METHOD_PASCALS", false);
                    _material.SetKeyword("GAUSSIAN_METHOD_WEIGHTS", true);
                    Volume.CalcWeights(_weights);
                    _material.SetFloatArray("Weights", _weights);
                    _material.SetFloat("Radius", Volume.Radius.value);
                    break;
            }
        }

        private Material _material;
        private GaussianMethod _currentMethod;

        private const int GAUSSIAN_WEIGHTS_MAX = 16;
        private float[] _weights = new float[GAUSSIAN_WEIGHTS_MAX];
    }

    public class GaussianBlurRendererFeature : RendererFeatureBase<GanssianBlurRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
