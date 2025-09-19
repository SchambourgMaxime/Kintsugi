using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public class HoverAnimationDemo : MonoBehaviour
    {
        [Header("Toggle Glow")]

        [Tooltip("The class that will be toggled on hover.")]
        public string HoverGlowClass = "g-hover-outline";

        [Tooltip("The animation name that will be toggled on hover.")]
        public string AnimationName = BlobAnimationAsset.DefaultName;

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

        public void OnEnable()
        {
            // Register glow on hover
            var hoverElementsQuery = Document.rootVisualElement.Query<Button>();
            GlowUtils.RegisterToggleGlowAndAnimationOnHover(hoverElementsQuery, BlobAnimationAsset.DefaultName, HoverGlowClass);
        }

        public void OnDisable() 
        {
            if (Document != null && Document.rootVisualElement != null)
            {
                var hoverElementsQuery = Document.rootVisualElement.Query<Button>();
                GlowUtils.UnregisterToggleGlowAndAnimationOnHover(hoverElementsQuery, BlobAnimationAsset.DefaultName, HoverGlowClass);
            }
        }
    }

}