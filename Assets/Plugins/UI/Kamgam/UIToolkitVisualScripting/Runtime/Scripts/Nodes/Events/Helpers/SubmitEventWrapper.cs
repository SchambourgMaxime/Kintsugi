#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    /// <summary>
    /// A wrapper for click and submit events. Either the ClickEvent or the SubmitEvent is set.
    /// </summary>
    public class SubmitEventWrapper : EventBase<SubmitEventWrapper>
    {
        public EventBase EventBase
        {
            get
            {
                return ClickEvent != null ? ClickEvent : NavigationSubmitEvent;
            }
        }

        public ClickEvent ClickEvent;
        public NavigationSubmitEvent NavigationSubmitEvent;

        public SubmitEventWrapper()
        {
            ClickEvent = null;
            NavigationSubmitEvent = null;
        }

        public new IEventHandler target
        {
            get
            {
                if (ClickEvent == null && NavigationSubmitEvent == null)
                    return null;

                return ClickEvent != null ? ClickEvent.target : NavigationSubmitEvent.target;
            }
        }

        public VisualElement targetElement => target as VisualElement;

        public new IEventHandler currentTarget
        {
            get
            {
                if (ClickEvent == null && NavigationSubmitEvent == null)
                    return null;

                return ClickEvent != null ? ClickEvent.currentTarget : NavigationSubmitEvent.currentTarget;
            }
        }
    }
}
#endif