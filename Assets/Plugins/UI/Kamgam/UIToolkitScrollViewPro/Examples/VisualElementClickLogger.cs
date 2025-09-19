using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class VisualElementClickLogger : MonoBehaviour
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

        List<VisualElement> _elements;

        // OnEnable gets called whenever a property is changed in the UIBuilder
        // and the UI gets rebuilt.
        void OnEnable()
        {
            _elements = Document.rootVisualElement.Query<VisualElement>().ToList();
            foreach (var ele in _elements)
            {
                if (ele is Button)
                    continue;

                if (ele.hierarchy.parent == null || ele.hierarchy.parent.name != "unity-content-container")
                    continue;

                //ele.RegisterCallback<ClickEvent>(onClick);
                ele.AddManipulator(new Clickable(evt => onClick(evt)));
            }
        }

        void OnDisable()
        {
            foreach (var ele in _elements)
            {
                ele.UnregisterCallback<ClickEvent>(onClick);
            }
        }

        private void onClick(EventBase evt)
        {
            Debug.Log("Clicked Element " + evt.currentTarget);
        }
    }
}
