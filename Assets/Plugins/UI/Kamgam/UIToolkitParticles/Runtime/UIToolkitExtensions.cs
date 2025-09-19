using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public static class UIToolkitExtensions
    {
        public static VisualElement QueryType(this UIDocument document, UIElementType type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            var sysType = UIElementTypeUtils.GetElementType(type);
            if (sysType != null)
            {
                return QueryType(document, sysType, name, className, predicate);
            }

            return null;
        }

        public static VisualElement QueryType(this VisualElement element, UIElementType type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            var sysType = UIElementTypeUtils.GetElementType(type);
            if (sysType != null)
            {
                return QueryType(element, sysType, name, className, predicate);
            }

            return null;
        }

        public static VisualElement QueryType(this UIDocument document, System.Type type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            if (document == null || document.rootVisualElement == null)
            {
                return null;
            }

            return QueryType(document.rootVisualElement, type, name, className, predicate);
        }

        public static VisualElement QueryType(this VisualElement element, System.Type type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            if (element == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(name))
                name = null;

            if (string.IsNullOrEmpty(className))
                className = null;

            var iter = element.Query<VisualElement>(name, className).Build();
            foreach (var ele in iter)
            {
                if (predicate != null && !predicate.Invoke(ele))
                    continue;

                var eleType = ele.GetType();
                if (eleType == type || eleType.IsSubclassOf(type))
                {
                    return ele;
                }
            }

            return null;
        }

        public static List<VisualElement> QueryTypes(this UIDocument document, UIElementType type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null, List<VisualElement> results = null)
        {
            prepareResults(ref results);

            var sysType = UIElementTypeUtils.GetElementType(type);
            if (sysType != null)
            {
                return QueryTypes(document, sysType, name, className, predicate, results);
            }

            return results;
        }

        public static List<VisualElement> QueryTypes(this UIDocument document, System.Type type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null, List<VisualElement> results = null)
        {
            prepareResults(ref results);

            if (document == null || document.rootVisualElement == null)
            {
                return results;
            }
            
            document.rootVisualElement.QueryTypes(type, name, className, predicate, results);

            return results;
        }

        public static List<VisualElement> QueryTypes(this VisualElement element, UIElementType type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null, List<VisualElement> results = null)
        {
            prepareResults(ref results);

            var sysType = UIElementTypeUtils.GetElementType(type);
            if (sysType != null)
            {
                return QueryTypes(element, sysType, name, className, predicate, results);
            }

            return results;
        }

        public static List<VisualElement> QueryTypes(this VisualElement element, System.Type type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null, List<VisualElement> results = null)
        {
            prepareResults(ref results);

            if (element == null)
            {
                return results;
            }

            if (string.IsNullOrEmpty(name))
                name = null;

            if (string.IsNullOrEmpty(className))
                className = null;

            var iter = element.Query<VisualElement>(name, className).Build();
            foreach (var ele in iter)
            {
                if (predicate != null && !predicate.Invoke(ele))
                    continue;

                var eleType = ele.GetType();
                if (eleType == type || eleType.IsSubclassOf(type))
                {
                    results.Add(ele);
                }
            }

            return results;
        }

        static void prepareResults(ref List<VisualElement> results)
        {
            if (results == null)
                results = new List<VisualElement>();

            results.Clear();
        }

        /// <summary>
        /// Tries to find the root element just above the normal UXML content elements.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static VisualElement GetDocumentRoot(this VisualElement element)
        {
            if (element == null)
                return null;

            if (element.panel == null)
            {
                // Element is detached. Will return null
                return null;
            }

            if (element.IsPartOfUIDocumentTree())
            {
                return element.panel.visualTree.Q(name: "UIDocument-container"); // Is there always an element with name "UIDocument-container" at the root in UIDocument?
            }
#if UNITY_EDITOR
            else if (element.IsPartOfBuilderTree())
            {
                // In the UIBuilder the visualTree is NOT the root of the document
                // but the root of the UIBuilder UI itself (the document is just a child).
                // To find the actual document root we have to use a query.
                return element.panel.visualTree.Q(name: "document"); // Is there always an element with name "document" at the root in UIBuilder?
            }
#endif
            else
            {
                return element.panel.visualTree;
            }
        }

        /// <summary>
        /// TODO: investigate on how to make this more robust.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>TRUE if the element is part of the UI Builder tree.</returns>
        public static bool IsPartOfBuilderTree(this VisualElement element)
        {
            // Search upwards
            var ele = element;
            while (ele != null)
            {
                var type = ele.GetType().Name;
                if (type == "BuilderCanvas" || type == "BuilderPane" || type == "BuilderViewport")
                {
                    return true;
                }
                ele = ele.parent;
            }

            // Search downwards
            ele = element;
            while (ele != null)
            {
                var type = ele.GetType().Name;
                if (type == "BuilderCanvas" || type == "BuilderPane" || type == "BuilderViewport")
                {
                    return true;
                }

                if (ele.childCount > 0)
                {
                    ele = ele[0];
                }
                else
                {
                    ele = null;
                }
            }

            return false;
        }

        /// <summary>
        /// TODO: investigate on how to make this more robust.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>TRUE if the element is part of the UI Document tree.</returns>
        public static bool IsPartOfUIDocumentTree(this VisualElement element)
        {
            // Search upwards
            var ele = element;
            while (ele != null)
            {
                var type = ele.GetType().Name;
                var name = ele.name;
                if (type == "TemplateContainer" && (name == "UIDocument-container" || type == "UIDocument"))
                {
                    return true;
                }
                ele = ele.parent;
            }

            // Search downwards
            ele = element;
            while (ele != null)
            {
                var type = ele.GetType().Name;
                var name = ele.name;
                if (type == "TemplateContainer" && (name == "UIDocument-container" || type == "UIDocument"))
                {
                    return true;
                }

                if (ele.childCount > 0)
                {
                    ele = ele[0];
                }
                else
                {
                    ele = null;
                }
            }

            return false;
        }
    }
}