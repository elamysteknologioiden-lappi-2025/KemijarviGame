using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation
{
    [CreateAssetMenu(fileName = "LWRPBackgroundRendererAsset", menuName = "XR/LWRPBackgroundRendererAsset")]
    public class LWRPBackgroundRendererAsset : ARBackgroundRendererAsset
    {
        /// <summary>
        /// we're going to reference all materials that we want to use so that they get built into the project
        /// </summary>
        [SerializeField]
        Material[] m_MaterialsUsed;

        [SerializeField]
        ARCameraManager m_CameraManager;

        public static bool useRenderPipeline {  get { return GraphicsSettings.renderPipelineAsset != null; } }

        static ARFoundationBackgroundRenderer _arBackgroundRenderer;
        public static ARFoundationBackgroundRenderer ARBackgroundRenderer
        {
            get { return _arBackgroundRenderer; }
        }

        public override ARFoundationBackgroundRenderer CreateARBackgroundRenderer()
        {
            _arBackgroundRenderer = useRenderPipeline ? new LWRPBackgroundRenderer() : new ARFoundationBackgroundRenderer();
            return _arBackgroundRenderer;
        }

    

        public override void CreateHelperComponents(GameObject cameraGameObject)
        {
            m_CameraManager = cameraGameObject.GetComponent<ARCameraManager>();
            Debug.Log("CreateHelperComponents");
            Debug.Assert(m_CameraManager != null, "camera manager must be non-null");
        }

        public override Material CreateCustomMaterial()
        {
            if (m_CameraManager == null)
            {
                Debug.Log("camera manager is null");
                return null;
            }

            // Try to create a material from the plugin's provided shader.
            var shaderName = m_CameraManager.shaderName + "LWRP";
            Debug.LogFormat("Creating material for shader '{0}'", shaderName);

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                 throw new InvalidOperationException(string.Format(
                    "Could not find shader named \"{0}\" required for LWRP video overlay on camera subsystem named \"{1}\".",
                    shaderName,
                    m_CameraManager.descriptor.id));
            }

            return new Material(shader);
        }
    }

}
