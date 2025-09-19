#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Get Pointer Event Pos (UITK)")]
    [UnitShortTitle("Get Pointer Pos (UITK)")]
    [TypeIcon(typeof(Vector2))]
    public class GetPointerEventPos : Unit
    {
        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput pos;

        [DoNotSerialize]
        public IPointerEvent evt;

        protected override void Definition()
        {
            eventIn = ValueInput<IPointerEvent>("Pointer Event");
            pos = ValueOutput<Vector2>("Position", extractTarget);
        }

        protected Vector2 extractTarget(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.position;
        }
    }
}
#endif