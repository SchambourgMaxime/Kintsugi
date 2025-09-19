using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
#if KAMGAM_VISUAL_SCRIPTING
    // Why? See: https://forum.unity.com/threads/unable-to-provide-a-default-for-getvalue-on-object-valueinput.1140022/#post-9138727
    [Unity.VisualScripting.Inspectable]
#endif

    /// <summary>
    /// Base class for animations. You can also makes your very own by implementing IGlowAnimation
    /// but it is recommended to use this since it takes care of all the scaffolding for you.
    /// </summary>
    public class SpikeAnimation : GlowAnimation
    {
        protected float _speed = 1f;
        public float Speed
        {
            get => _speed;
            set
            {
                if (_speed == value)
                    return;

                _speed = value;
                TriggerOnValueChanged();
            }
        }

        protected float _scale = 3f;
        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;

                _scale = value;
                TriggerOnValueChanged();
            }
        }

        protected int _frequency = 1;
        public int Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency == value)
                    return;

                _frequency = value;
                TriggerOnValueChanged();
            }
        }

        protected SinusMode _sinusMode = SinusMode.ClampPositive;
        public SinusMode SinusMode
        {
            get => _sinusMode;
            set
            {
                if (_sinusMode == value)
                    return;

                _sinusMode = value;
                TriggerOnValueChanged();
            }
        }

        protected bool _moveInside = false;
        public bool MoveInside
        {
            get => _moveInside;
            set
            {
                if (_moveInside == value)
                    return;

                _moveInside = value;
                TriggerOnValueChanged();
            }
        }

        protected float _progress = 0f;

        public override IGlowAnimation Copy()
        {
            var copy = new SpikeAnimation();
            copy.CopyValuesFrom(this);
            return copy;
        }

        public override void CopyValuesFrom(IGlowAnimation source)
        {
            base.CopyValuesFrom(source);

            var typedSource = source as SpikeAnimation;

            Speed = typedSource.Speed;
            Frequency = typedSource.Frequency;
            Scale = typedSource.Scale;
            SinusMode = typedSource.SinusMode;
            MoveInside = typedSource.MoveInside;
        }

        public override void Update()
        {
            _progress += DeltaTime * Speed;
        }

        public override void OnUpdateMesh(
            GlowManipulator manipulator,
            List<Vertex> vertices,
            List<ushort> triangles,
            List<ushort> outerIndices,
            List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices)
        {
            int oCount = outerIndices.Count;
            float progressStep = 1f / (oCount - 1f);
            ushort lastDisplacedInner = ushort.MaxValue;
            for (int i = 0; i < oCount; i++)
            {
                float progressCW = i * progressStep;
                float sinCW = Mathf.Sin(progressCW * 2f * Mathf.PI * Frequency + _progress);
                float displacement = (1 - SinusUtils.ApplySinusMode(Mathf.Sin(sinCW), SinusMode)) * Scale;

                var vector = GlowManipulator.DisplaceVertexOutwardsNormalized(vertices, outerToInnerIndices, outerIndices[i], displacement);

                // TODO: Move inside does not work properly for hard edges.
                if (MoveInside)
                {
                    ushort innerVertex = outerToInnerIndices[outerIndices[i]];
                    if (innerVertex != lastDisplacedInner)
                    {
                        lastDisplacedInner = innerVertex;
                        GlowManipulator.DisplaceVertex(vertices, innerVertex, vector);
                    }
                }
            }
        }
    }
}