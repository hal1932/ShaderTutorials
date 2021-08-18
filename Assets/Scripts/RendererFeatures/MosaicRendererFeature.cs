using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RendererFeatures
{
    public class MosaicRenderPass : RenderPassBase
    {
        protected override void OnUpdateRenderTargets()
        {
            _tempDesc = RenderTargetDescs[0];
            _tempDesc.width /= 16;
            _tempDesc.height /= 16;
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Mosaic");
            using (new CommandBufferFromPoolScope(cmd))
            {
                using (var temp = new TempRenderTarget(cmd, _tempDesc, _tempTargetName))
                {
                    cmd.Blit(RenderTargets[0], temp);
                    cmd.Blit(temp, RenderTargets[0]);
                }
                context.ExecuteCommandBuffer(cmd);
            }
        }

        private RenderTextureDescriptor _tempDesc;
        private readonly int _tempTargetName = Shader.PropertyToID("Mosaic");
    }

    public class MosaicRendererFeature : RendererFeatureBase<MosaicRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
