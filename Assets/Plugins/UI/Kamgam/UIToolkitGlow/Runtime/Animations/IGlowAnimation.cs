using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public interface IGlowAnimation
    {
        event System.Action<GlowAnimation> OnValueChanged;

        /// <summary>
        /// If set to -1 then Application.targetFrameRate will be used.
        /// </summary>
        int FrameRate { get; set; }

        GlowManipulator Manipulator { get; }

        string Name { get; set; }

        IGlowAnimation Copy();

        void CopyValuesFrom(IGlowAnimation source);

        void TriggerOnValueChanged();

        /// <summary>
        /// Call this to add an animation to a manipulator.
        /// </summary>
        /// <param name="manipulator"></param>
        void AddToManipulator(GlowManipulator manipulator);

        /// <summary>
        /// Call this to remove an animation from the Manipulator.
        /// </summary>
        void RemoveFromManipulator();

        /// <summary>
        /// Call this to remove an animation from a manipulator.
        /// </summary>
        /// <param name="manipulator"></param>
        void RemoveFromManipulator(GlowManipulator manipulator);

        /// <summary>
        /// Stops or pauses the animation.
        /// </summary>
        void Pause();

        /// <summary>
        /// Starts or resumes the animation.
        /// </summary>
        void Play();

        /// <summary>
        /// Called every "frame".<br />
        /// A frame is schedule with a delay of: "1000 / FrameRate" milliseconds.<br />
        /// It is recommended to not use Time.deltaTime here. Instead use "1f / FrameRate".
        /// </summary>
        void Update();

        /// <summary>
        /// Called whenever a mesh update is needed.
        /// Usually that's a few times per frame. It depends on whether a mesh change is triggered by some other code or not.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="outerIndices"></param>
        /// <param name="innerIndices"></param>
        /// <param name="outerToInnerIndices"></param>
        void OnUpdateMesh(
            GlowManipulator manipulator,
            List<Vertex> vertices,
            List<ushort> triangles,
            List<ushort> outerIndices,
            List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices);
    }
}