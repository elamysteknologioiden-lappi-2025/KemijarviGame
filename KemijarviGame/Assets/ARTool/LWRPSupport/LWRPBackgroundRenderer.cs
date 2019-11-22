
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

namespace UnityEngine.XR.ARFoundation
{

    public class LWRPBackgroundRenderer : ARFoundationBackgroundRenderer
    {
        CameraClearFlags _savedClearFlags;

        List<LWRPBackgroundRendererFeature> _rendererFeatures = new List<LWRPBackgroundRendererFeature>();

        /// <summary>
        /// When created thru editor scripting objet, the features are protected and unavailable. 
        /// We make them register to the ARFoundationBackgroundRenderer to assign the correct material
        /// </summary>
        /// <param name="feature">The LWRPBackgroundRendererFeature to add</param>
        public void RegisterRendererFeature(LWRPBackgroundRendererFeature feature)
        {
            _rendererFeatures.Add(feature);

            // Assign the correct material (enabled or not)
            feature.ARBackgroundMaterial = m_BackgroundMaterial;
        }

        protected override bool EnableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return false;

            camera = m_Camera ? m_Camera : Camera.main;

            if (camera == null)
                return false;

            // Clear flags
            _savedClearFlags = camera.clearFlags;
            camera.clearFlags = CameraClearFlags.Depth;

            // For all features, assign the correct material
            for (int i = 0; i < _rendererFeatures.Count; ++i)
                _rendererFeatures[i].ARBackgroundMaterial = m_BackgroundMaterial;

            return true;

        }


        protected override void DisableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return;

            camera = m_Camera ? m_Camera : Camera.main;
            if (camera == null)
                return;
            camera.clearFlags = _savedClearFlags;

            // For all features, disable material
            for (int i = 0; i < _rendererFeatures.Count; ++i)
                _rendererFeatures[i].ARBackgroundMaterial = null;
        }
    }

}
