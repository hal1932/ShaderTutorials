using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Renderers.Blurs
{
    public enum GaussianBlurMethod
    {
        PascalsTriangle,
        Weights,
    }

    public struct GaussianBlurContext : IRendererContext
    {
        public static readonly uint[] AcceptablePascalsRadius = new uint[] { 3, 5, 7 };
        public const int MaxWeightsRadius = 16;

        public RenderTargetIdentifier RenderTarget;

        public int WorkBuffer;
        public RenderTextureDescriptor WorkBufferDesc;

        public GaussianBlurMethod Method;
        public uint Radius;
        public float Sigma;

        public float[] Weights;

        public bool NeedToUpdateWeights(float sigma = default)
        {
            if (Method != GaussianBlurMethod.Weights)
            {
                return true;
            }
            if (Weights == default || Weights.Length != Radius)
            {
                return true;
            }
            if (sigma != default && sigma != Sigma)
            {
                return true;
            }
            return false;
        }
    }

    public struct GaussianBlurContextOperator
    {
        public void UpdateWorkBufferDesc(ref GaussianBlurContext context)
        {
            context.Weights = new float[GaussianBlurContext.MaxWeightsRadius];

            var sigma2 = context.Sigma * context.Sigma;
            var total = 0.0f;
            for (var i = 0; i < context.Radius; ++i)
            {
                context.Weights[i] = (float)Math.Exp(-(i * i) / sigma2);
                total += context.Weights[i] * (i == 0 ? 1 : 2);
            }
            for (var i = 0; i < context.Radius; ++i)
            {
                context.Weights[i] = context.Weights[i] / total;
            }

            context.WorkBuffer = Shader.PropertyToID("Wofk Buffer");
        }

        public void GetWorkBuffer(ref GaussianBlurContext context, CommandBuffer cmd)
        {
            cmd.GetTemporaryRT(context.WorkBuffer, context.WorkBufferDesc, FilterMode.Bilinear);
        }

        public void ReleaseWorkBuffer(ref GaussianBlurContext context, CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(context.WorkBuffer);
        }
    }

    public class GaussianBlurRenderer : RendererBase<GaussianBlurContext>
    {
        public void CreateMaterial()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/GaussianBlur");
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnRender(ref GaussianBlurContext context, CommandBuffer cmd)
        {
            UpdateParameters(ref context);
            cmd.Blit(context.RenderTarget, context.WorkBuffer, _material, 0);
            cmd.Blit(context.WorkBuffer, context.RenderTarget, _material, 1);
        }

        private void UpdateParameters(ref GaussianBlurContext context)
        {
            switch (context.Method)
            {
                case GaussianBlurMethod.PascalsTriangle:
                    {
                        var radiusIndex = Math.Min(
                            context.Radius - 1,
                            (uint)GaussianBlurContext.AcceptablePascalsRadius.Length - 1);
                        var raduis = GaussianBlurContext.AcceptablePascalsRadius[radiusIndex];

                        _material.SetKeyword("GAUSSIAN_METHOD_PASCALS", true);
                        _material.SetKeyword("GAUSSIAN_METHOD_WEIGHTS", false);
                        Array.ForEach(
                            GaussianBlurContext.AcceptablePascalsRadius,
                            size => _material.SetKeyword($"GAUSSIAN_KERNEL_{size}", size == raduis));
                    }
                    break;

                case GaussianBlurMethod.Weights:
                    {
                        _material.SetKeyword("GAUSSIAN_METHOD_PASCALS", false);
                        _material.SetKeyword("GAUSSIAN_METHOD_WEIGHTS", true);
                        _material.SetFloatArray("Weights", context.Weights);
                        _material.SetFloat("Radius", context.Radius);
                    }
                    break;
            }
        }

        private Material _material;
    }
}
