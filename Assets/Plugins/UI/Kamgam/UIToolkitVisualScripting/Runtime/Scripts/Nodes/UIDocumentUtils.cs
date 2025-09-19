#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public class UIDocumentUtils 
    {
        public static UIDocument GetDocument(Flow flow, ValueInput source, string documentVarName)
        {
            UIDocument doc = null;

            if (source != null)
            {
                doc = flow.GetValue<UIDocument>(source);
            }

            // 1st try to fetch the source from the game object
            if (doc == null && flow.stack.gameObject != null)
            {
                doc = flow.stack.gameObject.GetComponent<UIDocument>();
            }

            // 2nd try to fetch the source from the game object
            if (doc == null && !string.IsNullOrEmpty(documentVarName) && flow.stack.gameObject != null)
            {
                var localVars = flow.stack.gameObject.GetComponent<Variables>();
                if (localVars != null && localVars.declarations != null)
                {
                    doc = localVars.declarations.Get<UIDocument>(documentVarName);
                }
            }

            // 3rd try to fetch the source from the scene variables
            if (doc == null && !string.IsNullOrEmpty(documentVarName) && flow.stack.gameObject != null)
            {
                var sceneVars = SceneVariables.Instance(flow.stack.gameObject.scene);
                if(sceneVars != null && sceneVars.variables != null && sceneVars.variables.declarations != null)
                {
                    doc = sceneVars.variables.declarations.Get<UIDocument>(documentVarName);
                }
            }

            return doc;
        }
    }
}
#endif
