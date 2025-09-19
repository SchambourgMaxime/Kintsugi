#if !KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
using UnityEngine;

namespace Kamgam.UIToolkitBlurredBackground
{
    /// <summary>
    /// This component is added to the main camera to listen for OnPreRender and OnRenderImage messages.
    /// It exposes these messages as delegates. These are then used by the BlurRendererBuiltIn renderer.
    /// 
    /// TODO: Replace this with a single CommandBuffer to avoid adding components to scenes.
    /// See: https://docs.unity3d.com/Manual/GraphicsCommandBuffers.html
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [HelpURL("https://kamgam.com/unity/UIToolkitBlurredBackgroundManual.pdf")]
    public class BlurRenderEventForwarder : MonoBehaviour
    {
        static int _lastMainCameraId = int.MinValue + 1;
        static BlurRenderEventForwarder _forwarder;

        /// <summary>
        /// Returns true if the forwarder has been updated (aka a new main camera was detected).
        /// </summary>
        /// <returns></returns>
        public static bool Update()
        {
            var cam = Camera.main;

            if (cam == null)
                return false;

            if (_forwarder == null || _forwarder.gameObject == null || cam.GetInstanceID() != _lastMainCameraId)
            {
                _lastMainCameraId = cam.GetInstanceID();
                _forwarder = Camera.main.gameObject.GetComponent<BlurRenderEventForwarder>();

                if (_forwarder == null)
                {
                    // Destroy renderes on other cameras
#if UNITY_2023_1_OR_NEWER
                    var forwarders = GameObject.FindObjectsByType<BlurRenderEventForwarder>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                    var forwarders = GameObject.FindObjectsOfType<BlurRenderEventForwarder>(includeInactive: true);
#endif
                    for (int i = forwarders.Length - 1; i >= 0; i--)
                    {
                        Utils.SmartDestroy(forwarders[i]);
                    }

                    // Create renderer on camera
                    _forwarder = Camera.main.gameObject.AddComponent<BlurRenderEventForwarder>();
                    _forwarder.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
                }

                // Update active state
                Active = _active;

                return _forwarder != null;
            }

            return false;
        }

        protected Camera _camera;
        public Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = this.GetComponent<Camera>();
                }
                return _camera;
            }
        }

        public static System.Action<Camera> OnPreRenderFunc;
        public static System.Action<Camera, RenderTexture, RenderTexture> OnRenderImageFunc;

        public bool IsValid()
        {
            return Camera != null && Camera.main != Camera || gameObject != null && this != null && gameObject.activeInHierarchy && enabled && Camera.isActiveAndEnabled;
        }

        static bool _active;

        /// <summary>
        /// Activate or deactivate the renderer. Disable to save performance (no rendering will be done).
        /// </summary>
        public static bool Active
        {
            get
            {
                if (_forwarder != null)
                {
                    return _forwarder.enabled;
                }
                else
                {
                    return _active;
                }
            }
            set
            {
                _active = value;
                if (_forwarder != null)
                    _forwarder.enabled = value;
            }
        }

        void OnPreRender()
        {
            OnPreRenderFunc?.Invoke(Camera);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (OnRenderImageFunc != null)
            {
                OnRenderImageFunc?.Invoke(Camera, src, dest);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}
#else
using UnityEngine;
namespace Kamgam.UIToolkitBlurredBackground
{
    // Not needed. This is only here in case someone upgrades from BuiltIn to SRPs.
    public class BlurRenderEventForwarder : MonoBehaviour { }
}
#endif