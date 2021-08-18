using UnityEngine.Rendering.Universal;

namespace RendererFeatures
{
    public abstract class RendererFeatureBase<TRenderPass> : ScriptableRendererFeature
        where TRenderPass : RenderPassBase, new()
    {
        protected abstract RenderPassEvent ExecutionOrder { get; }
        protected abstract bool CanExecuteInSceneView { get; }

        protected TRenderPass RenderPass { get; private set; }

        protected virtual void OnCreate() { }
        protected virtual void OnAddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) { }

        public override void Create()
        {
            RenderPass = new TRenderPass();
            RenderPass.renderPassEvent = ExecutionOrder;
            OnCreate();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!CanExecuteInSceneView && renderingData.cameraData.isSceneViewCamera)
            {
                return;
            }

            RenderPass.SetRenderTarget(renderer.cameraColorTarget, renderingData.cameraData.cameraTargetDescriptor);
            OnAddRenderPasses(renderer, ref renderingData);
            renderer.EnqueuePass(RenderPass);
        }
    }
}
