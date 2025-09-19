using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public enum UIElementType
    {
        VisualElement = 0,
        BindableElement = 1,
        Button = 2,
        Image = 3,
        Label = 4,
        Scroller = 5,
        ScrollView = 6,
        TextField = 7,
        Foldout = 8,
        Slider = 9,
        SliderInt = 10,
        SliderMinMax = 11,
        Toggle = 12,
        ProgressBar = 13,
        DropdownField = 14,
        TextElement = 15
    }

    public static class UIElementTypeUtils
    {
        public static System.Type GetElementType(UIElementType type)
        {
            switch (type)
            {
                case UIElementType.VisualElement:
                    return typeof(VisualElement);

                case UIElementType.BindableElement:
                    return typeof(BindableElement);

                case UIElementType.TextElement:
                    return typeof(TextElement);

                case UIElementType.Button:
                    return typeof(Button);

                case UIElementType.Image:
                    return typeof(Image);

                case UIElementType.Label:
                    return typeof(Label);

                case UIElementType.Scroller:
                    return typeof(Scroller);

                case UIElementType.ScrollView:
                    return typeof(ScrollView);

                case UIElementType.TextField:
                    return typeof(TextField);

                case UIElementType.Foldout:
                    return typeof(Foldout);

                case UIElementType.Slider:
                    return typeof(Slider);

                case UIElementType.SliderInt:
                    return typeof(SliderInt);

                case UIElementType.SliderMinMax:
                    return typeof(MinMaxSlider);

                case UIElementType.Toggle:
                    return typeof(Toggle);

                case UIElementType.ProgressBar:
                    return typeof(ProgressBar);

                case UIElementType.DropdownField:
                    return typeof(DropdownField);

                default:
                    return null;
            }
        }

        public static UIElementType GetElementType(this VisualElement element)
        {
            if (element is Button)
                return UIElementType.Button;

            if (element is Image)
                return UIElementType.Image;

            if (element is Label)
                return UIElementType.Label;

            if (element is Scroller)
                return UIElementType.Scroller;

            if (element is ScrollView)
                return UIElementType.ScrollView;

            if (element is TextField)
                return UIElementType.TextField;

            if (element is Foldout)
                return UIElementType.Foldout;

            if (element is Slider)
                return UIElementType.Slider;

            if (element is SliderInt)
                return UIElementType.SliderInt;

            if (element is MinMaxSlider)
                return UIElementType.SliderMinMax;

            if (element is Toggle)
                return UIElementType.Toggle;

            if (element is ProgressBar)
                return UIElementType.ProgressBar;

            if (element is DropdownField)
                return UIElementType.DropdownField;


            // Base Types

            if (element is TextElement)
                return UIElementType.TextElement;

            if (element is BindableElement)
                return UIElementType.BindableElement;

            return UIElementType.VisualElement;
        }
    }
}