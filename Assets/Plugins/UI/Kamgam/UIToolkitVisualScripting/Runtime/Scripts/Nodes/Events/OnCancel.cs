#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnCancel = "UITK_OnCancel";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Cancel (UITK)")]
    public class OnCancel : UIToolkitEventBase<VisualElement>
    {
        protected override string getHookName()
        {
            return EventNames.OnCancel;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<PointerCancelEvent>(handlePointerCancelEvent);
            ve.RegisterCallback<NavigationCancelEvent>(handleCancelEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<PointerCancelEvent>(handlePointerCancelEvent);
            ve.UnregisterCallback<NavigationCancelEvent>(handleCancelEvent);
        }

        protected void handlePointerCancelEvent(PointerCancelEvent evt)
        {
            triggerEvent(evt.target as VisualElement);
        }

        protected void handleCancelEvent(NavigationCancelEvent evt)
        {
            triggerEvent(evt.target as VisualElement);
        }
    }
}
#endif