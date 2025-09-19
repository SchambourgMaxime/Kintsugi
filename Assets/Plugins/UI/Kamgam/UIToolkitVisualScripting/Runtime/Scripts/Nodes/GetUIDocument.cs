#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitTitle("Get UI Document")]
    [UnitCategory("UI Toolkit")]
    [TypeIcon(typeof(UIDocument))]
    public class GetUIDocument : Unit
    {
        /// <summary>
        /// Disable if you want the query to be executed every time.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorExpandTooltip]
        public bool cacheResult { get; set; } = true;

        protected bool _cacheIsValid;
        protected UIDocument _cachedResult;

        [DoNotSerialize]
        public ControlInput clearCacheTrigger;

        [DoNotSerialize]
        public ControlOutput cleared;

        [DoNotSerialize]
        public ValueInput variableName;

        [DoNotSerialize]
        public ValueOutput document { get; private set; }

        protected override void Definition()
        {
            _cacheIsValid = false;

            variableName = ValueInput<string>(nameof(variableName), "document");

            document = ValueOutput<UIDocument>(nameof(document), (flow) => { return getDocument(flow); });

            cleared = ControlOutput(nameof(cleared));
            clearCacheTrigger = ControlInput(nameof(clearCacheTrigger), (flow) => { ClearCache(); return cleared; });

            Succession(clearCacheTrigger, cleared);
        }

        protected UIDocument getDocument(Flow flow)
        {
            if (cacheResult && _cacheIsValid)
            {
                return _cachedResult;
            }

            _cachedResult = UIDocumentUtils.GetDocument(flow, null, flow.GetValue<string>(variableName));
            _cacheIsValid = true;

            return _cachedResult;
        }

        public virtual void ClearCache()
        {
            _cachedResult = null;
            _cacheIsValid = false;
        }
    }
}
#endif
