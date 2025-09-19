#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnOnButtonClick = "UITK_OnButtonClick";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Button Click (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Button))]
    public class OnOnButtonClick : UIToolkitEventBase<ClickEvent>
    {
        protected override string getHookName()
        {
            return EventNames.OnOnButtonClick;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var button = ve as Button;
            if (button != null)
            {
                button.RegisterCallback<ClickEvent>(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var button = ve as Button;
            if (button != null)
            {
                button.UnregisterCallback<ClickEvent>(handleEvent);
            }
        }

        protected void handleEvent(ClickEvent evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif