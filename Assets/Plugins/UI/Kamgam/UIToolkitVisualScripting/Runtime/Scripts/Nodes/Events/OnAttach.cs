#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnAttach = "UITK_OnAttach";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Attach (UITK)")]
    public class OnAttach : UIToolkitEventBase<AttachToPanelEvent>
    {
        protected override string getHookName()
        {
            return EventNames.OnAttach;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<AttachToPanelEvent>(handleEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<AttachToPanelEvent>(handleEvent);
        }

        protected void handleEvent(AttachToPanelEvent evt)
        {
            triggerEvent(evt);
        }
    }
}
#endif