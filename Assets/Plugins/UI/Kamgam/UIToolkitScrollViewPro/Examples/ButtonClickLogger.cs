using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class ButtonClickLogger : MonoBehaviour
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

        List<Button> _buttons;

        // OnEnable gets called whenever a property is changed in the UIBuilder
        // and the UI gets rebuilt.
        void OnEnable()
        {
            _buttons = Document.rootVisualElement.Query<Button>().ToList();
            foreach (var btn in _buttons)
            {
                btn.RegisterCallback<ClickEvent>(onClick);
                btn.RegisterCallback<NavigationSubmitEvent>(onSubmit);
            }
        }

        void OnDisable()
        {
            foreach (var btn in _buttons)
            {
                btn.UnregisterCallback<ClickEvent>(onClick);
                btn.UnregisterCallback<NavigationSubmitEvent>(onSubmit);
            }
        }

        private void onClick(ClickEvent evt)
        {
            Debug.Log("Clicked button: " + evt.target);
        }

        private void onSubmit(NavigationSubmitEvent evt)
        {
            Debug.Log("Submitted button: " + evt.target);
        }
    }
}
