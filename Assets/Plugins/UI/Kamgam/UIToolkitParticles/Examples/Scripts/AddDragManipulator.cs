using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public class AddDragManipulator : MonoBehaviour
    {
        public UIDocument Document;
        public string ElementName;

        void Start()
        {
            var element = Document.rootVisualElement.Q(name: ElementName);
            if (element != null)
            {
                var manipulator = new DragManipulator();
                element.AddManipulator(manipulator);
            }
        }
    }
}
