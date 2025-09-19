#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Get Pointer Event Keys (UITK)")]
    [UnitShortTitle("Get Pointer Keys (UITK)")]
    [TypeIcon(typeof(KeyCode))]
    public class GetPointerEventKeys : Unit
    {
        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput shift;

        [DoNotSerialize]
        public ValueOutput control;

        [DoNotSerialize]
        public ValueOutput alt;

        [DoNotSerialize]
        public ValueOutput command;

        [DoNotSerialize]
        public ValueOutput action;

        [DoNotSerialize]
        public IPointerEvent evt;

        protected override void Definition()
        {
            eventIn = ValueInput<IPointerEvent>("Pointer Event");
            shift = ValueOutput<bool>(nameof(shift), extractShift);
            control = ValueOutput<bool>(nameof(control), extractControl);
            alt = ValueOutput<bool>(nameof(alt), extractAlt);
            command = ValueOutput<bool>(nameof(command), extractCommand);
            action = ValueOutput<bool>(nameof(action), extractAction);
        }

        protected bool extractShift(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.shiftKey;
        }

        protected bool extractControl(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.ctrlKey;
        }

        protected bool extractAlt(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.altKey;
        }

        protected bool extractCommand(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.commandKey;
        }

        protected bool extractAction(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.actionKey;
        }
    }
}
#endif