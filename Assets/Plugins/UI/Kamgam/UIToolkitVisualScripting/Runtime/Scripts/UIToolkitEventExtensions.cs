#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting
{
    [IncludeInSettings(include: true)]
    public static class UIToolkitEventExtensions
    {
        public static VisualElement GetTarget(this EventBase evt)
        {
            return evt.target as VisualElement;
        }
    }
}
#endif