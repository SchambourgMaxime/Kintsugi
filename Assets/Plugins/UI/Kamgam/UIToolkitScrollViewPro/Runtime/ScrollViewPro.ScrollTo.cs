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
        public enum ScrollToAlign
        {
            Visible,
            Start,
            Center,
            End
        }

        /// <summary>
        /// The margin around the scroll target ordered x = Left, y = Top, z = Right, w = Bottom.<br />
        /// Margins are added based on best effort. If the contentContainer does not have a padding and clamping is ON
        /// then these values may be reduced or ignored.<br />
        /// This is used only if no margin was set in the ScrollTo... margin parameter.<br />
        /// HINT: You can use negative margins too.
        /// </summary>
        public Vector4 scrollToMargin = new Vector4(0f, 0f, 0f, 0f);

        protected IVisualElementScheduledItem _scrollToAnimation;
        protected Vector2 _scrollToAnimationScrollOffsetStart;
        protected Vector2 _scrollToAnimationScrollOffsetTarget;
        protected ScrollView.TouchScrollBehavior? _scrollToAnimationPreviousBehaviour;
        protected float _scrollToAnimationTime;
        protected float _scrollToAnimationDuration;
        protected Easing _scrollToAnimationEasing;

        /// <summary>
        /// Scroll to a specific child element.
        /// </summary>
        /// <param name="child">The child to scroll to.</param>
        /// <param name="alignX">How to align the child relative to the viewport.</param>
        /// <param name="alignY">How to align the child relative to the viewport.</param>
        /// <param name="margin">Margin around the child: left, top, right, bottom, (positive = outwards, negative = inwards).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void ScrollTo(
            VisualElement child,
            ScrollToAlign alignX = ScrollToAlign.Visible,
            ScrollToAlign alignY = ScrollToAlign.Visible,
            Vector4? margin = null)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (!contentContainer.Contains(child))
            {
                throw new ArgumentException("Cannot scroll to a VisualElement that's not a child of the ScrollView content-container.");
            }

            if (!margin.HasValue)
                margin = scrollToMargin;

            StopScrollToAnimation();

            float deltaX = 0f;
            if (scrollableWidth > 0f)
            {
                deltaX = getDeltaOffsetX(child, alignX, margin);
                // clamp
                if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Unrestricted)
                {
                    horizontalScroller.value = Mathf.Clamp(scrollOffset.x + deltaX, 0, scrollableWidth);
                }
                else
                {
                    horizontalScroller.value = scrollOffset.x + deltaX;
                }
            }

            float deltaY = 0f;
            if (scrollableHeight > 0f)
            {
                deltaY = getDeltaOffsetY(child, alignY, margin);
                // clamp
                if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Unrestricted)
                {
                    verticalScroller.value = Mathf.Clamp(scrollOffset.y + deltaY, 0, scrollableHeight);
                }
                else
                {
                    verticalScroller.value = scrollOffset.y + deltaY;
                }
            }

            if (deltaY != 0f || deltaX != 0f)
            {
                applyScrollOffset();
            }
        }

        /// <summary>
        /// Scroll to a specific child element.
        /// </summary>
        /// <param name="child">The child to scroll to.</param>
        /// <param name="duration"></param>
        /// <param name="easing"></param>
        /// <param name="align">How to align the child relative to the viewport.</param>
        /// <param name="margin">Margin around the child: left, top, right, bottom, (positive = outwards, negative = inwards).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void ScrollToAnimated(
            VisualElement child, float duration, Easing easing = Easing.Ease,
            ScrollToAlign alignX = ScrollToAlign.Visible,
            ScrollToAlign alignY = ScrollToAlign.Visible,
            Vector4? margin = null)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (!contentContainer.Contains(child))
            {
                throw new ArgumentException("Cannot scroll to a VisualElement that's not a child of the ScrollView content-container.");
            }

            if (!margin.HasValue)
                margin = scrollToMargin;

            if (duration <= 0.0001f)
            {
                ScrollTo(child, alignX, alignY, margin);
                return;
            }

            StopScrollToAnimation();

            float yDeltaOffset = 0, xDeltaOffset = 0;
            Vector2 targetScrollOffset = scrollOffset;
            if (scrollableWidth > 0)
            {
                xDeltaOffset = getDeltaOffsetX(child, alignX, margin);
                // clamp
                if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Unrestricted)
                {
                    targetScrollOffset.x = Mathf.Clamp(scrollOffset.x + xDeltaOffset, 0, scrollableWidth);
                }
                else
                {
                    targetScrollOffset.x = scrollOffset.x + xDeltaOffset;
                }
            }
            if (scrollableHeight > 0)
            {
                yDeltaOffset = getDeltaOffsetY(child, alignY, margin);
                // clamp
                if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Unrestricted)
                {
                    targetScrollOffset.y = Mathf.Clamp(scrollOffset.y + yDeltaOffset, 0, scrollableHeight);
                }
                else
                {
                    targetScrollOffset.y = scrollOffset.y + yDeltaOffset;
                }
            }

            if (yDeltaOffset == 0 && xDeltaOffset == 0)
                return;

            // Init animation parameters
            _scrollToAnimationTime = 0f;
            _scrollToAnimationDuration = duration;
            _scrollToAnimationEasing = easing;
            _scrollToAnimationScrollOffsetStart = scrollOffset;
            _scrollToAnimationScrollOffsetTarget = targetScrollOffset;
            _scrollToAnimationPreviousBehaviour = touchScrollBehavior;

            // cancel other animations
            _inertiaAndElasticityAnimation?.Pause();
            _velocity = Vector2.zero;

            // Start animation
            touchScrollBehavior = ScrollView.TouchScrollBehavior.Unrestricted;
            if (_scrollToAnimation == null)
            {
                _scrollToAnimation = base.schedule.Execute(scrollToAnimationStep).Every(_animationFrameDurationInMS);
            }
            else
            {
                _scrollToAnimation.Resume();
            }
        }

        protected void scrollToAnimationStep()
        {
            _scrollToAnimationTime += _animationFrameDurationInMS / 1000f;
            if (_scrollToAnimationTime < _scrollToAnimationDuration)
            {
                float t = EasingUtils.Ease(_scrollToAnimationEasing, _scrollToAnimationTime / _scrollToAnimationDuration);
                Vector2 newOffset = Vector2.LerpUnclamped(_scrollToAnimationScrollOffsetStart, _scrollToAnimationScrollOffsetTarget, t);
                scrollOffset = newOffset;
            }
            else
            {
                scrollOffset = _scrollToAnimationScrollOffsetTarget;
                StopScrollToAnimation();
            }
        }

        public void StopScrollToAnimation()
        {
            if (_scrollToAnimation != null && _scrollToAnimation.isActive)
            {
                // Reset behaviour only if the previous is known.
                if (_scrollToAnimationPreviousBehaviour.HasValue)
                    touchScrollBehavior = _scrollToAnimationPreviousBehaviour.Value;
                _scrollToAnimationPreviousBehaviour = null;

                _scrollToAnimation?.Pause();
            }
        }

        protected float getDeltaOffsetX(VisualElement child, ScrollToAlign align, Vector4? margin = null)
        {
            if (!margin.HasValue)
                margin = Vector4.zero;

            float posX = contentContainer.transform.position.x * -1;

            var contentBounds = contentViewport.worldBound;
            float viewMin = contentBounds.xMin + posX;
            float viewMax = contentBounds.xMax + posX;

            var childBounds = child.worldBound;
            float childMin = childBounds.xMin - margin.Value.x + posX;
            float childMax = childBounds.xMax + margin.Value.z + posX;

            if (float.IsNaN(childMin) || float.IsNaN(childMax))
                return 0f;

            // If child is inside view port then do not move if align == visible.
            if (align == ScrollToAlign.Visible && childMin >= viewMin && childMax <= viewMax)
                return 0f;

            float deltaDistance = getDeltaDistance(viewMin, viewMax, childMin, childMax, align);

            return deltaDistance * horizontalScroller.highValue / scrollableWidth;
        }

        protected float getDeltaOffsetY(VisualElement child, ScrollToAlign align, Vector4? margin = null)
        {
            if (!margin.HasValue)
                margin = Vector4.zero;

            float posY = contentContainer.transform.position.y * -1;

            var viewBounds = contentViewport.worldBound;
            float viewMin = viewBounds.yMin + posY;
            float viewMax = viewBounds.yMax + posY;

            // Child bounds with padding
            var childBounds = child.worldBound;
            float childMin = childBounds.yMin - margin.Value.y + posY;
            float childMax = childBounds.yMax + margin.Value.w + posY;

            if (float.IsNaN(childMin) || float.IsNaN(childMax))
                return 0f;

            // If child is inside view port then do not move if align == visible.
            if (align == ScrollToAlign.Visible && childMin >= viewMin && childMax <= viewMax)
                return 0f;

            float deltaDistance = getDeltaDistance(viewMin, viewMax, childMin, childMax, align);
            float factor = verticalScroller.highValue / scrollableHeight;
            
            return deltaDistance * verticalScroller.highValue / scrollableHeight;
        }

        protected float getDeltaDistance(
            float viewMin, float viewMax, float childMin, float childMax,
            ScrollToAlign align)
        {
            float viewSize = viewMax - viewMin;
            float childSize = childMax - childMin;

            switch (align)
            {
                case ScrollToAlign.Start:
                    return childMin - viewMin;

                case ScrollToAlign.Center:
                    return (childMin + childMax) * 0.5f - (viewMin + viewMax) * 0.5f;

                case ScrollToAlign.End:
                    return childMax - viewMax;

                case ScrollToAlign.Visible:
                default:
                    // Child is bigger than viewport.
                    if (childSize > viewSize)
                    {
                        // If child covers all of the view port -> do nothing.
                        if (viewMin > childMin && childMax > viewMax)
                        {
                            return 0f;
                        }
                        return (childMin > viewMin) ? (childMin - viewMin) : (childMax - viewMax);
                    }
                    float deltaDistance = childMax - viewMax;
                    if (deltaDistance < -1f)
                    {
                        deltaDistance = childMin - viewMin;
                    }
                    return deltaDistance;
            }
        }
    }
}
