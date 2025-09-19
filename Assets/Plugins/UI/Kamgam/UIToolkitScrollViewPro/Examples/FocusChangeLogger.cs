using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class FocusChangeLogger : MonoBehaviour
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

        // OnEnable gets called whenever a property is changed in the UIBuilder
        // and the UI gets rebuilt.
        void OnEnable()
        {
            Document.rootVisualElement.RegisterCallback<FocusInEvent>(onFocusIn);
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
