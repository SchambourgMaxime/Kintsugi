#if KAMGAM_VISUAL_SCRIPTING
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting
{
    /// <summary>
    /// A collection of common actions performed on visual elements.
    /// Of possible the return type is VisualElement to support easy chaining of nodes.
    /// </summary>
    public static partial class UIToolkitVisualElementExtensions
    {
        // B U T T O N

        public static Button GetButton(this VisualElement element)
        {
            return element as Button;
        }


        public static Clickable GetClickable(this VisualElement element)
        {
            var button = element as Button;
            if (button == null)
                return null;

            return button.clickable;
        }

        public static Clickable GetClickable(this Button button)
        {
            if (button == null)
                return null;

            return button.clickable;
        }


        public static VisualElement SetClickable(this VisualElement element, Clickable clickable)
        {
            var button = element as Button;
            if (button == null)
                return null;

            button.clickable = clickable;

            return button;
        }

        public static Button SetClickable(this Button button, Clickable clickable)
        {
            if (button == null)
                return button;

            button.clickable = clickable;

            return button;
        }
    }
}
#endif