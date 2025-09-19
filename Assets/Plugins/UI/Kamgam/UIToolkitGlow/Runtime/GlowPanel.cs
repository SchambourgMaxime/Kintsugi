using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
#if KAMGAM_VISUAL_SCRIPTING
    // Why? See: https://forum.unity.com/threads/unable-to-provide-a-default-for-getvalue-on-object-valueinput.1140022/#post-9138727
    [Unity.VisualScripting.Inspectable]
#endif
    public partial class GlowPanel
    {
        [System.NonSerialized]
        public VisualElement RootVisualElement;

        [System.NonSerialized]
        public GlowConfigRoot ConfigRoot;

        public GlowPanel(VisualElement rootVisualElement, GlowConfigRoot configRoot)
        {
            RootVisualElement = rootVisualElement;
            ConfigRoot = configRoot;
        }

        public void Enable()
        {
            if (!Panels.Contains(this))
                Panels.Add(this);

            UpdateGlowOnChildren();

            // Update glow elements because they have animations and animations need data from the glow document`s config root.
            var glowElements = RootVisualElement.Query<Glow>().Build();
            foreach (var glow in glowElements)
            {
                glow.UpdateGlowManipulator();
            }
        }

        public void Destroy()
        {
            RootVisualElement = null;
            Panels.Remove(this);
        }

        public GlowConfig GetConfigAt(int index)
        {
            if (index >= 0 && index < ConfigRoot.Configs.Count)
                return ConfigRoot.Configs[index];

            return null;
        }

        public GlowConfig GetConfigByClassName(string className)
        {
            return ConfigRoot.GetConfigByClassName(className);
        }

        /// <summary>
        /// Goes through the whole visual tree and adds/removes glow manipulators.
        /// </summary>
        public void UpdateGlowOnChildren()
        {
            if (RootVisualElement == null)
                return;

            RootVisualElement.Query<VisualElement>().ForEach(UpdateGlowOnElement);
        }

        /// <summary>
        /// Call this on any element which you have recently added (or removed) a glow class.
        /// </summary>
        /// <param name="element"></param>
        public void UpdateGlowOnElement(VisualElement element)
        {
            if (ConfigRoot == null)
                return;

            // Does the element have a shadow USS class?
            GlowConfig shadowClass = null;
            int count = ConfigRoot.Configs.Count;
            for (int i = 0; i < count; i++)
            {
                var config = GetConfigAt(i);
                
                if (config == null)
                    continue;

                if (element.ClassListContains(config.ClassName))
                {
                    shadowClass = config;
                    break;
                }
            }

            if (shadowClass == null)
            {
                // If the element has no shadow class then remove shadow manipulator.
                // Only do this is part of the managed manipulators in the pool.
                // Glow and Shadow elements are NOT managed. They add/remove manipulators on theirs own.
                var m = GetManipulator(element);
                if (m != null && ManipulatorPool.Contains(m))
                {
                    RemoveManipulator(element, m);
                    element.MarkDirtyRepaint();
                }
            }
            else
            {
                if (element.panel != null)
                {
                    AddOrReuseManipulator(element, shadowClass, addAnimation: true);
                    element.MarkDirtyRepaint();
                }
            }
        }
    }
}