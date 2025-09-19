using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class InfinityDemo : MonoBehaviour
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
                    _scrollView = Document.rootVisualElement.Q<ScrollViewPro>();
                }
                return _scrollView;
            }
        }

        // OnEnable gets called whenever a property is changed in the UIBuilder
        // and the UI gets rebuilt.
        void OnEnable()
        {
            // Dropdown
            var dropDown = Document.rootVisualElement.Q<DropdownField>();
            dropDown.choices = new List<string>();
            foreach (var e in Enum.GetValues(typeof(ScrollViewPro.ScrollToAlign)))
            {
                dropDown.choices.Add(e.ToString());
            }
            dropDown.index = 0;
            dropDown.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                var align = (ScrollViewPro.ScrollToAlign)Enum.GetValues(typeof(ScrollViewPro.ScrollToAlign)).GetValue(dropDown.index);
                ScrollView.focusSnapAlignX = align;
                Debug.Log("Changed align to: " + align);

                // Let's also update focusSnapInside to the most common setting automatically.
                if (align == ScrollViewPro.ScrollToAlign.Visible)
                    ScrollView.focusSnapInside = false;
                else
                    ScrollView.focusSnapInside = true;
            });
            
        }

        void OnDisable()
        {
            Document?.rootVisualElement?.UnregisterCallback<FocusInEvent>(onFocusIn);
        }

        private void onFocusIn(FocusInEvent evt)
        {
            Debug.Log("Focusing: " + evt.target);
        }
    }
}
