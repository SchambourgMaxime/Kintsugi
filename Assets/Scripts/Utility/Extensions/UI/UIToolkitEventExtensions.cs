using Unity.VisualScripting;

using UnityEngine.UIElements;

namespace VV.UIToolkitVisualScripting
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