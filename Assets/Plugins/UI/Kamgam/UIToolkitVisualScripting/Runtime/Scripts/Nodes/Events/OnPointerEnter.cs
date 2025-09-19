#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnPointerEnter = "UITK_OnPointerEnter";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Pointer Enter (UITK)")]
    public class OnPointerEnter : UIToolkitEventBase<PointerEventWrapper>
    {
        protected override string getHookName()
        {
            return EventNames.OnPointerEnter;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<PointerEnterEvent>(handlePointerEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<PointerEnterEvent>(handlePointerEvent);
        }

        protected void handlePointerEvent(PointerEnterEvent evt)
        {
            var wrapper = new PointerEventWrapper();
            wrapper.PointerEvent = evt;
            triggerEvent(wrapper);
        }
    }
}
#endif