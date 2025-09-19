using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
    // Copy this to your asset file.
    // [CreateAssetMenu(fileName = "UITK Glow Animation", menuName = "UI Toolkit/Glow/Animation", order = 403)]

    public abstract class GlowAnimationAsset : ScriptableObject
    {
        public static string DefaultName;
        public abstract string GetDefaultName();

        [SerializeField]
        protected string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;

                _name = value;

                if(_animation != null)
                {
                    _animation.Name = _name;
                }
            }
        }

        [SerializeField]
        protected int _frameRate = -1;
        public int FrameRate
        {
            get => _frameRate;
            set
            {
                if (_frameRate == value)
                    return;

                _frameRate = value;

                if (_animation != null)
                {
                    _animation.FrameRate = _frameRate;
                }
            }
        }

        protected IGlowAnimation _animation;

        protected virtual T getAnimation<T>(out bool createdNewCopy) where T : IGlowAnimation, new()
        {
            if (_animation == null)
            {
                var animation = createAnimation<T>();
                _animation = animation;

                createdNewCopy = true;
            }
            else
            {
                createdNewCopy = false;
            }

            return (T) _animation;
        }

        protected virtual T getAnimation<T>() where T : IGlowAnimation, new()
        {
            return getAnimation<T>(out bool _);
        }

        /// <summary>
        /// Returns a cached COPY of the original animation object.<br />
        /// This is done to avoid contaminating the original asset (especially in the editor).
        /// </summary>
        /// <returns></returns>
        public abstract IGlowAnimation GetAnimation();

        protected T createAnimation<T>() where T : IGlowAnimation, new()
        {
            var animation = new T();
            animation.Name = string.IsNullOrEmpty(Name) ? typeof(T).Name : Name;
            animation.FrameRate = FrameRate;

            _animation = animation;

            return animation;
        }

        protected void OnEnable()
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = GetDefaultName();
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

#if UNITY_EDITOR
        public virtual void onValuesChangedInInspector()
        {
            if (_animation == null)
                return;

            _animation.Name = Name;
            _animation.FrameRate = FrameRate;

            _animation.TriggerOnValueChanged();
        }

        protected virtual void OnValidate()
        {
            onValuesChangedInInspector();

            if (string.IsNullOrEmpty(Name))
            {
                Name = GetDefaultName();
            }
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlowAnimationAsset), editorForChildClasses: true)]
    public class GlowAnimationAssetEditor : UnityEditor.Editor
    {
        GlowAnimationAsset obj;

        public void OnEnable()
        {
            obj = target as GlowAnimationAsset;
        }

        public override void OnInspectorGUI()
        {
            if (string.IsNullOrEmpty(obj.Name))
            {
                name = obj.GetDefaultName();
                EditorUtility.SetDirty(obj);
            }

            if (GUILayout.Button("<- Back to Scene"))
            {
                var glowDocument = GlowDocument.FindFirst(requireConfigRoot: false);
                if (glowDocument != null)
                {
                    UnityEditor.Selection.objects = new GameObject[] { glowDocument.gameObject };
                    UnityEditor.EditorGUIUtility.PingObject(glowDocument.gameObject);
                }
            }

            // Check if asset is in global config list
            var configRoot = GlowConfigRoot.FindAssetInEditor();
            if(configRoot != null)
            {
                if (!configRoot.AnimationAssets.Contains(obj))
                {
                    configRoot.AnimationAssets.Add(obj);
                    EditorUtility.SetDirty(configRoot);
                    Logger.LogMessage("Added " + obj.name + " to " + obj.name);
                }
            }

            base.OnInspectorGUI();
        }
    }
#endif
}