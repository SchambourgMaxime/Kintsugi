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
    public class JitterAnimation : GlowAnimation
    {
        protected float _speed = 1f;
        /// <summary>
        /// Speed ranged from -1f to 1f.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                if (_speed == value)
                    return;

                _speed = value;
                _newDisplacementsTimer = -0.1f;
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
            var copy = new JitterAnimation();
            copy.CopyValuesFrom(this);
            return copy;
        }

        public override void CopyValuesFrom(IGlowAnimation source)
        {
            base.CopyValuesFrom(source);

            var typedSource = source as JitterAnimation;

            Speed = typedSource.Speed;
            Scale = typedSource.Scale;
            MoveInside = typedSource.MoveInside;
        }

        protected float _newDisplacementsTimer = -1f;

        public override void Update()
        {
            if (Mathf.Abs(Speed) > 0.001f)
                _newDisplacementsTimer -= DeltaTime;
        }

        private float getDisplacementDuration()
        {
            return 1.001f - Speed / 10f;
        }

        public Dictionary<ushort, float> _lastDisplacement = new Dictionary<ushort, float>();
        public Dictionary<ushort, float> _nextDisplacement = new Dictionary<ushort, float>();

        public override void OnUpdateMesh(
            GlowManipulator manipulator,
            List<Vertex> vertices,
            List<ushort> triangles,
            List<ushort> outerIndices,
            List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices)
        {
            if (_lastDisplacement.Count != vertices.Count || _newDisplacementsTimer < 0f)
            {
                _newDisplacementsTimer = getDisplacementDuration();
                newDisplacementValues(vertices);
            }

            int oCount = outerIndices.Count;
            float progressStep = 1f / (oCount - 1f);
            ushort lastDisplacedInner = ushort.MaxValue;
            for (int i = 0; i < oCount; i++)
            {
                var lastDisplacement = _lastDisplacement[outerIndices[i]];
                var nextDisplacement = _nextDisplacement[outerIndices[i]];
                float t = _newDisplacementsTimer / getDisplacementDuration();
                var vector = GlowManipulator.DisplaceVertexOutwardsNormalized(vertices, outerToInnerIndices, outerIndices[i],
                                Mathf.Lerp(nextDisplacement, lastDisplacement, t));

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

        protected void newDisplacementValues(List<Vertex> vertices)
        {
            ushort vCount = (ushort)vertices.Count;
            if (_lastDisplacement.Count != vCount)
            {
                _lastDisplacement.Clear();
                _nextDisplacement.Clear();
                for (ushort i = 0; i < vCount; i++)
                {
                    _lastDisplacement.Add(i, Random.Range(0f, Scale));
                    _nextDisplacement.Add(i, Random.Range(0f, Scale));
                }
            }
            else
            {
                for (ushort i = 0; i < vCount; i++)
                {
                    _lastDisplacement[i] = _nextDisplacement[i];
                    _nextDisplacement[i] = Random.Range(0f, Scale);
                }
            }
        }
    }
}