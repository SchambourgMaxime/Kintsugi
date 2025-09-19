using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Each Shadow creates a new manipulator that is not part of the config / UIDocument workflow.
    /// This uses a completely separate local modifier for each shadow element.<br />
    /// The shadow actually is a glow manipulator with some special settings (fill center).
    /// </summary>
    public class Shadow : VisualElement
    {
        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public static readonly string ussClassName = "kamgam-shadow";

        // Blur Width
        const float BlurWidthDefault = 30f;
        protected float _blurWidth = BlurWidthDefault;
        public float blurWidth
        {
            get => _blurWidth;
            set
            {
                if (_blurWidth == value)
                    return;

                _blurWidth = value;

                getConfig().Width = _blurWidth;
                getConfig().OverlapWidth = _blurWidth * 0.5f;
            }
        }

        // Inner Color
        static readonly Color InnerColorDefault = new Color(0f, 0f, 0f, 0.4f);
        protected Color _innerColor = InnerColorDefault;
        public Color innerColor
        {
            get => _innerColor;
            set
            {
                if (_innerColor == value)
                    return;

                _innerColor = value;

                getConfig().InnerColor = value;
            }
        }

        // Outer Color
        static readonly Color OuterColorDefault = Color.clear;
        protected Color _outerColor = OuterColorDefault;
        public Color outerColor
        {
            get => _outerColor;
            set
            {
                if (_outerColor == value)
                    return;

                _outerColor = value;

                getConfig().OuterColor = _outerColor;
            }
        }

        // Offset X
        const float OffsetXDefault = 0f;
        protected float _offsetX = OffsetXDefault;
        public float offsetX
        {
            get => _offsetX;
            set
            {
                if (_offsetX == value)
                    return;

                _offsetX = value;

                var offset = getConfig().Offset;
                offset.x = _offsetX;
                getConfig().Offset = offset;
            }
        }

        // Offset Y
        const float OffsetYDefault = 0f;
        protected float _offsetY = OffsetYDefault;
        public float offsetY
        {
            get => _offsetY;
            set
            {
                if (_offsetY == value)
                    return;

                _offsetY = value;

                var offset = getConfig().Offset;
                offset.y = _offsetY;
                getConfig().Offset = offset;
            }
        }

        // Scale X
        const float ScaleXDefault = 1f;
        protected float _scaleX = ScaleXDefault;
        public float scaleX
        {
            get => _scaleX;
            set
            {
                if (_scaleX == value)
                    return;

                _scaleX = value;

                var scale = getConfig().Scale;
                scale.x = _scaleX;
                getConfig().Scale = scale;
            }
        }

        // Scale Y
        const float ScaleYDefault = 1f;
        protected float _scaleY = ScaleYDefault;
        public float scaleY
        {
            get => _scaleY;
            set
            {
                if (_scaleY == value)
                    return;

                _scaleY = value;

                var scale = getConfig().Scale;
                scale.y = _scaleY;
                getConfig().Scale = scale;
            }
        }

        // Vertices per corner
        const float VertexDistanceDefault = 15;
        protected float _vertexDistance = VertexDistanceDefault;
        public float vertexDistance
        {
            get => _vertexDistance;
            set
            {
                if (_vertexDistance == value)
                    return;

                value = Mathf.Max(value, 1);

                _vertexDistance = value;
                getConfig().VertexDistance = _vertexDistance;
            }
        }

        // Layout First Child
        const bool LayoutFirstChildDefault = false;
        protected bool _layoutFirstChild = LayoutFirstChildDefault;
        public bool layoutFirstChild
        {
            get => _layoutFirstChild;
            set
            {
                if (_layoutFirstChild == value)
                    return;

                _layoutFirstChild = value;
                UpdateGlowManipulator();
                DoLayoutFirstChild();
            }
        }

        [System.NonSerialized]
        protected GlowConfig _config;

        [System.NonSerialized]
        protected GlowManipulator _glowManipulator;
        public GlowManipulator manipulator => _glowManipulator;

        /// <summary>
        /// Shortcut for the first child element.
        /// </summary>
        public VisualElement content
        {
            get
            {
                if (childCount == 0)
                    return null;

                return ElementAt(0);
            }
        }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<Shadow, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatAttributeDescription _blurWidth =
                new UxmlFloatAttributeDescription { name = "blur-width", defaultValue = BlurWidthDefault };

            UxmlColorAttributeDescription _innerColor =
                new UxmlColorAttributeDescription { name = "inner-color", defaultValue = InnerColorDefault };

            UxmlColorAttributeDescription _outerColor =
                new UxmlColorAttributeDescription { name = "outer-color", defaultValue = OuterColorDefault };

            UxmlFloatAttributeDescription _offsetX =
                new UxmlFloatAttributeDescription { name = "offset-x", defaultValue = OffsetXDefault };

            UxmlFloatAttributeDescription _offsetY =
                new UxmlFloatAttributeDescription { name = "offset-y", defaultValue = OffsetYDefault };

            UxmlFloatAttributeDescription _scaleX =
                new UxmlFloatAttributeDescription { name = "scale-x", defaultValue = ScaleXDefault };

            UxmlFloatAttributeDescription _scaleY =
                new UxmlFloatAttributeDescription { name = "scale-y", defaultValue = ScaleYDefault };

            UxmlFloatAttributeDescription _verticesPerCorner =
                new UxmlFloatAttributeDescription { name = "vertex-distance", defaultValue = VertexDistanceDefault };

            UxmlBoolAttributeDescription _layoutFirstChild =
                new UxmlBoolAttributeDescription { name = "layout-first-child", defaultValue = LayoutFirstChildDefault };


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var shadow = ve as Shadow;

                shadow.blurWidth = _blurWidth.GetValueFromBag(bag, cc);
                shadow.innerColor = _innerColor.GetValueFromBag(bag, cc);
                shadow.outerColor = _outerColor.GetValueFromBag(bag, cc);
                shadow.offsetX = _offsetX.GetValueFromBag(bag, cc);
                shadow.offsetY = _offsetY.GetValueFromBag(bag, cc);
                shadow.scaleX = _scaleX.GetValueFromBag(bag, cc);
                shadow.scaleY = _scaleY.GetValueFromBag(bag, cc); 
                shadow.vertexDistance = _verticesPerCorner.GetValueFromBag(bag, cc);
                shadow.layoutFirstChild = _layoutFirstChild.GetValueFromBag(bag, cc);

#if UNITY_EDITOR
                if (shadow.layoutFirstChild)
                {
                    shadow.DoLayoutFirstChild();
                }
#endif
            }
        }

        public Shadow()
        {
            AddToClassList(ussClassName);
            this.style.overflow = Overflow.Visible;

            // Init config
            _config = getConfig();

            // Register callbacks
            RegisterCallback<AttachToPanelEvent>(onAttachToPanel);
            RegisterCallback<GeometryChangedEvent>(onGeometryChanged);
        }

        protected void onAttachToPanel(AttachToPanelEvent evt)
        {
            DoLayoutFirstChild();
            UpdateGlowManipulator();
            MarkDirtyRepaint();
        }

        protected void onGeometryChanged(GeometryChangedEvent evt)
        {
            if (layoutFirstChild)
            {
                DoLayoutFirstChild();
            }
        }

        protected GlowConfig getConfig()
        {
            if (_config == null)
                _config = createShadowConfig();

            return _config;
        }

        protected GlowConfig createShadowConfig()
        {
            var config = new GlowConfig();
            config.ClassName = null;

            // Don't remove shadow configs based on class names.
            config.RemoveIfClassIsNoLongerPresentOnTarget = false;

            config.Width = blurWidth;
            config.SplitWidth = false;
            config.Widths = default;
            config.Scale = new Vector2(scaleX, scaleY);

            config.Offset = new Vector2(offsetX, offsetY);
            config.OffsetEverything = true;

            config.InnerColor = innerColor;
            config.OuterColor = outerColor;
            config.InheritBorderColors = false;
            config.UseRadialGradients = false;
            //config.InnerColors = default;
            //config.OuterColors = default;

            config.OverlapWidth = blurWidth * 0.5f;
            config.ForceSubdivision = false;
            config.PreserveHardCorners = false;
            config.FillCenter = true;
            config.VertexDistance = vertexDistance;

            return config;
        }

        public void UpdateGlowManipulator()
        {
            // There is no manipulator yet -> find or create one.
            if (_glowManipulator == null)
            {
                var config = createShadowConfig();
                _glowManipulator = new GlowManipulator(getConfig());
                _glowManipulator.RemoveOnPlayModeStateChange = false;
                this.AddManipulator(_glowManipulator);
            }
            
            // TODO: Find out why the manipulator gets detached.
            if (_glowManipulator.target == null)
            {
                _glowManipulator.Config = getConfig();
                this.AddManipulator(_glowManipulator);
            }
        }

        public void DoLayoutFirstChild()
        {
            if (childCount > 0)
            {
                var firstChild = ElementAt(0);
                firstChild.style.borderTopLeftRadius = resolvedStyle.borderTopLeftRadius;
                firstChild.style.borderTopRightRadius = resolvedStyle.borderTopRightRadius;
                firstChild.style.borderBottomLeftRadius = resolvedStyle.borderBottomLeftRadius;
                firstChild.style.borderBottomRightRadius = resolvedStyle.borderBottomRightRadius;

                firstChild.style.flexGrow = 1f;
                firstChild.style.flexShrink = 1f;
                firstChild.style.minWidth = new Length(100f, LengthUnit.Percent);
                firstChild.style.minHeight = new Length(100f, LengthUnit.Percent);

                firstChild.style.marginTop = 0f;
                firstChild.style.marginRight = 0f;
                firstChild.style.marginBottom = 0f;
                firstChild.style.marginLeft = 0f;
            }
        }
    }
}