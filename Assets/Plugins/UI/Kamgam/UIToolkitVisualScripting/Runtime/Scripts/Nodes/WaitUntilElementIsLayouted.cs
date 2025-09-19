#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    /// <summary>
    /// Delays flow by waiting until the document root node has valid width and height values.
    /// </summary>
    [IncludeInSettings(include: false)]
    [UnitTitle("Wait for Visual Element Layout")]
    [UnitShortTitle("Wait for Element Layout")]
    [UnitCategory("UI Toolkit")]
    [UnitOrder(2)]
    public class WaitUntilElementIsLayouted : WaitUnit
    {
        [DoNotSerialize]
        public ValueInput source;

        [DoNotSerialize]
        public ValueOutput target;

        protected override void Definition()
        {
            source = ValueInput<VisualElement>("Element");
            target = ValueOutput<VisualElement>("Element", (flow) => { return flow.GetValue<VisualElement>(source); });

            base.Definition();
        }

        protected override IEnumerator Await(Flow flow)
        {
            yield return new WaitUntil(() => hasValidDimensions(flow));

            yield return exit;
        }

        protected bool hasValidDimensions(Flow flow)
        {
            var ve = flow.GetValue<VisualElement>(source);
            return ve != null && !float.IsNaN(ve.contentRect.width);
        }
    }
}
#endif