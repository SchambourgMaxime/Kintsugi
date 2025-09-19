#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Kamgam.UIToolkitVisualScripting;
using UnityEngine.SceneManagement;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    [UnitTitle("UI Document Query One")]
    [UnitCategory("UI Toolkit")]
    [TypeIcon(typeof(UIDocument))]
    public class UIDocumentQueryOne: QueryOneBase<UIDocument>
    {
        /// <summary>
        /// If no "document" has been specified as source and if not UI Document
        /// component is found on the Script Machine then it will try to fetch the
        /// document from local and scene variables (current active scene) based
        /// on this name.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorExpandTooltip]
        public string documentVarName { get; set; } = "document";

        protected override string getSourceName()
        {
            return "Document";
        }

        protected override UIDocument getQueryRoot(Flow flow, ValueInput input)
        {
            return UIDocumentUtils.GetDocument(flow, input, documentVarName);
        }

        protected override VisualElement executeQuery(UIDocument queryRoot, UIElementType type, string name, string className)
        {
            if (type != UIElementType.VisualElement)
            {
                return queryRoot.QueryType(type, name, className);
            }
            else
            {
                return queryRoot.rootVisualElement.Q(name, className);
            }
        }
    }
}
#endif
