using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
    [CreateAssetMenu(fileName = "UITK Glow SpikeAnimation", menuName = "UI Toolkit/Glow/Animation > Spike", order = 403)]
    public class SpikeAnimationAsset : GlowAnimationAsset
    {
        public static new string DefaultName = "SpikeAnimation";
        public override string GetDefaultName() => DefaultName;

        [SerializeField]
        [Range(-20, 20)]
        protected float _speed = 1f;
        public float Speed
        {
            get => _speed;
            set
            {
                if (_speed == value)
                    return;

                _speed = value;

                if (_animation != null)
                {
                    (_animation as SpikeAnimation).Speed = _speed;
                }
            }
        }

        [SerializeField]
        [Range(-1, 20)]
        protected float _scale = 1f;
        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;

                _scale = value;

                if (_animation != null)
                {
                    (_animation as SpikeAnimation).Scale = _scale;
                }
            }
        }

        [SerializeField]
        [Range(1, 10)]
        protected int _frequency = 1;
        public int Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency == value)
                    return;

                _frequency = value;

                if (_animation != null)
                {
                    (_animation as SpikeAnimation).Frequency = _frequency;
                }
            }
        }

        [SerializeField]
        protected SinusMode _sinusMode = SinusMode.ClampPositive;
        public SinusMode SinusMode
        {
            get => _sinusMode;
            set
            {
                if (_sinusMode == value)
                    return;

                _sinusMode = value;
            }
        }

        [SerializeField]
        [Tooltip("EXPERIMENTAL: If enabled then it moves the inside vertices too.")]
        protected bool _moveInside = false;
        public bool MoveInside
        {
            get => _moveInside;
            set
            {
                if (_moveInside == value)
                    return;

                _moveInside = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override IGlowAnimation GetAnimation()
        {
            var animation = getAnimation<SpikeAnimation>(out bool createdNewCopy);
            if (createdNewCopy)
            {
                copyTo(animation);
            }

            return animation;
        }

        protected void copyTo(SpikeAnimation animation)
        {
            animation.Frequency = Frequency;
            animation.Speed = Speed;
            animation.Scale = Scale;
            animation.SinusMode = SinusMode;
            animation.MoveInside = MoveInside;
        }

#if UNITY_EDITOR
        public override void onValuesChangedInInspector()
        {
            if (_animation == null)
                return;

            copyTo(_animation as SpikeAnimation);

            base.onValuesChangedInInspector();
        }
#endif
    }
}