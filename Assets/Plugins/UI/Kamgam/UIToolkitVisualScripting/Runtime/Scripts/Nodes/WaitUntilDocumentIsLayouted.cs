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
    [UnitTitle("Wait for UI Document Layout")]
    [UnitShortTitle("Wait for UI Doc Layout")]
    [UnitCategory("UI Toolkit")]
    [UnitOrder(2)]
    public class WaitUntilDocumentIsLayouted : WaitUnit
    {
        [DoNotSerialize]
        public ValueInput source;

        [DoNotSerialize]
        public ValueOutput target;

        protected override void Definition()
        {
            source = ValueInput<UIDocument>("Document");
            target = ValueOutput<UIDocument>("Out", (flow) => { return flow.GetValue<UIDocument>(source); });

            base.Definition();
        }

        protected override IEnumerator Await(Flow flow)
        {
            yield return new WaitUntil(() => hasValidDimensions(flow));

            yield return exit;
        }

        protected bool hasValidDimensions(Flow flow)
        {
            var doc = flow.GetValue<UIDocument>(source);
            return doc != null && !float.IsNaN(doc.rootVisualElement.contentRect.width);
        }
    }
}
#endif