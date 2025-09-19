#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Kamgam.UIToolkitVisualScripting;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitTitle("VisualElement Query Many")]
    [UnitCategory("UI Toolkit")]
    [TypeIcon(typeof(UIDocument))]
    public class VisualElementQueryMany : QueryManyBase<VisualElement>
    {
        protected override string getSourceName()
        {
            return "Visual Element";
        }

        protected override VisualElement getQueryRoot(Flow flow, ValueInput input)
        {
            return flow.GetValue<VisualElement>(source);
        }

        protected override List<VisualElement> executeQuery(VisualElement queryRoot, UIElementType type, string name, string className)
        {
            if (type != UIElementType.VisualElement)
            {
                return queryRoot.QueryTypes(type, name, className);
            }
            else
            {
                return queryRoot.Query(name, className).ToList();
            }
        }
    }
}
#endif
