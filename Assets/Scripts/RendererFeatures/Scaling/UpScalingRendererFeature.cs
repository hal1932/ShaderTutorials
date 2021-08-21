using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes.Scalings;

namespace RendererFeatures.Scalings
{
    public class UpScalingRenderPass : RenderPassBase<UpScaling>
    {
        public enum ProfilingEvent
        {
            DownScale,
            Blinear,
            BlurBilinear,
            Gaussian,
        }

        public UpScalingRenderPass()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/UpScaling");
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnUpdateRenderTargets()
        {
            _downScaleDesc = RenderTargetDescs[0];
            _downScaleDesc.width /= 8;
            _downScaleDesc.height /= 8;
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("UpScaling");
            using (new CommandBufferFromPoolScope(cmd))
            {
                var target = RenderTargets[0];
                var targetDesc = RenderTargetDescs[0];

                using (var downScaled = new TempRenderTarget(cmd, _downScaleDesc, FilterMode.Bilinear, _downScaledName))
                {
                    using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.DownScale)))
                    {
                        cmd.Blit(target, downScaled);
                    }

                    switch (Volume.Method.value)
                    {
                        case UpScalingMethod.Blinear:
                            using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.Blinear)))
                            {
                                cmd.Blit(downScaled, target);
                            }
                            break;

                        case UpScalingMethod.BlurredBilinear:
                            using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.BlurBilinear)))
                            {
                                var tempDesc = _downScaleDesc;
                                tempDesc.width *= 2;
                                tempDesc.height *= 2;

                                using (var ping = new TempRenderTarget(cmd, tempDesc, FilterMode.Bilinear, _blurUpScaledPingName))
                                using (var pong = new TempRenderTarget(cmd, tempDesc, FilterMode.Bilinear, _blurUpScaledPongName))
                                {
                                    _material.SetFloat("UvOffsetSize", Volume.UvOffsetUnit.value);
                                    cmd.Blit(downScaled, ping);
                                    cmd.Blit(ping, pong, _material, 0);
                                    cmd.Blit(pong, target);
                                }
                            }
                            break;

                        case UpScalingMethod.Gaussian:
                            using (new ProfilingScope(cmd, ProfilingSampler.Get(ProfilingEvent.Gaussian)))
                            {
                                _material.SetFloat("UvOffsetSize", Volume.UvOffsetUnit.value);
                                Array.ForEach(
                                    UpScaling.GaussianKernelSizeCandidates,
                                    size => _material.SetKeyword($"GAUSSIAN_KERNEL_{size}", size == Volume.GaussianKernelSize));

                                using (var temp = new TempRenderTarget(cmd, targetDesc, FilterMode.Bilinear, _tempName))
                                {
                                    cmd.Blit(downScaled, temp, _material, 1);
                                    cmd.Blit(temp, target, _material, 2);
                                }
                            }
                            break;
                    }
                }

                context.ExecuteCommandBuffer(cmd);
            }
        }

        private Material _material;
        private RenderTextureDescriptor _downScaleDesc;
        private readonly int _downScaledName = Shader.PropertyToID("DownScaled");
        private readonly int _blurUpScaledPingName = Shader.PropertyToID("BlurUpScaledPing");
        private readonly int _blurUpScaledPongName = Shader.PropertyToID("BlurUpScaledPong");
        private readonly int _tempName = Shader.PropertyToID("Temp");
    }

    public class UpScalingRendererFeature : RendererFeatureBase<UpScalingRenderPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
