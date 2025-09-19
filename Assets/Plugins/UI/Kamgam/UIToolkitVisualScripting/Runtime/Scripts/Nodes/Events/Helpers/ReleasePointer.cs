#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Release Pointer (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.Navigation))]
    public class ReleasePointer : Unit
    {
        [DoNotSerialize]
        public ControlInput enter;

        [DoNotSerialize]
        public ControlOutput exit;

        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput eventOut;

        [DoNotSerialize]
        public IPointerEvent evt;

        protected override void Definition()
        {
            exit = ControlOutput(nameof(exit));
            enter = ControlInput(nameof(enter), (flow) => exit);

            eventIn = ValueInput<IPointerEvent>("Pointer Event");
            eventOut = ValueOutput<IPointerEvent>(nameof(eventOut), extractEvent);
        }

        protected IPointerEvent extractEvent(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);

            var capture = evt as EventBase;
            if (capture != null)
            {
                capture.target.ReleasePointer(evt.pointerId);
            }

            return evt;
        }
    }
}
#endif