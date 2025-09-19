#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using UnityEditor.Callbacks;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// One Script reloads or whenever the UI Builder is started it will check all
    /// panels for UI Elements with glow classes. If it finds any then it will add a
    /// glow manipulator to these.
    /// 
    /// It will do nothing in PLAY MODE. This class exists only to display the glow while
    /// editing in the UI Builder. Usually it adds the glow to the UI Builder window and
    /// the Game Window while editing.
    /// </summary>
    public static class UIEditorPanelObserver
    {
        [DidReloadScripts]
        public static void DidReloadScripts()
        {
            EditorApplication.update -= onUpdateAfterScriptReload;
            EditorApplication.update += onUpdateAfterScriptReload;
        }

        [MenuItem("Tools/" + Installer.AssetName + "/Refresh Preview", priority = 1)]
        private static void onUpdateAfterScriptReload()
        {
            EditorApplication.update -= onUpdateAfterScriptReload;
            EditorApplication.playModeStateChanged -= onPlayModeChanged;
            EditorApplication.playModeStateChanged += onPlayModeChanged;

            var wrapper = UIBuilderWindowWrapper.Instance;
            wrapper.OnBuilderWindowOpened -= RefreshGlowManipulators;
            wrapper.OnBuilderWindowOpened += RefreshGlowManipulators;

            wrapper.OnDocumentLoaded -= OnDocumentLoaded;
            wrapper.OnDocumentLoaded += OnDocumentLoaded;

            wrapper.OnHierarchyChanged -= onHierarchyChanged;
            wrapper.OnHierarchyChanged += onHierarchyChanged;

            Undo.undoRedoPerformed -= onUndo;
            Undo.undoRedoPerformed += onUndo;

            // Trigger refresh on panels
            EditorApplication.delayCall += RefreshGlowManipulators;
        }

        private static VisualElement getGameViewViewport()
        {
            // Find all panels
            _tmpAllPanels.Clear();
            UIElementsUtilityProxy.GetAllPanels(_tmpAllPanels);

            // Check if there are new panels that need a new registry.
            // Our goal is to only affect the UI BUILDER and the GAME view.
            var panelsWithRegistries = _editorGlowPanels.Keys;
            foreach (var panel in _tmpAllPanels)
            {
                if (panel.contextType == ContextType.Player)
                {
                    return panel.visualTree;
                }
            }

            return null;
        }

        private static void onHierarchyChanged()
        {
            RefreshGlowManipulatorsDelayed();
        }

        private static void onUndo()
        {
            var wrapper = UIBuilderWindowWrapper.Instance;
            if(wrapper.HasWindow)
            {
                RefreshGlowManipulatorsDelayed();

                // Another delay for undos (maybe the reload of the UI is taking too long?)
                // TODO: Investigate, delaying via EditorApplication.delayCall is too short for the game view.
                // Find a logical solution.
                EditorScheduler.Schedule(0.5f, RefreshGlowManipulatorsDelayed);
            }
        }

        private static void onPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                ClearEditorManipulators();
            }

            if (change == PlayModeStateChange.EnteredEditMode)
            {
                RefreshGlowManipulatorsDelayed();
            }
        }

        public static void ClearEditorManipulators()
        {
            foreach (var registry in _editorGlowPanels.Values)
            {
                registry.Destroy();
            }
            _editorGlowPanels.Clear();
        }

        static Dictionary<IPanel, GlowPanel> _editorGlowPanels = new Dictionary<IPanel, GlowPanel>();
        static List<IPanel> _tmpAllPanels = new List<IPanel>();
        static List<IPanel> _tmpPanelsToRemove = new List<IPanel>();

        public static void OnDocumentLoaded()
        {
            RefreshGlowManipulatorsDelayed();
        }

        public static void RefreshGlowManipulatorsDelayed()
        {
            EditorApplication.delayCall += () =>
            {
                RefreshGlowManipulators();
            };
        }

        /// <summary>
        /// Goes through all editor and player panels and adds manipulators to any element that has a class matching the configs in the config root.<br />
        /// The configs are taken from GlowConfigRoot FindAsset() which usually is the first config root asset found in the project.
        /// </summary>
        public static void RefreshGlowManipulators()
        {
            // Find all panels
            _tmpAllPanels.Clear();
            UIElementsUtilityProxy.GetAllPanels(_tmpAllPanels);

            // Check if there are new panels that need a new registry.
            // Our goal is to only affect the UI BUILDER and the GAME view.
            var panelsWithGlowPanels = _editorGlowPanels.Keys;

            int pCount = _tmpAllPanels.Count;
            for (int i = 0; i < pCount; i++)
            {
                var panel = _tmpAllPanels[i];

                // Skip GAME view panel if we are in play mode (because these are then upated by the regular player code).
                if (panel.contextType == ContextType.Player && EditorApplication.isPlayingOrWillChangePlaymode)
                    continue;

                // Skip all panels except UI BUILDER and GAME view.
                if (!UIElementsUtilityProxy.IsUIBuilderPanel(panel) && !UIElementsUtilityProxy.IsGameViewPanel(panel))
                    continue;

                // The panel may be updated twice here. Once after the geomentry changed event (for after compilation).
                panel.visualTree.RegisterCallback<GeometryChangedEvent>(onGeometryChanged);
                // And once regularly (for play mode changes, loaded document changes).
                refreshPanel(panel);
            }

            // Remove registries of unused panels
            // Usually not needed as we clean them up in ClearEditorManipulators().
            _tmpPanelsToRemove.Clear();
            foreach (var panel in _editorGlowPanels.Keys)
            {
                if (!_tmpAllPanels.Contains(panel))
                {
                    _tmpPanelsToRemove.Add(panel);
                }
            }
            foreach (var panel in _tmpPanelsToRemove)
            {
                _editorGlowPanels[panel].Destroy();
                _editorGlowPanels.Remove(panel);
            }
        }

        private static void onGeometryChanged(GeometryChangedEvent evt)
        {
            var ve = evt.target as VisualElement;
            ve.panel.visualTree.UnregisterCallback<GeometryChangedEvent>(onGeometryChanged);

            refreshPanel(ve.panel);
        }

        private static void refreshPanel(IPanel panel)
        {
            refreshVisualElementsOnPanel(panel);
            refreshGlowAndShadowsOnPanel(panel);
        }

        private static void refreshVisualElementsOnPanel(IPanel panel)
        {
            GlowPanel glowPanel;
            if (_editorGlowPanels.ContainsKey(panel))
            {
                glowPanel = _editorGlowPanels[panel];
            }
            else
            {
                glowPanel = new GlowPanel(panel.visualTree, null);
                _editorGlowPanels.Add(panel, glowPanel);
            }

            if (glowPanel.ConfigRoot == null)
            {
                glowPanel.ConfigRoot = GlowConfigRoot.FindConfigRoot();
            }

            glowPanel.UpdateGlowOnChildren();
        }

        private static void refreshGlowAndShadowsOnPanel(IPanel panel)
        {
            var glows = panel.visualTree.Query<Glow>().Build();
            foreach (var glow in glows)
            {
                glow.UpdateGlowManipulator();
                glow.MarkDirtyRepaint();
            }

            var shadows = panel.visualTree.Query<Shadow>().Build();
            foreach (var shadow in shadows)
            {
                shadow.UpdateGlowManipulator();
                shadow.MarkDirtyRepaint();
            }
        }
    }
}

#endif