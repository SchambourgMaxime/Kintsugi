using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public partial class ScrollViewPro
    {
        // If you want to know why all this is necessary then look at this:
        // https://forum.unity.com/threads/thoughts-on-the-ui-toolkit-focusing-algorithm-can-we-change-it.1480287/

        protected bool usesVerticalScroller => ((mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal) && isVerticalScrollDisplayed);
        protected bool usesHorizontalScroller => ((mode == ScrollViewMode.Horizontal || mode == ScrollViewMode.VerticalAndHorizontal) && isHorizontalScrollDisplayed);

        protected void onFocusIn(FocusInEvent evt)
        {
            if (!selectableScrollbars)
                return;
            
            if (focusable)
            {
                // Since the focusIn event is registered on the ScrollView this only happens
                // if the scroll view or an element inside it is selected.
                // If that happens we disable focusing of the scroll view.
                this.focusable = false;

                // If the ScrollView itself was focused then this means we have to
                // disable the focus and forward it to an element inside the scroll view.
                if (evt.target == this)
                {
                    // Use the original source as target.
                    var next = focusController.GetNextFocusable(evt.relatedTarget as Focusable, evt) as VisualElement;
                    var direction = ReflectionUtils.FocusChangeDirectionToNavigationDirection(evt.direction);

                    // Forward focus to the 'next' target unless
                    // the next target is not inside the scroll view (happens if the scroll view is empty).
                    // If this happens then select the first scroll bar (if there is one) instead.
                    if (next != null && next.IsChildOf(this))
                    {
                        // If the navigation direction was UP and there is a horizontal scroll bar then select the scroll bar.
                        if (usesHorizontalScroller && direction == NavigationMoveEvent.Direction.Up)
                        {
                            changeFocusAsync(this, horizontalScroller.slider);
                        }
                        // If the navigation direction was LEFT and there is a vertical scroll bar then select the scroll bar.
                        else if(usesVerticalScroller && direction == NavigationMoveEvent.Direction.Left)
                        {
                            changeFocusAsync(this, verticalScroller.slider);
                        }
                        // Else select the next element
                        else
                        {
                            changeFocusAsync(this, next);
                        }
                    }
                    else
                    {
                        if ((mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal)
                            && verticalScroller.enabledSelf && isVerticalScrollDisplayed && verticalScroller.slider.canGrabFocus)
                        {
                            changeFocusAsync(this, verticalScroller.slider);
                        }
                        else if(horizontalScroller.enabledSelf && isHorizontalScrollDisplayed && horizontalScroller.slider.canGrabFocus)
                        {
                            changeFocusAsync(this, horizontalScroller.slider);
                        }
                    }
                }
            }
        }

        protected void onFocusOut(FocusOutEvent evt)
        {
            if (!selectableScrollbars)
                return;

            if (evt.target == this)
                return;

            // Is focus leaving the scroll view? If yes then reenable focusability.
            var source = evt.target as VisualElement;
            var target = evt.relatedTarget as VisualElement;
            if (source.IsChildOf(this) && !target.IsChildOf(this))
            {
                schedule.Execute(restoreFocus);
            }
        }

        protected void restoreFocus()
        {
            this.focusable = selectableScrollbars;
        }

        protected void changeFocusAsync(Focusable from, Focusable to)
        {
            // We need a delay so the focus system picks it up. n2h: Find out why exactly.
            schedule.Execute(() =>
            {
                to?.Focus();
            });
        }

        // Notice that FocusSnap also listens to this event and may execute after this.
        protected void onNavigationMoveInContentContainer(NavigationMoveEvent evt)
        {
            if (!selectableScrollbars)
                return;

            var current = evt.target as VisualElement;
            var next = focusController.GetNextFocusable(evt.target as Focusable, evt) as VisualElement;

            // If the current selection is a slider (scroll bar or slider in ui) then do nothing.
            if (current.GetFirstAncestorOfType<Scroller>() != null)
            {
                return;
            }

            // It is navigating towards the vertical scroller. Let's check if it should be selected.
            if (evt.direction == NavigationMoveEvent.Direction.Right && usesVerticalScroller)
            {
                // It returns itself as next focusable if no other is found in the navigation direction.
                // If it jumps from right to left despite navigation direction being left to right then it
                // is a reset to the beginning. We catch that and select the scroller instead.
                if (current == next || !next.IsChildOf(this) || next.worldBound.center.x < current.worldBound.center.x)
                {
                    changeFocusAsync(current, verticalScroller.slider);
                    evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                    focusController.IgnoreEvent(evt);
#else
                    evt.PreventDefault();
#endif
                }
            }

            // It is navigating towards the horizontal scroller. Let's check if it should be selected.
            if (evt.direction == NavigationMoveEvent.Direction.Down && usesHorizontalScroller)
            {
                // It returns itself as next focusable if no other is found in the navigation direction.
                // If it jumps from right to left despite navigation direction being left to right then it
                // is a reset to the beginning. We catch that and select the scroller instead.
                if (current == next || !next.IsChildOf(this) || next.worldBound.center.y < current.worldBound.center.y)
                {
                    changeFocusAsync(current, horizontalScroller.slider);
                    evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                    focusController.IgnoreEvent(evt);
#else
                    evt.PreventDefault();
#endif
                }
            }
        }

        const long BlockedNavigationMoveOnSliderDelayMS = 750;

        protected long _lastBlockedNavigationMoveOnSliderTime = 0;

        protected void onNavigationMoveOnSlider(NavigationMoveEvent evt)
        {
            if (!selectableScrollbars)
                return;

            clampElastic();

            var target = evt.target as VisualElement;

            // Stop nav events from propagating if the direction aligns with the scroll bar direction,
            // unless the scroll bar has reached the end value.
            // If it has reached the end value (canScroll** = false) then wait a bit (BlockedNavigationMoveOnSliderDelayMS MS)
            // until the focus can move out of the scroll bar.

            if (target.IsChildOf(horizontalScroller))
            {
                bool directionIsHorizontal = evt.direction == NavigationMoveEvent.Direction.Left || evt.direction == NavigationMoveEvent.Direction.Right;
                bool canScrollHorizontally = (evt.direction == NavigationMoveEvent.Direction.Left && horizontalScroller.value > horizontalScroller.lowValue + 0.001f)
                                          || (evt.direction == NavigationMoveEvent.Direction.Right && horizontalScroller.value < horizontalScroller.highValue - 0.001f);
                if (target.IsChildOf(horizontalScroller))
                {
                    if (canScrollHorizontally)
                    {
                        _lastBlockedNavigationMoveOnSliderTime = evt.timestamp;
                    }
                    if (canScrollHorizontally || (directionIsHorizontal && evt.timestamp - _lastBlockedNavigationMoveOnSliderTime < BlockedNavigationMoveOnSliderDelayMS))
                    {
#if UNITY_2022_2_OR_NEWER
                        // Starting with Unity 2022.2 the sliders are using OnNavigationMove events.
                        // Since we block them here we have to trigger the correct slider response manually.
                        // See: https://github.com/Unity-Technologies/UnityCsReference/blob/2022.2/ModuleOverrides/com.unity.ui/Core/Controls/BaseSlider.cs
                        horizontalScroller.slider.OnNavigationMove(evt);
#endif

                        evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                        focusController.IgnoreEvent(evt);
#else
                        evt.PreventDefault();
#endif
                        return;
                    }
                }
            }

            if (target.IsChildOf(verticalScroller))
            {
                bool directionIsVertical = evt.direction == NavigationMoveEvent.Direction.Up || evt.direction == NavigationMoveEvent.Direction.Down;
                bool canScrollVertically = (evt.direction == NavigationMoveEvent.Direction.Up && verticalScroller.value > verticalScroller.lowValue + 0.001f)
                                        || (evt.direction == NavigationMoveEvent.Direction.Down && verticalScroller.value < verticalScroller.highValue - 0.001f);
                if (target.IsChildOf(verticalScroller))
                {
                    if (canScrollVertically)
                    {
                        _lastBlockedNavigationMoveOnSliderTime = evt.timestamp;
                    }
                    if (canScrollVertically || (directionIsVertical && evt.timestamp - _lastBlockedNavigationMoveOnSliderTime < BlockedNavigationMoveOnSliderDelayMS))
                    {
#if UNITY_2022_2_OR_NEWER
                        // Starting with Unity 2022.2 the sliders are using OnNavigationMove events.
                        // Since we block them here we have to trigger the correct slider response manually.
                        // See: https://github.com/Unity-Technologies/UnityCsReference/blob/2022.2/ModuleOverrides/com.unity.ui/Core/Controls/BaseSlider.cs
                        verticalScroller.slider.OnNavigationMove(evt);
#else
                        // In Unity 2022.1 or OLDER the scroller does NOT react to navigation events (it only supports the key down event).
                        // This means for controller support we have to trigger the key event manually (if it isn't a key event already).
                        if (evt.eventTypeId != KeyDownEvent.TypeId())
                        {
                            KeyCode keyCode = KeyCode.None;
                            switch (evt.direction)
                            {
                                case NavigationMoveEvent.Direction.Left:
                                    keyCode = KeyCode.LeftArrow;
                                    break;
                                case NavigationMoveEvent.Direction.Up:
                                    keyCode = KeyCode.UpArrow;
                                    break;
                                case NavigationMoveEvent.Direction.Right:
                                    keyCode = KeyCode.RightArrow;
                                    break;
                                case NavigationMoveEvent.Direction.Down:
                                    keyCode = KeyCode.DownArrow;
                                    break;
                            }
                            if (keyCode != KeyCode.None)
                            {
                                using (var keyDownEvent = KeyDownEvent.GetPooled(' ', keyCode, EventModifiers.None))
                                {
                                    evt.target.SendEvent(keyDownEvent);
                                }
                            }
                        }
#endif

                        evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                        focusController.IgnoreEvent(evt);
#else
                        evt.PreventDefault();
#endif

                        return;
                    }
                }
            }

            // If we land here then navigation direction is perpendicular to the scroller direction
            // OR the BlockedNavigationMoveOnSliderDelayMS threshold was hit.

            // Check if the next focusable is valid.
            // It may return itself as next focusable is none is found.
            // This happens if there are no focusables in the contentContainer (walls of text for example).
            var next = focusController.GetNextFocusable(evt.target as Focusable, evt) as VisualElement;
            if (next == evt.target)
            {
                // Select something outside the scroll view manually depending on the direction.
                next = focusController.GetNextFocusable(this as Focusable, evt) as VisualElement;
                if (next != null && next != this)
                {
                    changeFocusAsync(evt.target as Focusable, next);
                }
            }
        }
    }
}
