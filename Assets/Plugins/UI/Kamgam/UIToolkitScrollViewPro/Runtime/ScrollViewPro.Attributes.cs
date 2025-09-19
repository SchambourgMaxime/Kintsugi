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
    /// It aims to be API compatible with the Unity ScrollView and thus uses some of the ScrollView enums and types.
    /// </summary>
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class ScrollViewPro
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ScrollViewPro, UxmlTraits>
        {
        }

        /// <summary>
        /// The UxmlTraits use the same names and default values as the Unity ScrollView.
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public UxmlTraits()
            {
                focusable.defaultValue = false;
            }

            protected UxmlEnumAttributeDescription<ScrollViewMode> _scrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
            {
                name = "mode",
                defaultValue = ScrollViewMode.Vertical
            };

            protected UxmlEnumAttributeDescription<ScrollerVisibility> _horizontalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
            {
                name = "horizontal-scroller-visibility"
            };

            protected UxmlEnumAttributeDescription<ScrollerVisibility> _verticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
            {
                name = "vertical-scroller-visibility"
            };

            protected UxmlBoolAttributeDescription _scrollerButtons = new UxmlBoolAttributeDescription
            {
                name = "scroller-buttons",
                defaultValue = DefaultScrollerButtons
            };

            protected UxmlBoolAttributeDescription _focusableScrollbars = new UxmlBoolAttributeDescription
            {
                name = "focusable-scrollbars",
                defaultValue = DefaultFocusableScrollbars
            };

            protected UxmlFloatAttributeDescription _mouseWheelScrollSize = new UxmlFloatAttributeDescription
            {
                name = "mouse-wheel-scroll-size",
                defaultValue = DefaultMouseWheelScrollSpeed
            };

            protected UxmlFloatAttributeDescription _horizontalPageSize = new UxmlFloatAttributeDescription
            {
                name = "horizontal-page-size",
                defaultValue = UndefinedPageSize
            };

            protected UxmlFloatAttributeDescription _verticalPageSize = new UxmlFloatAttributeDescription
            {
                name = "vertical-page-size",
                defaultValue = UndefinedPageSize
            };

            protected UxmlEnumAttributeDescription<ScrollView.TouchScrollBehavior> _touchScrollBehavior = new UxmlEnumAttributeDescription<ScrollView.TouchScrollBehavior>
            {
                name = "touch-scroll-type",
                defaultValue = ScrollView.TouchScrollBehavior.Clamped
            };

            protected UxmlFloatAttributeDescription _scrollDecelerationRate = new UxmlFloatAttributeDescription
            {
                name = "scroll-deceleration-rate",
                defaultValue = DefaultScrollDecelerationRate
            };

            protected UxmlFloatAttributeDescription _elasticity = new UxmlFloatAttributeDescription
            {
                name = "elasticity",
                defaultValue = DefaultElasticity
            };

            protected UxmlIntAttributeDescription _animationFps = new UxmlIntAttributeDescription
            {
                name = "animation-fps",
                defaultValue = UndefinedAnimationFps
            };

            protected UxmlFloatAttributeDescription _dragThreshold = new UxmlFloatAttributeDescription
            {
                name = "drag-threshold",
                defaultValue = DefaultDragThreshold
            };

            protected UxmlBoolAttributeDescription _snap = new UxmlBoolAttributeDescription
            {
                name = "snap",
                defaultValue = DefaultSnapValue
            };

            protected UxmlStringAttributeDescription _snapTargetClasses = new UxmlStringAttributeDescription
            {
                name = "snap-target-classes",
                defaultValue = DefaultSnapTargetClasses
            };

            protected UxmlBoolAttributeDescription _snapTargetFocusables = new UxmlBoolAttributeDescription
            {
                name = "snap-target-focusables",
                defaultValue = DefaultSnapTargetFocusables
            };

            protected UxmlFloatAttributeDescription _snapDurationSec = new UxmlFloatAttributeDescription
            {
                name = "snap-duration-sec",
                defaultValue = DefaultSnapDurationSec
            };

            protected UxmlEnumAttributeDescription<ScrollToAlign> _snapAlignX = new UxmlEnumAttributeDescription<ScrollToAlign>
            {
                name = "snap-align-x",
                defaultValue = DefaultSnapAlignX
            };

            protected UxmlEnumAttributeDescription<ScrollToAlign> _snapAlignY = new UxmlEnumAttributeDescription<ScrollToAlign>
            {
                name = "snap-align-y",
                defaultValue = DefaultSnapAlignY
            };

            protected UxmlEnumAttributeDescription<Easing> _snapEase = new UxmlEnumAttributeDescription<Easing>
            {
                name = "snap-ease",
                defaultValue = DefaultSnapEase
            };

            protected UxmlBoolAttributeDescription _snapIncludeMargin = new UxmlBoolAttributeDescription
            {
                name = "snap-include-margin",
                defaultValue = DefaultSnapIncludeMargin
            };

            // I wish ..
            /*
            protected UxmlVector4AttributeDescription _snapMargin = new UxmlVector4AttributeDescription
            {
                name = "snap-margin",
                defaultValue = DefaultSnapMargin
            };
            */

            protected UxmlFloatAttributeDescription _snapVelocityThreshold = new UxmlFloatAttributeDescription
            {
                name = "snap-velocity-threshold",
                defaultValue = DefaultSnapVelocityThreshold
            };

            protected UxmlBoolAttributeDescription _focusSnap = new UxmlBoolAttributeDescription
            {
                name = "focus-snap",
                defaultValue = DefaultFocusSnap
            };

            protected UxmlBoolAttributeDescription _focusSnapOnPointer = new UxmlBoolAttributeDescription
            {
                name = "focus-snap-on-pointer",
                defaultValue = DefaultFocusSnapOnPointer
            };

            protected UxmlBoolAttributeDescription _focusSnapIncludeMargin = new UxmlBoolAttributeDescription
            {
                name = "focus-snap-include-margin",
                defaultValue = DefaultFocusSnapIncludeMargin
            };

            protected UxmlFloatAttributeDescription _focusSnapDurationSec = new UxmlFloatAttributeDescription
            {
                name = "focus-snap-duration-sec",
                defaultValue = DefaultFocusSnapDurationSec
            };

            protected UxmlEnumAttributeDescription<ScrollToAlign> _focusSnapAlignX = new UxmlEnumAttributeDescription<ScrollToAlign>
            {
                name = "focus-snap-align-x",
                defaultValue = DefaultFocusSnapAlignX
            };

            protected UxmlEnumAttributeDescription<ScrollToAlign> _focusSnapAlignY = new UxmlEnumAttributeDescription<ScrollToAlign>
            {
                name = "focus-snap-align-y",
                defaultValue = DefaultFocusSnapAlignY
            };

            protected UxmlEnumAttributeDescription<Easing> _focusSnapEase = new UxmlEnumAttributeDescription<Easing>
            {
                name = "focus-snap-ease",
                defaultValue = DefaultFocusSnapEase
            };

            // I wish ..
            /*
            protected UxmlVector4AttributeDescription _focusSnapMargin = new UxmlVector4AttributeDescription
            {
                name = "focus-snap-margin",
                defaultValue = DefaultFocusSnapMargin
            };
            */

            protected UxmlBoolAttributeDescription _focusSnapInside = new UxmlBoolAttributeDescription
            {
                name = "focus-snap-inside",
                defaultValue = DefaultFocusSnapInside
            };

            protected UxmlFloatAttributeDescription _focusSnapRepetitionDelay = new UxmlFloatAttributeDescription
            {
                name = "focus-snap-repetition-delay",
                defaultValue = DefaultFocusSnapRepetitionDelay
            };

            protected UxmlBoolAttributeDescription _infinite = new UxmlBoolAttributeDescription
            {
                name = "infinite",
                defaultValue = DefaultInfiniteValue
            };


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ScrollViewPro scrollView = (ScrollViewPro)ve;
                scrollView.mode = _scrollViewMode.GetValueFromBag(bag, cc);
                scrollView.horizontalScrollerVisibility = _horizontalScrollerVisibility.GetValueFromBag(bag, cc);
                scrollView.verticalScrollerVisibility = _verticalScrollerVisibility.GetValueFromBag(bag, cc);
                scrollView.scrollerButtons = _scrollerButtons.GetValueFromBag(bag, cc);
                scrollView.selectableScrollbars = _focusableScrollbars.GetValueFromBag(bag, cc);
                scrollView.mouseWheelScrollSize = _mouseWheelScrollSize.GetValueFromBag(bag, cc);
                scrollView.horizontalPageSize = _horizontalPageSize.GetValueFromBag(bag, cc);
                scrollView.verticalPageSize = _verticalPageSize.GetValueFromBag(bag, cc);
                scrollView.scrollDecelerationRate = _scrollDecelerationRate.GetValueFromBag(bag, cc);
                scrollView.touchScrollBehavior = _touchScrollBehavior.GetValueFromBag(bag, cc);
                scrollView.elasticity = _elasticity.GetValueFromBag(bag, cc);
                scrollView.animationFps = _animationFps.GetValueFromBag(bag, cc);

                scrollView.dragThreshold = _dragThreshold.GetValueFromBag(bag, cc);

                scrollView.snap = _snap.GetValueFromBag(bag, cc);
                scrollView.snapTargetClasses = _snapTargetClasses.GetValueFromBag(bag, cc);
                scrollView.snapTargetFocusables = _snapTargetFocusables.GetValueFromBag(bag, cc);
                scrollView.snapDurationSec = _snapDurationSec.GetValueFromBag(bag, cc);
                scrollView.snapAlignX = _snapAlignX.GetValueFromBag(bag, cc);
                scrollView.snapAlignY = _snapAlignY.GetValueFromBag(bag, cc);
                scrollView.snapEase = _snapEase.GetValueFromBag(bag, cc);
                // scrollView.snapMargin = _snapMargin.GetValueFromBag(bag, cc);
                scrollView.snapIncludeMargin = _snapIncludeMargin.GetValueFromBag(bag, cc);
                scrollView.snapVelocityThreshold = _snapVelocityThreshold.GetValueFromBag(bag, cc);

                scrollView.focusSnap = _focusSnap.GetValueFromBag(bag, cc);
                scrollView.focusSnapOnPointer = _focusSnapOnPointer.GetValueFromBag(bag, cc);
                scrollView.focusSnapIncludeMargin = _focusSnapIncludeMargin.GetValueFromBag(bag, cc);
                scrollView.focusSnapDurationSec = _focusSnapDurationSec.GetValueFromBag(bag, cc);
                scrollView.focusSnapAlignX = _focusSnapAlignX.GetValueFromBag(bag, cc);
                scrollView.focusSnapAlignY = _focusSnapAlignY.GetValueFromBag(bag, cc);
                scrollView.focusSnapEase = _focusSnapEase.GetValueFromBag(bag, cc);
                // scrollView.focusSnapMargin = _focusSnapMargin.GetValueFromBag(bag, cc);
                scrollView.focusSnapInside = _focusSnapInside.GetValueFromBag(bag, cc);
                scrollView.focusSnapRepetitionDelay = _focusSnapRepetitionDelay.GetValueFromBag(bag, cc);

                scrollView.infinite = _infinite.GetValueFromBag(bag, cc);
            }
        }
#endif

        // Use CustomStyleProperty<T> to fetch custom style properties from USS

        const float UndefinedFloatStyleValue = -1f;

        static readonly CustomStyleProperty<float> singleLineHeightFromStyle = new CustomStyleProperty<float>("--unity-metrics-single_line-height");
        protected float _singleLineHeightFromStyle = UndefinedFloatStyleValue;

        static readonly CustomStyleProperty<float> mouseWheelScrollSizeFromStyle = new CustomStyleProperty<float>("--mouse-wheel-scroll-size");
        protected float _mouseWheelScrollSizeFromStyle = UndefinedFloatStyleValue;

        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public static readonly string ussClassName = "unity-scroll-view";

        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public static readonly string ussSpecificClassName = "scroll-view-pro";

        /// <summary>
        /// USS class name of viewport elements in elements of this type.
        /// </summary>
        public static readonly string viewportUssClassName = ussClassName + "__content-viewport";

        /// <summary>
        /// USS class name of content elements in elements of this type.
        /// </summary>
        public static readonly string contentAndVerticalScrollUssClassName = ussClassName + "__content-and-vertical-scroll-container";

        /// <summary>
        /// USS class name of content elements in elements of this type.
        /// </summary>
        public static readonly string contentUssClassName = ussClassName + "__content-container";

        /// <summary>
        /// USS class name of horizontal scrollers in elements of this type.
        /// </summary>
        public static readonly string hScrollerUssClassName = ussClassName + "__horizontal-scroller";

        /// <summary>
        /// USS class name of vertical scrollers in elements of this type.
        /// </summary>
        public static readonly string vScrollerUssClassName = ussClassName + "__vertical-scroller";

        /// <summary>
        /// USS class name that's added when the ScrollView is in horizontal mode. ScrollViewMode.Horizontal
        /// </summary>
        public static readonly string horizontalVariantUssClassName = ussClassName + "--horizontal";

        /// <summary>
        /// USS class name that's added when the ScrollView is in vertical mode. ScrollViewMode.Vertical
        /// </summary>
        public static readonly string verticalVariantUssClassName = ussClassName + "--vertical";

        /// <summary>
        /// USS class name that's added when the ScrollView is in both horizontal and vertical
        /// mode. ScrollViewMode.VerticalAndHorizontal
        /// </summary>
        public static readonly string verticalHorizontalVariantUssClassName = ussClassName + "--vertical-horizontal";

        public static readonly string scrollVariantUssClassName = ussClassName + "--scroll";

        protected Vector2 _lowBounds;
        protected Vector2 _highBounds;

        protected ScrollerVisibility _horizontalScrollerVisibility;

        /// <summary>
        /// Specifies whether the horizontal scroll bar is visible.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("horizontal-scroller-visibility")]
#endif
        public ScrollerVisibility horizontalScrollerVisibility
        {
            get
            {
                return _horizontalScrollerVisibility;
            }
            set
            {
                _horizontalScrollerVisibility = value;
                updateScrollers(needsHorizontal, needsVertical, scrollerButtons);
            }
        }

        protected ScrollerVisibility _verticalScrollerVisibility;

        /// <summary>
        /// Specifies whether the vertical scroll bar is visible.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("vertical-scroller-visibility")]
#endif
        public ScrollerVisibility verticalScrollerVisibility
        {
            get
            {
                return _verticalScrollerVisibility;
            }
            set
            {
                _verticalScrollerVisibility = value;
                updateScrollers(needsHorizontal, needsVertical, scrollerButtons);
            }
        }

        const bool DefaultScrollerButtons = true;

        protected bool _scrollerButtons = DefaultScrollerButtons;

        /// <summary>
        /// Specifies whether the scrollbars should have buttons or not.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("scroller-buttons")]
#endif
        public bool scrollerButtons
        {
            get
            {
                return _scrollerButtons;
            }
            set
            {
                _scrollerButtons = value;
                updateScrollers(needsHorizontal, needsVertical, scrollerButtons);
            }
        }

        const bool DefaultFocusableScrollbars = false;

        protected bool _selectableScrollbars = DefaultFocusableScrollbars;

        /// <summary>
        /// Whether or not the scrollbars should be discoverable by the focus controller.<br />
        /// You should disable this is you are using 'focusSnap' as these two can create ambiguous
        /// situations.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("focusable-scrollbars")]
#endif
        public bool selectableScrollbars
        {
            get
            {
                return _selectableScrollbars;
            }
            set
            {
                if (_selectableScrollbars == value)
                    return;

                _selectableScrollbars = value;

                // This is important to be able to auto focus the scroll bars.
                focusable = value;

#if UNITY_EDITOR
                if (value && focusSnap)
                {
                    Debug.LogWarning("ScrollViewPro: You should NOT enable 'selectableScrollbars' and 'focusSnap' at the same time. These two can create ambiguous situations.");
                }
#endif
            }
        }

        public static readonly float DefaultMouseWheelScrollSpeed = 18f;

        protected float _mouseWheelScrollSize = DefaultMouseWheelScrollSpeed;

        /// <summary>
        /// How fast the scrol wheel will move the scroll view.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("mouse-wheel-scroll-size")]
#endif
        public float mouseWheelScrollSize
        {
            get
            {
                return _mouseWheelScrollSize;
            }
            set
            {
                _mouseWheelScrollSize = value;
            }
        }

        protected bool needsHorizontal => horizontalScrollerVisibility == ScrollerVisibility.AlwaysVisible || (horizontalScrollerVisibility == ScrollerVisibility.Auto && scrollableWidth > 0.001f);

        protected bool needsVertical => verticalScrollerVisibility == ScrollerVisibility.AlwaysVisible || (verticalScrollerVisibility == ScrollerVisibility.Auto && scrollableHeight > 0.001f);

        protected bool isVerticalScrollDisplayed => verticalScroller.resolvedStyle.display == DisplayStyle.Flex;

        protected bool isHorizontalScrollDisplayed => horizontalScroller.resolvedStyle.display == DisplayStyle.Flex;

        protected ScrollViewMode _mode = ScrollViewMode.Vertical;

        /// <summary>
        /// Controls how the ScrollView allows the user to scroll the contents. ScrollViewMode
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("mode")]
#endif
        public ScrollViewMode mode
        {
            get
            {
                return _mode;
            }
            set
            {
                if (_mode != value)
                {
                    SetScrollViewMode(value);
                }
            }
        }

        /// <summary>
        /// The current scrolling position.
        /// </summary>
        public Vector2 scrollOffset
        {
            get
            {
                return new Vector2(horizontalScroller.value, verticalScroller.value);
            }
            set
            {
                if (value != scrollOffset)
                {
                    horizontalScroller.value = value.x;
                    verticalScroller.value = value.y;
                    applyScrollOffset();
                }
            }
        }


        const float UndefinedPageSize = -1f;

        const float DefaultPageOverlapRatio = 0.1f;
        /// <summary>
        /// The page overlap if no page size is set.
        /// </summary>
        public float pageOverlapRatio { get; set; } = DefaultPageOverlapRatio;

        protected float _horizontalPageSize = UndefinedPageSize;

        /// <summary>
        /// This property is controlling the scrolling speed of the horizontal scroller.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("horizontal-page-size")]
#endif
        public float horizontalPageSize
        {
            get
            {
                return _horizontalPageSize;
            }
            set
            {
                _horizontalPageSize = value;
                updateHorizontalSliderPageSize();
            }
        }

        protected float _verticalPageSize = UndefinedPageSize;

        /// <summary>
        /// This property is controlling the scrolling speed of the vertical scroller.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("vertical-page-size")]
#endif
        public float verticalPageSize
        {
            get
            {
                return _verticalPageSize;
            }
            set
            {
                _verticalPageSize = value;
                updateVerticalSliderPageSize();
            }
        }

        // protected float scrollableWidth => contentContainer.GetBoundingBox().width - contentViewport.layout.width;
        protected float scrollableWidth
        {
            get
            {
                if (infinite)
                {
                    // If the scroll view is infinite then we move the children outside the bounds of
                    // contentContainer which makes it grow (posssibly infinitely). That's why we use the
                    // cached children bounds size instead.
                    if (_childrenBoundsWidth.HasValue)
                        return _childrenBoundsWidth.Value;
                }

                return contentContainer.GetBoundingBox().width - contentViewport.layout.width;
            }
        }


        //protected float scrollableHeight => contentContainer.GetBoundingBox().height - contentViewport.layout.height;
        protected float scrollableHeight
        {
            get
            {
                if (infinite)
                {
                    // If the scroll view is infinite then we move the children outside the bounds of
                    // contentContainer which makes it grow (posssibly infinitely). That's why we use the
                    // cached children bounds size instead.
                    if (_childrenBoundsHeight.HasValue)
                        return _childrenBoundsHeight.Value;
                }
                
                return contentContainer.GetBoundingBox().height - contentViewport.layout.height;
            }
        }
        

        protected bool hasInertia => scrollDecelerationRate > 0f;

        public static readonly float DefaultScrollDecelerationRate = 0.135f;

        protected float _scrollDecelerationRate = DefaultScrollDecelerationRate;

        /// <summary>
        /// Controls the rate at which the scrolling movement slows after a user scrolls
        /// using a touch interaction.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("scroll-deceleration-rate")]
#endif
        public float scrollDecelerationRate
        {
            get
            {
                return _scrollDecelerationRate;
            }
            set
            {
                _scrollDecelerationRate = Mathf.Max(0f, value);
            }
        }

        public static readonly float DefaultElasticity = 0.1f;

        protected float _elasticity = DefaultElasticity;

        /// <summary>
        /// The amount of elasticity to use when a user tries to scroll past the boundaries
        /// of the scroll view.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("elasticity")]
#endif
        public float elasticity
        {
            get
            {
                return _elasticity;
            }
            set
            {
                _elasticity = Mathf.Max(0f, value);
            }
        }

        protected ScrollView.TouchScrollBehavior _touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;

        /// <summary>
        /// The behavior to use when a user tries to scroll past the boundaries of the ScrollView
        /// content using a touch interaction.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("touch-scroll-type")]
#endif
        public ScrollView.TouchScrollBehavior touchScrollBehavior
        {
            get
            {
                return _touchScrollBehavior;
            }
            set
            {
                _touchScrollBehavior = value;
                if (_touchScrollBehavior == ScrollView.TouchScrollBehavior.Clamped)
                {
                    // Is true by default, though we have to reset it if the _touchScrollBehavior was not clamped before
                    ReflectionUtils.SetClamped(horizontalScroller.slider, true);
                    ReflectionUtils.SetClamped(verticalScroller.slider, true);
                }
                else
                {
                    // Allow movement outside the bounds for elastic and unrestricted.
                    ReflectionUtils.SetClamped(horizontalScroller.slider, false);
                    ReflectionUtils.SetClamped(verticalScroller.slider, false);
                }
            }
        }

        /// <summary>
        /// Represents the visible part of contentContainer.
        /// </summary>
        public VisualElement contentViewport { get; protected set; }

        /// <summary>
        /// Horizontal scrollbar.
        /// </summary>
        public Scroller horizontalScroller { get; protected set; }

        /// <summary>
        /// Vertical Scrollbar.
        /// </summary>
        public Scroller verticalScroller { get; protected set; }

        /// <summary>
        /// Whether the scroll view is currently animating the scroll position or not.<br />
        /// This is true if any of these animations is active: Inertia, Elasticity, ScrollTo.
        /// </summary>
        public bool isAnimating
        {
            get
            {
                return _scrollToAnimation.isActive && _inertiaAndElasticityAnimation.isActive;
            }
        }

        protected VisualElement _contentAndVerticalScrollContainer;

        protected VisualElement _contentContainer;

        /// <summary>
        /// Contains full content, potentially partially visible.
        /// </summary>
        public override VisualElement contentContainer => _contentContainer;
        

        protected void onHorizontalScrollDragElementChanged(GeometryChangedEvent evt)
        {
            if (!(evt.oldRect.size == evt.newRect.size))
            {
                updateHorizontalSliderPageSize();
            }
        }

        protected void onVerticalScrollDragElementChanged(GeometryChangedEvent evt)
        {
            if (!(evt.oldRect.size == evt.newRect.size))
            {
                updateVerticalSliderPageSize();
            }
        }

        protected void updateHorizontalSliderPageSize()
        {
            float pageSize = _horizontalPageSize;
            float width = horizontalScroller.resolvedStyle.width;

            if (_singleLineHeightFromStyle != UndefinedFloatStyleValue)
                pageSize = Mathf.Max(pageSize, _singleLineHeightFromStyle);

            if (width > 0f && Mathf.Approximately(_horizontalPageSize, UndefinedPageSize))
            {
                // PageSize: https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/ModuleOverrides/com.unity.ui/Core/Controls/Slider.cs#129
                float defaultPageSize = contentViewport.worldBound.width * (1f - pageOverlapRatio);
                pageSize = Mathf.Max(defaultPageSize, _singleLineHeightFromStyle);
            }

            if (pageSize >= 0f)
            {
                horizontalScroller.slider.pageSize = pageSize * (1f - pageOverlapRatio);
            }
        }

        protected void updateVerticalSliderPageSize()
        {
            float height = verticalScroller.resolvedStyle.height;
            float pageSize = _verticalPageSize;

            if (_singleLineHeightFromStyle != UndefinedFloatStyleValue)
                pageSize = Mathf.Max(pageSize, _singleLineHeightFromStyle);

            if (height > 0f && Mathf.Approximately(_verticalPageSize, UndefinedPageSize))
            {
                // PageSize: https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/ModuleOverrides/com.unity.ui/Core/Controls/Slider.cs#129
                float defaultPageSize = contentViewport.worldBound.height * (1f - pageOverlapRatio);
                pageSize = Mathf.Max(defaultPageSize, _singleLineHeightFromStyle);
            }

            if (pageSize >= 0f)
            {
                verticalScroller.slider.pageSize = pageSize * (1f - pageOverlapRatio);
            }
        }

        protected void applyScrollOffset()
        {
            Vector2 offset = scrollOffset;

            // Trim left and top relative positions.
            if (needsVertical)
            {
                offset.y += contentContainer.resolvedStyle.top;
            }

            if (needsHorizontal)
            {
                offset.x += contentContainer.resolvedStyle.left;
            }

            Vector3 position = contentContainer.transform.position;
            position.x = Mathf.Round(-offset.x);
            position.y = Mathf.Round(-offset.y);

            if (infinite)
            {
                if(mode == ScrollViewMode.Horizontal)
                    updateChildPositionsInInfinityX(-offset.x - contentContainer.transform.position.x);
                else if (mode == ScrollViewMode.Vertical)
                    updateChildPositionsInInfinityY(-offset.y - contentContainer.transform.position.y);
            }

            contentContainer.transform.position = position;
        }
    }
}
