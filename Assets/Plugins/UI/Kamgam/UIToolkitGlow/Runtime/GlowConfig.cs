using UnityEngine;
using System.Collections.Generic;


namespace Kamgam.UIToolkitGlow
{
#if KAMGAM_VISUAL_SCRIPTING
    // Why? See: https://forum.unity.com/threads/unable-to-provide-a-default-for-getvalue-on-object-valueinput.1140022/#post-9138727
    [Unity.VisualScripting.Inspectable]
#endif
    [System.Serializable]
    public class GlowConfig : ISerializationCallbackReceiver
    {
        [System.Serializable]
        public struct DirectionValues
        {
            public float Top;
            public float Right;
            public float Bottom;
            public float Left;

            public DirectionValues(float top, float right, float bottom, float left)
            {
                Top = top;
                Right = right;
                Bottom = bottom;
                Left = left;
            }

            public bool Equals(DirectionValues other)
            {
                return
                       Mathf.Approximately(Top, other.Top)
                    && Mathf.Approximately(Right, other.Right)
                    && Mathf.Approximately(Bottom, other.Bottom)
                    && Mathf.Approximately(Left, other.Left);
            }
        }


        [System.NonSerialized]
        public System.Action OnValueChanged;

        [Tooltip("This is the class name that you can add to your element.")]
        public string Name => ClassName;

        public const string DefaultGlowClassName = "glow-default";

        [Tooltip("Add this class name to your visual element to make it use this shadow profile.")]
        [SerializeField]
        protected string _className = DefaultGlowClassName;
        public string ClassName
        {
            get => _className;
            set
            {
                if (string.Compare(_className, value) == 0)
                    return;

                _className = value;
                TriggerValueChanged();
            }
        }

        [Header("Width")]
        [SerializeField]
        [ShowIf("_splitWidth", false, ShowIfAttribute.DisablingType.ReadOnly)]
        [Tooltip("Width of shadow in pixels outwards.")]
        [Min(0)]
        protected float _width;
        public float Width { 
            get => _width; 
            set
            {
                if (Mathf.Approximately(value, _width))
                    return;
                
                _width = value;
                TriggerValueChanged();
            }
        }

        [SerializeField]
        protected bool _splitWidth = false;
        public bool SplitWidth
        {
            get => _splitWidth;
            set
            {
                if (_splitWidth == value)
                    return;

                _splitWidth = value;
                TriggerValueChanged();
            }
        }

        [SerializeField]
        [ShowIf("_splitWidth", true)]
        protected DirectionValues _widths;
        public DirectionValues Widths
        {
            get => _widths;
            set
            {
                if (value.Equals(_widths))
                    return;

                _widths = value;
            }
        }

        [SerializeField]
        [Tooltip("Overlap defines how much the outline will overlap on the inside.\n" +
            "An overlap of 0.2 is recommended to make up for float inacurracies and the (still) missing anti aliasing.")]
        protected float _overlapWidth = 0.2f;
        public float OverlapWidth
        {
            get => _overlapWidth;
            set
            {
                if (Mathf.Approximately(_overlapWidth, value))
                    return;

                _overlapWidth = value;
                TriggerValueChanged();
            }
        }

        [SerializeField]
        [Tooltip("Useful for shadows. Scales the effect.")]
        protected Vector2 _scale = Vector2.one;
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;

                _scale = value;
                TriggerValueChanged();
            }
        }

        [Header("Colors")]
        [SerializeField]
        [Tooltip("If enabled then you can specify color gradients. They are wound around the element in clock-wise direction, starting top left.")]
        protected bool _useRadialGradients = false;
        public bool UseRadialGradients
        {
            get => _useRadialGradients;
            set
            {
                if (_useRadialGradients == value)
                    return;

                _useRadialGradients = value;
                TriggerValueChanged();
            }
        }

        [ShowIf("_useRadialGradients", false)]
        [Tooltip("Shadow start color at the closes points to the element.")]
        [SerializeField]
        protected Color _innerColor = new Color(0f, 0f, 0f, 0.3f);
        public Color InnerColor
        {
            get => _innerColor;
            set
            {
                if (_innerColor == value)
                    return;

                _innerColor = value;
                TriggerValueChanged();
            }
        }

        [ShowIf("_useRadialGradients", false)]
        [Tooltip("Shadow end color at the fartest points.")]
        [SerializeField]
        protected Color _outerColor = new Color(0f, 0f, 0f, 0f);
        public Color OuterColor
        {
            get => _outerColor;
            set
            {
                if (_outerColor == value)
                    return;

                _outerColor = value;
                TriggerValueChanged();
            }
        }

        /// <summary>
        /// Alias for setting both inner and outer color at the same time.
        /// </summary>
        public Color Color
        {
            get => _innerColor;
            set
            {
                if (_innerColor == value && _outerColor == value)
                    return;

                InnerColor = value;
                OuterColor = value;
            }
        }

        [SerializeField]
        [Tooltip("If enabled then the border color is muliplied by the color set here. HINT: Use white to simply use the border color.")]
        protected bool _inheritBorderColor = false;
        public bool InheritBorderColors
        {
            get => _inheritBorderColor;
            set
            {
                if (_inheritBorderColor == value)
                    return;

                _inheritBorderColor = value;
                TriggerValueChanged();
            }
        }

        [SerializeField]
        [ShowIf("_useRadialGradients", true)]
        protected Gradient _innerColors;
        public Gradient InnerColors
        {
            get => _innerColors;
            set
            {
                if (_innerColors == value)
                    return;

                _innerColors = value;
                TriggerValueChanged();
            }
        }

        [ShowIf("_useRadialGradients", true)]
        [SerializeField]
        protected Gradient _outerColors;
        public Gradient OuterColors
        {
            get => _outerColors;
            set
            {
                if (_outerColors == value)
                    return;

                _outerColors = value;
                TriggerValueChanged();
            }
        }

        [Header("Offset")]

        [SerializeField]
        protected Vector2 _offset;
        public Vector2 Offset
        {
            get => _offset;
            set
            {
                if (_offset == value)
                    return;

                _offset = value;
            }
        }

        [SerializeField]
        [Tooltip("Should the offset affect every vertex of the glow mesh or only the outer ones?\n" +
            "For glow you usually want to keep this turned OFF while for shadows you may want to turn this ON.")]
        protected bool _offsetEverything = false;
        public bool OffsetEverything
        {
            get => _offsetEverything;
            set
            {
                if (value == _offsetEverything)
                    return;

                _offsetEverything = value;
            }
        }


        [Header("Advanced Settings")]

        [SerializeField]
        [Tooltip("Force a uniform distance between vertices. Useful to smooth animations.\n" +
            "This is always ON if UseRadialGradients is enabled.")]
        protected bool _forceSudvision = false;
        public bool ForceSubdivision
        {
            get => _forceSudvision;
            set
            {
                if (value == _forceSudvision)
                    return;

                _forceSudvision = value;
            }
        }

        [SerializeField]
        [Tooltip("Should hard corners be kept hard or should they be rounded off as the glow extends outwards?")]
        protected bool _preserveHardCorners = false;
        public bool PreserveHardCorners
        {
            get => _preserveHardCorners;
            set
            {
                if (value == _preserveHardCorners)
                    return;

                _preserveHardCorners = value;
            }
        }

        [SerializeField]
        [Tooltip("Fills the center with inner color triangles. Useful for shadow generation.")]
        protected bool _fillCenter = false;
        public bool FillCenter
        {
            get => _fillCenter;
            set
            {
                if (value == _fillCenter)
                    return;

                _fillCenter = value;
            }
        }

        [SerializeField]
        [Range(10, 300)]
        public float _vertexDistance = 15;
        public float VertexDistance
        {
            get => _vertexDistance;
            set
            {
                if (_vertexDistance == value)
                    return;

                _vertexDistance = value;
                TriggerValueChanged();
            }
        }

        [SerializeField]
        protected string _animation;
        public string Animation
        {
            get => _animation;
            set
            {
                if (_animation == value)
                    return;

                _animation = value;
                TriggerValueChanged();
            }
        }

        [System.NonSerialized]
        public bool RemoveIfClassIsNoLongerPresentOnTarget = true;

        public GlowConfig Copy(bool copyOnValueChanged = true)
        {
            var copy = new GlowConfig();
            CopyValuesTo(copy, copyOnValueChanged);
            return copy;
        }

        public void CopyValuesTo(GlowConfig copy, bool copyOnValueChanged = true)
        {
            if (copyOnValueChanged)
                copy.OnValueChanged = OnValueChanged;

            copy._className = ClassName;

            copy._width = Width;
            copy._splitWidth = SplitWidth;
            copy._widths = Widths;
            copy._scale = Scale;

            copy._offset = Offset;
            copy._offsetEverything = OffsetEverything;

            copy._innerColor = InnerColor;
            copy._outerColor = OuterColor;
            copy._inheritBorderColor = InheritBorderColors;
            copy._useRadialGradients = UseRadialGradients;
            copy._innerColors = InnerColors;
            copy._outerColors = OuterColors;

            copy._overlapWidth = OverlapWidth;
            copy._forceSudvision = ForceSubdivision;
            copy._preserveHardCorners = PreserveHardCorners;
            copy._fillCenter = FillCenter;
            copy._vertexDistance = VertexDistance;
            copy._animation = Animation;

            copy.RemoveIfClassIsNoLongerPresentOnTarget = RemoveIfClassIsNoLongerPresentOnTarget;
        }

        public void TriggerValueChanged()
        {
            OnValueChanged?.Invoke();
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // used to prepopulate the values in the list if a new profile is generated.
            if (string.IsNullOrEmpty(ClassName) && Width == 0f)
            {
                ClassName = DefaultGlowClassName;
                Width = 10f;
                SplitWidth = false;
                Widths = default;
                Offset = Vector2.zero;
                InnerColor = new Color(0f, 0f, 0f, 0.3f);
                OuterColor = new Color(0f, 0f, 0f, 0f);
                VertexDistance = 15;
                OverlapWidth = 0.2f;
                UseRadialGradients = false;
                InnerColors = default;
                OuterColors = default;
            }
#endif
        }
    }
}