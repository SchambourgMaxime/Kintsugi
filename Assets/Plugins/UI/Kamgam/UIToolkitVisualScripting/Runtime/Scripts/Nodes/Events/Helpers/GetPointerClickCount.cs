#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitCategory("UI Toolkit/Event Helpers")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("Get Pointer Click Count (UITK)")]
    [UnitShortTitle("Get Clicks (UITK)")]
    [TypeIcon(typeof(int))]
    public class GetPointerClickCount : Unit
    {
        [DoNotSerialize]
        public ValueInput eventIn;

        [DoNotSerialize]
        public ValueOutput clicks;

        [DoNotSerialize]
        public IPointerEvent evt;

        protected override void Definition()
        {
            eventIn = ValueInput<IPointerEvent>("Pointer Event");
            clicks = ValueOutput<int>(nameof(clicks), extractClickCount);
        }

        protected int extractClickCount(Flow flow)
        {
            var evt = flow.GetValue<IPointerEvent>(eventIn);
            return evt.clickCount;
        }
    }
}
#endif