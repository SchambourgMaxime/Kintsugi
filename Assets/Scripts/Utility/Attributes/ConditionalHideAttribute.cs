using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VV.Utility
{
    /// <summary>
    /// Attribute to conditionally hide fields in the inspector based on a boolean field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string ConditionalSourceField { get; private set; }
        public bool HideWhenTrue { get; private set; }
        
        /// <summary>
        /// Hide field when the conditional source field is true/false
        /// </summary>
        /// <param name="conditionalSourceField">Name of the boolean field to check</param>
        /// <param name="hideWhenTrue">If true, hide when source field is true. If false, hide when source field is false</param>
        public ConditionalHideAttribute(string conditionalSourceField, bool hideWhenTrue = true)
        {
            ConditionalSourceField = conditionalSourceField;
            HideWhenTrue = hideWhenTrue;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHideAttribute = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHideAttribute, property);

            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHideAttribute = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHideAttribute, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHideAttribute, SerializedProperty property)
        {
            SerializedProperty sourcePropertyValue = null;
            
            // Get the parent object
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHideAttribute.ConditionalSourceField);
            sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                bool conditionValue = sourcePropertyValue.boolValue;
                return condHideAttribute.HideWhenTrue ? !conditionValue : conditionValue;
            }
            else
            {
                Debug.LogWarning($"ConditionalHide: Unable to find property '{condHideAttribute.ConditionalSourceField}' for field '{property.name}'");
                return true;
            }
        }
    }
#endif
}