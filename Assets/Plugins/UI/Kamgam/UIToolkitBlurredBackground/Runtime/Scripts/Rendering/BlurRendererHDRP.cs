#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.UIToolkitBlurredBackground
{
    public class BlurRendererHDRP : IBlurRenderer
    {
        protected int _blurIterations;
        public int Iterations
        {
            get
            {
                if (Pass != null)
                {
                    return Pass.BlurIterations;
                }
                return _blurIterations;
            }

            set
            {
                _blurIterations = value;
                if (Pass != null)
                {
                    Pass.BlurIterations = value;
                }
            }
        }

        protected float _offset = 1.5f;
        /// <summary>
        /// This is only used in the performance shader. Default is 1.5f. You can increase this AND reduce the blur strength to imporve performance. However, the quality will start to degrade rapidly.
        /// </summary>
        public float Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                if (Pass != null)
                {
                    Pass.Offset = value;
                }
            }
        }

        protected Vector2Int _resolution = new Vector2Int(512, 512);
        /// <summary>
        /// The texture resolution of the blurred image. Default is 512 x 512. Please use 2^n values like 256, 512, 1024, 2048. Reducing this will increase performance but decrease quality. Every frame your rendered image will be copied, resized and then blurred [BlurStrength] times.
        /// </summary>
        public Vector2Int Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                if (Pass != null)
                {
                    Pass.Resolution = _resolution;
                    Pass.UpdateRenderTextureResolutions();
                }
                
            }
        }

        protected ShaderQuality _quality = ShaderQuality.Medium;

        /// <summary>
        /// The used shader variant. If you are having performance problems with the gaussian shader then try the perfrmance one. It's faster yet the quality is worse (especially for low shader strengths).
        /// </summary>
        public ShaderQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                if (Pass != null)
                {
                    Pass.ShaderQuality = value;
                }
            }
        }

        protected bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                if (Pass != null)
                {
                    Pass.enabled = value;
                }
            }
        }

        protected GameObject _passGameObject;
        protected CustomPassVolume _passVolume;
        protected BlurredBackgroundPassHDRP _pass;
        public BlurredBackgroundPassHDRP Pass
        {
            get
            {
                if (_pass == null || _passVolume == null || _passGameObject == null)
                {
                    _pass = null;
                    _passVolume = null;
                    _passGameObject = null;

                    var volumes = Utils.FindRootObjectsByType<CustomPassVolume>(includeInactive: true);
                    foreach (var volume in volumes)
                    {
                        if (volume.isGlobal)
                        {
                            var type = typeof(BlurredBackgroundPassHDRP);
                            var passes = volume.customPasses;
                            foreach (var pass in volume.customPasses)
                            {
                                var uitkPass = pass as BlurredBackgroundPassHDRP;
                                if (uitkPass != null)
                                {
                                    _pass = uitkPass;
                                    _passVolume = volume;
                                    _passGameObject = volume.gameObject;
                                    goto EndOfLoop;
                                }
                            }
                        }
                    }
                }
                EndOfLoop:
                return _pass;
            }
        }

        public BlurRendererHDRP()
        {
            var cam = RenderUtils.GetGameViewCamera();
            createPassIfNecessary(cam);
        }

        void createPassIfNecessary(Camera cam = null)
        {
            if (Pass == null)
            {
                var go = new GameObject("UITK BlurredBackground Custom Pass Volume");
                go.hideFlags = HideFlags.DontSave;
                Utils.SmartDontDestroyOnLoad(go);

                var volume = go.AddComponent<CustomPassVolume>();
                volume.injectionPoint = CustomPassInjectionPoint.AfterPostProcess;
                volume.priority = 0;
                if (cam == null)
                {
                    volume.isGlobal = true;
                }
                else
                {
                    volume.isGlobal = false;
                    volume.targetCamera = cam;
                }
                

                var pass = volume.AddPassOfType<BlurredBackgroundPassHDRP>();
                pass.enabled = true;
                pass.targetColorBuffer = CustomPass.TargetBuffer.Camera;
                pass.targetDepthBuffer = CustomPass.TargetBuffer.Camera;
                pass.clearFlags = UnityEngine.Rendering.ClearFlag.None;

                // Important if HideFlags.HideAndDontSave is used or else the object will not be found by Pass.
                _pass = pass as BlurredBackgroundPassHDRP;
                _passVolume = volume;
                _passGameObject = go;

                // Init pass variables
                _pass.ShaderQuality = Quality;
                _pass.Resolution = Resolution;
                _pass.Offset = Offset;
                _pass.BlurIterations = Iterations;
            }
        }

        public Texture GetBlurredTexture()
        {
            if (Pass != null)
                return Pass.GetBlurredTexture();
            else
                return null;
        }

        /// <summary>
        /// Creates the pass objects if needed.
        /// </summary>
        /// <returns>Always false</returns>
        public bool Update()
        {
            // Create render pass if needed.
            // TODO: Investigate if adding the pass dynamically is possible in HDRP
            // see (URP): https://forum.unity.com/threads/urp-no-way-to-dynamically-access-modify-the-rendererfeatures-list-at-runtime.1342751/#post-8479169
            createPassIfNecessary();

            // Keep camera up to date (in case camera stacking is used or the active camera changes at runtime).
            if (    _passVolume != null
                && !_passVolume.isGlobal
                && (_passVolume.targetCamera == null || !_passVolume.targetCamera.isActiveAndEnabled))
            {
                _passVolume.targetCamera = RenderUtils.GetGameViewCamera();
            }

            return false;
        }
    }
}
#endif