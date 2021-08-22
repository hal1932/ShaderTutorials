using System;
using Renderers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Renderer.Blurs
{
    public struct KawaseBlurContext : IRendererContext
    {
        public const uint MaxBlurSize = 5;

        public RenderTargetIdentifier RenderTarget;
        public RenderTextureDescriptor RenderTargetDesc;

        public uint BlurSize;
        public int[] WorkBuffers;
        public Vector2[] WorkBufferSizes;

        public bool NeedToUpdateWorkBufferDescs(uint blurSize = default)
        {
            if (WorkBufferSizes == default || WorkBufferSizes == default)
            {
                return true;
            }

            if (blurSize != default && blurSize != BlurSize)
            {
                return true;
            }

            if (WorkBuffers.Length != blurSize * 2)
            {
                return true;
            }

            var requiredSize = new Vector2(RenderTargetDesc.width / 2, RenderTargetDesc.height / 2);
            if (WorkBufferSizes[0] != requiredSize)
            {
                return true;
            }

            return false;
        }
    }

    public struct KawaseBlurContextOperator
    {
        public void UpdateWorkBufferDescs(ref KawaseBlurContext context, uint maxBlurSize = default)
        {
            if (maxBlurSize == default)
            {
                maxBlurSize = context.BlurSize;
            }

            var sizes = new Vector2[maxBlurSize * 2];

            sizes[0] = new Vector2(context.RenderTargetDesc.width / 2, context.RenderTargetDesc.height / 2);
            sizes[1] = sizes[0];

            var blurSize = 1U;
            for (var i = 2; i < sizes.Length; i += 2)
            {
                var width = (int)sizes[i - 1].x;
                var height = (int)sizes[i - 1].y;
                if (width == 1 || height == 1)
                {
                    break;
                }

                sizes[i] = new Vector2(width / 2, height / 2);
                sizes[i + 1] = sizes[i];

                ++blurSize;
            }

            context.BlurSize = blurSize;

            context.WorkBuffers = new int[blurSize * 2];
            for (var i = 0; i < context.WorkBuffers.Length; ++i)
            {
                context.WorkBuffers[i] = Shader.PropertyToID($"Work Buffer {i}");
            }

            context.WorkBufferSizes = new Vector2[blurSize * 2];
            Array.Copy(sizes, context.WorkBufferSizes, blurSize * 2);
        }

        public void GetWorkBuffers(ref KawaseBlurContext context, CommandBuffer cmd)
        {
            var desc = context.RenderTargetDesc;
            for (var i = 0; i < context.WorkBufferSizes.Length; ++i)
            {
                desc.width = (int)context.WorkBufferSizes[i].x;
                desc.height = (int)context.WorkBufferSizes[i].y;
                cmd.GetTemporaryRT(context.WorkBuffers[i], desc, FilterMode.Bilinear);
            }
        }

        public void ReleaseWorkBuffers(ref KawaseBlurContext context, CommandBuffer cmd)
        {
            foreach (var buffer in context.WorkBuffers)
            {
                cmd.ReleaseTemporaryRT(buffer);
            }
        }
    }

    public class KawaseBlurRenderer : RendererBase<KawaseBlurContext>
    {
        enum ProfilingEvent
        {
            DownScale,
            Blur,
            Compose,
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        public void CreateMaterial()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/KawaseBlur");
        }

        protected override void OnRender(ref KawaseBlurContext context, CommandBuffer cmd)
        {
            var profiler = new Profiler<ProfilingEvent>(cmd);

            using (profiler.Scope(ProfilingEvent.DownScale))
            {
                // 縮小バッファを作成
                // target -> 0
                // 0 -> 2
                // 2 -> 4
                // 4 -> 6
                // ...
                cmd.Blit(context.RenderTarget, context.WorkBuffers[0]);
                for (var i = 2; i < context.BlurSize * 2; i += 2)
                {
                    cmd.Blit(context.WorkBuffers[i - 2], context.WorkBuffers[i]);
                }
            }

            using (profiler.Scope(ProfilingEvent.Blur))
            {
                // 縮小バッファにブラー
                // 0 -> 1
                // 2 -> 3
                // 4 -> 5
                // ...
                for (var i = 0; i < context.BlurSize * 2; i += 2)
                {
                    cmd.Blit(context.WorkBuffers[i], context.WorkBuffers[i + 1], _material, 0);
                }
            }

            using (profiler.Scope(ProfilingEvent.Compose))
            {
                // 合成
                _material.SetInt("BlurSize", (int)context.BlurSize);
                for (var i = 0; i < context.BlurSize * 2; ++i)
                {
                    cmd.SetGlobalTexture($"Texture{i}", context.WorkBuffers[i]);
                }
                cmd.Blit(context.RenderTarget, context.RenderTarget, _material, 1);
            }
        }

        private Material _material;
    }
}
