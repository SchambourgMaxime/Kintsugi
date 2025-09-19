using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    // Focus snap means that if a keyboard or controller is used then
    // the currently focused element will be animated into view.

    // The focus algorithm works similar to Unity's own focusing.
    // It will prefer focus targets in the direction of the last 
    // NavigationMove event. If no match is found then it will
    // fall back on using the _focusables list based on an index.

    // TODO: Replace with a focusable list that contains everything in the child (including child scroll views)
    // and then do dynamic discovery of next selection with a custom focusing algorithm (nav dicrection based)

    public partial class ScrollViewPro
    {
        protected List<VisualElement> _focusables = new List<VisualElement>();
        protected List<VisualElement> _childScrollViews = new List<VisualElement>();


        const bool DefaultFocusSnap = false;
        protected bool _focusSnap = DefaultFocusSnap;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap")]
#endif
        public bool focusSnap
        {
            get
            {
                return _focusSnap;
            }

            set
            {
                if (_focusSnap == value)
                    return;

                _focusSnap = value;

#if UNITY_EDITOR
                if (value && selectableScrollbars)
                {
                    Debug.LogWarning("ScrollViewPro: You should NOT enable 'focusSnap' and 'selectableScrollbars' at the same time. These two can create ambiguous situations.");
                }
#endif
            }
        }

        const bool DefaultFocusSnapOnPointer = false;
        protected bool _focusSnapOnPointer = DefaultFocusSnapOnPointer;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-on-pointer")]
#endif
        public bool focusSnapOnPointer
        {
            get
            {
                return _focusSnapOnPointer;
            }

            set
            {
                _focusSnapOnPointer = value;
            }
        }

        const float DefaultFocusSnapDurationSec = 0.4f;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-duration-sec")]
#endif
        public float focusSnapDurationSec { get; set; } = DefaultFocusSnapDurationSec;

        const ScrollToAlign DefaultFocusSnapAlignX = ScrollToAlign.Visible;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-align-x")]
#endif
        public ScrollToAlign focusSnapAlignX { get; set; } = DefaultFocusSnapAlignX;

        const ScrollToAlign DefaultFocusSnapAlignY = ScrollToAlign.Visible;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-align-y")]
#endif
        public ScrollToAlign focusSnapAlignY { get; set; } = DefaultFocusSnapAlignY;

        const Easing DefaultFocusSnapEase = Easing.Ease;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-ease")]
#endif
        public Easing focusSnapEase { get; set; } = DefaultFocusSnapEase;

        static Vector4 DefaultFocusSnapMargin = Vector4.zero;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-margin")]
#endif
        public Vector4 focusSnapMargin { get; set; } = DefaultFocusSnapMargin;

        static bool DefaultFocusSnapIncludeMargin = false;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-include-margin")]
#endif
        public bool focusSnapIncludeMargin { get; set; } = DefaultFocusSnapIncludeMargin;

        const bool DefaultFocusSnapInside = false;
        /// <summary>
        /// Should the snap scroll animation (alignment) be applied even if the element is
        /// already fully visible?
        /// Enabling this is handy in combination with ScrollViewPro.ScrollToAlignment.Center.
        /// Together they make sure the selected element is in the center of your scroll view.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-inside")]
#endif
        public bool focusSnapInside { get; set; } = DefaultFocusSnapInside;

        const bool DefaultInfiniteValue = false;

        protected bool _infinite = DefaultInfiniteValue;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("infinite")]
#endif
        public bool infinite
        {
            get => _infinite;
            set
            {
                _infinite = value;

                if (_infinite)
                {
                    horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                    verticalScrollerVisibility = ScrollerVisibility.Hidden;
                    touchScrollBehavior = ScrollView.TouchScrollBehavior.Unrestricted;

                    contentContainer.style.overflow = Overflow.Visible;

                    contentViewport.pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
                }
            }
        }

        const float DefaultFocusSnapRepetitionDelay = 0f;
        /// <summary>
        /// The repetition delay can be used to slow down how fast users can switch the focus.
        /// Especially handy for infinite scroll view when we may have to wait for the scroll to
        /// animation and repositioning to finish.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focus-snap-repetition-delay")]
#endif
        public float focusSnapRepetitionDelay { get; set; } = DefaultFocusSnapRepetitionDelay;

        protected float _lastFocusTime;

        protected int _focusedIndex = -1;

        /// <summary>
        /// Call this to update the list of focusable elements.
        /// Call this after adding or moving elements dynamically.
        /// </summary>
        public void RefreshFocusables()
        {
            if (panel.contextType != ContextType.Player)
                return;

            unregisterFocusListners(_focusables);
            _focusables.Clear();

            contentContainer
                .Query<VisualElement>()
                .Where(isFocusableChild)
                .ForEach(ve => _focusables.Add(ve));

            registerFocusListners(_focusables);

            // Sort by position
            if (_focusables.Count > 1)
            {
                if (hasValidLayout())
                    sortAllFocusables();
                else
                    schedule.When(hasValidLayout, sortAllFocusables);
            }

            contentContainer
                .Query<ScrollViewPro>()
                .Where((child) => !isInsideOtherScrollView(child))
                .ForEach(ve => _childScrollViews.Add(ve));

            contentContainer
                .Query<ScrollView>()
                .Where((child) => !isInsideOtherScrollView(child))
                .ForEach(ve => _childScrollViews.Add(ve));
        }

        protected bool hasValidLayout()
        {
            return !float.IsNaN(_focusables[0].resolvedStyle.width);
        }

        protected void sortAllFocusables()
        {
            sortAllFocusables(null, null);
        }

        private Dictionary<VisualElement, float> _tmpSortAllFocusablesUnappliedInfinityPositionChangesX;
        private Dictionary<VisualElement, float> _tmpSortAllFocusablesUnappliedInfinityPositionChangesY;

        /// <summary>
        /// Sort all the focusables based on their position.
        /// </summary>
        /// <param name="unappliedInfinityPositionChangesX">
        /// A dictionary containing any changes made to the "style.left/top" of
        /// the visual elements that have not yet been synced to the worldBounds.
        /// </param>
        protected void sortAllFocusables(
            Dictionary<VisualElement, float> unappliedInfinityPositionChangesX,
            Dictionary<VisualElement, float> unappliedInfinityPositionChangesY
            )
        {
            _tmpSortAllFocusablesUnappliedInfinityPositionChangesX = unappliedInfinityPositionChangesX;
            _tmpSortAllFocusablesUnappliedInfinityPositionChangesY = unappliedInfinityPositionChangesY;
            _focusables.Sort(sortFocusables);

            // Update index after reordering the focusables.
            for (int i = 0; i < _focusables.Count; i++)
            {
                if (_focusables[i].focusController.focusedElement == _focusables[i])
                {
                    _focusedIndex = i;
                }
            }

            // refresh index
            var ve = panel.focusController.focusedElement as VisualElement;
            if (ve != null && _focusables.Contains(ve))
            {
                _focusedIndex = _focusables.IndexOf(ve);
            }
        }

        protected float getFromDict(Dictionary<VisualElement, float> dict, VisualElement ve, float defaultValue = 0f)
        {
            if (dict != null && dict.ContainsKey(ve))
                return dict[ve];

            return defaultValue;
        }

        protected int sortFocusables(VisualElement a, VisualElement b)
        {
            if (mode == ScrollViewMode.Vertical)
            {
                float aMin = a.worldBound.yMin;
                aMin += getFromDict(_tmpSortAllFocusablesUnappliedInfinityPositionChangesY, a, 0f);

                float bMin = b.worldBound.yMin;
                bMin += getFromDict(_tmpSortAllFocusablesUnappliedInfinityPositionChangesY, b, 0f);

                return (int)Mathf.Sign(aMin - bMin);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                float aMin = a.worldBound.xMin;
                aMin += getFromDict(_tmpSortAllFocusablesUnappliedInfinityPositionChangesX, a, 0f);

                float bMin = b.worldBound.xMin;
                bMin += getFromDict(_tmpSortAllFocusablesUnappliedInfinityPositionChangesX, b, 0f);

                return (int)Mathf.Sign(aMin - bMin);
            }

            return 0;
        }

        /// <summary>
        /// Returns true only if the child is an element of THIS scroll
        /// view's content container.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        protected bool isFocusableChild(VisualElement child)
        {
            if (!child.canGrabFocus)
                return false;

            if (isInsideOtherScrollView(child))
                return false;

            if (!child.IsChildOf(contentContainer))
                return false;

            return true;
        }

        protected void registerFocusListners(List<VisualElement> elements)
        {
            foreach (var ve in elements)
            {
                ve.RegisterCallback<FocusEvent>(onElementFocused, TrickleDown.TrickleDown);
            }

            contentContainer.RegisterCallback<NavigationMoveEvent>(onNavigationMoveOnContent);
        }

        protected void unregisterFocusListners(List<VisualElement> elements)
        {
            foreach (var ve in elements)
            {
                ve.UnregisterCallback<FocusEvent>(onElementFocused, TrickleDown.TrickleDown);
            }

            contentContainer.UnregisterCallback<NavigationMoveEvent>(onNavigationMoveOnContent);
        }

        protected void onElementFocused(FocusEvent evt)
        {
            if (_focusables.Count == 0)
                RefreshFocusables();

            var ve = evt.target as VisualElement;

            _focusedIndex = _focusables.IndexOf(ve);

            if (!focusSnap)
                return;

            // Do not animate if focused element is already in the viewport.
            if (!focusSnapInside && isChildCompletelyInViewport(ve))
                return;

            // Do not react to focus changes due to dragging or pointing on children (if focusSnapOnPointer is disabled).
            if (!isDragging && (!_pointerDownOnChild || focusSnapOnPointer))
            {
                snapToAnimated(ve, true);
            }
        }

        public void snapToAnimated(VisualElement child, bool allowChildMargin = true)
        {
            if (!allowChildMargin)
            {
                // no margins at all
                ScrollToAnimated(child, focusSnapDurationSec, focusSnapEase, focusSnapAlignX, focusSnapAlignY, null);

                // Event
                SnapEvent.Dispatch(child, this);
            }

            var margin = focusSnapMargin;

            // Should we add the child margin in addition to the focusSnapMargin?
            if (focusSnapIncludeMargin)
            {
                margin.x += child.resolvedStyle.marginLeft;
                margin.y += child.resolvedStyle.marginTop;
                margin.z += child.resolvedStyle.marginRight;
                margin.w += child.resolvedStyle.marginBottom;
            }

            ScrollToAnimated(child, focusSnapDurationSec, focusSnapEase, focusSnapAlignX, focusSnapAlignY, margin);

            // Event
            SnapEvent.Dispatch(child, this);
        }

        // The default Unity selection skips children outside the viewport (unless we use TAB).
        // We use the navigation event to overwrite that behaviour and select off-screen elements too.
        protected void onNavigationMoveOnContent(NavigationMoveEvent evt)
        {
            if (!focusSnap)
                return;

            if (mode == ScrollViewMode.Horizontal && (evt.direction == NavigationMoveEvent.Direction.Up || evt.direction == NavigationMoveEvent.Direction.Down || evt.direction == NavigationMoveEvent.Direction.None))
                return;

            if (mode == ScrollViewMode.Vertical && (evt.direction == NavigationMoveEvent.Direction.Left || evt.direction == NavigationMoveEvent.Direction.Right || evt.direction == NavigationMoveEvent.Direction.None))
                return;

            // The repetition delay can be used to slow down how fast users can switch the focus.
            // Especially handy for infinite scroll views to wait for the scroll to animation
            // and repositioning to finish.
            if (focusSnapRepetitionDelay > 0.001f)
            {
                if (Time.realtimeSinceStartup - _lastFocusTime < focusSnapRepetitionDelay)
                {
                    evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                    focusController.IgnoreEvent(evt);
#else
                    evt.PreventDefault();
#endif
                    return;
                }
                _lastFocusTime = Time.realtimeSinceStartup;
            }

            // Make sure the focusables are sorted by position in infinite views.
            if (infinite && hasValidLayout())
            {
                /*
                foreach (var c in _focusables)
                {
                    Debug.Log("pre: " + c.name + " / " + (c.worldBound.x) + " / " + c.style.left.value.value + " / " + c.resolvedStyle.left + " / " + (c.worldBound.xMin + c.style.left.value.value));
                }
                */
                /*
                if (mode == ScrollViewMode.Horizontal)
                    updateChildPositionsInInfinityX(evt.direction == NavigationMoveEvent.Direction.Left ? 1f : (evt.direction == NavigationMoveEvent.Direction.Right ? -1f : 0f));
                if (mode == ScrollViewMode.Vertical)
                    updateChildPositionsInInfinityY(evt.direction == NavigationMoveEvent.Direction.Up ? 1f : (evt.direction == NavigationMoveEvent.Direction.Down ? -1f : 0f));
                
                sortAllFocusables();
                foreach (var c in _focusables)
                {
                    Debug.Log("post: " + c.name + " / " + (c.worldBound.x) + " / " + c.style.left.value.value + " / " + c.resolvedStyle.left + " / " + (c.worldBound.xMin + c.style.left.value.value));
                }
                */
            }

            var target = evt.target as VisualElement;

            // Skip if inside other scroll view, unless it is a scroller.
            if (isInsideOtherScrollView(target) && target.GetFirstAncestorOfType<Scroller>() == null)
                return;

            bool horizontalOrBoth = mode == ScrollViewMode.Horizontal || mode == ScrollViewMode.VerticalAndHorizontal;
            bool verticalOrBoth = mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal;

            bool positiveNavigationMovement =
                   evt.direction == NavigationMoveEvent.Direction.Down
                || evt.direction == NavigationMoveEvent.Direction.Right;

            bool negativeNavigationMovement =
                   evt.direction == NavigationMoveEvent.Direction.Up
                || evt.direction == NavigationMoveEvent.Direction.Left;

            // Problem:
            //
            // If a nested ScrollView is off screen and the next navigation movement should reveal it then
            // it will be skipped. Why? Because nested buttons are not part of _focusables AND the default
            // focus (GetNextFocusable()) ignores off-screen elements.
            // 
            // It also happens if the user is rapidly scrolling with the arrow keys (or controller) then the visual state
            // of the UI is lagging behind (ScrollToAnimated has not yet completed). This leads to GetNextFocusable()
            // returning wrong results as it only searches in elements that are already visible in the viewport.
            //
            // To fix this we are keeping track of the child scroll view and fall back on selection from them manually.
            // 
            // Related thread: https://forum.unity.com/threads/how-to-allow-off-screen-focus-with-navigation-events.1479567/

            // Get the focus candidate from the _focusables.
            VisualElement candidate = null;
            if (positiveNavigationMovement)
            {
                candidate = getPositiveFocusCandidate(evt.direction);
            }
            else if (negativeNavigationMovement)
            {
                candidate = getNegativeFocusCandidate(evt.direction);
            }

            // Check if we should use the default focus candidate.
            var next = focusController.GetNextFocusable(target, evt) as VisualElement;

            // Don't override navigation if next is in a child scroll view.
            bool isInOtherChildScrollView = next != null && next.IsChildOf(this) && !_focusables.Contains(next);
            if (isInOtherChildScrollView)
            {
                // Only use the default 'next' element if it is closer to the current selection than the 'candidate'.
                if (!isACloserToNavigationIntent(target, a: candidate, b: next, evt.direction, mode))
                {
                    // We want to scroll to this child view.
                    VisualElement childsParentScrollView = next.GetFirstAncestorOfType<ScrollViewPro>();
                    if (childsParentScrollView == null)
                        childsParentScrollView = next.GetFirstAncestorOfType<ScrollView>();
                    if (childsParentScrollView != null)
                    {
                        snapToAnimated(childsParentScrollView, false);
                    }

                    // But don't override.
                    return;
                }
            }

            // If next is focusable and in the right direction then use it.
            if (candidate != null && !isInOtherChildScrollView && _focusables.Contains(next)
                && isInNavigationDirection(target, next, evt.direction, allowSelf: false))
            {
                snapToAnimated(next, true);
                return;
            }

            // If the normal selection should be used then candidate will be null here.

            // We do have a candiate from default selection but there could be some other
            // elements that should be selected first (child scroll view, scroll bars).
            if (candidate != null)
            {
                // Now we have multiple options: candidate, scrollbars (if needed), childScrollViews.
                // We have to pick the closest one.
                var childScrollView = findClosest(evt, _childScrollViews);
                var closest = getClosestToNavigationIntent(target, evt.direction,
                    candidate,
                    childScrollView,
                    selectableScrollbars && usesVerticalScroller && verticalOrBoth ? verticalScroller.slider : null,
                    selectableScrollbars && usesHorizontalScroller && horizontalOrBoth ? horizontalScroller.slider : null
                    );

                if (closest != null)
                {
                    if (closest.canGrabFocus)
                    {
                        focusDuringNavigationEvent(evt, closest);
                    }
                    else
                    {
                        // Probably a child scroll view.
                        var scrollView = closest as ScrollViewPro;
                        if (scrollView != null)
                        {
                            snapToAnimated(scrollView, false);
                            scrollView.focusDuringNavigationEvent(evt, scrollView.findLastKnowFocusable());
                        }
                    }

                    return;
                }
            }

            // Is there a parent scroll view that should be notified?
            var parentScrollView = GetFirstAncestorOfType<ScrollViewPro>();
            if (parentScrollView != null)
            {
                parentScrollView.handleOnNavigationMoveFromChild(evt);
            }
        }

        /// <summary>
        /// Returns the last explicitly known focusable or searches for the first it can find in contentContainer.
        /// </summary>
        /// <returns>May return null.</returns>
        protected VisualElement findLastKnowFocusable()
        {
            if (_focusedIndex >= 0 && _focusables[_focusedIndex].canGrabFocus && _focusables[_focusedIndex].IsChildOf(contentContainer))
            {
                return _focusables[_focusedIndex];
            }
            
            var result = contentContainer
                .Query<VisualElement>()
                .Build()
                .FirstOrDefault((child) => child.canGrabFocus && contentContainer.Contains(child));

            return result;
        }

        protected VisualElement getPositiveFocusCandidate(NavigationMoveEvent.Direction direction)
        {
            VisualElement result;

            for (int i = _focusedIndex + 1; i < _focusables.Count; i++)
            {
                if (tryGetFocusableForIteration(_focusedIndex, i, direction, out result))
                    return result;
            }

            return null;
        }

        protected VisualElement getNegativeFocusCandidate(NavigationMoveEvent.Direction direction)
        {
            VisualElement result;

            for (int i = _focusedIndex - 1; i >= 0; i--)
            {
                if (tryGetFocusableForIteration(_focusedIndex, i, direction, out result))
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Returns the one that is closest with the navigation direction taken into account.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="direction"></param>
        /// <param name="candidates">Can contain null values. These will be ignored.</param>
        /// <returns></returns>
        protected VisualElement getClosestToNavigationIntent(VisualElement current, NavigationMoveEvent.Direction direction, params VisualElement[] candidates)
        {
            VisualElement result = null;

            // First pass:
            // We do  it like Unity, see https://github.com/Unity-Technologies/UnityCsReference/blob/2d0e5b524bcfbe13983d5245b4df16f68cf938b9/ModuleOverrides/com.unity.ui/Core/GameObjects/NavigateFocusRing.cs#L111
            Rect panelRect = panel.visualTree.worldBound;
            Rect currentRect = current.worldBound;

            // Grow to panel borders in the navigation direction.
            Rect validRect = new Rect(currentRect);
            if (direction == NavigationMoveEvent.Direction.Up) validRect.yMin = panelRect.yMin;
            else if (direction == NavigationMoveEvent.Direction.Down) validRect.yMax = panelRect.yMax;
            else if (direction == NavigationMoveEvent.Direction.Left) validRect.xMin = panelRect.xMin;
            else if (direction == NavigationMoveEvent.Direction.Right) validRect.xMax = panelRect.xMax;

            float minSqrDistance = float.MaxValue;
            foreach (var candidate in candidates)
            {
                if (candidate == null)
                    continue;

                Rect candidateRect = candidate.worldBound;
                if (candidateRect.Overlaps(validRect))
                {
                    float sqrDistance = getClosestSqrDistance(candidateRect, currentRect);
                    if(sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        result = candidate;
                    }
                }
            }

            // If we got a result in the first pass then skip and return.
            if (result != null)
                return result;

            // Second pass:
            // Do a broad search by BB distance.
            // TODO: Ignore those "behind" like in "isACloserToNavigationIntent".
            minSqrDistance = float.MaxValue;
            foreach (var candidate in candidates)
            {
                if (candidate == null)
                    continue;

                Rect candidateRect = candidate.worldBound;
                float sqrDistance = getClosestSqrDistance(candidateRect, currentRect);
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    result = candidate;
                }
            }

            return result;
        }

        protected bool isInNavigationDirection(VisualElement currentSelection, VisualElement target, NavigationMoveEvent.Direction navigationDirection, bool allowSelf = false)
        {
            if (target == null)
                return false;

            if (currentSelection == target && !allowSelf)
                return false;

            var pos = currentSelection.worldBound.position;
            var posTarget = target.worldBound.position;
            var distance = posTarget - pos;

            int navDirectionSign = (navigationDirection == NavigationMoveEvent.Direction.Down || navigationDirection == NavigationMoveEvent.Direction.Right) ? 1 : -1;

            // Horizontal
            if (navigationDirection == NavigationMoveEvent.Direction.Left || navigationDirection == NavigationMoveEvent.Direction.Right)
            {
                bool isInFront = (navDirectionSign > 0 && distance.x > 0) || (navDirectionSign < 0 && distance.x < 0);
                return isInFront;
            }
            // Vertical
            else // if (navigationDirection == NavigationMoveEvent.Direction.Up || navigationDirection == NavigationMoveEvent.Direction.Down)
            {
                bool isInFront = (navDirectionSign > 0 && distance.y > 0) || (navDirectionSign < 0 && distance.y < 0);
                return isInFront;
            }
        }

        /// <summary>
        /// Returns whether to prefer A or B as next selection with the navigation direction and mode taken in to account.<br />
        /// It ignores any element the is in the opposite direction of the navigation direction.
        /// </summary>
        /// <param name="currentSelection"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="navigationDirection"></param>
        /// <param name="mode"></param>
        /// <param name="preference">If set then this will be perfered if both are on the same side. If null then a distance check is used as tiebreaker.</param>
        /// <returns></returns>
        protected bool isACloserToNavigationIntent(
            VisualElement currentSelection, VisualElement a, VisualElement b, NavigationMoveEvent.Direction navigationDirection, ScrollViewMode mode)
        {
            if (a == null)
                return false;

            if (b == null)
                return true;

            var pos = currentSelection.worldBound.position;
            var posA = a.worldBound.position;
            var posB = b.worldBound.position;
            var distanceA = posA - pos;
            var distanceB = posB - pos;

            int navDirectionSign = (navigationDirection == NavigationMoveEvent.Direction.Down || navigationDirection == NavigationMoveEvent.Direction.Right) ? 1 : -1;

            if (mode == ScrollViewMode.Horizontal || navigationDirection == NavigationMoveEvent.Direction.Left || navigationDirection == NavigationMoveEvent.Direction.Right)
            {
                bool aIsBehind = (navDirectionSign > 0 && distanceA.x < 0) || (navDirectionSign < 0 && distanceA.x > 0);
                bool bIsBehind = (navDirectionSign > 0 && distanceB.x < 0) || (navDirectionSign < 0 && distanceB.x > 0);
                if (!aIsBehind && bIsBehind)
                {
                    return true;
                }
                else if (aIsBehind && !bIsBehind)
                {
                    return false;
                }
                else
                {
                    return Mathf.Abs(distanceA.x) < Mathf.Abs(distanceB.x);
                }
            }
            else if(mode == ScrollViewMode.Vertical || navigationDirection == NavigationMoveEvent.Direction.Up || navigationDirection == NavigationMoveEvent.Direction.Down)
            {
                bool aIsBehind = (navDirectionSign > 0 && distanceA.y < 0) || (navDirectionSign < 0 && distanceA.y > 0);
                bool bIsBehind = (navDirectionSign > 0 && distanceB.y < 0) || (navDirectionSign < 0 && distanceB.y > 0);
                if (!aIsBehind && bIsBehind)
                {
                    return true;
                }
                else if (aIsBehind && !bIsBehind)
                {
                    return false;
                }
                else
                {
                    return Mathf.Abs(distanceA.y) < Mathf.Abs(distanceB.y);
                }
            }
            else
            {
                return distanceA.sqrMagnitude < distanceB.sqrMagnitude;
            }
        }

        protected float getClosestSqrDistance(Rect a, Rect b)
        {
            float horizontalDistance = 0f;

            if (b.xMin > a.xMax)
            {
                horizontalDistance = b.xMin - a.xMax;
            }
            else if (a.xMin > b.xMax)
            {
                horizontalDistance = a.xMin - b.xMax;
            }

            float verticalDistance = 0f;

            if (b.yMin > a.yMax)
            {
                verticalDistance = b.yMin - a.yMax;
            }
            else if (a.yMin > b.yMax)
            {
                verticalDistance = a.yMin - b.yMax;
            }

            return horizontalDistance * horizontalDistance + verticalDistance * verticalDistance;
        }

        /// <summary>
        /// Tries to get a valid focusable at index.
        /// Returns a bool indicating whether or not the search should be stopped.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <param name="direction"></param>
        /// <param name="focusable">NULL if not valid</param>
        /// <returns>Whether or not to abort the search (true = abort).</returns>
        protected bool tryGetFocusableForIteration(int previous, int next, NavigationMoveEvent.Direction direction, out VisualElement focusable)
        {
            if (previous < 0 || previous > _focusables.Count- 1 || next > _focusables.Count - 1)
            {
                focusable = null;
                return true;
            }    

            // Ignore nested focusables.
            // DropDowns for example have a focusable label inside them.
            if (_focusables[next].IsChildOf(_focusables[previous]))
            {
                focusable = null;
                return false;
            }

            // Any visible child can be selected in the normal process.
            // If we find a child that is selectable in the normal process and
            // is in the right direction then we skip any special handling.
            if (isChildOverlappingViewport(_focusables[next])
                && (direction == NavigationMoveEvent.Direction.None || isInNavigationDirection(_focusables[previous], _focusables[next], direction))
               )
            {
                focusable = null;
                return true;
            }

            // Check if the child can be selected.
            if (_focusables[next].canGrabFocus)
            {
                focusable = _focusables[next];
                return true;
            }

            focusable = null;
            return false;
        }

        protected void focusDuringNavigationEvent(NavigationMoveEvent evt, VisualElement child)
        {
            if (child == null)
                return;

            /* Deactivated: Because it would abort if an element in a child scroll view is selected if that scroll view is off-screen.
             * TODO: Investigate.
            // Found a child that is selectable in the normal process -> skip any special handling.
            //if (isChildInViewport(child))
            //    return;
            */

            // Check if the child would not be selected (the default Unity selection skips children outside the viewport).
            if (child.canGrabFocus)
            {
                // If we found a child outside that is focusable then
                // cancel the navigation and ..
                evt.StopPropagation();
#if UNITY_6000_0_OR_NEWER
                focusController.IgnoreEvent(evt);
#else
                evt.PreventDefault();
#endif

                // .. select the next element manually.
                child.Focus();
            }
        }

        protected void handleOnNavigationMoveFromChild(NavigationMoveEvent evt)
        {
            // Find closest target in direction
            var target = findClosest(evt, _focusables);

            // trigger navigation override
            if (target != null)
            {
                focusDuringNavigationEvent(evt, target);
            }
        }

        protected VisualElement findClosest(NavigationMoveEvent evt, List<VisualElement> elements)
        {
            bool horizontalOrBoth = mode == ScrollViewMode.Horizontal || mode == ScrollViewMode.VerticalAndHorizontal;
            bool verticalOrBoth = mode == ScrollViewMode.Vertical || mode == ScrollViewMode.VerticalAndHorizontal;

            bool positiveNavigationMovement =
                   (evt.direction == NavigationMoveEvent.Direction.Down && verticalOrBoth)
                || (evt.direction == NavigationMoveEvent.Direction.Right && horizontalOrBoth);

            bool negativeNavigationMovement =
                   (evt.direction == NavigationMoveEvent.Direction.Up && verticalOrBoth)
                || (evt.direction == NavigationMoveEvent.Direction.Left && horizontalOrBoth);

            if (!positiveNavigationMovement && !negativeNavigationMovement)
                return null;

            // Find closest target in direction
            var target = evt.target as VisualElement;
            if (verticalOrBoth)
                target = findClosestToY(target, positiveNavigationMovement ? 1 : -1, elements);
            else
                target = findClosestToX(target, positiveNavigationMovement ? 1 : -1, elements);

            return target;
        }

        protected VisualElement findClosestToY(VisualElement target, int direction, List<VisualElement> elements)
        {
            float worldY = target.worldBound.center.y;
            float minDistance = float.MaxValue;
            float distance;
            VisualElement closest = null;
            foreach (var child in elements)
            {
                distance = child.worldBound.center.y - worldY;

                if (distance < 0f && direction > 0)
                    continue;
                if (distance > 0f && direction < 0)
                    continue;

                distance = Mathf.Abs(distance);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = child;
                }
            }

            return closest;
        }

        protected VisualElement findClosestToX(VisualElement target, int direction, List<VisualElement> elements)
        {
            float worldX = target.worldBound.center.x;
            float minDistance = float.MaxValue;
            float distance;
            VisualElement closest = null;
            foreach (var child in elements)
            {
                distance = child.worldBound.center.x - worldX;

                if (distance < 0f && direction > 0)
                    continue;
                if (distance > 0f && direction < 0)
                    continue;

                distance = Mathf.Abs(distance);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = child;
                }
            }

            return closest;
        }
    }
}
