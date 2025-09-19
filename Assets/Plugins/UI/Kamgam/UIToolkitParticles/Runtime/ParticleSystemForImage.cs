using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    [ExecuteAlways]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemForImage : MonoBehaviour
    {
        public const string TempParticleSystemPrefix = "[Temp]";

        public static Vector3 DefaultPosition = new Vector3(0f, 0f, -1000f);

        public string Guid;

        [SerializeField]
        protected Texture _texture;

        public Texture Texture
        {
            get => _texture;
            set
            {
                if (value != _texture)
                {
                    _texture = value;
                    RefreshInfosOnImages();
                }
            }
        }

        /// <summary>
        /// The transform used as position source if the Origin of the ParticleImage is set to Transform.
        /// </summary>
        public Transform OriginTransform;

        // Why multiple? Well, there can be an instance in the UIBuilder and one
        // in the GameView and maybe some more if the UI was instantiated multiple times
        // with the same guid.
        protected List<ParticleImage> _images = new List<ParticleImage>();

        protected bool? _prePausedDueToInactivityWasPlaying = null;
        protected bool _pausedDueToInactivity = false;

        public void PauseDueToInactivity()
        {
            if (_pausedDueToInactivity)
                return;

            _pausedDueToInactivity = true;
            _prePausedDueToInactivityWasPlaying = ParticleSystem.isPlaying;
            ParticleSystem.Pause();
        }

        public void Play(bool restart = false, bool withChildren = true)
        {
            resetPausedDueToActivitiy();
            if (restart)
                ParticleSystem.Simulate(0, withChildren: true, restart: true);
            ParticleSystem.Play(withChildren);
        }

        public void Pause(bool withChildren = true)
        {
            resetPausedDueToActivitiy();
            ParticleSystem.Pause(withChildren);
        }

        public void Stop(bool withChildren = true, ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
        {
            resetPausedDueToActivitiy();
            ParticleSystem.Stop(withChildren, stopBehaviour);
        }

        public void Reset(bool withChildren = true)
        {
            ParticleSystem.Simulate(0, withChildren, restart: true);
        }

        protected void resetPausedDueToActivitiy()
        {
            _pausedDueToInactivity = false;
            _prePausedDueToInactivityWasPlaying = null;
        }

        public void ResumeDueToActivity(bool restartOnShow)
        {
            if (!_pausedDueToInactivity)
                return;

            _pausedDueToInactivity = false;
            if (_prePausedDueToInactivityWasPlaying.HasValue && _prePausedDueToInactivityWasPlaying.Value)
            {
                if (restartOnShow)
                {
                    ParticleSystem.Simulate(0, withChildren: true, restart: true);
                }
                ParticleSystem.Play(withChildren: true);
                
            }
        }

        public void Defrag()
        {
            for (int i = _images.Count - 1; i >= 0; i--)
            {
                if (_images[i] == null || _images[i].panel == null)
                {
                    _images.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns the first particle image.
        /// If there is an active image it will return it first (prefers active images over inactive ones).
        /// </summary>
        /// <returns></returns>
        protected ParticleImage getFirstReferenceImage()
        {
            Defrag();

            if (_images == null || _images.Count == 0)
                return null;

            // Prefer active
            foreach (var img in _images)
            {
                if (img.panel == null || !img.IsActive)
                    continue;

#if UNITY_EDITOR
                if (Utils.IsPartOfUIBuilderButNotInTheCanvas(img))
                    continue;

                if (img.PreviewInGameView && Utils.IsPartOfUIBuilderCanvas(img))
                    continue;
#endif

                return img;
            }

            foreach (var img in _images)
            {
                if (img.panel == null)
                    continue;

#if UNITY_EDITOR
                if (Utils.IsPartOfUIBuilderButNotInTheCanvas(img))
                    continue;

                if (img.PreviewInGameView && Utils.IsPartOfUIBuilderCanvas(img))
                    continue;
#endif

                return img;
            }

            return null;
        }

        protected ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem
        {
            get
            {
                if (_particleSystem == null && this != null)
                {
                    _particleSystem = this.GetComponent<ParticleSystem>();
                }
                return _particleSystem;
            }
        }

        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = this.GetComponentInParent<UIDocument>(includeInactive: true);
                }
                return _document;
            }
        }

        public void OnEnable()
        {
            ParticleManager.Instance.RegisterParticleSystem(this);
        }

        public void OnDisable()
        {
            ParticleManager.Instance.UnregisterParticleSystem(this);
        }

        public void InitializeAfterCreation(ParticleImage image)
        {
            // Disable renderer, we don't need it.
            var renderer = ParticleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.enabled = false;

            // Make sure the system is always simulated.
            var main = ParticleSystem.main;
            main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.playOnAwake = false;
            main.maxParticles = 100;

            if (image.Texture != null)
                Texture = image.Texture;

            UpdateEmitterShape(image.EmitterShape);

#if UNITY_EDITOR
            EditorPlayParticles = true;
#endif
        }

        public void ResetTransform()
        {
            transform.position = DefaultPosition;
            transform.localRotation = Quaternion.identity;
        }

        public void ApplyPositionDelta(Vector2 delta)
        {
            transform.position = new Vector3(
                DefaultPosition.x + delta.x,
                DefaultPosition.y + delta.y,
                DefaultPosition.z
            );
            transform.localRotation = Quaternion.identity;
        }

        public void AddImage(ParticleImage img)
        {
            if(!_images.Contains(img))
                _images.Add(img);
        }

        public void RemoveImage(ParticleImage img)
        {
            if (_images.Contains(img))
                _images.Remove(img);
        }

        public void RefreshSystemOnImages()
        {
            foreach (var image in _images)
            {
                if(image != null)
                    image.ParticleSystemForImage = this;
            }
        }

        public void RefreshInfosOnImages()
        {
            foreach (var image in _images)
            {
                if (image == null)
                    continue;

                image.ParticleSystemForImage = this;
                image.Texture = Texture;
            }

            var img = getFirstReferenceImage();
            if (img != null)
                UpdateEmitterShape(img.EmitterShape);
        }

        public void ClearSystemOnImages()
        {
            foreach (var image in _images)
            {
                if (image != null)
                    image.ParticleSystemForImage = null;
            }
        }

        protected ParticlesEmitterShape? _lastKnownEmitterShape = null;
        protected ParticleSystemShapeType? _lastKnownEmitterShapeType = null;

        public void UpdateEmitterShape(ParticlesEmitterShape shape, bool forceRefresh = false)
        {
            if (ParticleSystem == null)
                return;

            var shapeModule = ParticleSystem.shape;
            bool changed = !_lastKnownEmitterShape.HasValue || shape != _lastKnownEmitterShape.Value;
            _lastKnownEmitterShape = shape;

            if (changed || forceRefresh)
            {
                if (!_lastKnownEmitterShapeType.HasValue)
                    _lastKnownEmitterShapeType = shapeModule.shapeType;

                switch (shape)
                {
                    case ParticlesEmitterShape.BoxFill:
                        var img = getFirstReferenceImage();
                        if (img != null && !float.IsNaN(img.resolvedStyle.width))
                        {
                            shapeModule.shapeType = ParticleSystemShapeType.Rectangle;
                            shapeModule.scale = new Vector3(
                                img.resolvedStyle.width / img.PixelsPerUnit,
                                img.resolvedStyle.height / img.PixelsPerUnit,
                                1.01f); // The 1.01f is used as a flag for none-user-created rect shape. If the z scale is 1.01f it is assume to have been set by this code.
                        }
                        break;

                    case ParticlesEmitterShape.System:
                    default:
                        // Only reset if there is a known value and it has not been changed by the user.
                        if (shapeModule.shapeType != ParticleSystemShapeType.Rectangle || !Mathf.Approximately(shapeModule.scale.z, 1.01f)) // Check for flag to determine if set by user.
                            _lastKnownEmitterShapeType = null; // Reset if user changed it
                        if (_lastKnownEmitterShapeType.HasValue)
                        {
                            // Reset to cone if rect was remembered as this is probably wrong.
                            if (_lastKnownEmitterShapeType.Value == ParticleSystemShapeType.Rectangle)
                                _lastKnownEmitterShapeType = ParticleSystemShapeType.Cone;
                            shapeModule.shapeType = _lastKnownEmitterShapeType.Value;
                            shapeModule.scale = Vector3.one;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the local pos inside the 3D particle system.
        /// </summary>
        /// <param name="normalizedLocalPosInVisualElement">0/0 = top left, 1/1 = bottom right</param>
        public Vector2 GetLocalPos3D(Vector2 normalizedLocalPosInVisualElement)
        {
            var img = getFirstReferenceImage();
            if (img != null && !float.IsNaN(img.resolvedStyle.width))
            {
                float dx = img.resolvedStyle.width / img.PixelsPerUnit * (normalizedLocalPosInVisualElement.x - img.GetOriginAsNormalizedPos().x);
                float dy = img.resolvedStyle.height / img.PixelsPerUnit * (normalizedLocalPosInVisualElement.y - img.GetOriginAsNormalizedPos().y);
                dy = -dy; // convert from y = down to y = up

                // Invert world space displacement
                //dx -= ParticleSystem.transform.position.x - DefaultPosition.x;
                //dy -= ParticleSystem.transform.position.y - DefaultPosition.y;

                return new Vector2(dx, dy);
            }

            return Vector2.zero;
        }

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool EditorPlayParticles = false;

        void OnValidate()
        {
            RefreshInfosOnImages();
        }
#endif
    }
}
