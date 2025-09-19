using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// The provider is just a wrapper around a glow config object so we can use it in UI Toolkit custom elements and on GlowDocuments.
    /// </summary>
    // We modify the execution order to ensure Awake() is called before UI Toolkit
    // instantiates the UI Elements (which it does via the event system which is
    // set to execuition order -1000 by default).
    [DefaultExecutionOrder(-1001)]
    public class GlowConfigRootProvider : MonoBehaviour
    {
        static GlowConfigRootProvider _provider;

        public static GlowConfigRoot GetConfigRoot()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
#endif
                if (_provider != null && _provider.ConfigRoot != null)
                {
                    return _provider.ConfigRoot;
                }
                return null;
#if UNITY_EDITOR
            }
            else
            {
#if UNITY_2023_1_OR_NEWER
                var provider = GameObject.FindFirstObjectByType<GlowConfigRootProvider>(FindObjectsInactive.Include);
#else
                var provider = GameObject.FindObjectOfType<GlowConfigRootProvider>(includeInactive: true);
#endif
                if (provider != null && provider.ConfigRoot != null)
                {
                    return provider.ConfigRoot;
                }
                else
                {
                    return GlowConfigRoot.FindAssetInEditor();
                }
            }
#endif
        }

        public GlowConfigRoot ConfigRoot;

        public void Awake()
        {
            _provider = this;
        }

        public void OnEnable()
        {
            _provider = this;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ConfigRoot == null)
            {
                var root = GlowConfigRoot.FindConfigRoot();
                if (root != null)
                {
                    ConfigRoot = root;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.EditorUtility.SetDirty(this.gameObject);
                }
            }
        }
#endif
    }
}