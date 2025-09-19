using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    /// <summary>
    /// This manager keeps track of all particle systems currently needed.
    /// </summary>
    public class ParticleManager
    {
        static ParticleManager _instance;
        public static ParticleManager Instance // This is triggered by the UI Toolkit Elements
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ParticleManager();
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        // Delay start if in between play mode changes.
                        // We need this because the UI Elements trigger the creation of the instance in between playmode changes.
                        EditorApplication.playModeStateChanged += delayedStart;
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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitIfDomainReloadIsTurnedOff()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !ParticleManagerUpdater.HasInstance && _instance != null)
            {
                _instance.Start();
            }
        }

        static void delayedStart(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode && _instance != null)
            {
                _instance.Start();
            }
            EditorApplication.playModeStateChanged -= delayedStart;
        }
#endif

        // -------------------

        /// <summary>
        /// Keeps track of how many active particle images there are.
        /// </summary>
        protected List<ParticleImage> _images = new List<ParticleImage>();

        /// <summary>
        /// Keeps track of how many particle systems MonoBehaviours there are.
        /// </summary>
        protected List<ParticleSystemForImage> _particleSystems = new List<ParticleSystemForImage>();

        public void Defrag()
        {
            for (int i = _particleSystems.Count-1; i >= 0; i--)
            {
                if (    _particleSystems[i] == null
                    ||  _particleSystems[i].gameObject == null
                    || (_particleSystems[i].gameObject != null && _particleSystems[i].gameObject.scene == null)
                    || (_particleSystems[i].gameObject != null && _particleSystems[i].gameObject.scene != null && !_particleSystems[i].gameObject.scene.isLoaded)
                    )
                {
                    _particleSystems.RemoveAt(i);
                }
            }

            for (int i = _images.Count - 1; i >= 0; i--)
            {
                if (_images[i] == null || _images[i].panel == null)
                {
                    _images.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Registers an image and hooks it up with the particle system in the scene.
        /// </summary>
        /// <param name="image"></param>
        public void RegisterElement(ParticleImage image)
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif

            Defrag();

            if (string.IsNullOrEmpty(image.Guid)) 
            {
                // The execution will be stopped if the image does not yet have a GUID.
                // Logger.Log("Registering image with empty GUID. This will not work. Element path: " + Utils.GetVisualElementPath(image));
                return;
            }

            if (!_images.Contains(image))
            {
                _images.Add(image);
            }

            // Find system (or create one)
            var system = FindParticleSystem(image.Guid);
            if (system == null)
            {
                system = createParticleSystemForImage(image);
            }
            if (system != null)
            {
                system.AddImage(image);
                system.RefreshInfosOnImages();
            }

            image.MarkDirtyRepaint();
        }

        private static ParticleSystemForImage createParticleSystemForImage(ParticleImage image)
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return null;
#endif

            Logger.Log("Trying to create a particle system for ParticleImage " + image.Guid);

            ParticleSystemForImage system = null;
            GameObject go = null;
            try
            {
                var documents = Utils.FindObjectsOfTypeFast<UIDocument>(includeInactive: true);
                UIDocument document = null;
                ParticleImage imgInDoc = null;
                foreach (var doc in documents)
                {
                    imgInDoc = doc.rootVisualElement.Q<ParticleImage>(className: image.Guid);
                    if (imgInDoc != null)
                    {
                        document = doc;
                        break;
                    }
                }
                if (document == null)
                {
#if UNITY_EDITOR
                    if (!EditorApplication.isPlayingOrWillChangePlaymode && image.CreateParticleSystem && UIToolkitParticlesSettings.GetOrCreateSettings().ShowNoDocumentWarning)
                    {
#endif
                        Logger.LogWarning(
                            "There is no UI Document containing this ParticleImage in the Scene. More infos below:\n\n" +
                            "Maybe you forgot to save your UI? Each ParticleImage needs a particle system in the scene with a matching GUID. Please open the " +
                            "scene with the UI Document that contains the UI layout you are editing BEFORE adding a Particle Image.\n" +
                            "If you have none yet then please add a UI Document using the current UI layout to the scene.\n\n" +
                            "Please remember that particles will only be shown if you have a matching particle system in the scene.\n\n" +
                            "Please double check that you are editing the UI that is linked in the source Asset of the UI Document in your scene.");
#if UNITY_EDITOR
                    }
#endif
                }
                if (document != null || !image.CreateParticleSystem)
                {
                    bool systemIsTemporary = (document == null && !image.CreateParticleSystem);
                    // Create new system if not found
                    go = new GameObject("Particles for Image " + image.GetGuidAbbreviation());
                    if (document != null)
                        go.transform.parent = document.gameObject.transform;
                    go.SetActive(false);
                    // If the image is in template mode then do not save the particle system.
                    if (systemIsTemporary)
                    {
                        go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
                        go.name = ParticleSystemForImage.TempParticleSystemPrefix + " " + go.name;
                    }

                    go.AddComponent<ParticleSystem>();
                    system = go.AddComponent<ParticleSystemForImage>();
                    system.ResetTransform();
                    system.Guid = image.Guid;
                    system.AddImage(image);
                    system.InitializeAfterCreation(image);

                    if (systemIsTemporary)
                    {
                        system.Play();
                    }

                    go.SetActive(true); // Will trigger OnEable()

#if UNITY_EDITOR
                    if (!systemIsTemporary)
                    {
                        UnityEditor.EditorUtility.SetDirty(go);
                        Selection.objects = new GameObject[] { go };
                    }
#endif

                    if (systemIsTemporary)
                        Logger.LogMessage("Temporary template particle system created (it will not persist).");
                    else
                        Logger.Log("Particle System created.");
                }
            }
            catch (System.Exception e)
            {
                if(go != null)
                    Utils.SmartDestroy(go);

                throw e;
            }

#if UNITY_EDITOR
            // Move component up (fail silently)
            if (system != null)
            {
                EditorApplication.delayCall += () => UnityEditorInternal.ComponentUtility.MoveComponentUp(system);
            }
#endif

            return system;
        }

        public void UnregisterElement(ParticleImage image)
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif

            if (_images.Contains(image))
            {
                _images.Remove(image);
            }

            Defrag();
        }

        public void LinkParticleImagesToSystem(string guid, ParticleSystemForImage system)
        {
            if (string.IsNullOrEmpty(guid))
                return;

            foreach (var image in _images)
            {
                if (image.Guid == guid)
                {
                    system.AddImage(image);
                }
            }
        }

        public void RegisterParticleSystem(ParticleSystemForImage system)
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif

            Defrag();

            if (!_particleSystems.Contains(system)) 
            {
                _particleSystems.Add(system);
            }

            LinkParticleImagesToSystem(system.Guid, system);
            system.RefreshSystemOnImages();
        }

        public void UnregisterParticleSystem(ParticleSystemForImage system)
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif

            if (_particleSystems.Contains(system))
            {
                _particleSystems.Remove(system);
            }

            system.ClearSystemOnImages();

            Defrag();
        }

        public ParticleSystemForImage FindParticleSystem(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            foreach (var system in _particleSystems)
            {
                if (system.Guid == guid)
                {
                    return system;
                }
            }

            // Try searching in the scenes and register if found.
            var systems = Utils.FindObjectsOfTypeFast<ParticleSystemForImage>(includeInactive: true);
            foreach (var system in systems)
            {
                RegisterParticleSystem(system);
            }

            // Repeat the search in the registered systems.
            foreach (var system in _particleSystems)
            {
                if (system.Guid == guid)
                {
                    return system;
                }
            }

            return null;
        }

        void Start()
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif

            // Register this classes Update() method in the Unity update loop for runtime and editor.
            ParticleManagerUpdater.Init(Update);

#if UNITY_EDITOR
            UIBuilderWindowWrapper.Instance.OnSelectionChanged += onSelectionChanged;
#endif
        }

#if UNITY_EDITOR
        void onSelectionChanged(List<VisualElement> elements)
        {
            foreach (var ve in elements)
            {
                var image = ve as ParticleImage;
                if (image != null)
                {
                    image.InitializeIfNecessary(immediate: false);
                    if (image.ParticleSystemForImage != null && image.ParticleSystemForImage.gameObject != null)
                    {
                        Selection.objects = new GameObject[] { image.ParticleSystemForImage.gameObject };
                        break;
                    }
                }
            }
        }
#endif

        public void Update()
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer)
                return;
#endif
            // Simulate the particle system in editor
#if UNITY_EDITOR
            updateInEditor();
#endif

            foreach (var image in _images)
            {
                if (image.IsActive)
                {
                    image.ParticleSystemForImage?.ResumeDueToActivity(image.RestartOnShow);
                    image.MarkDirtyRepaint();
                }
                else
                {
                    // One particle system may be shared among multiple images.
                    // Thus we only pause it if ALL the images are inactive.
                    string guid = image.Guid;
                    bool isAnyActive = false;
                    foreach (var img in _images)
                    {
                        if (img.Guid == guid && img.IsActive)
                        {
                            isAnyActive = true;
                            break;
                        }
                    }
                    if (!isAnyActive)
                    {
                        image.ParticleSystemForImage?.PauseDueToInactivity();
                    }
                }
            }
        }

#if UNITY_EDITOR

        protected List<string> _editorUpdatedGuids = new List<string>();

        protected void updateInEditor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            bool scheduledDefrag = false;

            // Simulate the particle system outside of play mode or if the particle system is not selected.
            _editorUpdatedGuids.Clear();
            foreach (var image in _images)
            {
                if (image == null || image.ParticleSystem == null)
                {
                    scheduledDefrag = true;
                    continue;
                }

                // Play particles only if they are not already playing due to being selected.
                bool isParticleSystemSelected = Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<ParticleSystemForImage>() == image.ParticleSystemForImage;
                bool isParticleSystemChildSelected = Selection.activeGameObject != null && image.ParticleSystemForImage != null && Selection.activeGameObject.transform.IsChildOf(image.ParticleSystemForImage.transform);
                bool isParticleImageSelected = false;
                var selection = UIBuilderWindowWrapper.Instance.Selection;
                foreach (var selectedElement in selection)
                {
                    var img = selectedElement as ParticleImage;
                    if (img != null && img.Guid == image.Guid)
                    {
                        isParticleImageSelected = true;
                    }
                }

                // If the particle system is selected then keep our own EditorPlayParticles flag in sync with the particle system state.
                // This allows the user to use either the custom inspector buttons or the in-scene gizmo ui.
                if (isParticleSystemSelected)
                {
                    image.ParticleSystemForImage.EditorPlayParticles = image.ParticleSystem.isPlaying;
                }

                if (   image.ParticleSystem != null
                    && image.ParticleSystemForImage.EditorPlayParticles
                    && !isParticleSystemSelected
                    && (isParticleSystemChildSelected || isParticleImageSelected)
                    && !_editorUpdatedGuids.Contains(image.Guid))
                {
                    _editorUpdatedGuids.Add(image.Guid);
                    image.ParticleSystem.Play();
                    image.ParticleSystem.Simulate(ParticleManagerUpdater.DeltaTime, withChildren: true, restart: false); // newTime, withChildren: true, restart: true, fixedTimeStep: true);
                    image.ParticleSystem.Pause(true);
                }
            }

            if (scheduledDefrag)
                Defrag();
        }
#endif
    }
}