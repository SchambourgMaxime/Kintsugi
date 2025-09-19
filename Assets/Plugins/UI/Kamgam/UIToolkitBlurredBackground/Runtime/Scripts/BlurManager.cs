using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitBlurredBackground
{
    /// <summary>
    /// This manager keeps track of whether or not the blur is needed and disables the
    /// rendering if not. This is done to save performance when no blurred UI is shown.
    /// </summary>
    public class BlurManager
    {
        static BlurManager _instance;
        public static BlurManager Instance // This is triggered by the UI Toolkit Elements
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BlurManager();
#if UNITY_EDITOR
                    if (!UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        // Delay start if in between play mode changes.
                        // We need this because the UI Elements trigger the creation of the instance in between playmode changes.
                        UnityEditor.EditorApplication.playModeStateChanged += delayedStart;
                    }
                    else
#endif
                    {
                        _instance.Start();
                    }
                    
                }
                return _instance;
            }
        }

#if UNITY_EDITOR
        static void delayedStart(UnityEditor.PlayModeStateChange change)
        {
            if (change == UnityEditor.PlayModeStateChange.EnteredPlayMode && _instance != null)
            {
                _instance.Start();
            }
            UnityEditor.EditorApplication.playModeStateChanged -= delayedStart;
        }
#endif

        // -------------------

        void Start()
        {
            // Register this classes Update() method in the Unity update loop for runtime and editor.
            BlurManagerUpdater.Init(Update); 
        }


        /// <summary>
        /// Defines how often the blur will be applied. Use with caution. It drains performance quickly.")]
        /// </summary>
        [System.NonSerialized]
        protected int _iterations = 1;
        public int Iterations
        {
            get
            {
                return _iterations;
            }

            set
            {
                if (value < 0)
                    value = 0;

                _iterations = value;
                Renderer.Iterations = value;
            }
        }

        [System.NonSerialized]
        protected float _offset = 10f;
        public float Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                if (value < 0f)
                    value = 0f;

                _offset = value;
                Renderer.Offset = value;
            }
        }

        [System.NonSerialized]
        protected Vector2Int _resolution = new Vector2Int(512, 512);
        public Vector2Int Resolution
        {
            get
            {
                return _resolution;
            }

            set
            {
                if (value.x < 2 || value.y < 2)
                    value = new Vector2Int(2, 2);

                _resolution = value;
                Renderer.Resolution = value;
            }
        }

        [System.NonSerialized]
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
                Renderer.Quality = _quality;
            }
        }

        public Texture GetBlurredTexture()
        {
            return Renderer.GetBlurredTexture();
        }

        [System.NonSerialized]
        protected IBlurRenderer _renderer;
        public IBlurRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
#if !KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
                    _renderer = new BlurRendererBuiltIn(); // BuiltIn
#elif KAMGAM_RENDER_PIPELINE_URP
                    _renderer = new BlurRendererURP(); // URP
#else
                    _renderer = new BlurRendererHDRP(); // HDRP
#endif
                    _renderer.Iterations = Iterations;
                }
                return _renderer;
            }

            set
            {
                _renderer = value;
            }
        }

        /// <summary>
        /// Keeps track of how many elements use the blurred texture. If none are using it then
        /// the rendering will be paused to save performance.
        /// </summary>
        protected List<VisualElement> _blurredBackgroundElements = new List<VisualElement>();

        public void AttachElement(VisualElement ele)
        {
            if (!_blurredBackgroundElements.Contains(ele))
            {
                _blurredBackgroundElements.Add(ele);
                ele.MarkDirtyRepaint();
            }

            if (Renderer != null)
            {
                Renderer.Active = shouldBeActive();
            }
        }

        public void DetachElement(VisualElement ele)
        {
            if (_blurredBackgroundElements.Contains(ele))  
            {
                _blurredBackgroundElements.Remove(ele);
            }

            if (Renderer != null)
            {
                Renderer.Active = shouldBeActive();
            }
        }

        protected bool shouldBeActive()
        {
            // Count the visible elements (display: none, visbility: hidden are not counted)
            int activeElements = 0;
            foreach (var ele in _blurredBackgroundElements)
            {
                if (ele.visible && ele.resolvedStyle.display != DisplayStyle.None && ele.enabledInHierarchy)
                {
                    activeElements++;
                }
            }

            return activeElements > 0 && _iterations > 0 && _offset > 0.0f;
        }

        public void Update()
        {
            // Disable rendering is no elements with blurred background are visible.
            Renderer.Active = shouldBeActive();

            // Keep the renderer in sync with the current main camera.
            if (Renderer.Active)
            {
                Renderer.Update();
            } 
        }
    }
}