#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;


namespace Kamgam.UIToolkitParticles
{
    public enum ParticlesLengthUnit
    {
        Pixels = 0,
        Percent = 1
    }

    public enum ParticlesOrigin
    {
        Center = 9
        , TopLeft = 10
        , TopRight = 11
        , BottomRight = 12
        , BottomLeft = 13

        , Element = 30
        , Transform = 31
    }

    public enum ParticlesEmitterShape
    {
        System = 0,
        BoxFill = 1
    }

#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class ParticleImage : Image
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ParticleImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlBoolAttributeDescription _playOnShow = new UxmlBoolAttributeDescription { name = "Play-On-Show", defaultValue = true };
            UxmlBoolAttributeDescription _restartOnShow = new UxmlBoolAttributeDescription { name = "Restart-On-Show", defaultValue = false };
            UxmlEnumAttributeDescription<ParticlesEmitterShape> _emitterShape = new UxmlEnumAttributeDescription<ParticlesEmitterShape> { name = "Emitter-Shape", defaultValue = ParticlesEmitterShape.System };
            UxmlEnumAttributeDescription<ParticlesOrigin> _origin = new UxmlEnumAttributeDescription<ParticlesOrigin> { name = "Origin", defaultValue = ParticlesOrigin.Center };
            UxmlEnumAttributeDescription<UIElementType> _originElementType = new UxmlEnumAttributeDescription<UIElementType> { name = "Origin-Element-Type", defaultValue = UIElementType.VisualElement };
            UxmlStringAttributeDescription _originElementName = new UxmlStringAttributeDescription { name = "Origin-Element-Name", defaultValue = "" };
            UxmlStringAttributeDescription _originElementClass = new UxmlStringAttributeDescription { name = "Origin-Element-Class", defaultValue = "" };

            UxmlFloatAttributeDescription _positionX = new UxmlFloatAttributeDescription { name = "Position-X", defaultValue = 0f };
            UxmlEnumAttributeDescription<ParticlesLengthUnit> _positionXUnit = new UxmlEnumAttributeDescription<ParticlesLengthUnit> { name = "Position-X-Unit", defaultValue = ParticlesLengthUnit.Percent };

            UxmlFloatAttributeDescription _positionY = new UxmlFloatAttributeDescription { name = "Position-Y", defaultValue = 0f };
            UxmlEnumAttributeDescription<ParticlesLengthUnit> _positionYUnit = new UxmlEnumAttributeDescription<ParticlesLengthUnit> { name = "Position-Y-Unit", defaultValue = ParticlesLengthUnit.Percent };

            UxmlColorAttributeDescription _tint = new UxmlColorAttributeDescription { name = "Tint", defaultValue = new Color(1f, 1f, 1f, 1f) };
            UxmlIntAttributeDescription _pixelsPerUnit = new UxmlIntAttributeDescription { name = "Pixels-Per-Unit", defaultValue = 50 };

            UxmlBoolAttributeDescription _addAttractor = new UxmlBoolAttributeDescription { name = "Add-Attractor", defaultValue = false };
            UxmlEnumAttributeDescription<UIElementType> _attractorElementType = new UxmlEnumAttributeDescription<UIElementType> { name = "Attractor-Element-Type", defaultValue = UIElementType.VisualElement };
            UxmlStringAttributeDescription _attractorElementName = new UxmlStringAttributeDescription { name = "Attractor-Element-Name", defaultValue = "" };
            UxmlStringAttributeDescription _attractorElementClass = new UxmlStringAttributeDescription { name = "Attractor-Element-Class", defaultValue = "" };

            UxmlBoolAttributeDescription _previewInGameView = new UxmlBoolAttributeDescription { name = "Preview-In-Game-View", defaultValue = false };
            UxmlBoolAttributeDescription _createParticleSystem = new UxmlBoolAttributeDescription { name = "Create-Particle-System", defaultValue = true };

            // What the UI Builder does is it finds (via reflection) the Init() call on the current element
            // and just calls it each time an attribute value has changed. This is why your Init() is called.
            // Source: https://forum.unity.com/threads/uxmltraits-and-custom-attributes-resetting-in-inspector.966215/#post-6311601

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var particleImage = ve as ParticleImage;

                particleImage.PlayOnShow = _playOnShow.GetValueFromBag(bag, cc);
                particleImage.RestartOnShow = _restartOnShow.GetValueFromBag(bag, cc);

                particleImage.PixelsPerUnit = _pixelsPerUnit.GetValueFromBag(bag, cc);
                particleImage.Tint = _tint.GetValueFromBag(bag, cc);
                particleImage.pickingMode = PickingMode.Ignore;

                particleImage.Origin = _origin.GetValueFromBag(bag, cc);
                particleImage.EmitterShape = _emitterShape.GetValueFromBag(bag, cc);

                particleImage.PositionX = _positionX.GetValueFromBag(bag, cc);
                particleImage.PositionXUnit = _positionXUnit.GetValueFromBag(bag, cc);
                particleImage.PositionY = _positionY.GetValueFromBag(bag, cc);
                particleImage.PositionYUnit = _positionYUnit.GetValueFromBag(bag, cc);

                particleImage.OriginElementType = _originElementType.GetValueFromBag(bag, cc);
                particleImage.OriginElementName = _originElementName.GetValueFromBag(bag, cc);
                particleImage.OriginElementClass = _originElementClass.GetValueFromBag(bag, cc);

                particleImage.AddAttractor = _addAttractor.GetValueFromBag(bag, cc);
                particleImage.AttractorElementType = _attractorElementType.GetValueFromBag(bag, cc);
                particleImage.AttractorElementName = _attractorElementName.GetValueFromBag(bag, cc);
                particleImage.AttractorElementClass = _attractorElementClass.GetValueFromBag(bag, cc);

                particleImage.PreviewInGameView = _previewInGameView.GetValueFromBag(bag, cc);
                particleImage.CreateParticleSystem = _createParticleSystem.GetValueFromBag(bag, cc);
            }
        }
#endif

        /// <summary>
        /// Should the particle system start playing whenever it is shown?
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Play-On-Show")]
#endif
        public bool PlayOnShow { get; set; } = true;

        /// <summary>
        /// Should the particle system restart if it is shown? NOTICE: This only has a effect if "PlayOnShow" is enabled.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Restart-On-Show")]
#endif
        public bool RestartOnShow { get; set; } = true;

        /// <summary>
        /// Triggered once the element is shown. It takes visibility, display, enable and opacity attributes into account.
        /// </summary>
        public event System.Action<ParticleImage> OnShow;

        /// <summary>
        /// Triggered once the element is hidden. It takes visibility, display, enable and opacity attributes into account.
        /// </summary>
        public event System.Action<ParticleImage> OnHide;

        public event System.Action OnInitialized;
        public bool IsInitialized;

        public const string GuidPrefix = "pi-";
        protected string _guid;
        public string Guid
        {
            get
            {
                if (_guid == null)
                {
                    _guid = GetGuidFromClasses();
                }
                return _guid;
            }
        }

        protected bool _createParticleSystem = true;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Create-Particle-System")]
#endif
        public bool CreateParticleSystem
        {
            get
            {
                return _createParticleSystem;
            }

            set
            {
                if (_createParticleSystem != value)
                {
                    _createParticleSystem = value;
                    if (value)
                    {
                        if (ParticleSystemForImage != null && ParticleSystemForImage.name.StartsWith(ParticleSystemForImage.TempParticleSystemPrefix))
                        {
                            Utils.SmartDestroy(ParticleSystemForImage.gameObject);
                        }
                    }
                    InitializeIfNecessary();
                }
            }
        }

        protected Texture _texture;

        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                if (ParticleSystemForImage != null)
                {
                    ParticleSystemForImage.Texture = value;
                }
            }
        }

        protected ParticleSystemForImage _particleSystemForImage;
        public ParticleSystemForImage ParticleSystemForImage
        {
            get
            {
                // Check if the game object is destroyed. Return null if it is.
                if (_particleSystemForImage != null && _particleSystemForImage.gameObject == null)
                    return null;

                return _particleSystemForImage;
            }

            set
            {
                _particleSystemForImage = value;
            }
        }

        public ParticleSystem ParticleSystem
        {
            get
            {
                if (ParticleSystemForImage == null)
                    return null;

                return ParticleSystemForImage.ParticleSystem;
            }
        }

        protected ParticleSystem.Particle[] _particles;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Origin")]
#endif
        public ParticlesOrigin Origin { get; set; } = ParticlesOrigin.Center;

        protected ParticlesEmitterShape _emitterShape = ParticlesEmitterShape.System;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Emitter-Shape")]
#endif
        public ParticlesEmitterShape EmitterShape
        {
            get
            {
                return _emitterShape;
            }

            set
            {
                if (_emitterShape == value)
                    return;

                _emitterShape = value;
                if (ParticleSystemForImage != null)
                {
                    ParticleSystemForImage.UpdateEmitterShape(value);
                }
            }
        }

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Position-X")]
#endif
        public float PositionX { get; set; } = 0f;
        public ParticlesLengthUnit PositionXUnit { get; set; } = ParticlesLengthUnit.Percent;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Position-Y")]
#endif
        public float PositionY { get; set; } = 0f;
        public ParticlesLengthUnit PositionYUnit { get; set; } = ParticlesLengthUnit.Percent;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Origin-Element-Type")]
#endif
        public UIElementType OriginElementType { get; set; } = UIElementType.VisualElement;

        protected bool _originElementIsDirty = true;
        protected VisualElement _originElement;
        public VisualElement OriginElement
        {
            get
            {
                // Query the element again if the query parameters have changed
                if (_originElementIsDirty)
                {
                    _originElementIsDirty = false;
                    _originElement = panel.visualTree.QueryType(OriginElementType, OriginElementName, OriginElementClass);
                }

                return _originElement;
            }

            set
            {
                _originElement = value;
                _originElementIsDirty = false;
            }
        }

        protected string _originElementName;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Origin-Element-Name")]
#endif
        public string OriginElementName
        {
            get => _originElementName;
            set
            {
                if (value != _originElementName)
                {
                    _originElementName = value;
                    _originElementIsDirty = true;
                }
            }
        }

        protected string _originElementClass;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Origin-Element-Class")]
#endif
        public string OriginElementClass
        {
            get => _originElementClass;
            set
            {
                if (value != _originElementClass)
                {
                    _originElementClass = value;
                    _originElementIsDirty = true;
                }
            }
        }

        protected Color _tint = new Color(1f, 1f, 1f, 1f);
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Tint")]
#endif
        public Color Tint
        {
            get => _tint;

            set
            {
                if (value != _tint)
                {
                    _tint = value;
                    MarkDirtyRepaint();
                }
            }
        }

        protected int _pixelsPerUnit = 50;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Pixels-Per-Unit")]
#endif
        public int PixelsPerUnit
        {
            get => _pixelsPerUnit;

            set
            {
                if (value != _pixelsPerUnit)
                {
                    _pixelsPerUnit = value;
                    MarkDirtyRepaint();
                }
            }
        }

        protected bool _previewInGameView = false;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Preview-In-Game-View")]
#endif
        public bool PreviewInGameView
        {
            get
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return true;
                }
#endif
                return _previewInGameView;
            }

            set
            {
                if (_previewInGameView == value)
                    return;

                _previewInGameView = value;
                ParticleSystemForImage?.UpdateEmitterShape(EmitterShape, forceRefresh: true);
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Notice that initialization is an async process if not executed manually from a script.
        /// </summary>
        /// <param name="immediate">Is only used at runtime.</param>
        /// <param name="onInitialized"></param>
        public void InitializeIfNecessary(bool immediate = true, System.Action onInitialized = null)
        {
            OnInitialized -= onInitialized;
            OnInitialized += onInitialized;

#if UNITY_EDITOR
            if (IsInPlayModeOrInBuild())
            {
#endif
                runtimeInitializeIfNecessary(immediate);
#if UNITY_EDITOR
            }
            else
            {
                // EDITOR
                // In the editor wait for one frame and then
                // add the GUID and register to the particle manager.
                UnityEditor.EditorApplication.update += editorInitializeIfNecessaryDelayed;
            }
#endif
        }

        protected async void runtimeInitializeIfNecessary(bool immediately = false)
        {
            // Wait at least one frame to give the scene time to activate the objects
            if (!immediately)
                await System.Threading.Tasks.Task.Delay(20);

            // Wait for UI Document to load and finish layouting.
            if (!immediately)
            {
                int maxWait = 5;
                while (float.IsNaN(resolvedStyle.width) && maxWait-- > 0)
                {
                    await System.Threading.Tasks.Task.Delay(20);
                }
            }

            if (string.IsNullOrEmpty(Guid))
            {
                _guid = createGuid();
                AddClass(_guid);
            }

            ParticleManager.Instance.RegisterElement(this);
            postInitialization();
        }

#if UNITY_EDITOR
        protected void editorInitializeIfNecessaryDelayed()
        {
            EditorApplication.update -= editorInitializeIfNecessaryDelayed;

            // If there is no GUID yet, then create one now.
            if (string.IsNullOrEmpty(Guid))
            {
                // Check there already is a guid class
                string guid = GetGuidFromClasses();
                _guid = guid;

                // If not then add one
                if (string.IsNullOrEmpty(guid))
                {
                    // Execute initialization only on the element in the UI Builder
                    if (IsPartOfUIBuilder() && !IsPartOfUIBuilderPreview())
                    {
                        guid = createGuid();
                        _guid = guid;
                        AddClass(guid);
                    }
                }
            }

            if (!IsPartOfUIBuilderPreview())
            {
                EditorApplication.update += editorRegisterDelayed;
            }
        }

        protected void editorRegisterDelayed()
        {
            EditorApplication.update -= editorRegisterDelayed;

            ParticleManager.Instance.RegisterElement(this);
            postInitialization();
        }
#endif

        protected void postInitialization()
        {
            if (ParticleSystemForImage != null && IsInPlayModeOrInBuild())
            {
                if (PlayOnShow)
                {
                    ParticleSystemForImage.Play(restart: RestartOnShow, withChildren: true);
                }
            }

            if (AddAttractor)
            {
                enableAttractor();
            }

            OnInitialized?.Invoke();
            IsInitialized = true;
        }

        protected string createGuid()
        {
            return GuidPrefix + System.Guid.NewGuid().ToString();
        }

        public string GetGuidAbbreviation()
        {
            if (string.IsNullOrEmpty(Guid))
                return "";

            string abbr = Guid.Replace(GuidPrefix, "").Substring(0, 8);
            return abbr;
        }

        public void AddClass(string className)
        {
            if (string.IsNullOrEmpty(className))
                return;

            // First add it to the element
            AddToClassList(className);

#if UNITY_EDITOR
            // Second, add it to the asset
            // Adding new class is based on:
            // https://github.com/Unity-Technologies/UnityCsReference/blob/ddda763e79614c13cb63ec8b9dac375fd3be2b24/Modules/UIBuilder/Editor/Builder/Inspector/BuilderInspectorInheritedStyles.cs#L146
            if (!IsInPlayModeOrInBuild())
            {
                UIBuilderWindowWrapper.Instance.AddClassToList(className);
            }
#endif
        }

        protected bool? _isPartOfUIBuilder = null;
        public bool IsPartOfUIBuilder()
        {
            if (!_isPartOfUIBuilder.HasValue)
            {
                _isPartOfUIBuilder = Utils.IsPartOfUIBuilderCanvas(this);
            }
            return _isPartOfUIBuilder.Value;
        }

        protected bool? _isPartOfUIBuilderPreview = null;
        public bool IsPartOfUIBuilderPreview()
        {
            if (!_isPartOfUIBuilderPreview.HasValue)
            {
                _isPartOfUIBuilderPreview = Utils.IsPartOfUIBuilderButNotInTheCanvas(this);
            }
            return _isPartOfUIBuilderPreview.Value;
        }

        public string GetGuidFromClasses()
        {
            if (_guid == null)
            {
                var classes = GetClasses();
                foreach (var className in classes)
                {
                    if (className.StartsWith(GuidPrefix))
                    {
                        _guid = className;
                        break;
                    }
                }
            }

            return _guid;
        }

        public void Pause(bool withChildren = true)
        {
            if (ParticleSystemForImage != null)
                ParticleSystemForImage.Pause(withChildren);
        }

        public void Play()
        {
            if (ParticleSystemForImage != null)
                ParticleSystemForImage.Play();
        }

        public void Stop(bool withChildren = true, ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
        {
            if (ParticleSystemForImage != null)
                ParticleSystemForImage.Stop(withChildren, stopBehaviour);
        }

        public bool IsPlaying()
        {
            if (ParticleSystemForImage != null && ParticleSystemForImage.ParticleSystem != null)
                return ParticleSystemForImage.ParticleSystem.isPlaying;
            return false;
        }

        public bool IsPaused()
        {
            if (ParticleSystemForImage != null && ParticleSystemForImage.ParticleSystem != null)
                return ParticleSystemForImage.ParticleSystem.isPaused;
            return false;
        }

        public ParticleImage()
        {
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<AttachToPanelEvent>(attach);
            RegisterCallback<DetachFromPanelEvent>(detach);
        }

        void attach(AttachToPanelEvent evt)
        {
#if UNITY_EDITOR
            // Skip if part of builder preview
            if (IsPartOfUIBuilderPreview())
                return;
#endif
            InitializeIfNecessary(immediate: false);
            waitForInitialLayout();
        }

        protected Rect? _initialContentRect = null;
        protected Rect? _initialWorldContentRect = null;
        protected async void waitForInitialLayout()
        {
            int tries = 100;
            while (float.IsNaN(layout.width) && tries-- > 0)
            {
                await System.Threading.Tasks.Task.Delay(5);
            }
            _initialContentRect = contentRect;
            _initialWorldContentRect = this.LocalToWorld(contentRect);
        }

        public bool IsActive => visible && enabledInHierarchy && resolvedStyle.display != DisplayStyle.None && resolvedStyle.opacity > 0f;

        public bool IsInPlayModeOrInBuild()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
        }

        public bool IsInEditor()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        void detach(DetachFromPanelEvent evt)
        {
#if UNITY_EDITOR
            // Skip if part of builder preview
            if (IsPartOfUIBuilderPreview())
                return;
#endif
            ParticleManager.Instance.UnregisterElement(this);
        }

        // Mesh Data caches
        Vertex[] _vertices;
        ushort[] _indices;

        public virtual void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            // Remember: "generateVisualContent is an addition to the default rendering, it's not a replacement"
            // See: https://forum.unity.com/threads/hp-bars-at-runtime-image-masking-or-fill.1076486/#post-6948578 

            if (ParticleManager.Instance == null)
                return;

            if (ParticleSystemForImage == null || ParticleSystemForImage.ParticleSystem == null)
                return;

            updateVisibilityEvents();

            int maxParticles = ParticleSystemForImage.ParticleSystem.main.maxParticles;

            if (_particles == null || _particles.Length < maxParticles)
                _particles = new ParticleSystem.Particle[maxParticles];

            if (_vertices == null || _vertices.Length < maxParticles * 4)
            {
                _vertices = new Vertex[maxParticles * 4];
                _indices = new ushort[maxParticles * 6];
            }

            // Fetch particles from ParticleSystem
            int numParticlesAlive = ParticleSystem.GetParticles(_particles);

            float width = float.IsNaN(resolvedStyle.width) ? 0f : resolvedStyle.width;
            float height = float.IsNaN(resolvedStyle.height) ? 0f : resolvedStyle.height;
            Vector3 origin = resolveOrigin(Origin, width, height);

            // Abort if num of active particles is zero
            if (numParticlesAlive == 0)
                return;

            MeshWriteData mwd = mgc.Allocate(_vertices.Length, _indices.Length, Texture);

            updateAttractorPosition(origin);

            int vertex = 0;
            int index = 0;

            // Cache values needed for every particle
            float startRotationConst = ParticleSystem.main.startRotation.constant * Mathf.Rad2Deg;

            // Create one quad per particle
            for (int i = 0; i < numParticlesAlive; i++)
            {
                var position = (_particles[i].position - ParticleSystemForImage.DefaultPosition) * PixelsPerUnit;
                position.y *= -1; // Convert from Y = Up to Y = Down.
                var size = _particles[i].GetCurrentSize(ParticleSystem) * PixelsPerUnit;
                var color = _particles[i].GetCurrentColor(ParticleSystem) * Tint;
                var rotation = Quaternion.Euler(_particles[i].rotation3D);

                // Interpret "alignToDirection" as always align to direction (not only at the start).
                if (ParticleSystem.shape.alignToDirection)
                {
                    var velocity2D = ((Vector2)_particles[i].velocity).normalized;
                    var angle = Mathf.Atan2(velocity2D.y, velocity2D.x) * Mathf.Rad2Deg;
                    rotation = Quaternion.Euler(0f, 0f, -angle + startRotationConst);
                }

                createQuad(ref vertex, _vertices, ref index, _indices, origin + position, rotation, size, color);
            }
            // Move all the other particles off screen and make them invisible
            for (int i = numParticlesAlive; i < maxParticles; i++)
            {
                var position = new Vector3(-300000, -300000, -300000);
                var size = _particles[i].GetCurrentSize(ParticleSystem) * 0.001f;
                var color = new Color(0, 0, 0, 0);
                var rotation = Quaternion.identity;
                createQuad(ref vertex, _vertices, ref index, _indices, position, rotation, size, color);
            }

            mwd.SetAllVertices(_vertices);
            mwd.SetAllIndices(_indices);
        }

        private Vector3 resolveOrigin(ParticlesOrigin originSetting, float width, float height)
        {
            var unitX = PositionXUnit;
            var unitY = PositionYUnit;

            if (originSetting == ParticlesOrigin.Element)
            {
                unitX = ParticlesLengthUnit.Pixels;
                unitY = ParticlesLengthUnit.Pixels;

                // Reset to Center if the origin type is "Element" but no valid element was found.
                if (OriginElement == null || parent == null)
                {
                    originSetting = ParticlesOrigin.Center;
                }
            }

            if (originSetting == ParticlesOrigin.Transform)
            {
                unitX = ParticlesLengthUnit.Pixels;
                unitY = ParticlesLengthUnit.Pixels;

                // Reset to Center if the origin type is "Transform" but not valid element was found.
                if (ParticleSystemForImage == null || ParticleSystemForImage.OriginTransform == null || panel == null || panel is not IPanel || Camera.main == null)
                {
                    originSetting = ParticlesOrigin.Center;
                }
            }

            // How far the particle origin should be from the calculated origin.
            Vector3 delta = new Vector3(
                PositionXUnit == ParticlesLengthUnit.Pixels ? PositionX : width * PositionX / 100f,
                PositionYUnit == ParticlesLengthUnit.Pixels ? PositionY : height * PositionY / 100f
            );

            // Reset the particle system position to the default
            if (originSetting != ParticlesOrigin.Element
                && originSetting != ParticlesOrigin.Transform
                && ParticleSystem.main.simulationSpace != ParticleSystemSimulationSpace.World)
            {
                ParticleSystemForImage.ResetTransform();
            }

            Vector3 origin = new Vector3(0, 0, 0);
            Vector2 deltaLocal = Vector2.zero;
            switch (originSetting)
            {
                case ParticlesOrigin.Element:
                    deltaLocal = getCenterPosLocalDelta(this, OriginElement);

                    if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                    {
                        // Move the particles inside the ParticleImage element.
                        origin.x = width * 0.5f + deltaLocal.x;
                        origin.y = height * 0.5f + deltaLocal.y;
                        ParticleSystemForImage.ResetTransform();
                    }
                    else if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
                    {
                        // Here we do NOT move the particles inside the VisualElement.
                        // Instead we move the particle system in the WORLD. That way we can retain the
                        // properties set by the SimulationSpace setting (world space particles will be left behind).
                        //
                        // Since this changes the world position of the particle system only ONE ParticleImage can control it.
                        // We have to choose. Is it controlled by the Image in the UI Builder or the one in the Game View.
                        // Usually we will pick the game view since that's the important one.
                        bool isPartOfUIBuilder = IsPartOfUIBuilder();
#if UNITY_EDITOR
                        if (!PreviewInGameView)
                        {
                            isPartOfUIBuilder = !isPartOfUIBuilder;
                        }
#endif
                        if (!IsInEditor() || !isPartOfUIBuilder)
                        {
                            deltaLocal.y *= -1; // y axis conversion (down to up)
                            var deltaInWorldSpace = deltaLocal / (float)PixelsPerUnit;
                            // Debug.Log(deltaPos + " > " + deltaPosLocal + " - " + zeroPosLocal + " > " + deltaInWorldSpace);
                            ParticleSystemForImage.ApplyPositionDelta(deltaInWorldSpace);
                        }

                        origin.x = width * 0.5f;
                        origin.y = height * 0.5f;
                    }
                    else
                    {
                        origin.x = width * 0.5f;
                        origin.y = height * 0.5f;
                    }

                    break;

                case ParticlesOrigin.Transform:
                    if (!IsPartOfUIBuilder())
                    {
                        Vector2 localPos = worldSpaceToUISpace(ParticleSystemForImage.OriginTransform.position, this, Camera.main);
                        if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                        {
                            // Move the particles inside the ParticleImage element.
                            origin = localPos;
                            ParticleSystemForImage.ResetTransform();
                        }
                        else if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
                        {
                            // Here we do NOT move the particles inside the VisualElement.
                            // Instead we move the particle system in the WORLD. That way we can retain the
                            // properties set by the SimulationSpace setting (old particles will be left behind).
                            deltaLocal = localPos - new Vector2(width * 0.5f, height * 0.5f);
                            deltaLocal.y *= -1; // y axis conversion (down to up)
                            var deltaInWorldSpace = deltaLocal / (float)PixelsPerUnit;
                            // Debug.Log(deltaLocal + " > " + " > " + deltaInWorldSpace);
                            ParticleSystemForImage.ApplyPositionDelta(deltaInWorldSpace);

                            origin.x = width * 0.5f;
                            origin.y = height * 0.5f;
                        }
                        else
                        {
                            origin.x = width * 0.5f;
                            origin.y = height * 0.5f;
                        }
                    }
                    else
                    {
                        origin.x = width * 0.5f;
                        origin.y = height * 0.5f;
                    }
                    break;

                case ParticlesOrigin.TopRight:
                    origin.x = width;
                    delta.x = -delta.x;
                    applyWorldSimulationSpace(ref origin, ref deltaLocal, delta);
                    break;

                case ParticlesOrigin.BottomRight:
                    origin.x = width;
                    origin.y = height;
                    delta.x = -delta.x;
                    delta.y = -delta.y;
                    applyWorldSimulationSpace(ref origin, ref deltaLocal, delta);
                    break;

                case ParticlesOrigin.BottomLeft:
                    origin.y = height;
                    delta.y = -delta.y;
                    applyWorldSimulationSpace(ref origin, ref deltaLocal, delta);
                    break;

                case ParticlesOrigin.Center:
                    origin.x = width * 0.5f;
                    origin.y = height * 0.5f;
                    applyWorldSimulationSpace(ref origin, ref deltaLocal, delta);
                    break;

                case ParticlesOrigin.TopLeft:
                default:
                    origin.x = 0f;
                    origin.y = 0f;
                    applyWorldSimulationSpace(ref origin, ref deltaLocal, delta);
                    break;
            }

            if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                return new Vector3(origin.x, origin.y, origin.z);
            }
            else
            {
                return new Vector3(origin.x + delta.x, origin.y + delta.y, origin.z);
            }
        }

        private void applyWorldSimulationSpace(ref Vector3 origin, ref Vector2 deltaLocal, Vector3 positionDeltaInPx)
        {
            if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                if (_initialWorldContentRect.HasValue)
                {
                    deltaLocal = getCenterPosLocalDelta(_initialWorldContentRect.Value.center, this);
                    // Here we do NOT move the particles inside the VisualElement.
                    // Instead we move the particle system in the WORLD. That way we can retain the
                    // properties set by the SimulationSpace setting (old particles will be left behind).
                    var deltaInWorldSpace = (deltaLocal + (Vector2)positionDeltaInPx) / (float)PixelsPerUnit; // we also add the position delta here.
                    deltaInWorldSpace.y *= -1; // y axis conversion (down to up)
                    // While in in play mode or in build do NOT change the particle system position if the change
                    // is coming from an element inside the UIBuilder.
                    if (!(IsInPlayModeOrInBuild() && IsPartOfUIBuilder()))
                    {
                        ParticleSystemForImage.ApplyPositionDelta(deltaInWorldSpace);
                    }

                    origin.x -= deltaLocal.x;
                    origin.y -= deltaLocal.y;
                }
            }
        }

        /// <summary>
        /// Returns the distance vector between the element's center and the target's center in target's local space.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector2 getCenterPosLocalDelta(VisualElement element, VisualElement target)
        {
            var sourceWorldCenter = element.LocalToWorld(element.contentRect.center);
            var targetWorldCenter = target.LocalToWorld(target.contentRect.center);
            var deltaPos = new Vector2(
                 targetWorldCenter.x - sourceWorldCenter.x
                , targetWorldCenter.y - sourceWorldCenter.y
            );

            // All of this is done to transform the deltas from one element to another.
            var deltaPosLocal = target.WorldToLocal(deltaPos);
            var zeroPosLocal = target.WorldToLocal(Vector2.zero);
            Vector2 deltaLocal = deltaPosLocal - zeroPosLocal;
            return deltaLocal;
        }

        /// <summary>
        /// Returns the distance vector between the world position and the target's center in target's local space.
        /// </summary>
        /// <param name="sourceWorldCenter"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector2 getCenterPosLocalDelta(Vector2 sourceWorldCenter, VisualElement target)
        {
            var targetWorldCenter = target.LocalToWorld(target.contentRect.center);
            var deltaPos = new Vector2(
                  targetWorldCenter.x - sourceWorldCenter.x
                , targetWorldCenter.y - sourceWorldCenter.y
            );

            // All of this is done to transform the deltas from one element to another.
            var deltaPosLocal = target.WorldToLocal(deltaPos);
            var zeroPosLocal = target.WorldToLocal(Vector2.zero);
            Vector2 deltaLocal = deltaPosLocal - zeroPosLocal;
            return deltaLocal;
        }

        /// <summary>
        /// Transform the absolute world space position to the local UI position based on the camera and the panel of the given ui target. The position is in the target's local space.
        /// </summary>
        /// <param name="worldSpacePos"></param>
        /// <param name="target"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        private Vector2 worldSpaceToUISpace(Vector3 worldSpacePos, VisualElement target, Camera cam = null)
        {
            if (cam == null)
                cam = Camera.main;

            var worldPos = RuntimePanelUtils.CameraTransformWorldToPanel(target.panel, worldSpacePos, cam);
            var localPos = target.WorldToLocal(worldPos);

            return localPos;
        }

        private void createQuad(ref int vertex, Vertex[] vertices, ref int index, ushort[] indices, Vector3 pos, Quaternion rotation, float size, Color tint)
        {
            // The coordinate system starts top left and clock-wise oriented tris are front facing.

            vertices[vertex].position = new Vector3(pos.x - size * 0.5f, pos.y - size * 0.5f, Vertex.nearZ);
            vertices[vertex].position = rotateAround(pos, vertices[vertex].position, rotation);
            vertices[vertex].uv = new Vector2(0f, 1f);
            vertices[vertex].tint = tint;
            vertex++;

            vertices[vertex].position = new Vector3(pos.x + size * 0.5f, pos.y - size * 0.5f, Vertex.nearZ);
            vertices[vertex].position = rotateAround(pos, vertices[vertex].position, rotation);
            vertices[vertex].uv = new Vector2(1f, 1f);
            vertices[vertex].tint = tint;
            vertex++;

            vertices[vertex].position = new Vector3(pos.x + size * 0.5f, pos.y + size * 0.5f, Vertex.nearZ);
            vertices[vertex].position = rotateAround(pos, vertices[vertex].position, rotation);
            vertices[vertex].uv = new Vector2(1f, 0f);
            vertices[vertex].tint = tint;
            vertex++;

            vertices[vertex].position = new Vector3(pos.x - size * 0.5f, pos.y + size * 0.5f, Vertex.nearZ);
            vertices[vertex].position = rotateAround(pos, vertices[vertex].position, rotation);
            vertices[vertex].uv = new Vector2(0f, 0f);
            vertices[vertex].tint = tint;
            vertex++;

            indices[index++] = (ushort)(vertex - 4);
            indices[index++] = (ushort)(vertex - 3);
            indices[index++] = (ushort)(vertex - 1);

            indices[index++] = (ushort)(vertex - 3);
            indices[index++] = (ushort)(vertex - 2);
            indices[index++] = (ushort)(vertex - 1);
        }

        private Vector3 rotateAround(Vector3 pivot, Vector3 point, float angle)
        {
            if (angle == 0f)
                return point;

            Quaternion rot = Quaternion.Euler(new Vector3(0f, 0f, angle));

            var result = point - pivot;
            result = rot * result;
            result = pivot + result;
            return result;
        }

        private Vector3 rotateAround(Vector3 pivot, Vector3 point, Quaternion rotation)
        {
            var result = point - pivot;
            result = rotation * result;
            result = pivot + result;
            return result;
        }

        public Vector2 GetOriginAsNormalizedPos()
        {
            switch (Origin)
            {
                case ParticlesOrigin.TopRight:
                    return new Vector2(1, 0);

                case ParticlesOrigin.BottomRight:
                    return new Vector2(1, 1);

                case ParticlesOrigin.BottomLeft:
                    return new Vector2(0, 1);

                case ParticlesOrigin.Center:
                    return new Vector2(0.5f, 0.5f);

                case ParticlesOrigin.TopLeft:
                default:
                    return new Vector2(0, 0);
            }
        }

        protected bool? _lastKnownVisibility = null;

        protected void updateVisibilityEvents()
        {
            bool isShown = IsShown();
            bool changed = false;

            if (_lastKnownVisibility.HasValue)
            {
                if (isShown != _lastKnownVisibility.Value)
                    changed = true;
            }
            else
            {
                changed = true;
            }

            _lastKnownVisibility = isShown;

            if (changed)
            {
                if (isShown)
                    onShow();
                else
                    onHide();
            }
        }

        protected void onShow()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return;
#endif

            if (PlayOnShow)
                ParticleSystemForImage.ResumeDueToActivity(RestartOnShow);

            OnShow?.Invoke(this);
        }

        protected void onHide()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return;
#endif

            OnHide?.Invoke(this);
        }

        public bool IsShown()
        {
            return visible && enabledInHierarchy && resolvedStyle.display != DisplayStyle.None && resolvedStyle.opacity > 0f;
        }

        public void Show()
        {
            visible = true;
            if (!enabledSelf)
                SetEnabled(true);
            style.display = DisplayStyle.Flex;
            style.opacity = 1f;
        }

        public void Hide(bool useVisibility = false)
        {
            if (useVisibility)
            {
                visible = false;
            }
            else
            {
                style.display = DisplayStyle.None;
            }
        }

        public void Toggle(bool useVisibility = false)
        {
            if (IsShown())
            {
                Hide(useVisibility);
            }
            else
            {
                Show();
            }
        }
    }
}
