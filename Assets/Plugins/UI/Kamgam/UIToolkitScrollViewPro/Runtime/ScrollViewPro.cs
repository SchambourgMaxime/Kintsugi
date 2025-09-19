using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    /// <summary>
    /// ScrollViewPro is a full rewrite of the Unity ScrollView.
    /// It aims to be API compatible and thus uses some of ScrollViews enums and types.
    /// </summary>
    public partial class ScrollViewPro : VisualElement
    {
        const string ScrollerDragElementName = "unity-dragger";

        protected List<VisualElement> _trackedChildren = new List<VisualElement>();

        public ScrollViewPro() : this(ScrollViewMode.Vertical)
        { }

        public ScrollViewPro(ScrollViewMode scrollViewMode)
        {
            RegisterCallback<CustomStyleResolvedEvent>(onStylesResolved);

            // Create hierarchy

            AddToClassList(ussClassName);
            AddToClassList(ussSpecificClassName);

            _contentAndVerticalScrollContainer = new VisualElement { name = "unity-content-and-vertical-scroll-container" };
            _contentAndVerticalScrollContainer.AddToClassList(contentAndVerticalScrollUssClassName);
            hierarchy.Add(_contentAndVerticalScrollContainer);

            contentViewport = new VisualElement { name = "unity-content-viewport" };
            contentViewport.pickingMode = PickingMode.Ignore;
            contentViewport.AddToClassList(viewportUssClassName);
            _contentAndVerticalScrollContainer.Add(contentViewport);

            _contentContainer = new VisualElement { name = "unity-content-container" };
            // According to Unity this is necessary to NOT clip absolute elements.
            ReflectionUtils.SetDisableCliping(_contentContainer, true);
            _contentContainer.AddToClassList(contentUssClassName);
            _contentContainer.usageHints = UsageHints.GroupTransform;
            contentViewport.Add(_contentContainer);

            SetScrollViewMode(scrollViewMode);

            // Horizontal Scrollbar
            horizontalScroller = new Scroller(0f, int.MaxValue,
                (float value) =>
                {
                    scrollOffset = new Vector2(value, scrollOffset.y);
                    applyScrollOffset();
                }, SliderDirection.Horizontal)
            {
                viewDataKey = "HorizontalScroller"
            };
            horizontalScroller.AddToClassList(hScrollerUssClassName);
            horizontalScroller.style.display = DisplayStyle.None;
            hierarchy.Add(horizontalScroller);

            // Vertical Scrollbar
            verticalScroller = new Scroller(0f, int.MaxValue,
                (float value) =>
                {
                    scrollOffset = new Vector2(scrollOffset.x, value);
                    applyScrollOffset();
                })
            {
                viewDataKey = "VerticalScroller"
            };
            verticalScroller.AddToClassList(vScrollerUssClassName);
            verticalScroller.style.display = DisplayStyle.None;
            _contentAndVerticalScrollContainer.Add(verticalScroller);

            // Ensure the clamping of the scrollbar sliders is properly initialized if
            // created via script (actually not necessary since sliders always start out clamped).
            // touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;

            horizontalPageSize = UndefinedPageSize;
            verticalPageSize = UndefinedPageSize;

            scrollOffset = Vector2.zero;

            isDragging = false;

            // Force update due to infinite.
            infinite = infinite;

            registerEvents();
        }

        protected void onStylesResolved(CustomStyleResolvedEvent evt)
        {
            evt.customStyle.TryGetValue(mouseWheelScrollSizeFromStyle, out _mouseWheelScrollSizeFromStyle);

            evt.customStyle.TryGetValue(singleLineHeightFromStyle, out _singleLineHeightFromStyle);
            updateHorizontalSliderPageSize();
            updateVerticalSliderPageSize();
        }

        protected virtual void registerEvents()
        {
            RegisterCallback<AttachToPanelEvent>(onAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(onDetachFromPanel);
            RegisterCallback<WheelEvent>(onScrollWheel);

            // According to the docs the layout is only computed after one frame has passed and
            // we can use the GeometryChangedEvent to detect that. That's why all the recalculating
            // layout stuff is in onGeomteryChanged.
            // See: https://docs.unity3d.com/ScriptReference/UIElements.VisualElement-layout.html
            contentViewport.RegisterCallback<GeometryChangedEvent>(onScrollViewGeometryChanged);
            contentContainer.RegisterCallback<GeometryChangedEvent>(onScrollViewGeometryChanged);

            verticalScroller.RegisterCallback<GeometryChangedEvent>(onScrollersGeometryChanged);
            horizontalScroller.RegisterCallback<GeometryChangedEvent>(onScrollersGeometryChanged);

            var horizontalDragger = getSliderDragElement(horizontalScroller.slider);
            var verticalDragger = getSliderDragElement(verticalScroller.slider);

            horizontalDragger.RegisterCallback<GeometryChangedEvent>(onHorizontalScrollDragElementChanged);
            verticalDragger.RegisterCallback<GeometryChangedEvent>(onVerticalScrollDragElementChanged);

            // These are used to clamp if elastic and to handle focus navigation.
            // See Base Slider: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/UIElements/Core/Controls/BaseSlider.cs#L385
            verticalScroller.lowButton.GetClickable().clicked += onRepeatClick;
            verticalScroller.lowButton.RegisterCallback<NavigationSubmitEvent>(onScrollerButtonSubmit);
            verticalScroller.highButton.GetClickable().clicked += onRepeatClick;
            verticalScroller.highButton.RegisterCallback<NavigationSubmitEvent>(onScrollerButtonSubmit);
            verticalScroller.RegisterCallback<NavigationMoveEvent>(onNavigationMoveOnSlider, TrickleDown.TrickleDown);
            verticalScroller.RegisterCallback<KeyDownEvent>(onKeyDownOnSlider, TrickleDown.TrickleDown);
            verticalScroller.RegisterCallback<KeyUpEvent>(onKeyUpOnSlider, TrickleDown.TrickleDown);

            verticalDragger.RegisterCallback<PointerUpEvent>(onPointerUpOnSliderDragger); // TODO: investigate why this is not triggering.

            horizontalScroller.lowButton.GetClickable().clicked += onRepeatClick;
            horizontalScroller.lowButton.RegisterCallback<NavigationSubmitEvent>(onScrollerButtonSubmit);
            horizontalScroller.highButton.GetClickable().clicked += onRepeatClick;
            horizontalScroller.highButton.RegisterCallback<NavigationSubmitEvent>(onScrollerButtonSubmit);
            horizontalScroller.RegisterCallback<NavigationMoveEvent>(onNavigationMoveOnSlider, TrickleDown.TrickleDown);
            horizontalScroller.RegisterCallback<KeyDownEvent>(onKeyDownOnSlider, TrickleDown.TrickleDown);
            horizontalScroller.RegisterCallback<KeyUpEvent>(onKeyUpOnSlider, TrickleDown.TrickleDown);

            horizontalDragger.RegisterCallback<PointerUpEvent>(onPointerUpOnSliderDragger); // TODO: investigate why this is not triggering.

            registerEventsForInfinity();

            // Focus Fix:
            // Since the scroller are not focusable (not in contentContainer) we have to find another way to make them selectable.
            // We check every navigation event and if it is at the edge and near the scroller then cancel the default and select the scroller instead.
            // If there is no focusable content within the contentContainer (like a long text without any buttons) then select the scroller upon focus.
            // Though, to allow focusing children inside the ScrollView we have to make the scrollview itself non-focusable while the focus is inside.
            this.focusable = selectableScrollbars;
            this.RegisterCallback<FocusInEvent>(onFocusIn, TrickleDown.TrickleDown);
            this.RegisterCallback<FocusOutEvent>(onFocusOut, TrickleDown.TrickleDown);
            contentContainer.RegisterCallback<NavigationMoveEvent>(onNavigationMoveInContentContainer);

            // Retain scrollOffset if display is set to none fix
            horizontalScroller.RegisterCallback<ChangeEvent<float>>(onHorizontalScrollerValueChanged);
            verticalScroller.RegisterCallback<ChangeEvent<float>>(onVerticalScrollerValueChanged);
            contentContainer.RegisterCallback<GeometryChangedEvent>(onScrollViewGeometryChangedCheckOffset);
        }

        // We use these to check when the scroll view is being hidden.
        // We then save the scrollOffset to reapply it later. This fixes a bug, see:
        // https://forum.unity.com/threads/scrollview-loses-scroll-position-after-hide-and-show-display-none-display-flex.1084706/
        protected float? _lastDisplayedScrollOffsetX;
        protected float? _lastDisplayedScrollOffsetY;

        protected void onHorizontalScrollerValueChanged(ChangeEvent<float> evt)
        {
            if (resolvedStyle.display == DisplayStyle.None && !_lastDisplayedScrollOffsetX.HasValue)
            {
                _lastDisplayedScrollOffsetX = evt.previousValue;
            }
            else
            {
                _lastDisplayedScrollOffsetX = null;
            }
        }

        protected void onVerticalScrollerValueChanged(ChangeEvent<float> evt)
        {
            if (resolvedStyle.display == DisplayStyle.None && !_lastDisplayedScrollOffsetY.HasValue)
            {
                _lastDisplayedScrollOffsetY = evt.previousValue;
            }
            else
            {
                _lastDisplayedScrollOffsetY = null;
            }
        }

        protected void onScrollViewGeometryChangedCheckOffset(GeometryChangedEvent evt)
        {
            var offset = scrollOffset;
            if (_lastDisplayedScrollOffsetX.HasValue)
            {
                offset.y = _lastDisplayedScrollOffsetX.Value;
                _lastDisplayedScrollOffsetX = null;
            }
            if (_lastDisplayedScrollOffsetY.HasValue)
            {
                offset.y = _lastDisplayedScrollOffsetY.Value;
                _lastDisplayedScrollOffsetY = null;
            }
            scrollOffset = offset;
        }

        protected static float GetClosestPowerOfTen(float positiveNumber)
        {
            if (positiveNumber <= 0)
                return 1;
            return Mathf.Pow(10, Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
        }

        protected void onKeyDownOnSlider(KeyDownEvent evt)
        {
            clampElastic();
        }

        protected void onKeyUpOnSlider(KeyUpEvent evt)
        {
            clampElastic();

            // Execute snap delayed (we have to wait or else the SET of the RepeatButton will override the Snap animation).
            if (snap)
                SnapDelayed();
        }

        protected void onPointerUpOnSliderDragger(PointerUpEvent evt)
        {
            // Execute snap delayed (we have to wait or else the SET of the RepeatButton will override the Snap animation).
            if (snap) 
                SnapDelayed();
        }

        protected void onRepeatClick()
        {
            clampElastic();

            // Execute snap delayed (we have to wait or else the SET of the RepeatButton will override the Snap animation).
            if (snap)
                SnapDelayed();
        }

        protected void onScrollerButtonSubmit(EventBase evt)
        {
            clampElastic();

            // Execute snap delayed (we have to wait or else the SET of the RepeatButton will override the Snap animation).
            if (snap)
                SnapDelayed();
        }

        protected void clampElastic()
        {
            if (touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic)
            {
                // Looks bad and does not work fast enough for repeat. N2H: Improve in future release.
                // if (needsClamping())
                //     startInertiaAndElasticityAnimation();

                clampScrollOffset();
            }
        }

        /*
        protected bool needsClamping()
        {
            if (   scrollOffset.x < 0
                || scrollOffset.y < 0
                || ((mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal) && scrollOffset.y > scrollableHeight)
                || ((mode == ScrollViewMode.Horizontal || mode == ScrollViewMode.VerticalAndHorizontal) && scrollOffset.x > scrollableWidth)
                )
            {
                return true;
            }

            return false;
        }
        */

        protected void clampScrollOffset()
        {
            if (scrollOffset.x < 0)
                scrollOffset = new Vector2(0, scrollOffset.y);

            if (scrollOffset.y < 0)
                scrollOffset = new Vector2(scrollOffset.x, 0);

            if ((mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal) && scrollOffset.y > scrollableHeight)
                scrollOffset = new Vector2(scrollOffset.x, scrollableHeight);

            if ((mode == ScrollViewMode.Horizontal || mode == ScrollViewMode.VerticalAndHorizontal) && scrollOffset.x > scrollableWidth)
                scrollOffset = new Vector2(scrollableWidth, scrollOffset.y);
        }

        protected VisualElement getSliderDragElement(Slider slider)
        {
            return slider.Q(ScrollerDragElementName);
        }

        protected void calculateBounds()
        {
            _lowBounds = new Vector2(
                Mathf.Min(horizontalScroller.lowValue, horizontalScroller.highValue),
                Mathf.Min(verticalScroller.lowValue, verticalScroller.highValue));

            _highBounds = new Vector2(
                Mathf.Max(horizontalScroller.lowValue, horizontalScroller.highValue),
                Mathf.Max(verticalScroller.lowValue, verticalScroller.highValue));
        }

        protected void SetScrollViewMode(ScrollViewMode mode)
        {
            _mode = mode;

            // Ensure the correct styles are used
            RemoveFromClassList(verticalVariantUssClassName);
            RemoveFromClassList(horizontalVariantUssClassName);
            RemoveFromClassList(verticalHorizontalVariantUssClassName);
            RemoveFromClassList(scrollVariantUssClassName);

            switch (mode)
            {
                case ScrollViewMode.Vertical:
                    AddToClassList(verticalVariantUssClassName);
                    AddToClassList(scrollVariantUssClassName);
                    break;

                case ScrollViewMode.Horizontal:
                    AddToClassList(horizontalVariantUssClassName);
                    AddToClassList(scrollVariantUssClassName);
                    break;

                case ScrollViewMode.VerticalAndHorizontal:
                    AddToClassList(verticalHorizontalVariantUssClassName);
                    AddToClassList(scrollVariantUssClassName);
                    break;
            }
        }

        protected virtual void onAttachToPanel(AttachToPanelEvent evt)
        {
            if (evt.destinationPanel == null || evt.destinationPanel.contextType != ContextType.Player)
                return;

            RefreshFocusables();

            contentContainer.RegisterCallback<PointerDownEvent>(onPointerDown, TrickleDown.TrickleDown);
            contentContainer.RegisterCallback<PointerMoveEvent>(onPointerMove, TrickleDown.NoTrickleDown);
            contentContainer.RegisterCallback<PointerCancelEvent>(onPointerCancel, TrickleDown.TrickleDown);
            contentContainer.RegisterCallback<PointerUpEvent>(onPointerUp, TrickleDown.TrickleDown);
            contentContainer.RegisterCallback<PointerCaptureOutEvent>(onPointerCaptureOut, TrickleDown.TrickleDown);
            contentContainer.RegisterCallback<WheelEvent>(onMouseWheelUsed, TrickleDown.TrickleDown);

            // We use this to allow dragging to start in infinite scroll views even if a click on the background happens.
            contentViewport.RegisterCallback<PointerDownEvent>(onPointerDownOnViewport, TrickleDown.TrickleDown);

            RefreshTrackedChildren();

            // Track changes to the scrollers (we need these to abort animations if the scrollers are used).
            horizontalScroller?.RegisterCallback<PointerDownEvent>(onHorizontalScrollerPointerDown, TrickleDown.TrickleDown);
            verticalScroller?.RegisterCallback<PointerDownEvent>(onVerticalScrollerPointerDown, TrickleDown.TrickleDown);
        }

        public void RefreshAfterHierarchyChange()
        {
            RefreshTrackedChildren();
            RefreshFocusables();
        }

        /// <summary>
        /// Call after adding or removing children dynamically.
        /// </summary>
        public void RefreshTrackedChildren()
        {
            // Track clicks on children to support dragging on them.
            List<VisualElement> children = contentContainer.Query().ToList();
            foreach (var child in children)
            {
                if (child == contentContainer)
                    continue;

                if (child is ScrollView)
                    continue;

                if (child is ScrollViewPro)
                    continue;

                if (isInsideOtherScrollView(child))
                    continue;

                if (child.GetFirstAncestorOfType<Scroller>() != null)
                    continue;

                if (!_trackedChildren.Contains(child))
                {
                    _trackedChildren.Add(child);

                    child.RegisterCallback<PointerDownEvent>(onPointerDownOnChild, TrickleDown.TrickleDown);
                    child.RegisterCallback<PointerMoveEvent>(onPointerMoveOnChild, TrickleDown.NoTrickleDown);
                    child.RegisterCallback<PointerCancelEvent>(onPointerCancelOnChild, TrickleDown.TrickleDown);
                    child.RegisterCallback<PointerUpEvent>(onPointerUpOnChild, TrickleDown.TrickleDown);
                }
            }
        }

        protected bool isInsideOtherScrollView(VisualElement ve)
        {
            var scrollViewPro = ve.GetFirstAncestorOfType<ScrollViewPro>();
            if (scrollViewPro != null && scrollViewPro != this)
                return true;

            var scrollView = ve.GetFirstAncestorOfType<ScrollView>();
            if (scrollView != null)
                return true;

            return false;
        }

        protected virtual void onDetachFromPanel(DetachFromPanelEvent evt)
        {
            if (evt.originPanel == null || evt.originPanel.contextType != ContextType.Player)
                return;

            contentContainer.UnregisterCallback<PointerDownEvent>(onPointerDown, TrickleDown.TrickleDown);
            contentContainer.UnregisterCallback<PointerMoveEvent>(onPointerMove, TrickleDown.TrickleDown);
            contentContainer.UnregisterCallback<PointerCancelEvent>(onPointerCancel, TrickleDown.TrickleDown);
            contentContainer.UnregisterCallback<PointerUpEvent>(onPointerUp, TrickleDown.TrickleDown);
            contentContainer.UnregisterCallback<PointerCaptureOutEvent>(onPointerCaptureOut, TrickleDown.TrickleDown);

            contentViewport.UnregisterCallback<PointerDownEvent>(onPointerDownOnViewport, TrickleDown.TrickleDown);

            // Release tracked children
            foreach (var child in _trackedChildren)
            {
                child.UnregisterCallback<PointerDownEvent>(onPointerDownOnChild, TrickleDown.TrickleDown);
                child.UnregisterCallback<PointerMoveEvent>(onPointerMoveOnChild, TrickleDown.TrickleDown);
                child.UnregisterCallback<PointerCancelEvent>(onPointerCancelOnChild, TrickleDown.TrickleDown);
                child.UnregisterCallback<PointerUpEvent>(onPointerUpOnChild, TrickleDown.TrickleDown);
            }
            _trackedChildren.Clear();

            // Release tracked scrollers
            horizontalScroller?.UnregisterCallback<PointerDownEvent>(onHorizontalScrollerPointerDown, TrickleDown.TrickleDown);
            verticalScroller?.UnregisterCallback<PointerDownEvent>(onVerticalScrollerPointerDown, TrickleDown.TrickleDown);

            // Cancel any ongoing animations.
            _scrollToAnimation?.Pause();
            _scrollToAnimation = null;
        }

        protected int _lastKnownChildCountInContentContainer = -1;

        protected void onScrollViewGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.oldRect.size != evt.newRect.size)
            {
                updateScrollers(needsHorizontal || isHorizontalScrollDisplayed,
                                needsVertical || isVerticalScrollDisplayed,
                                scrollerButtons);
                applyScrollOffset();

                refreshChildInfosInInfiniteScollViewIfNeeded();
            }

            var ve = evt.target as VisualElement;
            if (ve.childCount != _lastKnownChildCountInContentContainer)
            {
                _lastKnownChildCountInContentContainer = ve.childCount;
                RefreshAfterHierarchyChange();
            }
        }

        protected void adjustScrollers()
        {
            float horizontalFactor = ((contentContainer.GetBoundingBox().width > 1E-30f) ? (contentViewport.layout.width / contentContainer.GetBoundingBox().width) : 1f);
            float verticalFactor = ((contentContainer.GetBoundingBox().height > 1E-30f) ? (contentViewport.layout.height / contentContainer.GetBoundingBox().height) : 1f);
            horizontalScroller.Adjust(horizontalFactor);
            verticalScroller.Adjust(verticalFactor);
        }

        private StyleEnum<Overflow> _overflowBeforeFix;

        private void overflowWorldBoundsFix()
        {
            if (contentContainer.style.overflow == Overflow.Visible)
                contentContainer.style.overflow = _overflowBeforeFix;
        }

        protected void updateScrollers(bool displayHorizontal, bool displayVertical, bool showButtons)
        {
            adjustScrollers();

            // Experimental fix: contentContainer.worldBounds are not updating automatically
            // if the container uses flex grid (or similar) to layout children. We attempt a fix
            // by setting the overflow to "visible" which triggers a re-calculation and then reset
            // it to whatever it was. The only downside is we might overwrite user style changes to
            // contentContainer.style.overflow with this. TODO: find the root cause and fix it.
            _overflowBeforeFix = contentContainer.style.overflow;
            contentContainer.style.overflow = Overflow.Visible;
            schedule.Execute(overflowWorldBoundsFix);

            // Set availability
            horizontalScroller.SetEnabled(contentContainer.GetBoundingBox().width - contentViewport.layout.width > 0);
            verticalScroller.SetEnabled(contentContainer.GetBoundingBox().height - contentViewport.layout.height > 0);

            var newShowHorizontal = displayHorizontal && _horizontalScrollerVisibility != ScrollerVisibility.Hidden;
            var newShowVertical = displayVertical && _verticalScrollerVisibility != ScrollerVisibility.Hidden;
            var newHorizontalDisplay = newShowHorizontal ? DisplayStyle.Flex : DisplayStyle.None;
            var newVerticalDisplay = newShowVertical ? DisplayStyle.Flex : DisplayStyle.None;

            // Set display as necessary
            if (newHorizontalDisplay != horizontalScroller.style.display)
            {
                horizontalScroller.style.display = newHorizontalDisplay;
            }
            if (newVerticalDisplay != verticalScroller.style.display)
            {
                verticalScroller.style.display = newVerticalDisplay;
            }

            // Need to set always, for touch scrolling.
            verticalScroller.lowValue = 0f;
            verticalScroller.highValue = scrollableHeight;
            horizontalScroller.lowValue = 0f;
            horizontalScroller.highValue = scrollableWidth;

            if (!needsVertical || !(scrollableHeight > 0f))
            {
                // Make sure a value change is triggered (we use a very small value for this).
                verticalScroller.value = verticalScroller.value + 0.001f;
            }

            if (!needsHorizontal || !(scrollableWidth > 0f))
            {
                // Make sure a value change is triggered (we use a very small value for this).
                horizontalScroller.value = horizontalScroller.value + 0.001f;
            }

            // Buttons (show/hide and add/remove margins)
            if (showButtons != (horizontalScroller.lowButton.style.display == DisplayStyle.Flex))
            {
                horizontalScroller.slider.style.marginLeft = showButtons ? horizontalScroller.lowButton.style.width : 0f;
                horizontalScroller.slider.style.marginRight = showButtons ? horizontalScroller.highButton.style.width : 0f;
            }
            horizontalScroller.lowButton.style.display = showButtons ? DisplayStyle.Flex : DisplayStyle.None;
            horizontalScroller.highButton.style.display = showButtons ? DisplayStyle.Flex : DisplayStyle.None;

            if (showButtons != (verticalScroller.lowButton.style.display == DisplayStyle.Flex))
            {
                verticalScroller.slider.style.marginTop = showButtons ? verticalScroller.lowButton.style.height : 0f;
                verticalScroller.slider.style.marginBottom = showButtons ? verticalScroller.highButton.style.height : 0f;
            }
            verticalScroller.lowButton.style.display = showButtons ? DisplayStyle.Flex : DisplayStyle.None;
            verticalScroller.highButton.style.display = showButtons ? DisplayStyle.Flex : DisplayStyle.None;
        }

        protected void onScrollersGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.oldRect.size != evt.newRect.size)
            {
                if (needsHorizontal && _horizontalScrollerVisibility != ScrollerVisibility.Hidden)
                {
                    horizontalScroller.style.marginRight = verticalScroller.layout.width;
                }

                adjustScrollers();
            }
        }

        protected IVisualElementScheduledItem _scrollWheelScheduledAnimation;

        protected void onScrollWheel(WheelEvent evt)
        {
            StopAnimations();
            var canUseVerticalScroll = mode != ScrollViewMode.Horizontal && contentContainer.GetBoundingBox().height - layout.height > 0;
            var canUseHorizontalScroll = mode != ScrollViewMode.Vertical && contentContainer.GetBoundingBox().width - layout.width > 0;
            var verticalScrollDelta = evt.delta.y;
            if (canUseHorizontalScroll && evt.shiftKey)
            {
                verticalScrollDelta = 0f;
            }
            // If only horizontal scrolling is eabled then use the delta.y, otherwise fall back on delta.x for any device that can scroll
            // in two directions at the same time. However, if isWheelScrollHorizontallyKeyPressed is true then prefer horizontal scrolling.
            var horizontalScrollDelta = canUseHorizontalScroll && (!canUseVerticalScroll || evt.shiftKey) ? evt.delta.y : evt.delta.x;
            float mouseScrollFactor = mouseWheelScrollSize;
            if (_mouseWheelScrollSizeFromStyle != UndefinedFloatStyleValue)
                mouseScrollFactor = _mouseWheelScrollSizeFromStyle;

            bool scrollOffsetDidChange = false;

            if (canUseVerticalScroll)
            {
                var oldVerticalValue = verticalScroller.value;
                verticalScroller.value += verticalScrollDelta * (verticalScroller.lowValue < verticalScroller.highValue ? 1f : -1f) * mouseScrollFactor;

                if (/*nestedInteractionKind == NestedInteractionKind.StopScrolling ||*/ !Mathf.Approximately(verticalScroller.value, oldVerticalValue))
                {
                    evt.StopPropagation();
                    scrollOffsetDidChange = true;
                }
            }

            if (canUseHorizontalScroll)
            {
                var oldHorizontalValue = horizontalScroller.value;
                horizontalScroller.value += horizontalScrollDelta * (horizontalScroller.lowValue < horizontalScroller.highValue ? 1f : -1f) * mouseScrollFactor;

                if (/*nestedInteractionKind == NestedInteractionKind.StopScrolling || */ !Mathf.Approximately(horizontalScroller.value, oldHorizontalValue))
                {
                    evt.StopPropagation();
                    scrollOffsetDidChange = true;
                }
            }

            // Elastic for mouse wheel
            if (scrollOffsetDidChange)
            {
                // Update elastic behavior
                if (touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic)
                {
                    calculateBounds();
                    // Delay for as long as there are new scroll events. If none are left then execute.
                    if (_scrollWheelScheduledAnimation == null)
                        _scrollWheelScheduledAnimation = schedule.Execute(startInertiaAndElasticityAnimation);
                    _scrollWheelScheduledAnimation.ExecuteLater(delayMs: 50);
                }

                applyScrollOffset();
            }
        }

        protected void onHorizontalScrollerPointerDown(PointerDownEvent evt)
        {
            StopAnimations();
        }

        protected void onVerticalScrollerPointerDown(PointerDownEvent evt)
        {
            StopAnimations();
        }

        protected bool isChildOverlappingViewport(VisualElement child)
        {
            return areOverlapping(child, contentViewport);
        }

        protected bool areOverlapping(VisualElement a, VisualElement b)
        {
            return a.worldBound.Overlaps(b.worldBound);
        }

        protected bool isChildCompletelyInViewport(VisualElement child)
        {
            return isChildCompletelyInViewportY(child) && isChildCompletelyInViewportX(child);
        }

        protected bool isChildCompletelyInViewportX(VisualElement child)
        {
            float posX = contentContainer.transform.position.x * -1;

            var viewBounds = contentViewport.worldBound;
            float viewMin = viewBounds.xMin + posX;
            float viewMax = viewBounds.xMax + posX;

            // Child bounds with padding
            var childBounds = child.worldBound;
            float childMin = childBounds.xMin + posX;
            float childMax = childBounds.xMax + posX;

            if (float.IsNaN(childMin) || float.IsNaN(childMax))
                return false;

            // If child is inside view port
            if (childMin >= viewMin && childMax <= viewMax)
                return true;

            return false;
        }

        protected bool isChildCompletelyInViewportY(VisualElement child)
        {
            float posY = contentContainer.transform.position.y * -1;

            var viewBounds = contentViewport.worldBound;
            float viewMin = viewBounds.yMin + posY;
            float viewMax = viewBounds.yMax + posY;

            // Child bounds with padding
            var childBounds = child.worldBound;
            float childMin = childBounds.yMin + posY;
            float childMax = childBounds.yMax + posY;

            if (float.IsNaN(childMin) || float.IsNaN(childMax))
                return false;

            // If child is inside view port
            if (childMin >= viewMin && childMax <= viewMax)
                return true;

            return false;
        }

        protected bool isChildInScrollView(VisualElement child)
        {
            float posY = this.transform.position.y * -1;

            var viewBounds = this.worldBound;
            float viewMin = viewBounds.yMin + posY;
            float viewMax = viewBounds.yMax + posY;

            // Child bounds with padding
            var childBounds = child.worldBound;
            float childMin = childBounds.yMin + posY;
            float childMax = childBounds.yMax + posY;

            if (float.IsNaN(childMin) || float.IsNaN(childMax))
                return false;

            // If child is inside
            if (childMin >= viewMin && childMax <= viewMax)
                return true;

            return false;
        }
    }
}
