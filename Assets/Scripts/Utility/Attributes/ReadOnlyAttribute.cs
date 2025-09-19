using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace VV.Utility
{
    public class ReadOnlyAttribute : PropertyAttribute { }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var propertyField = new PropertyField(property);
            propertyField.SetEnabled(false);
            root.Add(propertyField);
            return root;
        }
    }
#endif
}