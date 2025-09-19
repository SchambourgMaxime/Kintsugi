using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
    [CreateAssetMenu(fileName = "UITK Glow JitterAnimation", menuName = "UI Toolkit/Glow/Animation > Jitter", order = 403)]
    public class JitterAnimationAsset : GlowAnimationAsset
    {
        public static new string DefaultName = "JitterAnimation";
        public override string GetDefaultName() => DefaultName;

        [SerializeField]
        [Range(0, 10)]
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
                    (_animation as JitterAnimation).Speed = _speed;
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
                    (_animation as JitterAnimation).Scale = _scale;
                }
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
            var animation = getAnimation<JitterAnimation>(out bool createdNewCopy);
            if (createdNewCopy)
            {
                copyTo(animation);
            }

            return animation;
        }

        protected void copyTo(JitterAnimation animation)
        {
            animation.Speed = Speed;
            animation.Scale = Scale;
            animation.MoveInside = MoveInside;
        }

#if UNITY_EDITOR
        public override void onValuesChangedInInspector()
        {
            if (_animation == null)
                return;

            copyTo(_animation as JitterAnimation);

            base.onValuesChangedInInspector();
        }
#endif
    }
}