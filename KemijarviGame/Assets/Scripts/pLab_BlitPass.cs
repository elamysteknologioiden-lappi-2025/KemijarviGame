using System.Collections.Generic;

namespace UnityEngine.Rendering.LWRP{
    internal class pLab_BlitPass : ScriptableRenderPass{
        public enum RenderTarget{
            Color,
            RenderTexture,
        }

        public Material material = null;
        public int passIndex = 0;
        public FilterMode filterMode { get; set; }
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        private RenderTargetHandle tmpTexture;
        private FilteringSettings filterSetting;
        private List<ShaderTagId> liftOfIds = new List<ShaderTagId>();



        public pLab_BlitPass(RenderPassEvent aEvent, Material aMaterial){
            this.renderPassEvent = aEvent;
            this.material = aMaterial;
        }

        public void Setup(RenderTargetIdentifier aSource, RenderTargetHandle aDestination){
            this.source = aSource;
            this.destination = aDestination;
        }

        public override void Execute(ScriptableRenderContext aContext, ref RenderingData aRenderingData){
            CommandBuffer cmd = CommandBufferPool.Get("pLab");

            DrawingSettings drawingSettings = CreateDrawingSettings(liftOfIds, ref aRenderingData, aRenderingData.cameraData.defaultOpaqueSortFlags);
            aContext.DrawRenderers(aRenderingData.cullResults, ref drawingSettings, ref filterSetting);
             
            RenderTextureDescriptor renderTextureDescriptor = aRenderingData.cameraData.cameraTargetDescriptor;
            renderTextureDescriptor.depthBufferBits = 0;


            cmd.GetTemporaryRT(tmpTexture.id, renderTextureDescriptor, filterMode);
            Blit(cmd, source, tmpTexture.Identifier(), material, passIndex);
            Blit(cmd, tmpTexture.Identifier(), source);

            aContext.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer aCmd){
            if (destination == RenderTargetHandle.CameraTarget)
                aCmd.ReleaseTemporaryRT(tmpTexture.id);
        }
    }
}
