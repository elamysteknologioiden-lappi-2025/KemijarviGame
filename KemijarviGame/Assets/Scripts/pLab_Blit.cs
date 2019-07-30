using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.LWRP
{
    public class pLab_Blit : ScriptableRendererFeature
    {
        [System.Serializable]
        public class pLab_BlitSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            public Material material = null;
            public int materialPassIndex = 0;

            public string textureId = "_BlitPassTexture";
        }

        public pLab_BlitSettings settings = new pLab_BlitSettings();
        RenderTargetHandle m_RenderTextureHandle;

        pLab_BlitPass blitPass;

        public override void Create(){
            blitPass = new pLab_BlitPass(settings.renderPassEvent, settings.material);

        }

        public override void AddRenderPasses(ScriptableRenderer aRenderer, ref RenderingData aRenderinData) {
            var source = aRenderer.cameraColorTarget;
            var destination = RenderTargetHandle.CameraTarget;

            blitPass.Setup(source, destination);
            aRenderer.EnqueuePass(blitPass);
        }
    }


}