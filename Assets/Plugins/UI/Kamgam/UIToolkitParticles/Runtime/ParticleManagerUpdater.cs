using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UIToolkitParticles
{
    /// <summary>
    /// This class ensure update is called in play and in edit mode.
    /// </summary>
    [HelpURL("https://kamgam.com/unity/UIToolkitBlurredBackgroundManual.pdf")]
    public class ParticleManagerUpdater : MonoBehaviour
    {
        static ParticleManagerUpdater _instance;
        static ParticleManagerUpdater instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Utils.FindRootObjectByType<ParticleManagerUpdater>(includeInactive: true);
                    if (_instance == null)
                    {
                        var go = new GameObject("UIToolkit Particles Updater");
                        _instance = go.AddComponent<ParticleManagerUpdater>();
                        _instance.hideFlags = HideFlags.DontSave;
                        Utils.SmartDontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

#if UNITY_EDITOR
        static double _lastUpdateTime = -1;
        public static float DeltaTime = 0f;
#endif

        public Action OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        public static void Init(Action updateFunc)
        {
#if !UNITY_EDITOR
            // Runtime
            instance.OnUpdate += updateFunc;
#else
            // Editor
            _action = updateFunc;
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                instance.OnUpdate += updateFunc;
            }
            else
            {
                EditorApplication.update -= updateInEditor;
                EditorApplication.update += updateInEditor;
            }

            EditorApplication.playModeStateChanged -= onPlayModeChanged;
            EditorApplication.playModeStateChanged += onPlayModeChanged;
#endif
        }

#if UNITY_EDITOR
        static Action _action;
        static void updateInEditor()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) // Just to be extra sure.
                _action.Invoke();

            // Calculate the delta time in the editor
            DeltaTime = (float)(EditorApplication.timeSinceStartup - _lastUpdateTime);
            if (_lastUpdateTime < 0)
            {
                DeltaTime = 0.02f; // Assume 60 fps for first frame.
                _lastUpdateTime = EditorApplication.timeSinceStartup - DeltaTime;
            }
            _lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        private static void onPlayModeChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.update -= updateInEditor;
                EditorApplication.update += updateInEditor;
            }
        }
#endif
    }
}

