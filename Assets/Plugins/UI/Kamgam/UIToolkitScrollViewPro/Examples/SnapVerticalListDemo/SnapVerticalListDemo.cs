using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class SnapVerticalListDemo : MonoBehaviour
    {
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

        protected ScrollViewPro _scrollView;
        public ScrollViewPro ScrollView
        {
            get
            {
                if (_scrollView == null)
                {
                    _scrollView = Document.rootVisualElement.Query<ScrollViewPro>();
                }
                return _scrollView;
            }
        }

        void OnEnable()
        {
            var labels = ScrollView.Query<Label>().Build();
            foreach (var label in labels)
            {
                label.RegisterCallback<SnapEvent>(onSnap);
            }
        }

        void OnDisable()
        {
            var labels = ScrollView.Query<Label>().Build();
            foreach (var label in labels)
            {
                label.UnregisterCallback<SnapEvent>(onSnap);
            }
        }

        protected Color prevSnappedBgColor;
        protected VisualElement prevSnappedElement;

        private void onSnap(SnapEvent evt)
        {
            Debug.Log("Snapped to: " + evt.target);

            // Reset revious selection
            if (prevSnappedElement != null)
            {
                prevSnappedElement.style.backgroundColor = prevSnappedBgColor;
                prevSnappedElement = null;
            }

            // mark snapped
            var ve = (evt.target as VisualElement);
            prevSnappedBgColor = ve.resolvedStyle.backgroundColor;
            ve.style.backgroundColor = Color.black;
            prevSnappedElement = ve;
        }
    }
}
