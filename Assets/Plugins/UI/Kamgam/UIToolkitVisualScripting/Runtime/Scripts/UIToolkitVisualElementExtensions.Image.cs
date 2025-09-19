#if KAMGAM_VISUAL_SCRIPTING
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting
{
    /// <summary>
    /// A collection of common actions performed on visual elements.
    /// Of possible the return type is VisualElement to support easy chaining of nodes.
    /// </summary>
    public static partial class UIToolkitVisualElementExtensions
    {
        // I M A G E

        public static Image GetImage(this VisualElement element)
        {
            return element as Image;
        }


        public static Sprite GetSprite(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return null;

            return image.sprite;
        }

        public static Sprite GetSprite(this Image image)
        {
            if (image == null)
                return null;

            return image.sprite;
        }


        public static Image SetSprite(this VisualElement element, Sprite sprite)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.sprite = sprite;

            return image;
        }

        public static Image SetSprite(this Image image, Sprite sprite)
        {
            if (image == null)
                return image;

            image.sprite = sprite;

            return image;
        }


        public static VectorImage GetVectorImage(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return null;

            return image.vectorImage;
        }

        public static VectorImage GetVectorImage(this Image image)
        {
            if (image == null)
                return null;

            return image.vectorImage;
        }


        public static Image SetVectorImage(this VisualElement element, VectorImage vectorImage)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.vectorImage = vectorImage;

            return image;
        }

        public static Image SetVectorImage(this Image image, VectorImage vectorImage)
        {
            if (image == null)
                return image;

            image.vectorImage = vectorImage;

            return image;
        }


        public static Rect GetSourceRect(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return default;

            return image.sourceRect;
        }

        public static Rect GetSourceRect(this Image image)
        {
            if (image == null)
                return default;

            return image.sourceRect;
        }


        public static Image SetSourceRect(this VisualElement element, Rect sourceRect)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.sourceRect = sourceRect;

            return image;
        }

        public static Image SetSourceRect(this Image image, Rect sourceRect)
        {
            if (image == null)
                return image;

            image.sourceRect = sourceRect;

            return image;
        }

        public static Rect GetUV(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return default;

            return image.uv;
        }

        public static Rect GetUV(this Image image)
        {
            if (image == null)
                return default;

            return image.uv;
        }


        public static Image SetUV(this VisualElement element, Rect uv)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.uv = uv;

            return image;
        }

        public static Image SetUV(this Image image, Rect uv)
        {
            if (image == null)
                return image;

            image.uv = uv;

            return image;
        }

        public static ScaleMode GetScaleMode(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return default;

            return image.scaleMode;
        }

        public static ScaleMode GetScaleMode(this Image image)
        {
            if (image == null)
                return default;

            return image.scaleMode;
        }


        public static Image SetScaleMode(this VisualElement element, ScaleMode scaleMode)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.scaleMode = scaleMode;

            return image;
        }

        public static Image SetScaleMode(this Image image, ScaleMode scaleMode)
        {
            if (image == null)
                return image;

            image.scaleMode = scaleMode;

            return image;
        }


        public static Color GetTintColor(this VisualElement element)
        {
            var image = element as Image;
            if (image == null)
                return default;

            return image.tintColor;
        }

        public static Color GetTintColor(this Image image)
        {
            if (image == null)
                return default;

            return image.tintColor;
        }


        public static Image SetTintColor(this VisualElement element, Color tintColor)
        {
            var image = element as Image;
            if (image == null)
                return image;

            image.tintColor = tintColor;

            return image;
        }

        public static Image SetTintColor(this Image image, Color tintColor)
        {
            if (image == null)
                return image;

            image.tintColor = tintColor;

            return image;
        }
    }
}
#endif