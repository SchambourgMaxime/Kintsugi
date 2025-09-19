using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(GlowConfigRootProvider))]
    public partial class GlowDocument : MonoBehaviour
    {
        protected GlowPanel _panel;
        public GlowPanel Panel
        {
            get
            {
                if (_panel == null)
                {
                    _panel = new GlowPanel(Document.rootVisualElement, GlowConfigRootProvider.GetConfigRoot());
                }
                return _panel;
            }

            private set
            {
                _panel = value;
            }
        }

        protected Action _onLateEnable;

        protected Coroutine _waitForPanelToBeReadyCoroutine;

        protected bool _isReady = false;

        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = this.GetComponent<UIDocument>();
                }
                return _document;
            }
        }

        public GlowConfigRoot ConfigRoot
        {
            get
            {
                return Panel.ConfigRoot;
            }
        }

        public void Awake()
        {
            if (!_glowDocuments.Contains(this))
                _glowDocuments.Add(this);

            if (Panel.ConfigRoot == null)
            {
                Panel.ConfigRoot = GlowConfigRootProvider.GetConfigRoot();
            }
        }

        public void OnEnable()
        {
            if (!_glowDocuments.Contains(this))
                _glowDocuments.Add(this);

            Panel.RootVisualElement = Document.rootVisualElement;

            // Wait for panel to become ready
            if (Panel.RootVisualElement != null && Panel.RootVisualElement.panel != null)
            {
                onPanelIsReady();
            }
            else
            {
                if (_waitForPanelToBeReadyCoroutine != null)
                    StopCoroutine(_waitForPanelToBeReadyCoroutine);
                _waitForPanelToBeReadyCoroutine = StartCoroutine(waitForPanelToBeAttached());
            }
        }

        protected IEnumerator waitForPanelToBeAttached()
        {
            while (Panel.RootVisualElement == null || Panel.RootVisualElement.panel == null)
            {
                yield return null;
            }

            onPanelIsReady();
        }

        private void onPanelIsReady()
        {
            Panel.Enable();

            _onLateEnable?.Invoke();
            _isReady = true;
        }

        public void OnDisable()
        {
            _isReady = false;
        }

        public void OnDestroy()
        {
            Panel?.Destroy();

            if (_glowDocuments.Contains(this))
                _glowDocuments.Remove(this);
        }

        /// <summary>
        /// Registers the on enable callback. This ensures all the manipulators are already added when this called.<br />
        /// If the glow document is already enabled then it is called immediately.<br />
        /// Use this if you want to do some changes on the glow manipulators.
        /// </summary>
        /// <param name="action"></param>
        public void RegisterOnEnable(Action action)
        {
            if (action == null)
                return;

            _onLateEnable -= action;
            _onLateEnable += action;

            // If already enabled the call this action immediately.
            if (_isReady)
            {
                action.Invoke();
            }
        }

        public void UnregisterOnEnable(Action action)
        {
            if (action == null)
                return;

            _onLateEnable -= action;
        }

        public GlowConfig GetConfigAt(int index)
        {
            if (Panel == null)
                return null;

            return Panel.GetConfigAt(index);
        }

        /// <summary>
        /// Goes through the whole visual tree and adds/removes glow manipulators.
        /// </summary>
        public void UpdateGlowOnChildren()
        {
            if (Panel == null)
                return;

            if (Panel.RootVisualElement == null)
                Panel.RootVisualElement = Document.rootVisualElement;

            Panel.UpdateGlowOnChildren();
        }

        /// <summary>
        /// Call this on any element which you have recently added (or removed) a glow class.
        /// </summary>
        /// <param name="element"></param>
        public void UpdateGlowOnElement(VisualElement element)
        {
            if (Panel == null)
                return;

            if (Panel.RootVisualElement == null)
                Panel.RootVisualElement = Document.rootVisualElement;

            Panel.UpdateGlowOnElement(element);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Panel.ConfigRoot == null)
            {
                var root = GlowConfigRoot.FindConfigRoot();
                if (root != null)
                {
                    Panel.ConfigRoot = root;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.EditorUtility.SetDirty(this.gameObject);
                }
            }

            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UpdateGlowOnChildren();
            }
            else
            {
                UIEditorPanelObserver.RefreshGlowManipulators();
            }
        }
#endif
    }
}