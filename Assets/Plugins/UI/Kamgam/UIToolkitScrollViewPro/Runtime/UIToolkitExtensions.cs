using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public static class UIToolkitExtensions
    {
        public static bool IsChildOf(this VisualElement element, VisualElement parent, bool recurseIntoChildren = true)
        {
            return parent.Contains(element);
        }
    }
}
