using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes.Blurs;

namespace RendererFeatures.Blurs
{
    public class HexaBlurRenderPass : RenderPassBase<HexaBlur>
    {
        public HexaBlurRenderPass()
        {
            var shader = Shader.Find("Hidden/HexaBlur");
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("HexaBlur");
            using (new CommandBufferFromPoolScope(cmd))
            {
                var target = RenderTargets[0];
                var targetDesc = RenderTargetDescs[0];

                _material.SetInt("Radius", 3);
                _material.SetFloat("StepScale", 1.0f);

                using (var up = new TempRenderTarget(cmd, targetDesc, "Hexa Up"))
                using (var left = new TempRenderTarget(cmd, targetDesc, "Hexa Left"))
                {
                    cmd.Blit(target, up, _material, 0);
                    cmd.Blit(up, left, _material, 1);
                    cmd.Blit(left, target, _material, 2);
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private Material _material;
    }

    public class HexaBlurRendererFeature : RendererFeatureBase<HexaBlurRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
