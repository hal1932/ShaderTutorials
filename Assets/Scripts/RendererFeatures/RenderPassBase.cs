using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RendererFeatures
{
    public abstract class RenderPassBase : ScriptableRenderPass, IDisposable
    {
        protected int RenderTargetCount { get; private set; }
        protected RenderTargetIdentifier[] RenderTargets { get; private set; } = new RenderTargetIdentifier[1];
        protected RenderTextureDescriptor[] RenderTargetDescs { get; private set; } = new RenderTextureDescriptor[1];

        protected virtual void OnConfigure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) { }
        protected virtual void OnUpdateRenderTargets() { }
        protected abstract void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData);
        protected virtual void OnFrameCleanup(CommandBuffer cmd) { }
        protected virtual void OnCleanup() { }

        public void SetRenderTarget(RenderTargetIdentifier renderTarget, RenderTextureDescriptor renderTargetDesc)
        {
            RenderTargetCount = 1;
            RenderTargets[0] = renderTarget;
            RenderTargetDescs[0] = renderTargetDesc;
            OnUpdateRenderTargets();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            OnConfigure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            OnExecute(context, ref renderingData);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            OnFrameCleanup(cmd);
        }

        #region IDisposable
        ~RenderPassBase()
            => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            OnCleanup();
        }

        private bool _disposed;
        #endregion
    }

    public abstract class RenderPassBase<TVolume> : RenderPassBase
        where TVolume : VolumeComponent, IPostProcessComponent
    {
        public TVolume Volume { get; }

        public RenderPassBase()
        {
            Volume = VolumeManager.instance.stack.GetComponent<TVolume>();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!Volume.IsActive())
            {
                return;
            }

            OnExecute(context, ref renderingData);
        }
    }
}
