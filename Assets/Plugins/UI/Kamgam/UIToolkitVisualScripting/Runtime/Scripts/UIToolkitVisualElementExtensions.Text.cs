#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Kamgam.UIToolkitVisualScripting
{
    /// <summary>
    /// A collection of common actions performed on visual elements.
    /// Of possible the return type is VisualElement to support easy chaining of nodes.
    /// </summary>
    public static partial class UIToolkitVisualElementExtensions
    {
        // L A B E L

        public static TextElement GetTextElement(this VisualElement element)
        {
            return element as TextElement;
        }

        public static Label GetLabel(this VisualElement element)
        {
            return element as Label;
        }

        public static TextField GetTextField(this TextField element)
        {
            return element as TextField;
        }

        public static string GetText(this Button button)
        {
            return button.text;
        }

        public static string GetText(this Label label)
        {
            return label.text;
        }

        public static string GetText(this TextElement textElement)
        {
            return textElement.text;
        }

        public static string GetText(this VisualElement element)
        {
            var textElement = element as TextElement;
            if (textElement == null)
                return null;

            return textElement.text;
        }

        public static VisualElement SetText(this VisualElement element, string text)
        {
            var textElement = element as TextElement;
            if (textElement == null)
                return element;

            textElement.text = text;
            return element;
        }

        public static TextElement SetText(this TextElement textElement, string text)
        {
            if (textElement == null)
                return textElement;

            textElement.text = text;
            return textElement;
        }

        public static Label SetText(this Label label, string text)
        {
            if (label == null)
                return label;

            label.text = text;
            return label;
        }

        public static Button SetText(this Button button, string text)
        {
            button.text = text;
            return button;
        }


        public static VisualElement AddText(this VisualElement element, string text)
        {
            var textElement = element as TextElement;
            if (textElement == null)
                return element;

            textElement.text += text;
            return element;
        }

        public static TextElement AddText(this TextElement textElement, string text)
        {
            if (textElement == null)
                return textElement;

            textElement.text += text;
            return textElement;
        }

        public static Label AddText(this Label label, string text)
        {
            if (label == null)
                return label;

            label.text += text;
            return label;
        }


        public static VisualElement SetFormattedText(this VisualElement element, string text, object[] arguments)
        {
            string str = string.Format(text, arguments);
            return element.SetText(str);
        }

        public static TextElement SetFormattedText(this TextElement textElement, string text, object[] arguments)
        {
            string str = string.Format(text, arguments);
            return textElement.SetText(str);
        }

        public static Label SetFormattedText(this Label label, string text, object[] arguments)
        {
            string str = string.Format(text, arguments);
            return label.SetText(str);
        }


        public static VisualElement SetFormattedText1(this VisualElement element, string text, object argument1)
        {
            string str = string.Format(text, argument1);
            return element.SetText(str);
        }

        public static TextElement SetFormattedText1(this TextElement textElement, string text, object argument1)
        {
            string str = string.Format(text, argument1);
            return textElement.SetText(str);
        }

        public static Label SetFormattedText1(this Label label, string text, object argument1)
        {
            string str = string.Format(text, argument1);
            return label.SetText(str);
        }


        public static VisualElement SetFormattedText2(this VisualElement element, string text, object argument1, object argument2)
        {
            string str = string.Format(text, argument1, argument2);
            return element.SetText(str);
        }

        public static TextElement SetFormattedText2(this TextElement textElement, string text, object argument1, object argument2)
        {
            string str = string.Format(text, argument1, argument2);
            return textElement.SetText(str);
        }

        public static Label SetFormattedText2(this Label label, string text, object argument1, object argument2)
        {
            string str = string.Format(text, argument1, argument2);
            return label.SetText(str);
        }


        public static bool GetEnableRichtText(this VisualElement element)
        {
            var textElement = element as TextElement;
            if (textElement == null)
                return false;

            return textElement.enableRichText;
        }

        public static VisualElement SetEnableRichtText(this VisualElement element, bool enabled)
        {
            var textElement = element as TextElement;
            if (textElement == null)
                return element;

            textElement.enableRichText = enabled;
            return element;
        }


        public static bool GetEnableRichtText(this TextElement textElement)
        {
            return textElement.enableRichText;
        }

        public static TextElement SetEnableRichtText(this TextElement textElement, bool enabled)
        {
            textElement.enableRichText = enabled;
            return textElement;
        }

        public static bool GetEnableRichtText(this Label label)
        {
            return label.enableRichText;
        }

        public static Label SetEnableRichtText(this Label label, bool enabled)
        {
            label.enableRichText = enabled;
            return label;
        }
    }
}
#endif