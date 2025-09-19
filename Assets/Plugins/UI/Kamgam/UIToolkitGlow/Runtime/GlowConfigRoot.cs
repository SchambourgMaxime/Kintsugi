using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
#if KAMGAM_VISUAL_SCRIPTING
    // Why? See: https://forum.unity.com/threads/unable-to-provide-a-default-for-getvalue-on-object-valueinput.1140022/#post-9138727
    [Unity.VisualScripting.Inspectable]
#endif
    [CreateAssetMenu(fileName = "UITK Glow Config Root", menuName = "UI Toolkit/Glow/Config Root", order = 402)]
    public class GlowConfigRoot : ScriptableObject
    {
        [System.NonSerialized]
        protected List<GlowConfig> _runtimeCopies;

        [SerializeField]
        protected List<GlowConfig> _configs;
        public List<GlowConfig> Configs
        {
            get
            {
                if (UseRuntimeCopy)
                {
                    if (_runtimeCopies == null)
                    {
                        _runtimeCopies = new List<GlowConfig>();
                        foreach (var config in _configs)
                        {
                            _runtimeCopies.Add(config.Copy());
                        }
                    }

                    return _runtimeCopies;
                }
                else
                {
                    return _configs;
                }
            }

            private set {}
        }

        protected List<IGlowAnimation> _animations = null;
        public List<IGlowAnimation> Animations
        {
            get
            {
                if (_animations == null)
                {
                    _animations = new List<IGlowAnimation>();

                    // Extract animations from asset upon deserialization.
                    foreach (var asset in AnimationAssets)
                    {
                        if (asset == null)
                            continue;

                        var anim = asset.GetAnimation();
                        if (!Animations.Contains(anim))
                        {
                            Animations.Add(anim);
                        }
                    }
                }

                return _animations;
            }
        }

        public List<GlowAnimationAsset> AnimationAssets = new List<GlowAnimationAsset>();

        public GlowConfig GetConfigByClassName(string className)
        {
            foreach (var config in Configs)
            {
                if (string.Compare(config.ClassName, className) == 0)
                {
                    return config;
                }
            }

            return null;
        }

        public IGlowAnimation GetAnimationByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            foreach (var animation in Animations)
            {
                if (string.IsNullOrEmpty(animation.Name))
                    continue;

                if (string.Compare(animation.Name, name) == 0)
                {
                    return animation;
                }
            }

            return null;
        }

        public T GetAnimationByName<T>(string name) where T : class, IGlowAnimation
        {
            var animation = GetAnimationByName(name);
            
            if(animation != null)
                return (T) animation;

            return null;
        }

        [Tooltip("Useful if you do not want to modify the asset at runtime while being in the Editor.")]
        public bool UseRuntimeCopy = false;

        public static GlowConfigRoot FindConfigRoot()
        {
            return GlowConfigRootProvider.GetConfigRoot();
        }

#if UNITY_EDITOR
        public static GlowConfigRoot FindAssetInEditor()
        {
            // Otherwise search in asset database and return first found.
            var guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(GlowConfigRoot).Name);
            if (guids.Length > 0)
            {
                string path;

                // First try locating the default glow config root by name.
                foreach (var guid in guids)
                {
                    path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                    if (path.Contains("UITK Glow Config Root"))
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<GlowConfigRoot>(path);
                }

                // If not found the simply use the first.
                path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GlowConfigRoot>(path);
            }

            return null;
        }

        public void OnValidate()
        {
            foreach (var config in Configs)
            {
                config.TriggerValueChanged();
            }

            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var doc = GlowDocument.Find(this);
                if(doc != null)
                {
                    doc.UpdateGlowOnChildren();
                }
            }
            else
            {
                UIEditorPanelObserver.RefreshGlowManipulators();
            }
        }
#endif
    }



#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlowConfigRoot))]
    public class GlowConfigRootEditor : UnityEditor.Editor
    {
        GlowConfigRoot obj;

        public void OnEnable()
        {
            obj = target as GlowConfigRoot;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("<- Back to UI Document"))
            {
                var provider = GlowDocument.Find(obj);
                if (provider != null)
                {
                    UnityEditor.Selection.objects = new GameObject[] { provider.gameObject };
                    UnityEditor.EditorGUIUtility.PingObject(provider.gameObject);
                }
            }

            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Refresh Preview", "Sometimes the preview in UI Builder or the Game View does not update automatically. Use this to force and update in the UI Builder and the Game View.")))
            {
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    var doc = GlowDocument.Find(obj);
                    if (doc != null)
                    {
                        doc.UpdateGlowOnChildren();
                    }
                }
                else
                {
                    UIEditorPanelObserver.RefreshGlowManipulators();
                }
            }
        }
    }
#endif
}