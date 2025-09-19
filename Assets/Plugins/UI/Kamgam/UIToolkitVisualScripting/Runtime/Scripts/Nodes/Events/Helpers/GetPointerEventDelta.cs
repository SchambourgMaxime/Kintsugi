#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Get Pointer Event Delta (UITK)")]
    [UnitShortTitle("Get Pointer Delta (UITK)")]
    [TypeIcon(typeof(Vector2))]
    public class GetPointerEventDelta : Unit
    {
        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput deltaPos;

        [DoNotSerialize]
        public ValueOutput deltaTime;

        [DoNotSerialize]
        public IPointerEvent evt;

        protected override void Definition()
        {
            eventIn = ValueInput<IPointerEvent>("Pointer Event");
            deltaPos = ValueOutput<Vector2>(nameof(deltaPos), extractDeltaPos);
            deltaTime = ValueOutput<float>(nameof(deltaTime), extractDeltaTime);
        }

        protected Vector2 extractDeltaPos(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.deltaPosition;
        }

        protected float extractDeltaTime(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.deltaTime;
        }
    }
}
#endif