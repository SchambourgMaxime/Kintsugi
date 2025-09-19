// EDITOR ONLY
// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if UNITY_EDITOR && (KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;
using UnityEditor.Callbacks;

namespace Kamgam.UIToolkitParticles
{
    public class UIBuilderWindowWrapper
    {
        public static double CheckForChangesIntervalInSec = 0.05; // 0.05 = 20 fps

        public event System.Action<List<VisualElement>> OnSelectionChanged;
        public event System.Action OnBuilderWindowOpened;
        public event System.Action OnBuilderWindowClosed;

        /// <summary>
        /// Is triggered if the 'name' or 'viewDataKey' of the last element in the selection is changed.
        /// </summary>
        public event System.Action<VisualElement> OnAttributeChanged;

        static UIBuilderWindowWrapper _instance;
        public static UIBuilderWindowWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIBuilderWindowWrapper();
                    EditorApplication.update += _instance.onEditorUpdate;
                }
                return _instance;
            }
        }

        protected Type _builderWindowType;
        public Type BuilderWindowType
        {
            get
            {
                if (_builderWindowType == null)
                {
                    _builderWindowType = getType("Unity.UI.Builder.Builder");
                }
                return _builderWindowType;
            }
        }

        protected Type _builderInspectorType;
        public Type BuilderInspectorType
        {
            get
            {
                if (_builderInspectorType == null)
                {
                    // BuilderInspector > BuilderPaneContent > VisualElement
                    // https://github.com/Unity-Technologies/UnityCsReference/blob/ddda763e79614c13cb63ec8b9dac375fd3be2b24/Modules/UIBuilder/Editor/Builder/Inspector/BuilderInspector.cs
                    // https://github.com/Unity-Technologies/UnityCsReference/blob/ddda763e79614c13cb63ec8b9dac375fd3be2b24/Modules/UIBuilder/Editor/Builder/BuilderPaneContent.cs
                    _builderInspectorType = getType("Unity.UI.Builder.BuilderInspector");
                }
                return _builderInspectorType;
            }
        }

        protected Type _builderInspectorInheritedStylesType;
        public Type BuilderInspectorInheritedStylesType 
        {
            get
            {
                if (_builderInspectorInheritedStylesType == null)
                {
                    _builderInspectorInheritedStylesType = getType("Unity.UI.Builder.BuilderInspectorInheritedStyles");
                }
                return _builderInspectorInheritedStylesType;
            }
        }

        protected object _inheritedStylesInspector;
        public object InheritedStylesInspector
        {
            get
            {
                if (_inheritedStylesInspector == null)
                {
                    if (BuilderWindow != null && BuilderInspectorInheritedStylesType != null)
                    {
                        var field = getFirstFieldMatchingTypeName(BuilderInspectorType, BuilderInspectorInheritedStylesType.FullName);
                        _inheritedStylesInspector = field.GetValue(BuilderInspector);
                    }
                }
                return _inheritedStylesInspector;
            }
        }

        public void AddClassToList(string className)
        {
            if (Instance.Selection.Count == 0 || Instance.Selection[0] is not ParticleImage)
                return;

            var methods = BuilderInspectorInheritedStylesType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var paramInfo = method.GetParameters();
                if (method.Name == "AddStyleClass" && !method.IsGenericMethod && paramInfo.Length == 1 && paramInfo[0].ParameterType == typeof(string))
                {
                    method.Invoke(InheritedStylesInspector, new object[] { className });
                    break;
                }
            }
        }

        protected Type getType(string fullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var inspectorType = assembly.GetType(fullName);
                if (inspectorType != null)
                {
                    return inspectorType;
                }
            }

            return null;
        }
    
        // Actually a BuilderInspector > BuilderPaneContent > VisualElement but VisualElement is the first accessible type so we use it here.
        protected VisualElement _builderInspector;

        /// <summary>
        /// Returns null if the inspector type was not found or if the ui builder window is not opened.
        /// </summary>
        public VisualElement BuilderInspector
        {
            get
            {
                if (_builderInspector == null)
                {
                    if (BuilderWindow != null)
                    {
                        if (BuilderInspectorType != null)
                        {
                            var field = getFirstFieldMatchingTypeName(BuilderWindowType, BuilderInspectorType.FullName);
                            _builderInspector = field.GetValue(BuilderWindow) as VisualElement;
                        }
                    }
                }
                return _builderInspector;
            }
        }

        protected FieldInfo getFirstFieldMatchingTypeName(System.Type type, string fullName)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fields != null && fields.Length > 0)
            {
                // Use first inspector that is found in the fields
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].FieldType.FullName == fullName)
                    {
                        return fields[i];
                    }
                }
            }

            return null;
        }

        protected EditorWindow _builderWindow;
        /// <summary>
        /// Returns null if the window type was not found or if the window is not opened.
        /// </summary>
        public EditorWindow BuilderWindow
        {
            get
            {
                if (_builderWindow == null)
                {
                    var type = BuilderWindowType;
                    if (type != null)
                    {
                        var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                        foreach (var window in windows)
                        {
                            if (window.GetType() == BuilderWindowType)
                            {
                                _builderWindow = window;
                                break;
                            }
                        }
                    }
                }
                return _builderWindow;
            }
        }


        protected Type _builderSelectionType;
        protected PropertyInfo _builderSelectionProperty;
        protected PropertyInfo _selectionSelectionProperty;
        protected PropertyInfo _selectionDocumentRootElement;
        /// <summary>
        /// public void Select(IBuilderSelectionNotifier source, VisualElement ve)
        /// </summary>
        protected MethodInfo _selectionSelectionMethod;

        protected List<VisualElement> _previousSelection = new List<VisualElement>();
        protected List<VisualElement> _selection = new List<VisualElement>();
        public List<VisualElement> Selection
        {
            get
            {
                if (_selection == null)
                {
                    _selection = new List<VisualElement>();
                }
                _selection.Clear();

                var window = BuilderWindow;
                if (window != null)
                {
                    cacheTypesAndProperties();

                    if (BuilderWindow != null && _builderSelectionType != null && _builderSelectionProperty != null && _selectionSelectionProperty != null)
                    {

                        // Is of Type BuilderSelection
                        var builderSelection = _builderSelectionProperty.GetValue(BuilderWindow);
                        if (builderSelection != null)
                        {
                            // is of Type List<VisualElement>
                            var selection = _selectionSelectionProperty.GetValue(builderSelection) as IEnumerable<VisualElement>;
                            if (selection != null)
                            {
                                _selection.AddRange(selection);
                            }
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Builder window, types or properties not found.");
                    }
                }

                return _selection;
            }
        }

        /// <summary>
        /// The document root inside the UI Builder window.<br />
        /// NOTICE: This is not the same view hierarchy as in the UIDocument.
        /// Those two are different instances of the same uxml source.
        /// </summary>
        public VisualElement DocumentRootElement
        {
            get
            {
                var window = BuilderWindow;
                if (window != null)
                {
                    cacheTypesAndProperties();

                    if (BuilderWindow != null && _builderSelectionType != null && _builderSelectionProperty != null && _selectionSelectionProperty != null)
                    {
                        // Is of Type BuilderSelection
                        var builderSelection = _builderSelectionProperty.GetValue(BuilderWindow);
                        if (builderSelection != null)
                        {
                            // is of Type VisualElement
                            var root = _selectionDocumentRootElement.GetValue(builderSelection) as VisualElement;
                            return root;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Builder window, types or properties not found.");
                    }
                }

                return null;
            }
        }

        protected BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        private void cacheTypesAndProperties()
        {
            var type = BuilderWindowType;
            if (type != null && _builderSelectionProperty == null)
            {
                _builderSelectionProperty = type.GetProperty("selection", _flags);
                if (_builderSelectionProperty != null && _builderSelectionType == null)
                {
                    _builderSelectionType = _builderSelectionProperty.PropertyType;
                    if (_builderSelectionType != null && _selectionSelectionProperty == null)
                    {
                        // Yes, the same name again. Type is IEnumerable<VisualElement>, mostly likely a List<VisualElement>.
                        // See: https://github.com/Unity-Technologies/UnityCsReference/blob/a935c76939be457b82c70f54fe2cc7dd40fb9090/Modules/UIBuilder/Editor/Builder/BuilderSelection.cs
                        _selectionSelectionProperty = _builderSelectionType.GetProperty("selection", _flags);
                        _selectionSelectionMethod = _builderSelectionType.GetMethod("Select", _flags);
                        _selectionDocumentRootElement = _builderSelectionType.GetProperty("documentRootElement", _flags);
                    }
                }
            }
        }

        // We are caching the last
        protected VisualElement _lastSelectedElement;
        protected string _lastSelectedElementName;
        protected string _lastSelectedElementViewDataKey;

        protected double _lastUpdateTime = -999;
        protected EditorWindow _lastBuilderWindow = null;

        void onEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime > CheckForChangesIntervalInSec)
            {
                _lastUpdateTime = EditorApplication.timeSinceStartup;
                updateBuilderWindow();
                updateSelection();
                updateLastSelectedAttributes();
            }
        }

        void updateBuilderWindow()
        {
            if (_lastBuilderWindow != BuilderWindow)
            {
                _lastBuilderWindow = BuilderWindow;
                if (BuilderWindow != null)
                {
                    OnBuilderWindowOpened?.Invoke();
                }
                else
                {
                    OnBuilderWindowClosed?.Invoke();
                }
            }
        }

        void updateSelection()
        {
            var selection = Selection;

            bool selectionChanged = false;
            if (_previousSelection.Count != selection.Count)
            {
                selectionChanged = true;
            }
            else if (selection.Count > 0)
            {
                for (int i = 0; i < selection.Count; i++)
                {
                    if (_previousSelection[i] != selection[i])
                    {
                        selectionChanged = true;
                        break;
                    }
                }
            }

            if (selectionChanged)
            {
                _previousSelection.Clear();
                _previousSelection.AddRange(selection);

                OnSelectionChanged?.Invoke(selection);
            }
        }

        void updateLastSelectedAttributes()
        {
            // Deselected
            if (_lastSelectedElement != null && (_selection == null || _selection.Count == 0))
            {
                _lastSelectedElement = null;
                _lastSelectedElementName = null;
                _lastSelectedElementViewDataKey = null;
            }

            if (_selection == null || _selection.Count == 0)
                return;

            var element = _selection[_selection.Count - 1];

            if (_lastSelectedElement != element)
            {
                _lastSelectedElement = element;
                _lastSelectedElementName = element.name;
                _lastSelectedElementViewDataKey = element.viewDataKey;
            }
            // Changed name or viewDataKey
            else if (_lastSelectedElementName != element.name
                  || _lastSelectedElementViewDataKey != element.viewDataKey)
            {
                _lastSelectedElementName = element.name;
                _lastSelectedElementViewDataKey = element.viewDataKey;

                OnAttributeChanged?.Invoke(element);
            }
        }

        public void Select(VisualElement element)
        {
            if (element == null)
                return;

            if (element.panel == null)
            {
                Logger.LogWarning("The element is detached. Will abort.");
                return;
            }

            cacheTypesAndProperties();

            if (BuilderWindow != null && _builderSelectionType != null && _builderSelectionProperty != null && _selectionSelectionProperty != null && _selectionSelectionMethod != null)
            {
                // Is of Type BuilderSelection
                var builderSelection = _builderSelectionProperty.GetValue(BuilderWindow);
                if (builderSelection != null)
                {
                    // public void Select(IBuilderSelectionNotifier source, VisualElement ve)
                    // See: https://github.com/Unity-Technologies/UnityCsReference/blob/a935c76939be457b82c70f54fe2cc7dd40fb9090/Modules/UIBuilder/Editor/Builder/BuilderSelection.cs
                    _selectionSelectionMethod.Invoke(builderSelection, new object[] { null, element });
                }
            }
            else
            {
                Logger.LogWarning("Builder window, types, properties or methods not found. Aborting Select().");
            }
        }

        [DidReloadScripts]
        public static void InitOnLoad()
        {
            var _ = UIBuilderWindowWrapper.Instance;
        }
    }
}

#endif