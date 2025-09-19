using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// This shows how you can do an animation on an outline purely in code from a MonoBehaviour.
    /// </summary>
    public class AnimationFromMonoBehaviourDemo : MonoBehaviour
    {
        [Header("Animation")]

        [System.NonSerialized]
        protected float _progress = 0f;

        [Range(0, 20)]
        public float Speed = 1f;

        [Range(1, 10)]
        public int Frequency = 1;

        public string ElementName = "Animation";

        protected GlowDocument _glowDoc;
        public GlowDocument GlowDoc
        {
            get
            {
                if (_glowDoc == null)
                {
                    _glowDoc = this.GetComponent<GlowDocument>();
                }
                return _glowDoc;
            }
        }

        protected GlowManipulator _manipulator;

        public void OnEnable()
        {
            GlowDoc.RegisterOnEnable(OnGlowEnabled);
        }

        public void OnGlowEnabled()
        {
            var element = GlowDoc.Document.rootVisualElement.Q<VisualElement>(name: ElementName);
            _manipulator = GlowPanel.GetManipulator(element);
            _manipulator.OnBeforeMeshWrite -= updateMesh;
            _manipulator.OnBeforeMeshWrite += updateMesh;
        }

        public void Update()
        {
            if (_manipulator != null)
            {
                // Here we mark the element for a repaint with animation.
                // This will trigger a new mesh generation cycle (OnBeforeMeshWrite -> updateMesh()).
                _manipulator.MarkDirtyAnimation();

                // This value is what actually drives the movement in updateMesh() below.
                _progress += Time.deltaTime * Speed;
            }
        }

        protected void updateMesh(
            GlowManipulator manipulator,
            List<Vertex> vertices,
            List<ushort> triangles,
            List<ushort> outerIndices,
            List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices)
        {
            int oCount = outerIndices.Count;
            float progressStep = 1f / (oCount-1f);
            for (int i = 0; i < oCount; i++)
            {
                float progressCW = i * progressStep;
                float sinCW = Mathf.Sin(progressCW * 2f * Mathf.PI * Frequency + _progress);
                GlowManipulator.DisplaceVertexOutwardsNormalized(vertices, outerToInnerIndices, outerIndices[i], Mathf.Max( 0f, 3f * sinCW));
            } 
        }

        
    }

}