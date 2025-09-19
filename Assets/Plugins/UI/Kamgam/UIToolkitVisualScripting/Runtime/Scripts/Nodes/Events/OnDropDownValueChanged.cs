#if KAMGAM_VISUAL_SCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnDropDownValueChanged = "UITK_OnDropDownValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Dropdown Value Changed (UITK)")]
    [UnitShortTitle("On Dropdown Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Dropdown))]
    public class OnDropDownValueChanged : UIToolkitChangeEventBase<string>
    {
        protected override string getHookName()
        {
            return EventNames.OnDropDownValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var drodown = ve as DropdownField;
            if (drodown != null)
            {
                drodown.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var dropdown = ve as DropdownField;
            if (dropdown != null)
            {
                dropdown.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<string> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif