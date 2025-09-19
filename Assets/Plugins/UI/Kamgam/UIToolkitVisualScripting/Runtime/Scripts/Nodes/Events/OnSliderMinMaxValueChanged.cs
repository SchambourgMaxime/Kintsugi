#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnSliderMinMaxValueChanged = "UITK_OnSliderMinMaxValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Slider MinMax Value Changed (UITK)")]
    [UnitShortTitle("On Slider MinMax Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Slider))]
    public class OnSliderMinMaxValueChanged : UIToolkitChangeEventBase<Vector2>
    {
        protected override string getHookName()
        {
            return EventNames.OnSliderMinMaxValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var slider = ve as MinMaxSlider;
            if (slider != null)
            {
                slider.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var slider = ve as MinMaxSlider;
            if (slider != null)
            {
                slider.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<Vector2> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif