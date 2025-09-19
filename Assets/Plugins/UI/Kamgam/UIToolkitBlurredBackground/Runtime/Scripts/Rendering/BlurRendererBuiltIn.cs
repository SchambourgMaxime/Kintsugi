#if !KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.UIToolkitBlurredBackground
{
    /// <summary>
    /// Uses command buffers to hook into the rendering camera and extract a blurred image.
    /// </summary>
    public class BlurRendererBuiltIn : IBlurRenderer
    {
        public event Action OnPostRender;

        protected BlurredBackgroundBufferBuiltIn _renderBuffer;
        public BlurredBackgroundBufferBuiltIn RenderBuffer
        {
            get
            {
                if (_renderBuffer == null)
                {
                    _renderBuffer = new BlurredBackgroundBufferBuiltIn(BlurredBackgroundBufferBuiltIn.CameraEventForBlur);
                }
                return _renderBuffer;
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
                    if (!_active)
                    {
                        RenderBuffer.Active = value;
                        RenderBuffer.ClearBuffers();
                    }
                    else
                    {
                        var cam = RenderUtils.GetGameViewCamera();
                        RenderBuffer.Active = value;
                        RenderBuffer.AddBuffer(cam);
                    }
                }
            }
        }

        public int Iterations
        {
            get
            {
                return RenderBuffer.Iterations;
            }

            set
            {
                RenderBuffer.Iterations = value;
            }
        }

        public float Offset
        {
            get
            {
                return RenderBuffer.Offset;
            }

            set
            {
                RenderBuffer.Offset = value;
            }
        }

        public Vector2Int Resolution
        {
            get
            {
                return RenderBuffer.Resolution;
            }
            set
            {
                RenderBuffer.Resolution = value;
            }
        }

        public ShaderQuality Quality
        {
            get
            {
                return RenderBuffer.Quality;
            }
            set
            {
                RenderBuffer.Quality = value;
            }
        }

        /// <summary>
        /// The material is used in screen space overlay canvases.
        /// </summary>
        public Material GetMaterial()
        {
            return RenderBuffer.Material;
        }

        public Texture GetBlurredTexture()
        {
            return RenderBuffer.GetBlurredTexture();
        }

        protected Color _additiveColor = new Color(0, 0, 0, 0);
        public Color AdditiveColor
        {
            get
            {
                return _additiveColor;
            }
            set
            {
                _additiveColor = value;

                RenderBuffer.AdditiveColor = value;
            }
        }

        /// <summary>
        /// Called in the Update loop.
        /// </summary>
        public bool Update()
        {
            var gameCam = RenderUtils.GetGameViewCamera();
            _renderBuffer?.UpdateActiveCamera(gameCam);

            OnPostRender?.Invoke();

            return true;
        }

        ~BlurRendererBuiltIn()
        {
            _renderBuffer?.ClearBuffers();
        }
    }
}
#endif