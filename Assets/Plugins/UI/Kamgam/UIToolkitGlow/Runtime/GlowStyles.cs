using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public static class GlowStyles
    {
        public static CustomStyleProperty<float> Width = new CustomStyleProperty<float>("--glow-width");
        public static CustomStyleProperty<float> OverlapWidth = new CustomStyleProperty<float>("--glow-overlap-width");
        public static CustomStyleProperty<bool> SplitWidth = new CustomStyleProperty<bool>("--glow-split-width");
        public static CustomStyleProperty<float> WidthLeft = new CustomStyleProperty<float>("--glow-width-left");
        public static CustomStyleProperty<float> WidthTop = new CustomStyleProperty<float>("--glow-width-top");
        public static CustomStyleProperty<float> WidthRight = new CustomStyleProperty<float>("--glow-width-right");
        public static CustomStyleProperty<float> WidthBottom = new CustomStyleProperty<float>("--glow-width-bottom");
        public static CustomStyleProperty<float> OffsetX = new CustomStyleProperty<float>("--glow-offset-x");
        public static CustomStyleProperty<float> OffsetY = new CustomStyleProperty<float>("--glow-offset-y");
        public static CustomStyleProperty<bool> OffsetEverything = new CustomStyleProperty<bool>("--glow-offset-everything");
        public static CustomStyleProperty<float> ScaleX = new CustomStyleProperty<float>("--glow-scale-x");
        public static CustomStyleProperty<float> ScaleY = new CustomStyleProperty<float>("--glow-scale-y");
        public static CustomStyleProperty<Color> InnerColor = new CustomStyleProperty<Color>("--glow-inner-color");
        public static CustomStyleProperty<Color> OuterColor = new CustomStyleProperty<Color>("--glow-outer-color");
        public static CustomStyleProperty<bool> InheritBorderColors = new CustomStyleProperty<bool>("--glow-inherit-border-colors");
        public static CustomStyleProperty<bool> ForceSubdivision = new CustomStyleProperty<bool>("--glow-force-subdivision");
        public static CustomStyleProperty<bool> PreserveHardCorners = new CustomStyleProperty<bool>("--glow-preserve-hard-corners");
        public static CustomStyleProperty<bool> FillCenter = new CustomStyleProperty<bool>("--glow-fill-center");
        public static CustomStyleProperty<float> VertexDistance = new CustomStyleProperty<float>("--glow-vertex-distance");

        // TODO: Support animations. To do this the manipulator would need to know what animations it has connected.
        // public static CustomStyleProperty<string> AnimationName = new CustomStyleProperty<string>("--glow-animation-name");

        
        public static float ResolveStyle(CustomStyleProperty<float> property, VisualElement element, float defaultValue)
        {
            if (element.customStyle.TryGetValue(property, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static int ResolveStyle(CustomStyleProperty<int> property, VisualElement element, int defaultValue)
        {
            if (element.customStyle.TryGetValue(property, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static bool ResolveStyle(CustomStyleProperty<bool> property, VisualElement element, bool defaultValue)
        {
            if (element.customStyle.TryGetValue(property, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static Color ResolveStyle(CustomStyleProperty<Color> property, VisualElement element, Color defaultValue)
        {
            if (element.customStyle.TryGetValue(property, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static string ResolveStyle(CustomStyleProperty<string> property, VisualElement element, string defaultValue)
        {
            if (element.customStyle.TryGetValue(property, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}