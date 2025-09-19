#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnSubmit = "UITK_OnSubmit";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Submit (UITK)")]
    [UnitShortTitle("On Submit (UITK)")]
    public class OnSubmit : UIToolkitEventBase<SubmitEventWrapper>
    {
        protected override string getHookName()
        {
            return EventNames.OnSubmit;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            ve.RegisterCallback<ClickEvent>(handleClickEvent);
            ve.RegisterCallback<NavigationSubmitEvent>(handleSubmitEvent);
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            ve.UnregisterCallback<ClickEvent>(handleClickEvent);
            ve.UnregisterCallback<NavigationSubmitEvent>(handleSubmitEvent);
        }

        protected void handleClickEvent(ClickEvent evt)
        {
            var data = new SubmitEventWrapper();
            data.ClickEvent = evt;
            triggerEvent(data);
        }

        protected void handleSubmitEvent(NavigationSubmitEvent evt)
        {
            var data = new SubmitEventWrapper();
            data.NavigationSubmitEvent = evt;
            triggerEvent(data);
        }
    }
}
#endif