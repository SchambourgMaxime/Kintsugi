#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;
using UnityEditor.Callbacks;
using System.Reflection.Emit;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// Wrapper for the internal "Builder" class.
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/a935c76939be457b82c70f54fe2cc7dd40fb9090/Modules/UIBuilder/Editor/Builder/Builder.cs
    /// </summary>
    public class UIBuilderWindowWrapper
    {
        public static double CheckForChangesIntervalInSec = 0.2; // 0.2 = 5 fps

        public event System.Action<List<VisualElement>> OnSelectionChanged;
        public event System.Action OnBuilderWindowOpened;
        public event System.Action OnBuilderWindowClosed;
        public event System.Action OnDocumentLoaded;
        public event System.Action OnHierarchyChanged;

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
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach(var assembly in assemblies)
                    {
                        var builderWindow = assembly.GetType("Unity.UI.Builder.Builder");
                        if (builderWindow != null)
                        {
                            return builderWindow;
                        }
                    }
                }
                return _builderWindowType;
            }
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

        /// <summary>
        /// Returns aflse if the UIBuilder type was not found or if the window is not opened.
        /// </summary>
        public bool HasWindow
        {
            get => BuilderWindow != null;
        }


        protected Type _builderSelectionType;
        protected PropertyInfo _builderSelectionProperty;
        protected PropertyInfo _selectionSelectionProperty;
        protected PropertyInfo _selectionDocumentRootElement;
        /// <summary>
        /// public void Select(IBuilderSelectionNotifier source, VisualElement ve)
        /// </summary>
        protected MethodInfo _selectionSelectionMethod;

        protected List<VisualElement> _previousSelection = new  List<VisualElement>();
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
                if(window != null)
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
        protected TextElement _canvasTitle;
        protected VisualElement _viewport;

        void onEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime > CheckForChangesIntervalInSec)
            {
                _lastUpdateTime = EditorApplication.timeSinceStartup;
                updateBuilderWindow();
                updateSelection();
                updateLastSelectedAttributes();
                updateHierarchyChanged();
            }
        }

        void updateBuilderWindow()
        {
            if(_lastBuilderWindow != BuilderWindow)
            {
                _lastBuilderWindow = BuilderWindow;
                if (BuilderWindow != null)
                {
                    onBuilderWindowOpenend();
                }
                else
                {
                    onBuilderWindowClosed();
                }
            }
        }

        protected void onBuilderWindowOpenend()
        {
            OnBuilderWindowOpened?.Invoke();

            if (_canvasTitle == null)
            {
                // TODO: Improve document changed detection.
                // Right now it is based on the TITLE text changing (OMG). It won't trigger if two document have the same name.
                _canvasTitle = BuilderWindow.rootVisualElement.Q<TextElement>(name: "title", className: "unity-builder-canvas__title");
                _canvasTitle.RegisterCallback<ChangeEvent<string>>(onTitleChanged);
            }

            if (_viewport == null)
            {
                _viewport = BuilderWindow.rootVisualElement.Q(className: "unity-builder-viewport");
            }
        }

        protected void onBuilderWindowClosed()
        {
            OnBuilderWindowClosed?.Invoke();

            if (_canvasTitle != null)
            {
                _canvasTitle.UnregisterCallback<ChangeEvent<string>>(onTitleChanged);
                _canvasTitle = null;
            }
        }

        protected void onTitleChanged(ChangeEvent<string> evt)
        {
            OnDocumentLoaded?.Invoke();
        }

        int _lastViewportChildCount;

        void updateHierarchyChanged()
        {
            if (_viewport == null)
                return;

            int count = 0;
            getTotalChildCount(_viewport, ref count);

            if(_lastViewportChildCount != count)
            {
                OnHierarchyChanged?.Invoke();
            }
            _lastViewportChildCount = count;
        }
        
        static void getTotalChildCount(VisualElement element, ref int count)
        {
            if (element == null)
                return;

            count += element.childCount;

            int childCount = element.childCount;
            for (int i = childCount-1; i >= 0; i--)
            {
                getTotalChildCount(element.ElementAt(i), ref count);
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