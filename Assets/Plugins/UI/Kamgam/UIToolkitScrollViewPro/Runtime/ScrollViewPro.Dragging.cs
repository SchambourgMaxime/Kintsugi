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
        const string DragIgnoreClassName = "svp-drag-ignore";

        /// <summary>
        /// Whether the scroll view is currently being dragged or not.
        /// </summary>
        public bool isDragging { get; private set; }

        public const int FallbackAnimationFps = 60;
        const int UndefinedAnimationFps = -1;

        protected int _animationFps = UndefinedAnimationFps;

        /// <summary>
        /// Defines how quickly the animations will be updated.<br />
        /// If set to -1 then Application.targetFrameRate will be used.<br />
        /// If Application.targetFrameRate is -1 too then FallbackAnimationFps (60) will be used.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("animation-fps")]
#endif
        public int animationFps {
            get => _animationFps;
            set
            {
                if (_animationFps != value)
                {
                    _animationFps = value;
                    int fps = value;
                    if (fps <= 0)
                        fps = Application.targetFrameRate;
                    if (fps <= 0)
                        fps = FallbackAnimationFps;

                    _animationFrameDurationInMS = 1000 / fps;
                }
            }
        }

        protected int _animationFrameDurationInMS = 16;

        public const float DefaultDragThreshold = 20f;
        /// <summary>
        /// If the mouse moves more than the sqrt of this distance then it is treated as a drag/scroll event.
        /// Otherwise it is treated as a click. 
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("drag-threshold")]
#endif
        public float dragThreshold { get; set; } = DefaultDragThreshold;

        float _lastVelocityLerpTime;
        protected Vector2 _velocity;
        protected IVisualElementScheduledItem _inertiaAndElasticityAnimation;

        // Mouse pos an offset are used for dragging AND child threshold detection.
        protected Vector2 _pointerDownPos;
        protected Vector2 _pointerDownScrollOffset;

        // Drag pointer id is only set if the contentContainer has captured the pointer for dragging.
        protected int _capturedDragPointerId = PointerId.invalidPointerId;

        protected void captureDragPointer(IEventHandler handler, int pointerId)
        {
            if (pointerId == PointerId.invalidPointerId)
                return;

            if (!PointerCaptureHelper.HasPointerCapture(handler, pointerId))
            {
                PointerCaptureHelper.CapturePointer(handler, pointerId);
                _capturedDragPointerId = pointerId;
            }
        }

        protected bool hasCapturedDragPointer() 
        {
            return _capturedDragPointerId != PointerId.invalidPointerId && PointerCaptureHelper.HasPointerCapture(contentContainer, _capturedDragPointerId);
        }

        protected void releaseDragPointer(IEventHandler handler, int pointerId)
        {
            if (pointerId == PointerId.invalidPointerId)
                return;

            if (PointerCaptureHelper.HasPointerCapture(handler, pointerId))
            {
                PointerCaptureHelper.ReleasePointer(handler, pointerId);
            }

            if (pointerId == _capturedDragPointerId)
            {
                _capturedDragPointerId = PointerId.invalidPointerId;
            }
        }

        protected void startDragging<T>(PointerEventBase<T> evt) where T : PointerEventBase<T>, new()
        {
            // Ignore multiple pointers (use primary only).
            if (hasCapturedDragPointer() || !evt.isPrimary)
                return;

            calculateBounds();

            captureDragPointer(contentContainer, evt.pointerId);

            _pointerDownPos = evt.position;
            _pointerDownScrollOffset = scrollOffset;

            isDragging = true;
        }

        protected void stopDragging(bool startAnimation = false)
        {
            releaseDragPointer(contentContainer, _capturedDragPointerId);

            if (hasInertia || touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic)
            {
                if (startAnimation)
                {
                    startInertiaAndElasticityAnimation();
                }
            }

            isDragging = false;
        }

        /// <summary>
        /// Returns true if the event was cancelled.
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        protected bool ignoreDragAndCancelEvent(EventBase evt)
        {
            if (evt.target == null)
                return false;

            var ve = evt.target as VisualElement;
            if (ve != null && ve.ClassListContains(DragIgnoreClassName))
            {
                evt.StopImmediatePropagation();
                return true;
            }

            return false;
        }

        protected void onPointerDown(PointerDownEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            StopAnimations();
            startDragging(evt);
        }
        
        protected void onPointerDownOnViewport(PointerDownEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            if (infinite)
                startDragging(evt);
        }

        public void StopAnimations()
        {
            _inertiaAndElasticityAnimation?.Pause();
            _scrollWheelScheduledAnimation?.Pause();

            StopScrollToAnimation(); // See "ScrollToAnimated".
        }

        protected void onPointerMove(PointerMoveEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            if (hasCapturedDragPointer())
            {
                handleDrag(evt);
            }
        }

        protected void handleDrag(PointerMoveEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            Vector2 deltaPos = (Vector2)evt.position - _pointerDownPos;

            // Scroll offset is the inverse of position, thus we have to subtract deltaPos.
            var newScrollOffset = _pointerDownScrollOffset - deltaPos;

            // Clamp based on scroll behaviour
            if (touchScrollBehavior == ScrollView.TouchScrollBehavior.Clamped)
            {
                newScrollOffset = Vector2.Max(newScrollOffset, _lowBounds);
                newScrollOffset = Vector2.Min(newScrollOffset, _highBounds);
            }
            else if (touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic)
            {
                newScrollOffset.x = computeElasticOffset(
                    scrollOffset.x,
                    deltaPos.x, _pointerDownScrollOffset.x,
                    _lowBounds.x, _lowBounds.x - contentViewport.resolvedStyle.width,
                    _highBounds.x, _highBounds.x + contentViewport.resolvedStyle.width);

                newScrollOffset.y = computeElasticOffset(
                    scrollOffset.y,
                    deltaPos.y, _pointerDownScrollOffset.y,
                    _lowBounds.y, _lowBounds.y - contentViewport.resolvedStyle.height,
                    _highBounds.y, _highBounds.y + contentViewport.resolvedStyle.height);
            }

            // Reset x or y based on scroll mode.
            switch (mode)
            {
                case ScrollViewMode.Vertical:
                    newScrollOffset.x = scrollOffset.x;
                    break;
                case ScrollViewMode.Horizontal:
                    newScrollOffset.y = scrollOffset.y;
                    break;
                default:
                    break;
            }

            // Calculate velocity
            // Velocity is updated just like in Unitys own ScrollView.
            if (hasInertia)
            {
                if (scrollOffset == _lowBounds || scrollOffset == _highBounds)
                {
                    _velocity = Vector2.zero;
                }
                else
                {
                    if (_lastVelocityLerpTime > 0f)
                    {
                        float dT = Time.unscaledTime - _lastVelocityLerpTime;
                        _velocity = Vector2.Lerp(_velocity, Vector2.zero, dT * 10f);
                    }

                    _lastVelocityLerpTime = Time.unscaledTime;
                    float unscaledDeltaTime = Time.unscaledDeltaTime;
                    Vector2 b = (newScrollOffset - scrollOffset) / unscaledDeltaTime;
                    _velocity = Vector2.Lerp(_velocity, b, unscaledDeltaTime * 10f);
                }
            }

            // Finally set new scroll offset.
            scrollOffset = newScrollOffset;
        }

        protected void onPointerCancel(PointerCancelEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            releaseDragPointer(contentContainer, evt.pointerId);
            stopDragging(startAnimation: false);
        }

        protected void onPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            // Ignore all except contentContainer
            if (evt.target != contentContainer)
            {
                // Stop dragging if capture out was triggered by
                // some external target (child scroll view).
                if (isDragging)
                {
                    stopDragging(startAnimation: false);
                }

                return;
            }

            releaseDragPointer(contentContainer, evt.pointerId);
            stopDragging(startAnimation: false);
        }

        protected void onPointerUp(PointerUpEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            releaseDragPointer(contentContainer, evt.pointerId);
            stopDragging(startAnimation: true);

            if (snap && !hasInertia)
                Snap();
        }

        // The mouse down on children is only used to inform the scroll view of a drag.
        // If one is detected then the mouse capture will be handed over to the content container.

        protected bool _pointerDownOnChild = false;

        protected void onPointerDownOnChild(PointerDownEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            StopAnimations();

            releaseDragPointer(contentContainer, _capturedDragPointerId);
            stopDragging(startAnimation: false);

            PointerCaptureHelper.CapturePointer(evt.target, evt.pointerId);

            _pointerDownPos = evt.position;
            _pointerDownScrollOffset = scrollOffset;

            _pointerDownOnChild = true;
        }

        protected void onPointerMoveOnChild(PointerMoveEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            if (PointerCaptureHelper.HasPointerCapture(evt.target, evt.pointerId))
            {
                var movementSinceDown = (Vector2)evt.position - _pointerDownPos;
                if (movementSinceDown.sqrMagnitude > dragThreshold * dragThreshold)
                {
                    PointerCaptureHelper.ReleasePointer(evt.target, evt.pointerId);
                    captureDragPointer(contentContainer, evt.pointerId);

                    _pointerDownOnChild = false;
                }
            }
        }

        protected void onPointerCancelOnChild(PointerCancelEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            if (PointerCaptureHelper.HasPointerCapture(evt.target, evt.pointerId))
            {
                PointerCaptureHelper.ReleasePointer(evt.target, evt.pointerId);
            }

            stopDragging(startAnimation: false);

            _pointerDownOnChild = false;
        }

        protected void onPointerUpOnChild(PointerUpEvent evt)
        {
            if (ignoreDragAndCancelEvent(evt))
                return;

            if (PointerCaptureHelper.HasPointerCapture(evt.target, evt.pointerId))
            {
                PointerCaptureHelper.ReleasePointer(evt.target, evt.pointerId);

                _pointerDownOnChild = false;
            }
        }


        /// <summary>
        /// Compute the new scroll view offset from a pointer delta, taking elasticity into account.
        /// Low and high limits are the values beyond which the scrollview starts to show resistance to scrolling (elasticity).
        /// Low and high hard limits are the values beyond which it is infinitely hard to scroll.
        /// </summary>
        /// <param name="currentOffset"></param>
        /// <param name="deltaPointer"></param>
        /// <param name="initialScrollOffset"></param>
        /// <param name="lowLimit"></param>
        /// <param name="hardLowLimit"></param>
        /// <param name="highLimit"></param>
        /// <param name="hardHighLimit"></param>
        /// <returns></returns>
        protected static float computeElasticOffset(
            float currentOffset,
            float deltaPointer, float initialScrollOffset,
            float lowLimit, float hardLowLimit,
            float highLimit, float hardHighLimit)
        {
            // Short circuit if inside all limits.
            float targetOffset = initialScrollOffset - deltaPointer;
            // Extra margins for equal state (with if the initial state of the scroll view).
            if (targetOffset > lowLimit - 0.001f && targetOffset < highLimit + 0.001f)
            {
                return targetOffset;
            }

            // Here it is between the limit and the hard limit.
            float limit = targetOffset < lowLimit ? lowLimit : highLimit;
            float hardLimit = targetOffset < lowLimit ? hardLowLimit : hardHighLimit;
            float span = hardLimit - limit;
            float delta = targetOffset - limit;
            float normalizedDelta = delta / span;
            // 0.3f = the content will stop at 30% of the scroll view size.
            float ratio = (1f - (normalizedDelta - 1) * (normalizedDelta - 1)) * 0.3f;
            if (normalizedDelta < 1f)
            {
                return limit + span * ratio;
            }
            else
            {
                return currentOffset;
            }
        }


        protected void startInertiaAndElasticityAnimation()
        {
            calcInitialSpringBackVelocity();

            // Reset if not moved for a while. Done to avoid inertia
            // animation in case the pointer was not moved for some time.
            if (Time.unscaledTime - _lastVelocityLerpTime > 0.2f)
                _velocity = Vector2.zero;

            if (_inertiaAndElasticityAnimation == null)
            {
                _inertiaAndElasticityAnimation = base.schedule.Execute(inertiaAndElasticityAnimationStep).Every(_animationFrameDurationInMS);
            }
            else
            {
                _inertiaAndElasticityAnimation.Resume();
            }
        }

        protected Vector2 _springBackVelocity;

        protected void calcInitialSpringBackVelocity()
        {
            if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Elastic)
            {
                _springBackVelocity = Vector2.zero;
                return;
            }

            if (scrollOffset.x < _lowBounds.x)
            {
                _springBackVelocity.x = _lowBounds.x - scrollOffset.x;
            }
            else if (scrollOffset.x > _highBounds.x)
            {
                _springBackVelocity.x = _highBounds.x - scrollOffset.x;
            }
            else
            {
                _springBackVelocity.x = 0f;
            }

            if (scrollOffset.y < _lowBounds.y)
            {
                _springBackVelocity.y = _lowBounds.y - scrollOffset.y;
            }
            else if (scrollOffset.y > _highBounds.y)
            {
                _springBackVelocity.y = _highBounds.y - scrollOffset.y;
            }
            else
            {
                _springBackVelocity.y = 0f;
            }
        }

        protected void inertiaAndElasticityAnimationStep()
        {
            inertiaAnimationStep();
            elasticityAnimationStep();

            // If none of the animations needs updating then pause.
            if (_springBackVelocity == Vector2.zero && _velocity == Vector2.zero)
            {
                _inertiaAndElasticityAnimation.Pause();
            }
        }

        protected void elasticityAnimationStep()
        {
            if (touchScrollBehavior != ScrollView.TouchScrollBehavior.Elastic)
            {
                _springBackVelocity = Vector2.zero;
                return;
            }

            // Unity ScrollView uses Time.unscaledDeltaTime internally which makes the spring back
            // animation very slow on high fps (like in the Editor). To avoid that we use the delay
            // of the animation as delta time.
            float deltaTime = _animationFrameDurationInMS / 1000f;

            Vector2 vector = scrollOffset;
            if (vector.x < _lowBounds.x)
            {
                vector.x = Mathf.SmoothDamp(vector.x, _lowBounds.x, ref _springBackVelocity.x, elasticity, float.PositiveInfinity, deltaTime);
                if (Mathf.Abs(_springBackVelocity.x) < 1f)
                {
                    _springBackVelocity.x = 0f;
                }
            }
            else if (vector.x > _highBounds.x)
            {
                vector.x = Mathf.SmoothDamp(vector.x, _highBounds.x, ref _springBackVelocity.x, elasticity, float.PositiveInfinity, deltaTime);
                if (Mathf.Abs(_springBackVelocity.x) < 1f)
                {
                    _springBackVelocity.x = 0f;
                }
            }
            else
            {
                _springBackVelocity.x = 0f;
            }

            if (vector.y < _lowBounds.y)
            {
                vector.y = Mathf.SmoothDamp(vector.y, _lowBounds.y, ref _springBackVelocity.y, elasticity, float.PositiveInfinity, deltaTime);
                if (Mathf.Abs(_springBackVelocity.y) < 1f)
                {
                    _springBackVelocity.y = 0f;
                }
            }
            else if (vector.y > _highBounds.y)
            {
                vector.y = Mathf.SmoothDamp(vector.y, _highBounds.y, ref _springBackVelocity.y, elasticity, float.PositiveInfinity, deltaTime);
                if (Mathf.Abs(_springBackVelocity.y) < 1f)
                {
                    _springBackVelocity.y = 0f;
                }
            }
            else
            {
                _springBackVelocity.y = 0f;
            }

            scrollOffset = vector;
        }

        protected void inertiaAnimationStep()
        {
            // Unity ScrollView uses Time.unscaledDeltaTime internally which makes the spring back
            // animation very slow on high fps (like in the Editor). To avoid that we use the delay
            // of the animation as delta time.
            float deltaTime = _animationFrameDurationInMS / 1000f;

            if (hasInertia && _velocity != Vector2.zero)
            {
                _velocity *= Mathf.Pow(scrollDecelerationRate, deltaTime);

                // Set to 0 if close to zero or if out of bounds and behaviour is elastic.
                if (Mathf.Abs(_velocity.x) < 1f || (touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic && (scrollOffset.x < _lowBounds.x || scrollOffset.x > _highBounds.x)))
                {
                    _velocity.x = 0f;
                }

                if (Mathf.Abs(_velocity.y) < 1f || (touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic && (scrollOffset.y < _lowBounds.y || scrollOffset.y > _highBounds.y)))
                {
                    _velocity.y = 0f;
                }

                scrollOffset += _velocity * deltaTime;
            }
            else
            {
                _velocity = Vector2.zero;
            }

            handleSnappingWhileInteriaAnimation();
        }
    }
}
