#if KAMGAM_VISUAL_SCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnTextFieldValueChanged = "UITK_OnTextFieldValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Text Field Value Changed (UITK)")]
    [UnitShortTitle("On Text Field Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.InputField))]
    public class OnTextFieldValueChanged : UIToolkitChangeEventBase<string>
    {
        protected override string getHookName()
        {
            return EventNames.OnTextFieldValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var textfield = ve as TextField;
            if (textfield != null)
            {
                textfield.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var textfield = ve as TextField;
            if (textfield != null)
            {
                textfield.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<string> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif