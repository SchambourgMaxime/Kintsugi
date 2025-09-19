#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnSliderValueChanged = "UITK_OnSliderValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Slider Value Changed (UITK)")]
    [UnitShortTitle("On Slider Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Slider))]
    public class OnSliderValueChanged : UIToolkitChangeEventBase<float>
    {
        protected override string getHookName()
        {
            return EventNames.OnSliderValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var slider = ve as Slider;
            if (slider != null)
            {
                slider.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var slider = ve as Slider;
            if (slider != null)
            {
                slider.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<float> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif