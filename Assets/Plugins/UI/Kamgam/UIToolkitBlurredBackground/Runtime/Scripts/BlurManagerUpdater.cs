using System;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UIToolkitBlurredBackground
{
    /// <summary>
    /// This class ensure update is called in play and in edit mode.
    /// </summary>
    [HelpURL("https://kamgam.com/unity/UIToolkitBlurredBackgroundManual.pdf")]
    public partial class BlurManagerUpdater : MonoBehaviour
    {
        static BlurManagerUpdater _instance;
        static BlurManagerUpdater instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Utils.FindRootObjectByType<BlurManagerUpdater>(includeInactive: true);
                    if (_instance == null)
                    {
                        var go = new GameObject("UIToolkit BlurredBackground Updater");
                        _instance = go.AddComponent<BlurManagerUpdater>();
                        _instance.hideFlags = HideFlags.DontSave;
                        Utils.SmartDontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

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

