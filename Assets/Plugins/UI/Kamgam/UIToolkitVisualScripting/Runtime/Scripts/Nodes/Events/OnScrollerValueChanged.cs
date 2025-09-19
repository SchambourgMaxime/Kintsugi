#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnScrollerValueChanged = "UITK_OnScrollerValueChanged";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Scrollber Value Changed (UITK)")]
    [UnitShortTitle("On Scroller Changed (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Scrollbar))]
    public class OnScrollerValueChanged : UIToolkitChangeEventBase<float>
    {
        protected override string getHookName()
        {
            return EventNames.OnScrollerValueChanged;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var scroller = ve as Scroller;
            if (scroller != null)
            {
                scroller.Q<Slider>()?.RegisterValueChangedCallback(handleEvent);
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var scroller = ve as Scroller;
            if (scroller != null)
            {
                scroller.Q<Slider>()?.UnregisterValueChangedCallback(handleEvent);
            }
        }

        protected void handleEvent(ChangeEvent<float> evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif