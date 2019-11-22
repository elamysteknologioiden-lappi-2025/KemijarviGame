using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.LWRP;

namespace UnityEngine.XR.ARFoundation
{
    public class LWRPBackgroundRendererFeature : ScriptableRendererFeature
    {

        [System.Serializable]
        public class LWRPBackgroundRendererSettings
        {
            public RenderPassEvent Event = RenderPassEvent.BeforeRendering;
        }

        public LWRPBackgroundRendererSettings settings = new LWRPBackgroundRendererSettings();

        // AR Background rendering enables and disables by nullifying material, the render feature needs to propagate this
        Material _arBackgroundMaterial = null;
        public Material ARBackgroundMaterial
        {
            get { return _arBackgroundMaterial;  }
            set {
                _arBackgroundMaterial = value;

                if (_backgroundRenderPass != null)
                    _backgroundRenderPass.m_blitMaterial = _arBackgroundMaterial;
            }
        }

        private LWRPBackgroundRenderPass _backgroundRenderPass;


        public override void Create()
        {
            // Register to renderer is necessary
            if (LWRPBackgroundRendererAsset.useRenderPipeline)
            {
                LWRPBackgroundRenderer renderer = LWRPBackgroundRendererAsset.ARBackgroundRenderer as LWRPBackgroundRenderer;
        
                if (_backgroundRenderPass == null)
                    _backgroundRenderPass = new LWRPBackgroundRenderPass(settings.Event);

                if (renderer != null) renderer.RegisterRendererFeature(this);
            }

             
        }

        // Overriding AddPasses, adding custom rendering passes
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _backgroundRenderPass.Setup(renderer.cameraColorTarget, ARBackgroundMaterial);
            renderer.EnqueuePass(_backgroundRenderPass);
        }

    }
}
