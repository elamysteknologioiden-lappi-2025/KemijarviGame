using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.LWRP;

namespace UnityEngine.XR.ARFoundation
{
    public class LWRPBackgroundRenderPass : ScriptableRenderPass
    {
        public Material m_blitMaterial;
        RenderTargetIdentifier _targetIdentifier;

        const string m_ProfilerTag = "ArBackgroundRendererPass";


        /// <summary>
        /// Create the BackgroundRendererPass
        /// </summary>
        public LWRPBackgroundRenderPass(RenderPassEvent evt)
        {
  
            renderPassEvent = evt;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="ident">Destination Render Target</param>
        public bool Setup(RenderTargetIdentifier ident, Material blitMaterial)
        {
            _targetIdentifier = ident;
            m_blitMaterial = blitMaterial;
            return m_blitMaterial != null;
        }


        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_blitMaterial != null)
            {
                var cmd = CommandBufferPool.Get(m_ProfilerTag);

                RenderBufferLoadAction colorLoadOp = RenderBufferLoadAction.DontCare;
                RenderBufferStoreAction colorStoreOp = RenderBufferStoreAction.Store;

                cmd.SetRenderTarget(_targetIdentifier, colorLoadOp, colorStoreOp);

                cmd.Blit(null, _targetIdentifier, m_blitMaterial);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

        }
    }
}