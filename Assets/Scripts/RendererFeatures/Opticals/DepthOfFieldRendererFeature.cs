using Renderer.Blurs;
using Renderers.Blurs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Volumes.Opticals;

namespace RendererFeatures.Blurs
{
    public class DepthOfFieldRendererPass : RenderPassBase<Volumes.Opticals.DepthOfField>
    {
        public enum ProfilingEvent
        {
            DownScale,
            Blur,
        }

        public DepthOfFieldRendererPass()
        {
            _material = CoreUtils.CreateEngineMaterial("Hidden/DepthOfField");

            _kawaseBlur = new KawaseBlurRenderer();
            _kawaseBlur.CreateMaterial();
            _kawaseBlurContext = new KawaseBlurContext();

            _gaussianBlur = new GaussianBlurRenderer();
            _gaussianBlur.CreateMaterial();
            _gaussianBlurContext = new GaussianBlurContext();

            _hexaBlur = new HexaBlurRenderer();
            _hexaBlur.CreateMaterial();
            _hexaBlurContext = new HexaBlurContext();
        }

        protected override void OnUpdateRenderTargets()
        {
            _kawaseBlurContext.RenderTargetDesc = RenderTargetDescs[0];
            _gaussianBlurContext.RenderTargetDesc = RenderTargetDescs[0];
            _hexaBlurContext.RenderTargetDesc = RenderTargetDescs[0];
        }

        protected override void OnCleanup()
        {
            CoreUtils.Destroy(_material);
        }

        protected override void OnExecute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("DepthOfField");
            using (new CommandBufferFromPoolScope(cmd))
            {
                switch (Volume.Method.value)
                {
                    case BlurMethod.Kawase:
                        {
                            _kawaseBlurContext.BlurSize = (uint)Volume.KawaseRadius.value;

                            var contextOp = new KawaseBlurContextOperator();
                            if (_kawaseBlurContext.NeedToUpdateWorkBufferDescs())
                            {
                                contextOp.UpdateWorkBufferDescs(ref _kawaseBlurContext);
                            }

                            _kawaseBlurContext.RenderTarget = RenderTargets[0];
                            contextOp.GetWorkBuffers(ref _kawaseBlurContext, cmd);
                            _kawaseBlur.Render(ref _kawaseBlurContext, cmd);
                            contextOp.ReleaseWorkBuffers(ref _kawaseBlurContext, cmd);
                        }
                        break;

                    case BlurMethod.Gaussian:
                        {
                            _gaussianBlurContext.Method = Volume.GaussianMethod.value;
                            _gaussianBlurContext.Radius = (uint)Volume.GaussianRadius.value;
                            _gaussianBlurContext.Sigma = Volume.GaussianSigma.value;

                            var contextOp = new GaussianBlurContextOperator();
                            if (_gaussianBlurContext.NeedToUpdateWeights())
                            {
                                contextOp.UpdateWorkBufferDesc(ref _gaussianBlurContext);
                            }

                            _gaussianBlurContext.RenderTarget = RenderTargets[0];
                            contextOp.GetWorkBuffer(ref _gaussianBlurContext, cmd);
                            _gaussianBlur.Render(ref _gaussianBlurContext, cmd);
                            contextOp.ReleaseWorkBuffer(ref _gaussianBlurContext, cmd);
                        }
                        break;

                    case BlurMethod.Hexa:
                        {
                            _hexaBlurContext.Radius = (uint)Volume.HexaRadius.value;
                            _hexaBlurContext.StepScale = Volume.HexaStepScale.value;

                            var contextOp = new HexaBlurContextOperator();
                            if (_hexaBlurContext.NeedToUpdateWorkBuffers())
                            {
                                contextOp.UpdateWorkBufferDescs(ref _hexaBlurContext);
                            }

                            _hexaBlurContext.RenderTarget = RenderTargets[0];
                            contextOp.GetWorkBuffers(ref _hexaBlurContext, cmd);
                            _hexaBlur.Render(ref _hexaBlurContext, cmd);
                            contextOp.ReleaseWorkBuffers(ref _hexaBlurContext, cmd);
                        }
                        break;
                }
                context.ExecuteCommandBuffer(cmd);
            }
        }

        private Material _material;

        private KawaseBlurRenderer _kawaseBlur;
        private KawaseBlurContext _kawaseBlurContext;

        private GaussianBlurRenderer _gaussianBlur;
        private GaussianBlurContext _gaussianBlurContext;

        private HexaBlurRenderer _hexaBlur;
        private HexaBlurContext _hexaBlurContext;
    }

    public class DepthOfFieldRendererFeature : RendererFeatureBase<DepthOfFieldRendererPass>
    {
        protected override RenderPassEvent ExecutionOrder => RenderPassEvent.BeforeRenderingPostProcessing;
        protected override bool CanExecuteInSceneView => false;
    }
}
