using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public class AnimationsForElements : MonoBehaviour
    {
        public GlowConfigRoot ConfigRoot;

        [Serializable]
        public struct ElementWithAnimation
        {
            public string ElementName;
            public GlowAnimationAsset AnimationTemplate;
        }

        public List<ElementWithAnimation> Animations;

        public bool LinkAnimationsToTemplate = true;

        protected GlowDocument _glowDoc;
        public GlowDocument GlowDoc
        {
            get
            {
                if (_glowDoc == null)
                {
                    _glowDoc = this.GetComponent<GlowDocument>();
                }
                return _glowDoc;
            }
        }

        public void OnEnable()
        {
            // Wait for the GlowDocument to add all the glow manipulators.
            GlowDoc.RegisterOnEnable(OnGlowEnabled);
        }

        public void OnGlowEnabled()
        {
            foreach (var anim in Animations)
            {
                var element = GlowDoc.Document.rootVisualElement.Q<VisualElement>(name: anim.ElementName);
                if (element == null)
                    continue;

                var manipulator = GlowPanel.GetManipulator(element);
                GlowAnimation.AddAnimationCopyTo(
                    anim.AnimationTemplate.Name,
                    manipulator,
                    configRoot: GlowDoc.ConfigRoot,
                    linkToTemplate: LinkAnimationsToTemplate,
                    reuseExisting: true);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ConfigRoot == null)
            {
                ConfigRoot = GlowConfigRoot.FindConfigRoot();
                if (ConfigRoot != null)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.EditorUtility.SetDirty(this.gameObject);
                }
            }
        }
#endif
    }

}