using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public partial class GlowPanel
    {
        public static List<GlowManipulator> ManipulatorPool = new List<GlowManipulator>();

        public static List<GlowPanel> Panels = new List<GlowPanel>();

        public static void AddClass(VisualElement element, string className)
        {
            if (element == null)
                return;

            element.AddToClassList(className);

            GlowPanel panel = GetPanel(element);
            if (panel != null)
                panel.UpdateGlowOnElement(element);
        }

        public static void RemoveClass(VisualElement element, string className)
        {
            if (element == null)
                return;

            element.RemoveFromClassList(className);

            GlowPanel panel = GetPanel(element);
            if (panel != null)
                panel.UpdateGlowOnElement(element);
        }

        public static GlowPanel GetPanel(VisualElement element)
        {
            var glowDocuments = GlowDocument.GetGlowDocuments();
            foreach (var glowDocument in glowDocuments)
            { 
                if (glowDocument.Document == null || glowDocument.Document.gameObject == null)
                    continue;

                // If the element is null then simply return the first found document.
                if (element == null)
                    return glowDocument.Panel;

                if (glowDocument.Document.rootVisualElement.Contains(element))
                {
                    return glowDocument.Panel;
                }
            }

            // Secondy try:
            // Try with parent of element (useful for dynamically added elements that may not yet be part of the panel).
            element = element.parent;
            foreach (var glowDocument in glowDocuments)
            {
                if (glowDocument.Document == null || glowDocument.Document.gameObject == null)
                    continue;

                // If the element is null then simply return the first found document.
                if (element == null)
                    return glowDocument.Panel;

                if (glowDocument.Document.rootVisualElement.Contains(element))
                {
                    return glowDocument.Panel;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if a manipulator was found.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="manipulator"></param>
        /// <returns></returns>
        public static bool TryGetManipulator(VisualElement element, out GlowManipulator manipulator)
        {
            manipulator = GetManipulator(element);
            return manipulator != null;
        }

        /// <summary>
        /// Returns NULL if no manipulator was found.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static GlowManipulator GetManipulator(VisualElement element)
        {
            foreach (var m in ManipulatorPool)
            {
                if (m != null && m.target != null && m.target == element)
                    return m;
            }

            var glow = element as Glow;
            if (glow != null)
                return glow.manipulator;

            var shadow = element as Shadow;
            if (shadow != null)
                return shadow.manipulator;

            return null;
        }

        public static GlowManipulator AddOrReuseManipulator(VisualElement element, GlowConfig config, bool addAnimation)
        {
            var existingManipulator = GetManipulator(element);
            if (existingManipulator != null)
            {
                // Update existing manipulator because the config may have changed
                bool didChange = existingManipulator.Config != config;
                if (didChange)
                {
                    existingManipulator.Config = config;
                    existingManipulator.target.MarkDirtyRepaint();
                }

                // Skip if already added.
                return existingManipulator;
            }

            // Find a manipulator to reuse.
            GlowManipulator manipulator = null;
            foreach (var m in ManipulatorPool)
            {
                if (m.target == null)
                {
                    manipulator = m;
                    break;
                }
            }

            // Create new if necessary
            if (manipulator == null)
            {
                manipulator = new GlowManipulator(config);
                ManipulatorPool.Add(manipulator);
            }
            else
            {
                manipulator.Config = config;
            }

            element.AddManipulator(manipulator);

            // Clean up dangling manipulators 
            manipulator.OnUnregisterCallbacksOnTarget -= onManipulatorUnregistered;
            manipulator.OnUnregisterCallbacksOnTarget += onManipulatorUnregistered;

            // Add Animation if necessary
            if (addAnimation
#if UNITY_EDITOR
                && UnityEditor.EditorApplication.isPlaying
                && element.panel != null && element.panel.contextType == ContextType.Player
#endif
                )
            {
                if (!string.IsNullOrEmpty(config.Animation))
                {
                    var existingAnimation = GlowAnimation.GetAnimationOnManipulatorByName(manipulator, config.Animation);
                    if (existingAnimation == null)
                    {
                        GlowAnimation.AddAnimationCopyTo(
                            config.Animation,
                            manipulator,
                            configRoot: null,
                            linkToTemplate: true,
                            reuseExisting: true
                        );
                    }
                }
            }

            // TODO: Maybe clean up the manipulator in Editor in editor mode on detach.
            // Investigate.

            return manipulator;
        }

        static void onManipulatorUnregistered(GlowManipulator manipulator)
        {
            ManipulatorPool.Remove(manipulator);
        }

        public static void RemoveManipulator(VisualElement element, GlowManipulator manipulator, bool removeFromPool = false)
        {
            element.RemoveManipulator(manipulator);
            manipulator.target = null;

            if (removeFromPool)
            {
                if (ManipulatorPool.Contains(manipulator))
                    ManipulatorPool.Remove(manipulator);
            }
        }
    }
}