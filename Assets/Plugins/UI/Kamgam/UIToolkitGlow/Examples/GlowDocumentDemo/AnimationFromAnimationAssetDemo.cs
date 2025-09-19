using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public class AnimationFromAnimationAssetDemo : MonoBehaviour
    {
        [Header("Animations")]

        public GlowConfigRoot ConfigRoot;

        public string ElementName = "Animation";
        public GlowAnimationAsset Animation;

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
            var element = GlowDoc.Document.rootVisualElement.Q<VisualElement>(name: ElementName);
            var manipulator = GlowPanel.GetManipulator(element);

            // Adding an animation the long way:
            var animation = Animation.GetAnimation();
            animation?.AddToManipulator(manipulator);

            // Shortcut:
            //GlowAnimation.AddAnimationTo(RadialBlobAnimationAsset.DefaultName, manipulator);

            // Another shortcut for adding a copy instead of the original:
            // GlowAnimation.AddAnimationCopyTo(RadialBlobAnimationAsset.DefaultName, manipulator, ConfigRoot, linkToOriginal: true);
        }
    }

}