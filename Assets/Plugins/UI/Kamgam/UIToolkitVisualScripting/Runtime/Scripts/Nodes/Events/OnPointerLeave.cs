#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnPointerLeave = "UITK_OnPointerLeave";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Pointer Leave (UITK)")]
    public class OnPointerLeave : UIToolkitEventBase<PointerEventWrapper>
    {
        protected override string getHookName()
        {
            return EventNames.OnPointerLeave;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<PointerLeaveEvent>(handlePointerEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<PointerLeaveEvent>(handlePointerEvent);
        }

        protected void handlePointerEvent(PointerLeaveEvent evt)
        {
            var wrapper = new PointerEventWrapper();
            wrapper.PointerEvent = evt;
            triggerEvent(wrapper);
        }
    }
}
#endif