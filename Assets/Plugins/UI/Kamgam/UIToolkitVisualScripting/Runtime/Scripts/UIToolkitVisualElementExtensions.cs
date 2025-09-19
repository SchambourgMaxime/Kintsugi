#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Kamgam.UIToolkitVisualScripting
{
    /// <summary>
    /// A collection of common actions performed on visual elements.
    /// Of possible the return type is VisualElement to support easy chaining of nodes.
    /// </summary>
    [IncludeInSettings(include: true)]
    public static partial class UIToolkitVisualElementExtensions
    {
        // V I S U A L   E L E M E N T

        public static bool GetEnabled(this VisualElement element) => element.enabledSelf;

        public static VisualElement SetEnabled(this VisualElement element, bool enable)
        {
            element.SetEnabled(enable);
            return element;
        }

        public static string GetName(this VisualElement element) => element.name;

        public static VisualElement SetName(this VisualElement element, string name)
        {
            element.name = name;
            return element;
        }

        public static PickingMode GetPickingMode(this VisualElement element) => element.pickingMode;

        public static VisualElement SetPickingMode(this VisualElement element, PickingMode pickingMode)
        {
            element.pickingMode = pickingMode;
            return element;
        }

        public static string GetViewDataKey(this VisualElement element) => element.viewDataKey;

        public static VisualElement SetViewDataKey(this VisualElement element, string viewDataKey)
        {
            element.viewDataKey = viewDataKey;
            return element;
        }

        public static object GetUserData(this VisualElement element) => element.userData;

        public static VisualElement SetUserData(this VisualElement element, object userData)
        {
            element.userData = userData;
            return element;
        }

        public static int GetTabIndex(this VisualElement element) => element.tabIndex;

        public static VisualElement SetTabIndex(this VisualElement element, int tabIndex)
        {
            element.tabIndex = tabIndex;
            return element;
        }

        public static Rect GetLayout(this VisualElement element) => element.layout;

        public static VisualElement GetContentContainer(this VisualElement element) => element.contentContainer;

        public static string GetTooltip(this VisualElement element) => element.tooltip;

        public static VisualElement SetTooltip(this VisualElement element, string tooltip)
        {
            element.tooltip = tooltip;
            return element;
        }

        public static int GetChildCount(this VisualElement element) => element.childCount;

        public static VisualElement GetChildAt(this VisualElement element, int index) => element.ElementAt(index);
        public static VisualElement ChildAt(this VisualElement element, int index) => element.ElementAt(index);

        public static int GetIndexOf(this VisualElement element, VisualElement child) => element.IndexOf(child);

        public static int GetIndex(this VisualElement element)
        {
            if (element.parent == null)
                return 0;

            return element.parent.GetIndexOf(element);
        }


        public static VisualElement GetParent(this VisualElement element) => element.parent;

        public static IEnumerable<VisualElement> GetChildren(this VisualElement element) => element.Children();

        public static IPanel GetPanel(this VisualElement element) => element.panel;

        public static VisualElement AddChild(this VisualElement element, VisualElement child)
        {
            element.Add(child);
            return element;
        }

        public static bool ContainsChild(this VisualElement element, VisualElement child) => element.Contains(child);

        public static VisualElement Clear(this VisualElement element)
        {
            element.Clear();
            return element;
        }

        public static VisualElement InsertAt(this VisualElement element, int index, VisualElement child)
        {
            element.Insert(index, child);
            return element;
        }

        public static VisualElement RemoveChildAt(this VisualElement element, int index)
        {
            element.RemoveAt(index);
            return element;
        }

        public static VisualElement RemoveChild(this VisualElement element, VisualElement child)
        {
            element.Remove(child);
            return element;
        }

        public static VisualElement SortChildren(this VisualElement element, Comparison<VisualElement> comp)
        {
            element.Sort(comp);
            return element;
        }

        public static VisualElement MakeFirst(this VisualElement element)
        {
            element.SendToBack();
            return element;
        }

        public static VisualElement MoveToBack(this VisualElement element)
        {
            element.SendToBack();
            return element;
        }

        public static VisualElement MakeLast(this VisualElement element)
        {
            element.BringToFront();
            return element;
        }

        public static VisualElement MoveToFront(this VisualElement element)
        {
            element.BringToFront();
            return element;
        }

        public static VisualElement MoveBehind(this VisualElement element, VisualElement sibling)
        {
            element.PlaceBehind(sibling);
            return element;
        }

        public static VisualElement MoveInFront(this VisualElement element, VisualElement sibling)
        {
            element.PlaceInFront(sibling);
            return element;
        }

        public static VisualElement MoveUp(this VisualElement element)
        {
            if (element.parent == null)
                return element;

            int index = element.parent.IndexOf(element);
            if (index > 0 && index < element.parent.childCount)
            {
                var sibling = element.parent.ElementAt(index - 1);
                element.PlaceBehind(sibling);
            }

            return element;
        }

        public static VisualElement MoveDown(this VisualElement element)
        {
            if (element.parent == null)
                return element;

            int index = element.parent.IndexOf(element);
            if (index >= 0 && index < element.parent.childCount - 1)
            {
                var sibling = element.parent.ElementAt(index + 1);
                element.PlaceInFront(sibling);
            }

            return element;
        }

        public static VisualElement GetSibling(this VisualElement element, int indexDelta)
        {
            if (element.parent == null)
                return null;

            int index = element.parent.IndexOf(element) + indexDelta;
            if (index >= 0 && index < element.parent.childCount)
            {
                var sibling = element.parent.ElementAt(index);
                return sibling;
            }

            return null;
        }

        public static VisualElement SetClassEnabled(this VisualElement element, string className, bool enable)
        {
            element.EnableInClassList(className, enable);
            return element;
        }

        public static VisualElement ToggleClass(this VisualElement element, string className)
        {
            element.ToggleInClassList(className);
            return element;
        }

        public static VisualElement AddClass(this VisualElement element, string className)
        {
            element.AddToClassList(className);
            return element;
        }

        public static VisualElement RemoveClass(this VisualElement element, string className)
        {
            element.RemoveFromClassList(className);
            return element;
        }

        public static bool HasClass(this VisualElement element, string className) => element.ClassListContains(className);

        public static VisualElement ClearClasses(this VisualElement element)
        {
            element.ClearClassList();
            return element;
        }

        public static IEnumerable<string> GetAllClasses(this VisualElement element) => element.GetClasses();


        static List<string> _tmpClassNamesList = new List<string>();
        public static List<string> GetAllClassesAsTemporaryList(this VisualElement element, List<string> resultList = null)
        {
            var results = resultList != null ? resultList : _tmpClassNamesList;

            results.Clear();

            var classes = element.GetClasses();
            foreach (var className in classes)
            {
                results.Add(className);
            }

            return results;
        }

        public static bool ContainsLocalPoint(this VisualElement element, Vector2 localPoint) => element.ContainsPoint(localPoint);

        public static bool DoesOverlap(this VisualElement element, Rect rectangle) => element.Overlaps(rectangle);

        public static VisualElement SetFocus(this VisualElement element)
        {
            element.Focus();
            return element;
        }
        public static VisualElement Repaint(this VisualElement element)
        {
            element.MarkDirtyRepaint();
            return element;
        }

        public static VisualElement TriggerEvent(this VisualElement element, EventBase e)
        {
            element.SendEvent(e);
            return element;
        }



        // S T Y L E S

        public static bool HasValue<T>(this StyleEnum<T> style) where T : struct, IConvertible
        {
            return style == StyleKeyword.Null;
        }

        public static bool HasValue(this StyleLength style)
        {
            return style == StyleKeyword.Null;
        }

        public static bool HasValue(this StyleFloat style)
        {
            return style == StyleKeyword.Null;
        }

        public static bool GetVisible(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.visibility == Visibility.Visible;
            }
            else
            {
                return element.style.visibility.value == Visibility.Visible;
            }
        }

        public static VisualElement SetVisible(this VisualElement element, bool visible)
        {
            element.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
            return element;
        }

        public static VisualElement ResetVisibility(this VisualElement element)
        {
            element.style.visibility = StyleKeyword.Null;
            return element;
        }

        public static DisplayStyle GetDisplay(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.display;
            }
            else
            {
                return element.style.display.value;
            }
        }
        public static VisualElement SetDisplay(this VisualElement element, DisplayStyle display)
        {
            element.style.display = display;
            return element;
        }

        public static VisualElement ResetDisplay(this VisualElement element)
        {
            element.style.display = StyleKeyword.Null;
            return element;
        }

        public static float GetOpacity(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.opacity;
            }
            else
            {
                return element.style.opacity.value;
            }
        }

        /// <summary>
        /// Sets the opactiy of the object.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="opacity">Range is from 0.0f to 1.0f</param>
        /// <returns></returns>
        public static VisualElement SetOpacity(this VisualElement element, float opacity)
        {
            element.style.opacity = opacity;
            return element;
        }

        public static VisualElement ResetOpacity(this VisualElement element)
        {
            element.style.opacity = StyleKeyword.Null;
            return element;
        }

        public static Overflow GetOverflow(this VisualElement element)
        {
            // Seems like element.resolvedStyle.overflow; is not a thing. TODO: Investigate.
            return element.style.overflow.value;
        }

        public static VisualElement SetOverflow(this VisualElement element, Overflow overflow)
        {
            element.style.overflow = overflow;
            return element;
        }

        public static VisualElement ResetOverflow(this VisualElement element)
        {
            element.style.overflow = StyleKeyword.Null;
            return element;
        }

        public static FlexDirection GetFlexDirection(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.flexDirection;
            }
            else
            {
                return element.style.flexDirection.value;
            }
        }

        public static VisualElement SetFlexDirection(this VisualElement element, FlexDirection flexDirection)
        {
            element.style.flexDirection = flexDirection;
            return element;
        }

        public static VisualElement ResetFlexDirection(this VisualElement element)
        {
            element.style.flexDirection = StyleKeyword.Null;
            return element;
        }

        public static float GetFlexGrow(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.flexGrow;
            }
            else
            {
                return element.style.flexGrow.value;
            }
        }

        public static VisualElement SetFlexGrow(this VisualElement element, float flexGrow)
        {
            element.style.flexGrow = flexGrow;
            return element;
        }

        public static VisualElement ResetFlexGrow(this VisualElement element)
        {
            element.style.flexGrow = StyleKeyword.Null;
            return element;
        }

        public static float GetFlexShrink(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.flexShrink;
            }
            else
            {
                return element.style.flexShrink.value;
            }
        }

        public static VisualElement SetFlexShrink(this VisualElement element, float flexShrink)
        {
            element.style.flexShrink = flexShrink;
            return element;
        }

        public static VisualElement ResetFlexShrink(this VisualElement element)
        {
            element.style.flexShrink = StyleKeyword.Null;
            return element;
        }

        public static Wrap GetFlexWrap(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.flexWrap;
            }
            else
            {
                return element.style.flexWrap.value;
            }
        }

        public static VisualElement SetFlexWrap(this VisualElement element, Wrap flexWrap)
        {
            element.style.flexWrap = flexWrap;
            return element;
        }

        public static VisualElement ResetFlexWrap(this VisualElement element)
        {
            element.style.flexWrap = StyleKeyword.Null;
            return element;
        }

        public static Align GetAlignContent(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.alignContent;
            }
            else
            {
                return element.style.alignContent.value;
            }
        }

        public static VisualElement SetAlignContent(this VisualElement element, Align alignContent)
        {
            element.style.alignContent = alignContent;
            return element;
        }

        public static VisualElement ResetAlignContent(this VisualElement element)
        {
            element.style.alignContent = StyleKeyword.Null;
            return element;
        }

        public static Align GetAlignItems(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.alignItems;
            }
            else
            {
                return element.style.alignItems.value;
            }
        }

        public static VisualElement SetAlignItems(this VisualElement element, Align alignItems)
        {
            element.style.alignItems = alignItems;
            return element;
        }

        public static VisualElement ResetAlignItems(this VisualElement element)
        {
            element.style.alignItems = StyleKeyword.Null;
            return element;
        }

        public static Align GetAlignSelf(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.alignSelf;
            }
            else
            {
                return element.style.alignSelf.value;
            }
        }

        public static VisualElement SetAlignSelf(this VisualElement element, Align alignSelf)
        {
            element.style.alignSelf = alignSelf;
            return element;
        }

        public static VisualElement ResetAlignSelf(this VisualElement element)
        {
            element.style.alignSelf = StyleKeyword.Null;
            return element;
        }

        public static TextAnchor GetAlignText(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityTextAlign;
            }
            else
            {
                return element.style.unityTextAlign.value;
            }
        }
        public static VisualElement SetAlignText(this VisualElement element, TextAnchor unityTextAlign)
        {
            element.style.unityTextAlign = unityTextAlign;
            return element;
        }

        public static VisualElement ResetAlignText(this VisualElement element)
        {
            element.style.unityTextAlign = StyleKeyword.Null;
            return element;
        }

        public static TextOverflow GetTextOverflow(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.textOverflow;
            }
            else
            {
                return element.style.textOverflow.value;
            }
        }

        public static VisualElement SetTextOverflow(this VisualElement element, TextOverflow overflow)
        {
            element.style.textOverflow = overflow;
            return element;
        }

        public static VisualElement ResetTextOverflow(this VisualElement element)
        {
            element.style.textOverflow = StyleKeyword.Null;
            return element;
        }

        public static float GetFontSize(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.fontSize;
            }
            else
            {
                return element.style.fontSize.value.value;
            }
        }

        public static VisualElement SetFontSize(this VisualElement element, float size)
        {
            element.style.fontSize = size;
            return element;
        }

        public static VisualElement ResetFontSize(this VisualElement element)
        {
            element.style.fontSize = StyleKeyword.Null;
            return element;
        }

        public static float GetTextOutlineWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityTextOutlineWidth;
            }
            else
            {
                return element.style.unityTextOutlineWidth.value;
            }
        }

        public static VisualElement SetTextOutlineWidth(this VisualElement element, float width)
        {
            element.style.unityTextOutlineWidth = width;
            return element;
        }

        public static VisualElement ResetTextOutlineWidth(this VisualElement element)
        {
            element.style.unityTextOutlineWidth = StyleKeyword.Null;
            return element;
        }

        public static Color GetTextOutlineColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityTextOutlineColor;
            }
            else
            {
                return element.style.unityTextOutlineColor.value;
            }
        }

        public static VisualElement SetTextOutlineColor(this VisualElement element, Color color)
        {
            element.style.unityTextOutlineColor = color;
            return element;
        }

        public static VisualElement ResetTextOutlineColor(this VisualElement element)
        {
            element.style.unityTextOutlineColor = StyleKeyword.Null;
            return element;
        }

        public static FontStyle GetFontStyle(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityFontStyleAndWeight;
            }
            else
            {
                return element.style.unityFontStyleAndWeight.value;
            }
        }

        public static VisualElement SetFontStyle(this VisualElement element, FontStyle style)
        {
            element.style.unityFontStyleAndWeight = style;
            return element;
        }

        public static VisualElement ResetFontStyle(this VisualElement element)
        {
            element.style.unityFontStyleAndWeight = StyleKeyword.Null;
            return element;
        }

        public static StyleFont GetFont(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityFont;
            }
            else
            {
                return element.style.unityFont.value;
            }
        }

        public static VisualElement SetFont(this VisualElement element, StyleFont font)
        {
            element.style.unityFont = font;
            return element;
        }

        public static VisualElement ResetFont(this VisualElement element)
        {
            element.style.unityFont = StyleKeyword.Null;
            return element;
        }

        public static FontDefinition GetFontDefinition(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityFontDefinition;
            }
            else
            {
                return element.style.unityFontDefinition.value;
            }
        }

        public static VisualElement SetFontDefinition(this VisualElement element, FontDefinition definition)
        {
            element.style.unityFontDefinition = definition;
            return element;
        }

        public static VisualElement ResetFontDefinition(this VisualElement element)
        {
            element.style.unityFontDefinition = StyleKeyword.Null;
            return element;
        }

        public static Color GetColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.color;
            }
            else
            {
                return element.style.color.value;
            }
        }

        public static VisualElement SetColor(this VisualElement element, Color color)
        {
            element.style.color = color;
            return element;
        }

        public static VisualElement ResetColor(this VisualElement element)
        {
            element.style.color = StyleKeyword.Null;
            return element;
        }

        public static Color GetBackgroundColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.backgroundColor;
            }
            else
            {
                return element.style.backgroundColor.value;
            }
        }

        public static VisualElement SetBackgroundColor(this VisualElement element, Color color)
        {
            element.style.backgroundColor = color;
            return element;
        }

        public static VisualElement ResetBackgroundColor(this VisualElement element)
        {
            element.style.backgroundColor = StyleKeyword.Null;
            return element;
        }

        public static Background GetBackgroundImage(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.backgroundImage;
            }
            else
            {
                return element.style.backgroundImage.value;
            }
        }

        public static VisualElement SetBackgroundImage(this VisualElement element, Background image)
        {
            element.style.backgroundImage = image;
            return element;
        }

        public static VisualElement SetBackgroundImage(this VisualElement element, Sprite sprite)
        {
            var style = element.style.backgroundImage.value;
            style.sprite = sprite;
            element.style.backgroundImage = style;
            return element;
        }

        public static VisualElement SetBackgroundImage(this VisualElement element, VectorImage vectorImage)
        {
            var style = element.style.backgroundImage.value;
            style.vectorImage = vectorImage;
            element.style.backgroundImage = style;
            return element;
        }

        public static VisualElement SetBackgroundImage(this VisualElement element, Texture2D texture)
        {
            var style = element.style.backgroundImage.value;
            style.texture = texture;
            element.style.backgroundImage = style;
            return element;
        }

        public static VisualElement SetBackgroundImage(this VisualElement element, RenderTexture renderTexture)
        {
            var style = element.style.backgroundImage.value;
            style.renderTexture = renderTexture;
            element.style.backgroundImage = style;
            return element;
        }

        public static VisualElement ResetBackgroundImage(this VisualElement element)
        {
            element.style.backgroundImage = StyleKeyword.Null;
            return element;
        }

        public static Color GetBackgroundImageTint(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.unityBackgroundImageTintColor;
            }
            else
            {
                return element.style.unityBackgroundImageTintColor.value;
            }
        }

        public static VisualElement SetBackgroundImageTint(this VisualElement element, Color tintColor)
        {
            element.style.unityBackgroundImageTintColor = tintColor;
            return element;
        }

        public static VisualElement ResetBackgroundImageTint(this VisualElement element)
        {
            element.style.unityBackgroundImageTintColor = StyleKeyword.Null;
            return element;
        }

        public static Color GetBorderLeftColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderLeftColor;
            }
            else
            {
                return element.style.borderLeftColor.value;
            }
        }

        public static VisualElement ResetBorderLeftColor(this VisualElement element)
        {
            element.style.borderLeftColor = StyleKeyword.Null;
            return element;
        }

        public static Color GetBorderRightColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderRightColor;
            }
            else
            {
                return element.style.borderRightColor.value;
            }
        }

        public static VisualElement ResetBorderRightColor(this VisualElement element)
        {
            element.style.borderRightColor = StyleKeyword.Null;
            return element;
        }

        public static Color GetBorderTopColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderTopColor;
            }
            else
            {
                return element.style.borderTopColor.value;
            }
        }

        public static VisualElement ResetBorderTopColor(this VisualElement element)
        {
            element.style.borderTopColor = StyleKeyword.Null;
            return element;
        }

        public static Color GetBorderBottomColor(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderBottomColor;
            }
            else
            {
                return element.style.borderBottomColor.value;
            }
        }

        public static VisualElement ResetBorderBottomColor(this VisualElement element)
        {
            element.style.borderTopColor = StyleKeyword.Null;
            return element;
        }

        public static Color GetBorderColor(this VisualElement element) => element.style.borderLeftColor.value;

        public static VisualElement SetBorderColor(this VisualElement element, Color color)
        {
            element.style.borderLeftColor = color;
            element.style.borderTopColor = color;
            element.style.borderRightColor = color;
            element.style.borderBottomColor = color;
            return element;
        }

        public static VisualElement ResetBorderColor(this VisualElement element)
        {
            element.style.borderLeftColor = StyleKeyword.Null;
            element.style.borderTopColor = StyleKeyword.Null;
            element.style.borderRightColor = StyleKeyword.Null;
            element.style.borderBottomColor = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetBorderColor(this VisualElement element, Color? left = null, Color? top = null, Color? right = null, Color? bottom = null)
        {
            if (left.HasValue)
                element.style.borderLeftColor = left.Value;
            if (top.HasValue)
                element.style.borderTopColor = top.Value;
            if (right.HasValue)
                element.style.borderRightColor = right.Value;
            if (bottom.HasValue)
                element.style.borderBottomColor = bottom.Value;

            return element;
        }

        public static VisualElement SetBorderLeftColor(this VisualElement element, Color color)
        {
            element.style.borderLeftColor = color;
            return element;
        }

        public static VisualElement SetBorderTopColor(this VisualElement element, Color color)
        {
            element.style.borderTopColor = color;
            return element;
        }

        public static VisualElement SetBorderRightColor(this VisualElement element, Color color)
        {
            element.style.borderRightColor = color;
            return element;
        }

        public static VisualElement SetBorderBottomColor(this VisualElement element, Color color)
        {
            element.style.borderBottomColor = color;
            return element;
        }

        public static float GetBorderLeftWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderLeftWidth;
            }
            else
            {
                return element.style.borderLeftWidth.value;
            }
        }

        public static float GetBorderRightWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderRightWidth;
            }
            else
            {
                return element.style.borderRightWidth.value;
            }
        }

        public static float GetBorderTopWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderTopWidth;
            }
            else
            {
                return element.style.borderTopWidth.value;
            }
        }

        public static float GetBorderBottomWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderBottomWidth;
            }
            else
            {
                return element.style.borderBottomWidth.value;
            }
        }

        public static float GetBorderWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderLeftWidth;
            }
            else
            {
                return element.style.borderLeftWidth.value;
            }
        }

        public static VisualElement SetBorderWidth(this VisualElement element, float width, bool delta = false)
        {
            SetBorderLeftWidth(element, width, delta);
            SetBorderRightWidth(element, width, delta);
            SetBorderTopWidth(element, width, delta);
            SetBorderBottomWidth(element, width, delta);
            return element;
        }

        public static VisualElement ResetBorderWidth(this VisualElement element)
        {
            element.style.borderLeftWidth = StyleKeyword.Null;
            element.style.borderTopWidth = StyleKeyword.Null;
            element.style.borderRightWidth = StyleKeyword.Null;
            element.style.borderBottomWidth = StyleKeyword.Null;
            return element;
        }

        public static VisualElement ResetBorderLeftWidth(this VisualElement element)
        {
            element.style.borderLeftWidth = StyleKeyword.Null;
            return element;
        }

        public static VisualElement ResetBorderTopWidth(this VisualElement element)
        {
            element.style.borderTopWidth = StyleKeyword.Null;
            return element;
        }

        public static VisualElement ResetBorderRightWidth(this VisualElement element)
        {
            element.style.borderRightWidth = StyleKeyword.Null;
            return element;
        }

        public static VisualElement ResetBorderBottomWidth(this VisualElement element)
        {
            element.style.borderBottomWidth = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetBorderWidths(this VisualElement element, float? left = null, float? top = null, float? right = null, float? bottom = null, bool delta = false)
        {
            if (left.HasValue)
                SetBorderLeftWidth(element, left.Value, delta);
            if (right.HasValue)
                SetBorderRightWidth(element, right.Value, delta);
            if (top.HasValue)
                SetBorderTopWidth(element, top.Value, delta);
            if (bottom.HasValue)
                SetBorderBottomWidth(element, bottom.Value, delta);
            return element;
        }

        public static VisualElement SetBorderLeftWidth(this VisualElement element, float width, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderLeftWidth.HasValue() ? element.style.borderLeftWidth.value : element.resolvedStyle.borderLeftWidth;
                element.style.borderLeftWidth = value + width;
            }
            else
            {
                element.style.borderLeftWidth = width;
            }
            return element;
        }

        public static VisualElement SetBorderRightWidth(this VisualElement element, float width, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderRightWidth.HasValue() ? element.style.borderRightWidth.value : element.resolvedStyle.borderRightWidth;
                element.style.borderRightWidth = value + width;
            }
            else
            {
                element.style.borderRightWidth = width;
            }
            return element;
        }

        public static VisualElement SetBorderTopWidth(this VisualElement element, float width, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderTopWidth.HasValue() ? element.style.borderTopWidth.value : element.resolvedStyle.borderTopWidth;
                element.style.borderTopWidth = value + width;
            }
            else
            {
                element.style.borderTopWidth = width;
            }
            return element;
        }

        public static VisualElement SetBorderBottomWidth(this VisualElement element, float width, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderBottomWidth.HasValue() ? element.style.borderBottomWidth.value : element.resolvedStyle.borderBottomWidth;
                element.style.borderBottomWidth = value + width;
            }
            else
            {
                element.style.borderBottomWidth = width;
            }
            return element;
        }

        public static float GetBorderRadius(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderTopLeftRadius;
            }
            else
            {
                return element.style.borderTopLeftRadius.value.value;
            }
        }

        public static VisualElement SetBorderRadius(this VisualElement element, float radius, LengthUnit unit, bool delta = false)
        {
            SetBorderTopLeftRadius(element, radius, unit, delta);
            SetBorderTopRightRadius(element, radius, unit, delta);
            SetBorderBottomLeftRadius(element, radius, unit, delta);
            SetBorderBottomRightRadius(element, radius, unit, delta);
            return element;
        }

        public static VisualElement SetBorderRadius(this VisualElement element, float radius, bool delta = false)
        {
            SetBorderTopLeftRadius(element, radius, delta);
            SetBorderTopRightRadius(element, radius, delta);
            SetBorderBottomLeftRadius(element, radius, delta);
            SetBorderBottomRightRadius(element, radius, delta);
            return element;
        }

        public static VisualElement ResetBorderRadius(this VisualElement element)
        {
            element.style.borderTopLeftRadius = StyleKeyword.Null;
            element.style.borderTopRightRadius = StyleKeyword.Null;
            element.style.borderBottomLeftRadius = StyleKeyword.Null;
            element.style.borderBottomRightRadius = StyleKeyword.Null;
            return element;
        }

        public static float GetBorderTopLeftRadius(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderTopLeftRadius;
            }
            else
            {
                return element.style.borderTopLeftRadius.value.value;
            }
        }

        public static float GetBorderTopRightRadius(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderTopRightRadius;
            }
            else
            {
                return element.style.borderTopRightRadius.value.value;
            }
        }

        public static float GetBorderBottomLeftRadius(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderBottomLeftRadius;
            }
            else
            {
                return element.style.borderBottomLeftRadius.value.value;
            }
        }

        public static float GetBorderBottomRightRadius(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.borderBottomRightRadius;
            }
            else
            {
                return element.style.borderBottomRightRadius.value.value;
            }
        }

        public static VisualElement SetBorderTopLeftRadius(this VisualElement element, float radius, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderTopLeftRadius.HasValue() ? element.style.borderTopLeftRadius.value.value : element.resolvedStyle.borderTopLeftRadius;
                element.style.borderTopLeftRadius = new Length(value + radius, unit);
            }
            else
            {
                element.style.borderTopLeftRadius = new Length(radius, unit);
            }

            return element;
        }

        public static VisualElement SetBorderTopLeftRadius(this VisualElement element, float radius, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderTopLeftRadius.HasValue() ? element.style.borderTopLeftRadius.value.value : element.resolvedStyle.borderTopLeftRadius;
                element.style.borderTopLeftRadius = value + radius;
            }
            else
            {
                element.style.borderTopLeftRadius = radius;
            }

            return element;
        }

        public static VisualElement ResetBorderTopLeftRadius(this VisualElement element)
        {
            element.style.borderTopLeftRadius = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetBorderTopRightRadius(this VisualElement element, float radius, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderTopRightRadius.HasValue() ? element.style.borderTopRightRadius.value.value : element.resolvedStyle.borderTopRightRadius;
                element.style.borderTopRightRadius = new Length(value + radius, unit);
            }
            else
            {
                element.style.borderTopRightRadius = new Length(radius, unit);
            }

            return element;
        }

        public static VisualElement SetBorderTopRightRadius(this VisualElement element, float radius, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderTopRightRadius.HasValue() ? element.style.borderTopRightRadius.value.value : element.resolvedStyle.borderTopRightRadius;
                element.style.borderTopRightRadius = value + radius;
            }
            else
            {
                element.style.borderTopRightRadius = radius;
            }

            return element;
        }

        public static VisualElement ResetBorderTopRightRadius(this VisualElement element)
        {
            element.style.borderTopRightRadius = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetBorderBottomLeftRadius(this VisualElement element, float radius, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderBottomLeftRadius.HasValue() ? element.style.borderBottomLeftRadius.value.value : element.resolvedStyle.borderBottomLeftRadius;
                element.style.borderBottomLeftRadius = new Length(value + radius, unit);
            }
            else
            {
                element.style.borderBottomLeftRadius = new Length(radius, unit);
            }

            return element;
        }

        public static VisualElement SetBorderBottomLeftRadius(this VisualElement element, float radius, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderBottomLeftRadius.HasValue() ? element.style.borderBottomLeftRadius.value.value : element.resolvedStyle.borderBottomLeftRadius;
                element.style.borderBottomLeftRadius = value + radius;
            }
            else
            {
                element.style.borderBottomLeftRadius = radius;
            }

            return element;
        }

        public static VisualElement ResetBorderBottomLeftRadius(this VisualElement element)
        {
            element.style.borderBottomLeftRadius = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetBorderBottomRightRadius(this VisualElement element, float radius, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderBottomRightRadius.HasValue() ? element.style.borderBottomRightRadius.value.value : element.resolvedStyle.borderBottomRightRadius;
                element.style.borderBottomRightRadius = new Length(value + radius, unit);
            }
            else
            {
                element.style.borderBottomRightRadius = new Length(radius, unit);
            }

            return element;
        }

        public static VisualElement SetBorderBottomRightRadius(this VisualElement element, float radius, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.borderBottomRightRadius.HasValue() ? element.style.borderBottomRightRadius.value.value : element.resolvedStyle.borderBottomRightRadius;
                element.style.borderBottomRightRadius = value + radius;
            }
            else
            {
                element.style.borderBottomRightRadius = radius;
            }

            return element;
        }

        public static VisualElement ResetBorderBottomRightRadius(this VisualElement element)
        {
            element.style.borderBottomRightRadius = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetMarginLeft(this VisualElement element, float margin, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginLeft.HasValue() ? element.style.marginLeft.value.value : element.resolvedStyle.marginLeft;
                element.style.marginLeft = new Length(value + margin, unit);
            }
            else
            {
                element.style.marginLeft = new Length(margin, unit);
            }
            return element;
        }

        public static VisualElement SetMarginLeft(this VisualElement element, float margin, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginLeft.HasValue() ? element.style.marginLeft.value.value : element.resolvedStyle.marginLeft;
                element.style.marginLeft = value + margin;
            }
            else
            {
                element.style.marginLeft = margin;
            }
            return element;
        }

        public static VisualElement ResetMarginLeft(this VisualElement element)
        {
            element.style.marginLeft = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetMarginTop(this VisualElement element, float margin, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginTop.HasValue() ? element.style.marginTop.value.value : element.resolvedStyle.marginTop;
                element.style.marginTop = new Length(value + margin, unit);
            }
            else
            {
                element.style.marginTop = new Length(margin, unit);
            }
            return element;
        }

        public static VisualElement SetMarginTop(this VisualElement element, float margin, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginTop.HasValue() ? element.style.marginTop.value.value : element.resolvedStyle.marginTop;
                element.style.marginTop = value + margin;
            }
            else
            {
                element.style.marginTop = margin;
            }
            return element;
        }

        public static VisualElement ResetMarginTop(this VisualElement element)
        {
            element.style.marginTop = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetMarginRight(this VisualElement element, float margin, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginRight.HasValue() ? element.style.marginRight.value.value : element.resolvedStyle.marginRight;
                element.style.marginRight = new Length(value + margin, unit);
            }
            else
            {
                element.style.marginRight = new Length(margin, unit);
            }
            return element;
        }

        public static VisualElement SetMarginRight(this VisualElement element, float margin, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginRight.HasValue() ? element.style.marginRight.value.value : element.resolvedStyle.marginRight;
                element.style.marginRight = value + margin;
            }
            else
            {
                element.style.marginRight = margin;
            }
            return element;
        }

        public static VisualElement ResetMarginRight(this VisualElement element)
        {
            element.style.marginRight = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetMarginBottom(this VisualElement element, float margin, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginBottom.HasValue() ? element.style.marginBottom.value.value : element.resolvedStyle.marginBottom;
                element.style.marginBottom = new Length(value + margin, unit);
            }
            else
            {
                element.style.marginBottom = new Length(margin, unit);
            }
            return element;
        }

        public static VisualElement SetMarginBottom(this VisualElement element, float margin, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.marginBottom.HasValue() ? element.style.marginBottom.value.value : element.resolvedStyle.marginBottom;
                element.style.marginBottom = value + margin;
            }
            else
            {
                element.style.marginBottom = margin;
            }
            return element;
        }

        public static VisualElement ResetMarginBottom(this VisualElement element)
        {
            element.style.marginBottom = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetMargin(this VisualElement element, float margin, LengthUnit unit, bool delta = false)
        {
            SetMarginLeft(element, margin, unit, delta);
            SetMarginTop(element, margin, unit, delta);
            SetMarginRight(element, margin, unit, delta);
            SetMarginBottom(element, margin, unit, delta);
            return element;
        }

        public static VisualElement SetMargin(this VisualElement element, float margin, bool delta = false)
        {
            SetMarginLeft(element, margin, delta);
            SetMarginTop(element, margin, delta);
            SetMarginRight(element, margin, delta);
            SetMarginBottom(element, margin, delta);
            return element;
        }

        public static VisualElement ResetMargin(this VisualElement element)
        {
            element.style.marginLeft = StyleKeyword.Null;
            element.style.marginTop = StyleKeyword.Null;
            element.style.marginRight = StyleKeyword.Null;
            element.style.marginBottom = StyleKeyword.Null;
            return element;
        }

        public static float GetMargin(this VisualElement element, bool resolved = false)
        {
            return GetMarginBottom(element, resolved);
        }

        public static float GetMarginLeft(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.marginLeft;
            }
            else
            {
                return element.style.marginLeft.value.value;
            }
        }

        public static float GetMarginTop(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.marginTop;
            }
            else
            {
                return element.style.marginTop.value.value;
            }
        }

        public static float GetMarginRight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.marginRight;
            }
            else
            {
                return element.style.marginRight.value.value;
            }
        }

        public static float GetMarginBottom(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.marginBottom;
            }
            else
            {
                return element.style.marginBottom.value.value;
            }
        }

        public static VisualElement SetPaddingLeft(this VisualElement element, float padding, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingLeft.HasValue() ? element.style.paddingLeft.value.value : element.resolvedStyle.paddingLeft;
                element.style.paddingLeft = new Length(value + padding, unit);
            }
            else
            {
                element.style.paddingLeft = new Length(padding, unit);
            }
            return element;
        }

        public static VisualElement SetPaddingLeft(this VisualElement element, float padding, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingLeft.HasValue() ? element.style.paddingLeft.value.value : element.resolvedStyle.paddingLeft;
                element.style.paddingLeft = value + padding;
            }
            else
            {
                element.style.paddingLeft = padding;
            }
            return element;
        }

        public static VisualElement ResetPaddingLeft(this VisualElement element)
        {
            element.style.paddingLeft = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetPaddingTop(this VisualElement element, float padding, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingTop.HasValue() ? element.style.paddingTop.value.value : element.resolvedStyle.paddingTop;
                element.style.paddingTop = new Length(value + padding, unit);
            }
            else
            {
                element.style.paddingTop = new Length(padding, unit);
            }
            return element;
        }

        public static VisualElement SetPaddingTop(this VisualElement element, float padding, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingTop.HasValue() ? element.style.paddingTop.value.value : element.resolvedStyle.paddingTop;
                element.style.paddingTop = value + padding;
            }
            else
            {
                element.style.paddingTop = padding;
            }
            return element;
        }

        public static VisualElement ResetPaddingTop(this VisualElement element)
        {
            element.style.paddingTop = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetPaddingRight(this VisualElement element, float padding, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingRight.HasValue() ? element.style.paddingRight.value.value : element.resolvedStyle.paddingRight;
                element.style.paddingRight = new Length(value + padding, unit);
            }
            else
            {
                element.style.paddingRight = new Length(padding, unit);
            }
            return element;
        }

        public static VisualElement SetPaddingRight(this VisualElement element, float padding, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingRight.HasValue() ? element.style.paddingRight.value.value : element.resolvedStyle.paddingRight;
                element.style.paddingRight = value + padding;
            }
            else
            {
                element.style.paddingRight = padding;
            }
            return element;
        }

        public static VisualElement ResetPaddingRight(this VisualElement element)
        {
            element.style.paddingRight = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetPaddingBottom(this VisualElement element, float padding, LengthUnit unit, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingBottom.HasValue() ? element.style.paddingBottom.value.value : element.resolvedStyle.paddingBottom;
                element.style.paddingBottom = new Length(value + padding, unit);
            }
            else
            {
                element.style.paddingBottom = new Length(padding, unit);
            }

            return element;
        }

        public static VisualElement SetPaddingBottom(this VisualElement element, float padding, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.paddingBottom.HasValue() ? element.style.paddingBottom.value.value : element.resolvedStyle.paddingBottom;
                element.style.paddingBottom = value + padding;
            }
            else
            {
                element.style.paddingBottom = padding;
            }

            return element;
        }

        public static VisualElement ResetPaddingBottom(this VisualElement element)
        {
            element.style.paddingBottom = StyleKeyword.Null;
            return element;
        }

        public static VisualElement SetPadding(this VisualElement element, float padding, LengthUnit unit, bool delta = false)
        {
            SetPaddingLeft(element, padding, unit, delta);
            SetPaddingTop(element, padding, unit, delta);
            SetPaddingRight(element, padding, unit, delta);
            SetPaddingBottom(element, padding, unit, delta);
            return element;
        }

        public static VisualElement SetPadding(this VisualElement element, float padding, bool delta = false)
        {
            SetPaddingLeft(element, padding, delta);
            SetPaddingTop(element, padding, delta);
            SetPaddingRight(element, padding, delta);
            SetPaddingBottom(element, padding, delta);
            return element;
        }

        public static VisualElement ResetPadding(this VisualElement element)
        {
            element.style.paddingLeft = StyleKeyword.Null;
            element.style.paddingTop = StyleKeyword.Null;
            element.style.paddingRight = StyleKeyword.Null;
            element.style.paddingBottom = StyleKeyword.Null;
            return element;
        }

        public static float GetPadding(this VisualElement element, bool resolved = false)
        {
            return GetPaddingLeft(element, resolved);
        }

        public static float GetPaddingLeft(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.paddingLeft;
            }
            else
            {
                return element.style.paddingLeft.value.value;
            }
        }

        public static float GetPaddingTop(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.paddingTop;
            }
            else
            {
                return element.style.paddingTop.value.value;
            }
        }

        public static float GetPaddingRight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.paddingRight;
            }
            else
            {
                return element.style.paddingRight.value.value;
            }
        }

        public static float GetPaddingBottom(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.paddingBottom;
            }
            else
            {
                return element.style.paddingBottom.value.value;
            }
        }

        public static Vector2 GetSize(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return new Vector2(element.resolvedStyle.width, element.resolvedStyle.height);
            }
            else
            {
                return new Vector2(element.style.width.value.value, element.style.height.value.value);
            }
        }

        public static VisualElement SetSizeScalar(this VisualElement element, float width, float height, LengthUnit unit = default, bool delta = false)
        {
            SetWidth(element, width, unit, delta);
            SetHeight(element, height, unit, delta);

            return element;
        }

        public static VisualElement SetSize(this VisualElement element, Vector2 size, LengthUnit unit = default, bool delta = false)
        {
            SetWidth(element, size.x, unit, delta);
            SetHeight(element, size.y, unit, delta);

            return element;
        }

        public static float GetWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.width;
            }
            else
            {
                return element.style.width.value.value;
            }
        }

        public static VisualElement SetWidth(this VisualElement element, float width, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                var length = element.style.width.value;
                // If the style is not yet defined then take the value from the resolved style.
                if (!element.style.width.HasValue())
                {
                    length = element.resolvedStyle.width;
                }
                length.value += width;
                element.style.width = length;
            }
            else
            {
                var length = new Length(width, unit);
                element.style.width = length;
            }

            return element;
        }

        public static VisualElement ResetWidth(this VisualElement element)
        {
            element.style.width = StyleKeyword.Null;
            return element;
        }

        public static float GetHeight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.height;
            }
            else
            {
                return element.style.height.value.value;
            }
        }

        public static VisualElement SetHeight(this VisualElement element, float height, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                var length = element.style.height.value;
                // If the style is not yet defined then take the value from the resolved style.
                if (!element.style.height.HasValue())
                {
                    length = element.resolvedStyle.height;
                }
                length.value += height;
                element.style.height = length;
            }
            else
            {
                var length = new Length(height, unit);
                element.style.height = length;
            }

            return element;
        }

        public static VisualElement ResetHeight(this VisualElement element)
        {
            element.style.height = StyleKeyword.Null;
            return element;
        }

        public static float GetMinWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.minWidth.value;
            }
            else
            {
                return element.style.minWidth.value.value;
            }
        }

        public static VisualElement SetMinWidth(this VisualElement element, float width, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.minWidth.HasValue() ? element.style.minWidth.value.value : element.resolvedStyle.minWidth.value;
                element.style.minWidth = new Length(value + width, unit);
            }
            else
            {
                var length = new Length(width, unit);
                element.style.minWidth = length;
            }

            return element;
        }

        public static VisualElement ResetMinWidth(this VisualElement element)
        {
            element.style.minWidth = StyleKeyword.Null;
            return element;
        }

        public static float GetMaxWidth(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.maxWidth.value;
            }
            else
            {
                return element.style.maxWidth.value.value;
            }
        }

        public static VisualElement SetMaxWidth(this VisualElement element, float width, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.maxWidth.HasValue() ? element.style.maxWidth.value.value : element.resolvedStyle.maxWidth.value;
                element.style.maxWidth = new Length(value + width, unit);
            }
            else
            {
                var length = new Length(width, unit);
                element.style.maxWidth = length;
            }

            return element;
        }

        public static VisualElement ResetMaxWidth(this VisualElement element)
        {
            element.style.maxWidth = StyleKeyword.Null;
            return element;
        }

        public static float GetMinHeight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.minHeight.value;
            }
            else
            {
                return element.style.minHeight.value.value;
            }
        }

        public static VisualElement SetMinHeight(this VisualElement element, float height, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.minHeight.HasValue() ? element.style.minHeight.value.value : element.resolvedStyle.minHeight.value;
                element.style.minHeight = new Length(value + height, unit);
            }
            else
            {
                var length = new Length(height, unit);
                element.style.minHeight = length;
            }

            return element;
        }

        public static VisualElement ResetMinHeight(this VisualElement element)
        {
            element.style.minHeight = StyleKeyword.Null;
            return element;
        }

        public static float GetMaxHeight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.maxHeight.value;
            }
            else
            {
                return element.style.maxHeight.value.value;
            }
        }

        public static VisualElement SetMaxHeight(this VisualElement element, float height, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                float value = element.style.maxHeight.HasValue() ? element.style.maxHeight.value.value : element.resolvedStyle.maxHeight.value;
                element.style.maxHeight = new Length(value + height, unit);
            }
            else
            {
                var length = new Length(height, unit);
                element.style.maxHeight = length;
            }

            return element;
        }

        public static VisualElement ResetMaxHeight(this VisualElement element)
        {
            element.style.maxHeight = StyleKeyword.Null;
            return element;
        }

        public static Position GetPosition(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.position;
            }
            else
            {
                return element.style.position.value;
            }
        }

        public static VisualElement SetPosition(this VisualElement element, Position position)
        {
            element.style.position = position;
            return element;
        }

        public static VisualElement ResetPosition(this VisualElement element)
        {
            element.style.position = StyleKeyword.Null;
            return element;
        }

        public static Vector2 GetPosTopLeft(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return new Vector2(element.resolvedStyle.left, element.resolvedStyle.top);
            }
            else
            {
                return new Vector2(element.style.left.value.value, element.style.top.value.value);
            }
        }

        /// <summary>
        /// Just an alias for SetPosLeftTopScalar to make fuzzy finding easier.
        /// </summary>
        public static VisualElement SetPositionTopLeftScalar(this VisualElement element, float top, float left, LengthUnit unit = default, bool delta = false)
        {
            return SetPositionLeftTopScalar(element, left, top, unit, delta);
        }

        public static VisualElement SetPositionLeftTop(this VisualElement element, Vector2 leftTop, LengthUnit unit = default, bool delta = false)
        {
            return SetPositionLeftTopScalar(element, leftTop.x, leftTop.y, unit, delta);
        }

        public static VisualElement SetPositionLeftTopScalar(this VisualElement element, float left, float top, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.left = new Length(element.style.left.value.value + left, unit);
                element.style.top = new Length(element.style.top.value.value + top, unit);
            }
            else
            {
                element.style.left = new Length(left, unit);
                element.style.top = new Length(top, unit);
            }

            return element;
        }

        public static VisualElement ResetPositionLeftTop(this VisualElement element)
        {
            element.style.left = StyleKeyword.Null;
            element.style.top = StyleKeyword.Null;

            return element;
        }

        public static Vector2 GetPositionRightBottom(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return new Vector2(element.resolvedStyle.right, element.resolvedStyle.bottom);
            }
            else
            {
                return new Vector2(element.style.right.value.value, element.style.bottom.value.value);
            }
        }

        /// <summary>
        /// Just an alias for SetPosRightBottomScalar to make fuzzy finding easier.
        /// </summary>
        public static VisualElement SetPositionBottomRightScalar(this VisualElement element, float bottom, float right, LengthUnit unit = default, bool delta = false)
        {
            return SetPositionRightBottomScalar(element, right, bottom, unit, delta);
        }

        public static VisualElement SetPositionRightBottom(this VisualElement element, Vector2 rightBottom, LengthUnit unit = default, bool delta = false)
        {
            return SetPositionRightBottomScalar(element, rightBottom.x, rightBottom.y, unit, delta);
        }

        public static VisualElement SetPositionRightBottomScalar(this VisualElement element, float right, float bottom, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.right = new Length(element.style.right.value.value + right, unit);
                element.style.bottom = new Length(element.style.bottom.value.value + bottom, unit);
            }
            else
            {
                element.style.right = new Length(right, unit);
                element.style.bottom = new Length(bottom, unit);
            }

            return element;
        }

        public static VisualElement ResetPositionRightBottom(this VisualElement element)
        {
            element.style.right = StyleKeyword.Null;
            element.style.bottom = StyleKeyword.Null;

            return element;
        }

        public static float GetPositionLeft(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.left;
            }
            else
            {
                return element.style.left.value.value;
            }
        }

        public static VisualElement SetPositionLeft(this VisualElement element, float left, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.left = new Length(element.style.left.value.value + left, unit);
            }
            else
            {
                element.style.left = new Length(left, unit);
            }

            return element;
        }

        public static VisualElement ResetPositionLeft(this VisualElement element)
        {
            element.style.left = StyleKeyword.Null;

            return element;
        }

        public static float GetPositionTop(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.top;
            }
            else
            {
                return element.style.top.value.value;
            }
        }

        public static VisualElement SetPositionTop(this VisualElement element, float top, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.top = new Length(element.style.top.value.value + top, unit);
            }
            else
            {
                element.style.top = new Length(top, unit);
            }

            return element;
        }

        public static VisualElement ResetPositionTop(this VisualElement element)
        {
            element.style.top = StyleKeyword.Null;

            return element;
        }

        public static float GetPositionRight(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.right;
            }
            else
            {
                return element.style.right.value.value;
            }
        }

        public static VisualElement SetPositionRight(this VisualElement element, float right, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.right = new Length(element.style.right.value.value + right, unit);
            }
            else
            {
                element.style.right = new Length(right, unit);
            }

            return element;
        }

        public static void ResetPositionRight(this VisualElement element)
        {
            element.style.right = StyleKeyword.Null;
        }

        public static float GetPositionBottom(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.bottom;
            }
            else
            {
                return element.style.bottom.value.value;
            }
        }

        public static VisualElement SetPositionBottom(this VisualElement element, float bottom, LengthUnit unit = default, bool delta = false)
        {
            if (delta)
            {
                element.style.bottom = new Length(element.style.bottom.value.value + bottom, unit);
            }
            else
            {
                element.style.bottom = new Length(bottom, unit);
            }

            return element;
        }

        public static VisualElement ResetPositionBottom(this VisualElement element)
        {
            element.style.bottom = StyleKeyword.Null;

            return element;
        }

        public static float GetRotation(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.rotate.angle.value;
            }
            else
            {
                return element.style.rotate.value.angle.value;
            }
        }

        public static VisualElement SetRotation(this VisualElement element, float angle, AngleUnit unit = AngleUnit.Degree, bool delta = false)
        {
            if (delta)
            {
                var angleWithUnit = new Angle(element.style.rotate.value.angle.value + angle, unit);
                element.style.rotate = new Rotate(angleWithUnit);
            }
            else
            {
                var angleWithUnit = new Angle(angle, unit);
                element.style.rotate = new Rotate(angleWithUnit);
            }

            return element;
        }

        public static VisualElement ResetRotation(this VisualElement element)
        {
            element.style.rotate = StyleKeyword.Null;

            return element;
        }

        public static Vector3 GetScale(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.scale.value;
            }
            else
            {
                return element.style.scale.value.value;
            }
        }

        public static VisualElement SetScale(this VisualElement element, Vector3 scale, bool delta = false)
        {
            if (delta)
            {
                var v = getScaleVector(element);
                element.style.scale = new Scale(v + scale);
            }
            else
            {
                element.style.scale = new Scale(scale);
            }

            return element;
        }

        private static Vector3 getScaleVector(VisualElement element)
        {
            Vector3 v;
            if (element.style.scale == StyleKeyword.Null)
            {
                v = Vector3.one;
            }
            else
            {
                v = element.style.scale.value.value;
            }

            return v;
        }

        public static VisualElement ResetScale(this VisualElement element)
        {
            element.style.scale = StyleKeyword.Null;

            return element;
        }

        public static float GetUniformScale(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.scale.value.x;
            }
            else
            {
                return element.style.scale.value.value.x;
            }
        }

        public static VisualElement SetUniformScale(this VisualElement element, float scale, bool delta = false)
        {
            if (delta)
            {
                var v = getScaleVector(element);
                SetScale(element, new Vector3(v.x + scale, v.y + scale, v.z + scale));
            }
            else
            {
                SetScale(element, new Vector3(scale, scale, scale));
            }

            return element;
        }

        public static float GetScaleX(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.scale.value.x;
            }
            else
            {
                return element.style.scale.value.value.x;
            }
        }

        public static VisualElement SetScaleX(this VisualElement element, float scaleX, bool delta = false)
        {
            // Init scale with 1/1/1 if not yet defined.
            Vector3 v = getScaleVector(element);

            // Apply scale X
            if (delta)
            {
                v.x += scaleX;
            }
            else
            {
                v.x = scaleX;
            }

            element.style.scale = new Scale(v);

            return element;
        }

        public static float GetScaleY(this VisualElement element, bool resolved = false)
        {
            if (resolved)
            {
                return element.resolvedStyle.scale.value.y;
            }
            else
            {
                return element.style.scale.value.value.y;
            }
        }

        public static VisualElement SetScaleY(this VisualElement element, float scaleY, bool delta = false)
        {
            // Init scale with 1/1/1 if not yet defined.
            Vector3 v = getScaleVector(element);

            // Apply scale Y
            if (delta)
            {
                v.y += scaleY;
            }
            else
            {
                v.y = scaleY;
            }

            element.style.scale = new Scale(v);

            return element;
        }
    }
}
#endif