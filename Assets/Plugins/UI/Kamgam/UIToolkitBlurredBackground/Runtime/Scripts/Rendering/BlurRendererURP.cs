#if KAMGAM_RENDER_PIPELINE_URP
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Kamgam.UIToolkitBlurredBackground
{
    public class BlurRendererURP : IBlurRenderer
    {
        public event System.Action OnPostRender;

        protected BlurredBackgroundPassURP _screenSpacePass;
        public BlurredBackgroundPassURP ScreenSpacePass
        {
            get
            {
                if (_screenSpacePass == null)
                {
                    _screenSpacePass = new BlurredBackgroundPassURP();
                    // NOTICE: This is now overridden in onBeginCameraRendering().
                    _screenSpacePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

                    _screenSpacePass.OnPostRender += onPostRender;
                }
                return _screenSpacePass;
            }
        }

        protected bool _active;

        /// <summary>
        /// Activate or deactivate the renderer. Disable to save performance (no rendering will be done).
        /// </summary>
        public bool Active
        {
            get => _active;
            set
            {
                if (value != _active)
                {
                    _active = value;

                    ScreenSpacePass.Active = value;
                }
            }
        }

        protected int _iterations = 1;
        public int Iterations
        {
            get
            {
                return _iterations;
            }

            set
            {
                _iterations = value;

                ScreenSpacePass.Iterations = value;
            }
        }

        protected float _offset = 1.5f;
        public float Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                _offset = value;

                ScreenSpacePass.Offset = value;
            }
        }

        protected Vector2Int _resolution = new Vector2Int(512, 512);
        public Vector2Int Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;

                ScreenSpacePass.Resolution = value;
            }
        }

        protected ShaderQuality _quality = ShaderQuality.Medium;
        public ShaderQuality Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;

                ScreenSpacePass.Quality = value;
            }
        }

        protected Color _additiveColor = new Color(0,0,0,0);
        public Color AdditiveColor
        {
            get
            {
                return _additiveColor;
            }
            set
            {
                _additiveColor = value;

                ScreenSpacePass.AdditiveColor = value;
            }
        }

        /// <summary>
        /// The material is used in screen space overlay canvases.
        /// </summary>
        public Material GetMaterial(RenderMode renderMode)
        {
            return ScreenSpacePass.Material;
        }

        public Texture GetBlurredTexture()
        {
            return ScreenSpacePass.GetBlurredTexture();
        }

        public BlurRendererURP()
        {
            RenderPipelineManager.beginCameraRendering += onBeginCameraRendering;

            if (ScreenSpacePass != null)
                ScreenSpacePass.OnPostRender += onPostRender;

            // Needed to avoid "Render Pipeline error : the XR layout still contains active passes. Executing XRSystem.EndLayout() right" Errors in Unity 2023
            // Also needed in normal URP to reset the render textures after play mode.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeChanged;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += onSceneOpened;
#endif
        }

        ~BlurRendererURP()
        {
            if (_screenSpacePass != null)
                _screenSpacePass.OnPostRender -= onPostRender;
        }

        protected void clearRenderTargets()
        {
            _screenSpacePass?.ClearRenderTargets();
        }

#if UNITY_EDITOR
        void onPlayModeChanged(UnityEditor.PlayModeStateChange obj)
        {
            if (obj == UnityEditor.PlayModeStateChange.ExitingPlayMode || obj == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                clearRenderTargets();
            }
        }

        void onSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                clearRenderTargets();
            }
        }
#endif

        const string Renderer2DTypeName = "Renderer2D";

        private Camera[] _tmpAllCameras = new Camera[10];

        void onBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (    cam == null
                || !cam.isActiveAndEnabled)
                return;

            // All of this is only to support multiple-camera setups with render textures.
            // The blur only needs to be done on one camera (usually the main camera). That's
            // why the stop on all other cameras.
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                if (cam != mainCam)
                    return;
            }
            else
            {
                // No main camera -> let's check if there are cameras that
                // are NOT rendering into render textures.
                Camera firstCamWithoutRenderTexture = null;
                int camCount = Camera.allCamerasCount;
                int maxCamCount = _tmpAllCameras.Length;
                // alloc new array if needed
                if(camCount > maxCamCount)
                {
                    _tmpAllCameras = new Camera[camCount + 5];
                }
                Camera.GetAllCameras(_tmpAllCameras);
                for (int i = 0; i < maxCamCount; i++)
                {
                    // Null out old references
                    if(i >= camCount)
                    {
                        _tmpAllCameras[i] = null;
                        continue;
                    }

                    var cCam = _tmpAllCameras[i];

                    if (cCam == null || !cCam.isActiveAndEnabled)
                        continue;

                    if (cCam != null && cCam.targetTexture == null)
                    {
                        firstCamWithoutRenderTexture = cCam;
                        break;
                    }
                }

                // If there are some then use the first we an find. Which means we abort the blur pass on all others.
                if (firstCamWithoutRenderTexture != null && cam != firstCamWithoutRenderTexture)
                    return;

                // If there are only cameras with render textures then we ignore it.
                // This means that in setups with cameras that are only rendered in to textures
                // no blur will occur.
                if (firstCamWithoutRenderTexture == null)
                    return;
            }

            var data = cam.GetUniversalAdditionalCameraData();

            if (data == null)
                return;

            // Turns out the list is always empty and the enqueuing is a per frame action.

            // Check if we are using the 2D renderer (skip check if already using "BeforeRenderingPostProcessing" event).
            if (cam.orthographic && ScreenSpacePass.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing)
            {
                if (cam.GetUniversalAdditionalCameraData().scriptableRenderer.GetType().Name.EndsWith(Renderer2DTypeName))
                {
                    // If yes then change the event from AfterRenderingPostProcessing to BeforeRenderingPostProcessing.
                    // Sadly accessing PostPro render results is not supported in URP 2D, see:
                    // https://forum.unity.com/threads/urp-2d-how-to-access-camera-target-after-post-processing.1465124/
                    // https://forum.unity.com/threads/7-3-1-renderpassevent-afterrenderingpostprocessing-is-broken.873604/#post-8422710
                    ScreenSpacePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                }
            }

            data.scriptableRenderer.EnqueuePass(ScreenSpacePass);
        }

        protected void onPostRender()
        {
            OnPostRender?.Invoke();
        }

        /// <summary>
        /// Not needed in SRPs.
        /// </summary>
        public bool Update()
        {
            return true;
        }
    }
}
#endif