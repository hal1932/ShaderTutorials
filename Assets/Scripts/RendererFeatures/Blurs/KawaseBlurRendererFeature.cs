using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes.Blurs;

namespace RendererFeatures.Blurs
{
    public class KawaseBlurRenderPass : RenderPassBase<KawaseBlur>
    {
        public enum ProfilingEvent
        {
            DownScale,
            Blur,
        }

        public KawaseBlurRenderPass()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/KawaseBlur");
            _textures = Enumerable.Range(0, BufferCount * 2)
                .Select(i => Shader.PropertyToID($"Texture{i}"))
                .ToArray();
        }

        protected override void OnCleanup()
        {
            //CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("KawaseBlur");
            using (new CommandBufferFromPoolScope(cmd))
            {
                var desc = RenderTargetDescs[0];
                for (var i = 0; i < _textures.Length; i += 2)
                {
                    desc.width /= 2;
                    desc.height /= 2;

                    cmd.GetTemporaryRT(_textures[i], desc, FilterMode.Bilinear);
                    cmd.GetTemporaryRT(_textures[i + 1], desc, FilterMode.Bilinear);
                }

                // 縮小バッファを作成
                // target -> 0
                // 0 -> 2
                // 2 -> 4
                // 4 -> 6
                // ...
                using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.DownScale)))
                {
                    cmd.Blit(RenderTargets[0], _textures[0]);
                    for (var i = 2; i < _textures.Length; i += 2)
                    {
                        cmd.Blit(_textures[i - 2], _textures[i]);
                    }
                }

                // 縮小バッファにブラー
                // 0 -> 1
                // 2 -> 3
                // 4 -> 5
                // ...
                using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.Blur)))
                {
                    for (var i = 0; i < _textures.Length; i += 2)
                    {
                        cmd.Blit(_textures[i], _textures[i + 1], _material, 0);
                    }
                }

                // 合成
                for (var i = 0; i < _textures.Length; ++i)
                {
                    cmd.SetGlobalTexture(_textures[i], _textures[i]);
                }
                cmd.Blit(RenderTargets[0], RenderTargets[0], _material, 1);

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private const int BufferCount = 5;

        private Material _material;
        private int[] _textures;
    }

    public class KawaseBlurRendererFeature : RendererFeatureBase<KawaseBlurRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
