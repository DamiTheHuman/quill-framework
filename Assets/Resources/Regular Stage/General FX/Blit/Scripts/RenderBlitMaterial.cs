using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderBlitMaterial : ScriptableRendererFeature
{
    private class CustomRenderPass : ScriptableRenderPass
    {
        public Material _Material;
        private CustomRPSettings _CustomRPSettings;
        private RenderTargetHandle _TemporaryColorTexture;

        private RenderTargetIdentifier _Source;
        private RenderTargetHandle _Destination;

        public CustomRenderPass(CustomRPSettings settings) => this._CustomRPSettings = settings;

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this._Source = source;
            this._Destination = destination;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) => this._TemporaryColorTexture.Init("_TemporaryColorTexture");

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("My Pass");

            if (this._Destination == RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(this._TemporaryColorTexture.id, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Point);
                cmd.Blit(this._Source, this._TemporaryColorTexture.Identifier());
                cmd.Blit(this._TemporaryColorTexture.Identifier(), this._Source, this._CustomRPSettings.m_Material);
            }
            else
            {
                cmd.Blit(this._Source, this._Destination.Identifier(), this._CustomRPSettings.m_Material, 0);
            }
            cmd.Blit(this._Source, this._Destination.Identifier(), this._Material);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (this._Destination == RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(this._TemporaryColorTexture.id);
            }
        }
    }

    [System.Serializable]
    public class CustomRPSettings
    {
        public Material m_Material;
    }

    public CustomRPSettings m_CustomRPSettings = new CustomRPSettings();
    private CustomRenderPass _ScriptablePass;
    public RenderPassEvent _RenderPassEvents = RenderPassEvent.AfterRenderingTransparents;

    public override void Create() => this._ScriptablePass = new CustomRenderPass(this.m_CustomRPSettings)
    {
        renderPassEvent = this._RenderPassEvents
    };

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        this._ScriptablePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(this._ScriptablePass);
    }
}
