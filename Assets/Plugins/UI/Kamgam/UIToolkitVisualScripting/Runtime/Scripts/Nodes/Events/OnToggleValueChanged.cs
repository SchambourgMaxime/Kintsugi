#if KAMGAM_VISUAL_SCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnToggleValueChanged = "UITK_OnToggleValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Toggle Value Changed (UITK)")]
    [UnitShortTitle("On Toggle Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Toggle))]
    public class OnToggleValueChanged : UIToolkitChangeEventBase<bool>
    {
        protected override string getHookName()
        {
            return EventNames.OnToggleValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var toggle = ve as Toggle;
            if (toggle != null)
            {
                toggle.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var dropdown = ve as Toggle;
            if (dropdown != null)
            {
                dropdown.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<bool> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif