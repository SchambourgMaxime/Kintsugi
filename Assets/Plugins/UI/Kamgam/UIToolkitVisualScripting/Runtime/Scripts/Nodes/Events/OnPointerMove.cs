#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnPointerMove = "UITK_OnPointerMove";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Pointer Move (UITK)")]
    public class OnPointerMove : UIToolkitEventBase<PointerEventWrapper>
    {
        protected override string getHookName()
        {
            return EventNames.OnPointerMove;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<PointerMoveEvent>(handlePointerEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<PointerMoveEvent>(handlePointerEvent);
        }

        protected void handlePointerEvent(PointerMoveEvent evt)
        {
            var wrapper = new PointerEventWrapper();
            wrapper.PointerEvent = evt;
            triggerEvent(wrapper);
        }
    }
}
#endif