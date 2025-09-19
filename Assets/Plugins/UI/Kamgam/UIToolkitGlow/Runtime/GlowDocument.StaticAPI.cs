using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public partial class GlowDocument : MonoBehaviour
    {
        static readonly IList<GlowDocument> _glowDocuments = new List<GlowDocument>();

        /// <summary>
        /// Returns the list of enabled glow documents at runtime.<br />
        /// In the editor (if not playing) it will search for GlowDocument components in the active scene.
        /// </summary>
        /// <returns></returns>
        public static IList<GlowDocument> GetGlowDocuments()
        {
            IList<GlowDocument> documents;
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
#endif
                documents = _glowDocuments;
#if UNITY_EDITOR
            }
            else
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
#if UNITY_2023_1_OR_NEWER
                documents = GameObject.FindObjectsByType<GlowDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                documents = GameObject.FindObjectsOfType<GlowDocument>(includeInactive: true);
#endif
            }
#endif

            return documents;
        }

        public static GlowDocument FindFirst(bool requireConfigRoot = true)
        {
            return Find(null, requireConfigRoot);
        }

        /// <summary>
        /// Returns the first glow document that matches the criteria.
        /// </summary>
        /// <param name="configRoot">The config root (you can pass NULL here)</param>
        /// <param name="requireConfigRoot">If "configRoot" is set to null then this will specify whether or not the doc needs to have a config root.</param>
        /// <returns></returns>
        public static GlowDocument Find(GlowConfigRoot configRoot, bool requireConfigRoot = true)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
#endif
                foreach (var doc in _glowDocuments)
                {
                    if (doc == null)
                        continue;

                    // If no config root has been specified then return the first doc matching "requireConfigRoot".
                    if (configRoot == null && (!requireConfigRoot || doc.ConfigRoot != null))
                        return doc;

                    if (doc.ConfigRoot == configRoot)
                        return doc;
                }
#if UNITY_EDITOR
            }
#endif

#if UNITY_2023_1_OR_NEWER
            var docs = GameObject.FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var docs = GameObject.FindObjectsOfType<UIDocument>(includeInactive: true);
#endif
            foreach (var doc in docs)
            {
                if (doc.gameObject.TryGetComponent<GlowDocument>(out var comp))
                {
                    if (comp.Panel.ConfigRoot == configRoot)
                    {
                        return comp;
                    }
                }
            }

            // Loosen search and select the first UI Document with a doc on it.
            foreach (var doc in docs)
            {
                if (doc.gameObject.TryGetComponent<GlowDocument>(out var comp))
                {
                    return comp;
                }
            }

            return null;
        }

        public static UIDocument FindFirstUIDocument()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
#endif
                foreach (var doc in _glowDocuments)
                {
                    if (doc == null && doc.Document != null)
                        continue;

                    return doc.Document;
                }
#if UNITY_EDITOR
            }
#endif

#if UNITY_2023_1_OR_NEWER
            var uiDoc = GameObject.FindFirstObjectByType<UIDocument>(FindObjectsInactive.Include);
#else
            var uiDoc = GameObject.FindObjectOfType<UIDocument>(includeInactive: true);
#endif

            return uiDoc;
        }
    }
}