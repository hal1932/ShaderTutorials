using UnityEngine;
using UnityEngine.Rendering;

namespace Renderers.Blurs
{
    public struct HexaBlurContext : IRendererContext
    {
        public RenderTargetIdentifier RenderTarget;

        public uint Radius;
        public float StepScale;

        public int WorkBuffer0;
        public int WorkBuffer1;
        public RenderTextureDescriptor WorkBufferDesc;

        public bool NeedToUpdateWorkBuffers()
        {
            return true;
        }
    }

    public struct HexaBlurContextOperator
    {
        public void UpdateWorkBufferDescs(ref HexaBlurContext context)
        {
            context.WorkBuffer0 = Shader.PropertyToID("WorkBuffer0");
            context.WorkBuffer1 = Shader.PropertyToID("WorkBuffer1");
        }

        public void GetWorkBuffers(ref HexaBlurContext context, CommandBuffer cmd)
        {
            var desc = context.WorkBufferDesc;
            cmd.GetTemporaryRT(context.WorkBuffer0, desc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(context.WorkBuffer1, desc, FilterMode.Bilinear);
        }

        public void ReleaseWorkBuffers(ref HexaBlurContext context, CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(context.WorkBuffer0);
            cmd.ReleaseTemporaryRT(context.WorkBuffer1);
        }
    }

    public class HexaBlurRenderer : RendererBase<HexaBlurContext>
    {
        public void CreateMaterial()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/HexaBlur");
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnRender(ref HexaBlurContext context, CommandBuffer cmd)
        {
            _material.SetInt("Radius", (int)context.Radius);
            _material.SetFloat("StepScale", context.StepScale);
            cmd.Blit(context.RenderTarget, context.WorkBuffer0, _material, 0);
            cmd.Blit(context.WorkBuffer0, context.WorkBuffer1, _material, 1);
            cmd.Blit(context.WorkBuffer1, context.RenderTarget, _material, 2);
        }

        private Material _material;
    }
}
