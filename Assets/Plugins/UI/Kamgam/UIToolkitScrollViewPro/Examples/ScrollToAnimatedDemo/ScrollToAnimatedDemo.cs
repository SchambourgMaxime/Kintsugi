using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;
using System.Linq;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class ScrollToAnimatedDemo : MonoBehaviour
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

        protected Easing _easing = Easing.Ease;
        protected float _duration = 1f;
        protected ScrollViewPro.ScrollToAlign _scrollToAlignmentY = ScrollViewPro.ScrollToAlign.End;
        protected Vector4 _scrollToMargin = new Vector4(0f, 0f, 0f, 0f);

        void OnEnable()
        {
            _document = null;
            _scrollView = null;

            for (int i = 0; i < 5; i++)
            {
                var target = ScrollView.Children().ToList()[i];
                var ele = Document.rootVisualElement.Q("Scroll" + (i + 1));
                ele.RegisterCallback<ClickEvent>(e =>
                {
                    ScrollView.ScrollToAnimated(target, _duration, _easing, ScrollViewPro.ScrollToAlign.Visible, _scrollToAlignmentY, _scrollToMargin);
                });
                ele.RegisterCallback<NavigationSubmitEvent>(e =>
                {
                    ScrollView.ScrollToAnimated(target, _duration, _easing, ScrollViewPro.ScrollToAlign.Visible, _scrollToAlignmentY, _scrollToMargin);
                });
            }

            // Easing Dropdown
            var dropDown = Document.rootVisualElement.Q<DropdownField>("Easing");
            dropDown.choices = new List<string>();
            dropDown.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                _easing = (Easing)Enum.GetValues(typeof(Easing)).GetValue(dropDown.index);
            });
            foreach (var e in Enum.GetValues(typeof(Easing)))
            {
                dropDown.choices.Add(e.ToString());
            }
            dropDown.index = 2;

            // Duration input
            var duration = Document.rootVisualElement.Q<TextField>("Duration");
            duration.value = _duration.ToString();
            duration.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                _duration = float.Parse(duration.value, System.Globalization.CultureInfo.InvariantCulture);
            });

            // AlignY Dropdown
            var alignYDropDown = Document.rootVisualElement.Q<DropdownField>("AlignY");
            alignYDropDown.choices = new List<string>();
            alignYDropDown.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                _scrollToAlignmentY = (ScrollViewPro.ScrollToAlign)Enum.GetValues(typeof(ScrollViewPro.ScrollToAlign)).GetValue(alignYDropDown.index);
            });
            foreach (var e in Enum.GetValues(typeof(ScrollViewPro.ScrollToAlign)))
            {
                alignYDropDown.choices.Add(e.ToString());
            }
            alignYDropDown.index = 0;

            // Margin input
            var margin = Document.rootVisualElement.Q<TextField>("Margin");
            margin.value = _scrollToMargin.x.ToString();
            margin.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                var fmt = new System.Globalization.NumberFormatInfo();
                fmt.NegativeSign = "-";
                fmt.NumberDecimalSeparator = ".";
                var value = float.Parse(margin.value, fmt);
                _scrollToMargin = new Vector4(value, value, value, value);
            });
        }
    }
}
