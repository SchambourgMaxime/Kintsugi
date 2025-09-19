using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Kamgam.UIToolkitParticles
{
    public static class Utils
    {
        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
#endif
            {
                GameObject.Destroy(obj);
            }
        }

        public static void SmartDontDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                GameObject.DontDestroyOnLoad(go);
            }
#else
            GameObject.DontDestroyOnLoad(go);
#endif
        }

        private static List<GameObject> _tmpSceneObjects = new List<GameObject>();

        public static List<T> FindRootObjectsByType<T>(bool includeInactive) where T : Component
        {
            var results = new List<T>();
            FindRootObjectsByType(includeInactive, results);
            return results;
        }

        /// <summary>
        /// A simple replacement for GameObject.FindObjectsOfType<T>. It checks the ROOT objects in ALL opened or loaded scenes.
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <param name="results">A list that will be cleared and then filled with the results.</param>
        /// <returns></returns>
        public static void FindRootObjectsByType<T>(bool includeInactive, IList<T> results) where T : Component
        {
            if (results == null)
            {
                results = new List<T>();
            }
            else
            {
                results.Clear();
            }

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    continue;

                scene.GetRootGameObjects(_tmpSceneObjects);

                foreach (var obj in _tmpSceneObjects)
                {
                    var comp = obj.GetComponent<T>();
                    if (comp == null)
                        continue;

                    if (!includeInactive && !comp.gameObject.activeInHierarchy)
                        continue;

                    results.Add(comp);
                }
            }
        }

        /// <summary>
        /// A simple replacement for GameObject.FindObjectsOfType<T>. It checks the ROOT objects in ALL opened or loaded scenes.
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T FindRootObjectByType<T>(bool includeInactive) where T : Component
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    continue;

                if (!scene.isLoaded)
                    continue;

                scene.GetRootGameObjects(_tmpSceneObjects);

                foreach (var obj in _tmpSceneObjects)
                {
                    var comp = obj.GetComponent<T>();
                    if (comp == null)
                        continue;

                    if (!includeInactive && !comp.gameObject.activeInHierarchy)
                        continue;

                    return comp;
                }
            }

            return default;
        }

        public static T[] FindObjectsOfTypeFast<T>(bool includeInactive = false) where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return GameObject.FindObjectsByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return GameObject.FindObjectsOfType<T>(includeInactive);
#endif
        }

        public static T FindObjectOfTypeFast<T>(bool includeInactive = false) where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return GameObject.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#else
            return GameObject.FindObjectOfType<T>(includeInactive);
#endif
        }

        public static string GetVisualElementPath(VisualElement ve)
        {
            if (ve == null)
                return "";

            string path = "";
            var parent = ve.parent;
            while (parent != null)
            {
                path += " / " + parent.name + "(" + parent.GetType().FullName + ")";
                parent = parent.parent;
            }

            return path;
        }

        /// <summary>
        /// Anything that has an EditorPanel as parent or is a child of a BuilderCanvas is
        /// considerd to be part of the builder.
        /// </summary>
        /// <param name="ve"></param>
        /// <returns></returns>
        public static bool IsPartOfUIBuilderCanvas(VisualElement ve)
        {
#if UNITY_EDITOR
            if (ve == null)
                return false;

            // Shortcut if not an editor panel.
            if (ve.panel != null && ve.panel.contextType != ContextType.Editor) 
            {
                return false;
            }

            bool isInBuilderCanvas = Utils.isInBuilderCanvas(ve);
            return isInBuilderCanvas;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns true if the element is part of an EditorPanel and if it is NOT a
        /// child of "UI.Builder.BuilderCanvas".
        /// </summary>
        /// <param name="ve"></param>
        /// <returns></returns>
        public static bool IsPartOfUIBuilderButNotInTheCanvas(VisualElement ve)
        {
#if UNITY_EDITOR
            if (ve == null)
                return false;

            // Shortcut if not an editor panel.
            if (ve.panel != null && ve.panel.contextType != ContextType.Editor)
            {
                return false;
            }

            bool isInBuilderCanvas = Utils.isInBuilderCanvas(ve);
            return !isInBuilderCanvas;
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        private static bool isInBuilderCanvas(VisualElement element)
        {
            var parent = element.parent;
            while (parent != null)
            {
                if (parent.GetType().FullName.Contains("UI.Builder.BuilderCanvas"))
                {
                    return true;
                }
                parent = parent.parent;
            }

            return false;
        }
#endif
    }
}