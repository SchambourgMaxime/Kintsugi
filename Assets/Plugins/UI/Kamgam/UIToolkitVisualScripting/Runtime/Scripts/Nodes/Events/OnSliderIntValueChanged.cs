#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnSliderIntValueChanged = "UITK_OnSliderIntValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Slider Int Value Changed (UITK)")]
    [UnitShortTitle("On Slider Int Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Slider))]
    public class OnSliderIntValueChanged : UIToolkitChangeEventBase<int>
    {
        protected override string getHookName()
        {
            return EventNames.OnSliderIntValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var slider = ve as SliderInt;
            if (slider != null)
            {
                slider.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var slider = ve as SliderInt;
            if (slider != null)
            {
                slider.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<int> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif