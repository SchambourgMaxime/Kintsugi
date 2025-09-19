#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Get Event Target (UITK)")]
    [TypeIcon(typeof(UIDocument))]
    public class GetEventTarget : Unit
    {
        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput target;

        [DoNotSerialize]
        public EventBase evt;

        protected override void Definition()
        {
            eventIn = ValueInput<EventBase>("Event");
            target = ValueOutput<VisualElement>(nameof(target), extractTarget);
        }

        protected VisualElement extractTarget(Flow flow)
        {
            var evt = flow.GetValue<EventBase>(eventIn);
            var ve = evt.target as VisualElement;
            return ve;
        }
    }
}
#endif