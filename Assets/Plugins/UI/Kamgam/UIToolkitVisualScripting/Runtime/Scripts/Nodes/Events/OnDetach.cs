#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnDetach = "UITK_OnDetach";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Detach (UITK)")]
    public class OnDetach : UIToolkitEventBase<DetachFromPanelEvent>
    {
        protected override string getHookName()
        {
            return EventNames.OnDetach;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<DetachFromPanelEvent>(handleEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<DetachFromPanelEvent>(handleEvent);
        }

        protected void handleEvent(DetachFromPanelEvent evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif