using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public static class ReflectionUtils
    {
        static PropertyInfo _disableClipingPropInfo;
        static bool _disableClipingPropInfoRetrieved = false;

        public static void SetDisableCliping(VisualElement ele, bool value)
        {
            if (_disableClipingPropInfo == null && !_disableClipingPropInfoRetrieved)
            {
                _disableClipingPropInfo = typeof(VisualElement).GetProperty("disableCliping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _disableClipingPropInfoRetrieved = true;
            }

            _disableClipingPropInfo?.SetValue(ele, value);
        }


        static PropertyInfo _boundingBoxPropInfo;
        static bool _boundingBoxPropInfoRetrieved = false;

        public static Rect GetBoundingBox(this VisualElement ele)
        {
            if (_boundingBoxPropInfo == null && !_boundingBoxPropInfoRetrieved)
            {
                _boundingBoxPropInfo = typeof(VisualElement).GetProperty("boundingBox", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _boundingBoxPropInfoRetrieved = true;
            }

            if (_boundingBoxPropInfo != null)
                return (Rect)_boundingBoxPropInfo.GetValue(ele);
            else
                return ele.localBound;
        }


        static MethodInfo _incrementVersionMethodInfo;
        static bool _incrementVersionMethodInfoRetrieved = false;

        public static int VersionChangeTypeRepaint = 0x800;

        public static void IncrementVersion(VisualElement ele, int changeType)
        {
            if (_incrementVersionMethodInfo == null && !_incrementVersionMethodInfoRetrieved)
            {
                _incrementVersionMethodInfo = typeof(VisualElement).GetMethod("IncrementVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _incrementVersionMethodInfoRetrieved = true;
            }

            if (_incrementVersionMethodInfo != null)
                _incrementVersionMethodInfo.Invoke(ele, new object[] { changeType });
        }


        static MethodInfo _roundToPixelGridMethodInfo;
        static bool _roundToPixelGridRetrieved = false;

        public static float GUIUtility_RoundToPixelGrid(float v)
        {
            if (_incrementVersionMethodInfo == null && !_roundToPixelGridRetrieved)
            {
                _roundToPixelGridMethodInfo = typeof(GUIUtility).GetMethod("RoundToPixelGrid", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                _roundToPixelGridRetrieved = true;
            }

            if (_roundToPixelGridMethodInfo != null)
            {
                return (float)_roundToPixelGridMethodInfo.Invoke(null, new object[] { v });
            }
            else
            {
                return Mathf.Round(v);
            }
        }


        static MethodInfo _getRootVisualContainerMethodInfo;
        static bool _getRootVisualContainerRetrieved = false;

        public static VisualElement GetRootVisualContainer(VisualElement ele)
        {
            if (_incrementVersionMethodInfo == null && !_getRootVisualContainerRetrieved)
            {
                _getRootVisualContainerMethodInfo = typeof(VisualElement).GetMethod("GetRootVisualContainer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _getRootVisualContainerRetrieved = true;
            }

            if (_getRootVisualContainerMethodInfo != null)
            {
                return (VisualElement)_getRootVisualContainerMethodInfo.Invoke(ele, new object[] {});
            }
            else
            {
                return ele.parent;
            }
        }

        static PropertyInfo _sliderClampedPropertyInfo;
        static bool _sliderClampedPropertyInfoRetrieved = false;

        public static void SetClamped(this Slider slider, bool clamped)
        {
            if (_sliderClampedPropertyInfo == null && !_sliderClampedPropertyInfoRetrieved)
            {
                _sliderClampedPropertyInfo = typeof(Slider).GetProperty("clamped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                _sliderClampedPropertyInfoRetrieved = true;
            }

            _sliderClampedPropertyInfo?.SetValue(slider, clamped);
        }

        static PropertyInfo _focusRingPropertyInfo;
        static bool _focusRingPropertyInfoRetrieved = false;

        public static IFocusRing GetFocusRing(this FocusController focusController)
        {
            if (_focusRingPropertyInfo == null && !_focusRingPropertyInfoRetrieved)
            {
                _focusRingPropertyInfo = typeof(FocusController).GetProperty("focusRing", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                _focusRingPropertyInfoRetrieved = true;
            }

            return (IFocusRing)_focusRingPropertyInfo?.GetValue(focusController);
        }

        public static Focusable GetNextFocusable(this FocusController focusController, Focusable focusable, EventBase evt)
        {
            var ring = focusController.GetFocusRing();
            var direction = ring.GetFocusChangeDirection(focusable, evt);
            return ring.GetNextFocusable(focusable, direction);
        }

        public static Focusable GetNextFocusable(this FocusController focusController, Focusable focusable, FocusInEvent evt)
        {
            var ring = focusController.GetFocusRing();
            return ring.GetNextFocusable(focusable, evt.direction);
        }

        // Based on:
        // https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/ModuleOverrides/com.unity.ui/Core/GameObjects/NavigateFocusRing.cs
        public static readonly int FocusChangeDirectionLeft = 1;
        public static readonly int FocusChangeDirectionRight = 2;
        public static readonly int FocusChangeDirectionUp = 3;
        public static readonly int FocusChangeDirectionDown = 4;
        public static readonly int FocusChangeDirectionNext = 5;
        public static readonly int FocusChangeDirectionPrevious = 6;

        public static NavigationMoveEvent.Direction FocusChangeDirectionToNavigationDirection(FocusChangeDirection direction)
        {
            if (direction == FocusChangeDirectionLeft)
                return NavigationMoveEvent.Direction.Left;

            else if (direction == FocusChangeDirectionRight)
                return NavigationMoveEvent.Direction.Right;

            else if (direction == FocusChangeDirectionUp)
                return NavigationMoveEvent.Direction.Up;

            else if (direction == FocusChangeDirectionDown)
                return NavigationMoveEvent.Direction.Down;

            /*
            else if (direction == FocusChangeDirectionNext)
                return NavigationMoveEvent.Direction.None;

            else if (direction == FocusChangeDirectionPrevious)
                return NavigationMoveEvent.Direction.None;
            */
            else
                return NavigationMoveEvent.Direction.None;
        }

        static FieldInfo _repeatClickableFieldInfo;
        static bool _repeatClickableFieldInfoRetrieved = false;

        public static Clickable GetClickable(this RepeatButton button)
        {
            if (_repeatClickableFieldInfo == null && !_repeatClickableFieldInfoRetrieved)
            {
                _repeatClickableFieldInfo = typeof(RepeatButton).GetField("m_Clickable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                _repeatClickableFieldInfoRetrieved = true;
            }

            return (Clickable)_repeatClickableFieldInfo?.GetValue(button);
        }

        static MethodInfo _onNavigationMoveMethodInfo;
        static bool _onNavigationMoveMethodInfoRetrieved = false;

        public static void OnNavigationMove<TValueType>(this BaseSlider<TValueType> slider, NavigationMoveEvent evt)
             where TValueType : System.IComparable<TValueType>
        {
            if (_onNavigationMoveMethodInfo == null && !_onNavigationMoveMethodInfoRetrieved)
            {
                _onNavigationMoveMethodInfo = typeof(BaseSlider<TValueType>).GetMethod("OnNavigationMove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                _onNavigationMoveMethodInfoRetrieved = true;
            }

            _onNavigationMoveMethodInfo.Invoke(slider, new object[] {evt});
        }
    }
}