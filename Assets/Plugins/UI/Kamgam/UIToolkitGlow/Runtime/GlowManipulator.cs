using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// The glow manipulator is resposible for generating the mesh.<br />
    /// It is best to avoid mesh rebuilds. You can force a refresh on the 
    /// vertices without all the heavy calculations by calling MarkDirtyAnimation().
    /// If you combine this with setting the OnBeforeMeshWrite delegate then you can
    /// change the positions and colors of the existing vertices without recalculating the whole mesh.
    /// </summary>
    public class GlowManipulator : MeshManipulator<GlowManipulator>
    {
        public GlowConfig _config;
        public GlowConfig Config
        {
            get => _config;
            set
            {
                if (_config == value)
                    return;

                if (_config != null)
                {
                    _config.OnValueChanged -= onValueChanged;
                }

                _config = value;

                if (_config != null)
                {
                    _config.OnValueChanged += onValueChanged;
                }
            }
        }

        public bool RemoveOnPlayModeStateChange = true;

        public delegate void OnBeforeMeshWriteDelegate(
            GlowManipulator manipulator,
            List<Vertex> vertices, List<ushort> triangles,
            List<ushort> outerIndices, List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices);

        /// <summary>
        /// Set this to modify the vertices and triangles just before they are written to the mesh.
        /// </summary>
        public OnBeforeMeshWriteDelegate OnBeforeMeshWrite;

        // Indices of outer vertices in clock-wise order starting at the top-left-top corner position.
        protected List<ushort> _outerIndices = new List<ushort>(100);
        protected List<ushort> _innerIndices = new List<ushort>(100);

        // A dictionary that makes it easy to match oute indices to inner indices.
        protected Dictionary<ushort, ushort> _outerToInnerIndices = new Dictionary<ushort, ushort>(100);

        protected List<Vertex> _vertices = new List<Vertex>(100);
        protected List<ushort> _triangles = new List<ushort>(100);

        protected bool _dirtyAnimation = false;

        /// <summary>
        /// Marks the mesh a dirty for animation. Which means it will not recalculate
        /// the mesh but you can use the OnBeforeMeshWrite delegate to update the vertex properties.
        /// </summary>
        public void MarkDirtyAnimation()
        {
            if (target == null)
                return;

            _dirtyAnimation = true;
            target.MarkDirtyRepaint();
        }

        public GlowManipulator(GlowConfig config)
        {
            Config = config;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChanged;
#endif
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.RegisterCallbacksOnTarget();

            target.RegisterCallback<DetachFromPanelEvent>(onDetach);
            
            if (_config != null)
            {
                _config.OnValueChanged -= onValueChanged;
                _config.OnValueChanged += onValueChanged;
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            base.UnregisterCallbacksFromTarget();

            target.UnregisterCallback<DetachFromPanelEvent>(onDetach);

            if (_config != null)
            {
                _config.OnValueChanged -= onValueChanged;
            }
        }

        private void onDetach(DetachFromPanelEvent evt)
        {
            Clear();
            _dirtyAnimation = false;
        }

        public void Clear()
        {
            _outerIndices.Clear();
            _innerIndices.Clear();

            _vertices.Clear();
            _triangles.Clear();

            _outerToInnerIndices.Clear();

            _tmpVerticesForMeshCallback.Clear();
        }

#if UNITY_EDITOR
        private void onPlayModeStateChanged(PlayModeStateChange change)
        {
            if (RemoveOnPlayModeStateChange)
            {
                if (change == PlayModeStateChange.ExitingPlayMode || change == PlayModeStateChange.ExitingEditMode)
                {
                    if (target != null)
                        target.RemoveManipulator(this);
                }
            }
        }
#endif

        protected void onValueChanged()
        {
            if (target == null)
                return;

            Clear();
            target.MarkDirtyRepaint();
        }

        private Rect _lastContentRect;

        // Remember: "generateVisualContent is an addition to the default rendering, it's not a replacement"
        // See: https://forum.unity.com/threads/hp-bars-at-runtime-image-masking-or-fill.1076486/#post-6948578 

        protected override void generateVisualContent(MeshGenerationContext mgc)
        {
            if (_dirtyAnimation)
            {
                // Check if the target rect has changed. If yes, then do a full mesh generation.
                if (_lastContentRect == null || _lastContentRect != mgc.visualElement.contentRect)
                {
                    generateVisualContentFromScratch(mgc);
                }
                else
                {
                    generateVisualContentForAnimation(mgc);
                }

                _dirtyAnimation = false;
            }
            else
            {
                generateVisualContentFromScratch(mgc);
            }
        }

        protected List<Vertex> _tmpVerticesForMeshCallback = new List<Vertex>();

        protected void generateVisualContentForAnimation(MeshGenerationContext mgc)
        {
            if (OnBeforeMeshWrite == null || target.panel.contextType != ContextType.Player)
                return;

            // Make a copy for the animation to operate on.
            copyVerticesToMeshCallbackTmp();

            // Let it be modified.
            OnBeforeMeshWrite?.Invoke(this, _tmpVerticesForMeshCallback, _triangles, _outerIndices, _innerIndices, _outerToInnerIndices);

            writeMeshData(mgc, _tmpVerticesForMeshCallback, _triangles);
        }

        protected void copyVerticesToMeshCallbackTmp()
        {
            _tmpVerticesForMeshCallback.Clear();

            int vCount = _vertices.Count;
            for (int i = 0; i < vCount; i++)
            {
                _tmpVerticesForMeshCallback.Add(_vertices[i]);
            }
        }

        protected void generateVisualContentFromScratch(MeshGenerationContext mgc)
        {
            var target = mgc.visualElement;
            
            float width = GlowStyles.ResolveStyle(GlowStyles.Width, target, Config.Width);

            if (Config == null || Mathf.Approximately(width, 0f))
            {
                return;
            }

            _lastContentRect = target.contentRect;

            // If the config has a class name then check if the element still has the class.
            // If not then remove the manipulator.
            //
            // Hopefully one day we get a classAdded or class Remove event, see:
            // https://forum.unity.com/threads/event-for-visualelement-class-list-change.1410444/
            if (Config.RemoveIfClassIsNoLongerPresentOnTarget && !target.ClassListContains(Config.ClassName))
            {
                target.RemoveManipulator(this);
                return;
            }

            var contentRect = target.contentRect;
            var resolvedStyle = target.resolvedStyle;

            // Calculate the rect from which we can then calculate the corner
            // curves based on the radii (i.e. content rect without padding).
            Rect contentRectWithoutPadding = contentRect;

            float paddingLeft = Mathf.Max(0, resolvedStyle.paddingLeft);
            float paddingRight = Mathf.Max(0, resolvedStyle.paddingRight);
            float paddingTop = Mathf.Max(0, resolvedStyle.paddingTop);
            float paddingBottom = Mathf.Max(0, resolvedStyle.paddingBottom);

            float borderLeft = Mathf.Max(0, resolvedStyle.borderLeftWidth);
            float borderRight = Mathf.Max(0, resolvedStyle.borderRightWidth);
            float borderTop = Mathf.Max(0, resolvedStyle.borderTopWidth);
            float borderBottom = Mathf.Max(0, resolvedStyle.borderBottomWidth);

            // Calculate outer border radii
            float radiusTopLeft = Mathf.Max(0, resolvedStyle.borderTopLeftRadius);
            float radiusTopRight = Mathf.Max(0, resolvedStyle.borderTopRightRadius);
            float radiusBottomLeft = Mathf.Max(0, resolvedStyle.borderBottomLeftRadius);
            float radiusBottomRight = Mathf.Max(0, resolvedStyle.borderBottomRightRadius);

            // Keep in mind that each radius is max half the size of the outerRect and these can differ
            // on each axis (x / y). Thus we need onve value for each axis for each corner.
            Vector2 cornerSizeTopLeft = new Vector2(
                Mathf.Clamp(radiusTopLeft, 0, resolvedStyle.width * 0.5f),
                Mathf.Clamp(radiusTopLeft, 0, resolvedStyle.height * 0.5f)
            );

            Vector2 cornerSizeTopRight = new Vector2(
                Mathf.Clamp(radiusTopRight, 0, resolvedStyle.width * 0.5f),
                Mathf.Clamp(radiusTopRight, 0, resolvedStyle.height * 0.5f)
            );

            Vector2 cornerSizeBottomLeft = new Vector2(
                Mathf.Clamp(radiusBottomLeft, 0, resolvedStyle.width * 0.5f),
                Mathf.Clamp(radiusBottomLeft, 0, resolvedStyle.height * 0.5f)
            );

            Vector2 cornerSizeBottomRight = new Vector2(
                Mathf.Clamp(radiusBottomRight, 0, resolvedStyle.width * 0.5f),
                Mathf.Clamp(radiusBottomRight, 0, resolvedStyle.height * 0.5f)
            );


            // Resolve custom styles
            // float width = GlowStyles.ResolveStyle(GlowStyles.Width, target, Config.Width);
            float overlapWidth = GlowStyles.ResolveStyle(GlowStyles.OverlapWidth, target, Config.OverlapWidth);
            bool splitWidth = GlowStyles.ResolveStyle(GlowStyles.SplitWidth, target, Config.SplitWidth);
            float widthLeft = GlowStyles.ResolveStyle(GlowStyles.WidthLeft, target, Config.Widths.Left);
            float widthTop = GlowStyles.ResolveStyle(GlowStyles.WidthTop, target, Config.Widths.Top);
            float widthRight = GlowStyles.ResolveStyle(GlowStyles.WidthRight, target, Config.Widths.Right);
            float widthBottom = GlowStyles.ResolveStyle(GlowStyles.WidthBottom, target, Config.Widths.Bottom);
            float offsetX = GlowStyles.ResolveStyle(GlowStyles.OffsetX, target, Config.Offset.x);
            float offsetY = GlowStyles.ResolveStyle(GlowStyles.OffsetY, target, Config.Offset.y);
            var offset = new Vector2(offsetX, offsetY);
            bool offsetEverything = GlowStyles.ResolveStyle(GlowStyles.OffsetEverything, target, Config.OffsetEverything);
            float scaleX = GlowStyles.ResolveStyle(GlowStyles.ScaleX, target, Config.Scale.x);
            float scaleY = GlowStyles.ResolveStyle(GlowStyles.ScaleY, target, Config.Scale.y);
            var scale = new Vector2(scaleX, scaleY);
            Color innerColor = GlowStyles.ResolveStyle(GlowStyles.InnerColor, target, Config.InnerColor);
            Color outerColor = GlowStyles.ResolveStyle(GlowStyles.OuterColor, target, Config.OuterColor);
            bool inheritBorderColors = GlowStyles.ResolveStyle(GlowStyles.InheritBorderColors, target, Config.InheritBorderColors);
            bool forceSubdivision = GlowStyles.ResolveStyle(GlowStyles.ForceSubdivision, target, Config.ForceSubdivision);
            bool preserveHardCorners = GlowStyles.ResolveStyle(GlowStyles.PreserveHardCorners, target, Config.PreserveHardCorners);
            bool fillCenter = GlowStyles.ResolveStyle(GlowStyles.FillCenter, target, Config.FillCenter);
            float vertexDistance = GlowStyles.ResolveStyle(GlowStyles.VertexDistance, target, Config.VertexDistance);

            // Mesh generation
            // Vertices are ordered clockwise (top, right, bottom, left). Vertices are shared between triangles,
            // except for the very first and last (inner and outer).

            // Calc corner positions
            Vector2 cornerTopLeft = new Vector2(
                contentRect.xMin - paddingLeft - borderLeft + cornerSizeTopLeft.x,
                contentRect.yMin - paddingTop - borderTop + cornerSizeTopLeft.y);
            Vector2 cornerTopRight = new Vector2(
                contentRect.xMax + paddingRight + borderRight - cornerSizeTopRight.x,
                contentRect.yMin - paddingTop - borderTop + cornerSizeTopRight.y);
            Vector2 cornerBottomLeft = new Vector2(
                contentRect.xMin - paddingLeft - borderLeft + cornerSizeBottomLeft.x,
                contentRect.yMax + paddingBottom + borderBottom - cornerSizeBottomLeft.y);
            Vector2 cornerBottomRight = new Vector2(
                contentRect.xMax + paddingRight + borderRight - cornerSizeBottomRight.x,
                contentRect.yMax + paddingBottom + borderBottom - cornerSizeBottomRight.y);

            vertexDistance = Mathf.Max(5f, vertexDistance);

            Color borderTopColor = Color.white;
            Color borderRightColor = Color.white;
            Color borderBottomColor = Color.white;
            Color borderLeftColor = Color.white;

            if (inheritBorderColors)
            {
                borderTopColor = target.resolvedStyle.borderTopColor;
                borderRightColor = target.resolvedStyle.borderRightColor;
                borderBottomColor = target.resolvedStyle.borderBottomColor;
                borderLeftColor = target.resolvedStyle.borderLeftColor;
            }

            // If the overlap is bigger than half the size then we have to shrink it.
            float overlapX;
            if (overlapWidth < contentRect.width * 0.5f)
            {
                overlapX = overlapWidth;
            }
            else
            {
                overlapX = contentRect.width * 0.5f;
            }

            float overlapY;
            if (overlapWidth < contentRect.height * 0.5f)
            {
                overlapY = overlapWidth;
            }
            else
            {
                overlapY = contentRect.height * 0.5f;
            }

            // Apply overlap to corners
            cornerTopLeft.x -= Mathf.Min(0f, cornerSizeTopLeft.x - overlapX);
            cornerTopLeft.y -= Mathf.Min(0f, cornerSizeTopLeft.y - overlapY);

            cornerTopRight.x += Mathf.Min(0f, cornerSizeTopRight.x - overlapX);
            cornerTopRight.y -= Mathf.Min(0f, cornerSizeTopRight.y - overlapY);

            cornerBottomRight.x += Mathf.Min(0f, cornerSizeBottomRight.x - overlapX);
            cornerBottomRight.y += Mathf.Min(0f, cornerSizeBottomRight.y - overlapY);

            cornerBottomLeft.x -= Mathf.Min(0f, cornerSizeBottomLeft.x - overlapX);
            cornerBottomLeft.y += Mathf.Min(0f, cornerSizeBottomLeft.y - overlapY);

            bool topLeftWasHardCorner = cornerSizeTopLeft.x <= 0 && cornerSizeTopLeft.y <= 0;
            bool topRightWasHardCorner = cornerSizeTopRight.x <= 0 && cornerSizeTopRight.y <= 0;
            bool bottomRightWasHardCorner = cornerSizeBottomRight.x <= 0 && cornerSizeBottomRight.y <= 0;
            bool bottomLeftWasHardCorner = cornerSizeBottomLeft.x <= 0 && cornerSizeBottomLeft.y <= 0; 

            // .. and corner size

            // If else shenanegans are for when overlap is negative and it's a hard corner
            if (preserveHardCorners && topLeftWasHardCorner)
            {
                cornerSizeTopLeft.x = 0f;
                cornerTopLeft.x -= Mathf.Max(0f, cornerSizeTopLeft.x - overlapX);
            }
            else
            {
                cornerSizeTopLeft.x = Mathf.Max(0f, cornerSizeTopLeft.x - overlapX);
            }
            if (preserveHardCorners && topLeftWasHardCorner)
            {
                cornerSizeTopLeft.y = 0f;
                cornerTopLeft.y -= Mathf.Max(0f, cornerSizeTopLeft.y - overlapY);
            }
            else
            {
                cornerSizeTopLeft.y = Mathf.Max(0f, cornerSizeTopLeft.y - overlapY);
            }

            if (preserveHardCorners && topRightWasHardCorner)
            {
                cornerSizeTopRight.x = 0f;
                cornerTopRight.x += Mathf.Max(0f, cornerSizeTopRight.x - overlapX);
            }
            else
            {
                cornerSizeTopRight.x = Mathf.Max(0f, cornerSizeTopRight.x - overlapX);
            }
            if (preserveHardCorners && topRightWasHardCorner)
            {
                cornerSizeTopRight.y = 0f;
                cornerTopRight.y -= Mathf.Max(0f, cornerSizeTopRight.y - overlapY);
            }
            else
            {
                cornerSizeTopRight.y = Mathf.Max(0f, cornerSizeTopRight.y - overlapY);
            }

            if (preserveHardCorners && bottomRightWasHardCorner)
            {
                cornerSizeBottomRight.x = 0f;
                cornerBottomRight.x += Mathf.Max(0f, cornerSizeBottomRight.x - overlapX);
            }
            else
            {
                cornerSizeBottomRight.x = Mathf.Max(0f, cornerSizeBottomRight.x - overlapX);
            }
            if (preserveHardCorners && bottomRightWasHardCorner)
            {
                cornerSizeBottomRight.y = 0f;
                cornerBottomRight.y += Mathf.Max(0f, cornerSizeBottomRight.y - overlapY);
            }
            else
            {
                cornerSizeBottomRight.y = Mathf.Max(0f, cornerSizeBottomRight.y - overlapY);
            }

            if (preserveHardCorners && bottomLeftWasHardCorner)
            {
                cornerSizeBottomLeft.x = 0f;
                cornerBottomLeft.x -= Mathf.Max(0f, cornerSizeBottomLeft.x - overlapX);
            }
            else
            {
                cornerSizeBottomLeft.x = Mathf.Max(0f, cornerSizeBottomLeft.x - overlapX);
            }
            if (preserveHardCorners && bottomLeftWasHardCorner)
            {
                cornerSizeBottomLeft.y = 0f;
                cornerBottomLeft.y += Mathf.Max(0f, cornerSizeBottomLeft.y - overlapY);
            }
            else
            {
                cornerSizeBottomLeft.y = Mathf.Max(0f, cornerSizeBottomLeft.y - overlapY);
            }

            widthTop = (splitWidth ? widthTop : width) + overlapWidth;
            widthRight = (splitWidth ? widthRight : width) + overlapWidth;
            widthBottom = (splitWidth ? widthBottom : width) + overlapWidth;
            widthLeft = (splitWidth ? widthLeft : width) + overlapWidth;

            // Scale
            if (!Mathf.Approximately(scale.x, 1.0f) ||
                !Mathf.Approximately(scale.y, 1.0f))
            {
                cornerTopLeft = contentRect.center + (cornerTopLeft - contentRect.center) * scale;
                cornerTopRight = contentRect.center + (cornerTopRight - contentRect.center) * scale;
                cornerBottomRight = contentRect.center + (cornerBottomRight - contentRect.center) * scale;
                cornerBottomLeft = contentRect.center + (cornerBottomLeft - contentRect.center) * scale;

                cornerSizeTopLeft *= scale;
                cornerSizeTopRight *= scale;
                cornerSizeBottomRight *= scale;
                cornerSizeBottomLeft *= scale;

                widthTop *= scale.y;
                widthRight *= scale.x;
                widthBottom *= scale.y;
                widthLeft *= scale.x;
            }

            Clear();

            // Sides (indices)

            // Top
            ushort innerCornerStartIndex = 0;
            ushort outerCornerStartIndex = 1;
            createOuterSide(
                _vertices, _triangles,
                cornerTopLeft, cornerSizeTopLeft, cornerTopRight, cornerSizeTopRight, 0, -1,
                widthTop,
                offset, offsetEverything,
                innerColor, outerColor,
                vertexDistance,
                Config.UseRadialGradients,
                borderTopColor,
                innerCornerStartIndex,
                outerCornerStartIndex,
                out ushort outerTopRightTopInnerIndex,
                out ushort outerTopRightTopOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices,
                forceSubdivision,
                createSideA: true
                );
            
            // move to top-left-top-outer vertex to the left if the outer corner is hard.
            if(topLeftWasHardCorner && preserveHardCorners)
            {
                // Use outerCornerAIndex
                addToVertexPosition(1, x: -((splitWidth ? widthLeft : width) + overlapWidth), y: 0f);
            }

            // Top-Right
            float approxCornerArc = (cornerSizeTopRight.x + widthRight + cornerSizeTopRight.y + widthTop) * 0.8f;
            createOuterCorner(
                _vertices, _triangles,
                cornerTopRight, cornerSizeTopRight,
                widthRight,
                widthTop,
                offset, offsetEverything,
                innerColor, outerColor,
                preserveHardCorners && topRightWasHardCorner,
                outerTopRightTopInnerIndex, outerTopRightTopOuterIndex,
                Mathf.Max(1, Mathf.CeilToInt(approxCornerArc / vertexDistance)),
                borderRightColor,
                0,
                out ushort outerTopRightRightInnerIndex,
                out ushort outerTopRightRightOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices
                );

            // Right
            createOuterSide(
                _vertices, _triangles,
                cornerTopRight, cornerSizeTopRight, cornerBottomRight, cornerSizeBottomRight, 1, 0,
                widthRight,
                offset, offsetEverything,
                innerColor, outerColor,
                vertexDistance,
                Config.UseRadialGradients,
                borderRightColor,
                outerTopRightRightInnerIndex,
                outerTopRightRightOuterIndex,
                out ushort outerBottomRightRightInnerIndex,
                out ushort outerBottomRightRightOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices,
                forceSubdivision,
                createSideA: false
                );

            // Bottom-Right
            approxCornerArc = (cornerSizeBottomRight.x + widthRight + cornerSizeBottomRight.y + widthBottom) * 0.8f;
            createOuterCorner(
                _vertices, _triangles,
                cornerBottomRight, cornerSizeBottomRight,
                widthRight,
                widthBottom,
                offset, offsetEverything,
                innerColor, outerColor,
                preserveHardCorners && bottomRightWasHardCorner,
                outerBottomRightRightInnerIndex, outerBottomRightRightOuterIndex,
                Mathf.Max(1, Mathf.CeilToInt(approxCornerArc / vertexDistance)),
                borderBottomColor,
                1,
                out ushort outerBottomRightBottomInnerIndex,
                out ushort outerBottomRightBottomOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices
                );

            // Bottom
            createOuterSide(
                _vertices, _triangles,
                cornerBottomRight, cornerSizeBottomRight, cornerBottomLeft, cornerSizeBottomLeft, 0, 1,
                widthBottom,
                offset, offsetEverything,
                innerColor, outerColor,
                vertexDistance,
                Config.UseRadialGradients,
                borderBottomColor,
                outerBottomRightBottomInnerIndex,
                outerBottomRightBottomOuterIndex,
                out ushort outerBottomLeftBottomInnerIndex,
                out ushort outerBottomLeftBottomOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices,
                forceSubdivision,
                createSideA: false
                );

            // Bottom-Left
            approxCornerArc = (cornerSizeBottomLeft.x + widthLeft + cornerSizeBottomLeft.y + widthBottom) * 0.8f;
            createOuterCorner(
                _vertices, _triangles,
                cornerBottomLeft, cornerSizeBottomLeft,
                widthLeft,
                widthBottom,
                offset, offsetEverything,
                innerColor, outerColor,
                preserveHardCorners && bottomLeftWasHardCorner,
                outerBottomLeftBottomInnerIndex, outerBottomLeftBottomOuterIndex,
                Mathf.Max(1, Mathf.CeilToInt(approxCornerArc / vertexDistance)),
                borderLeftColor,
                2,
                out ushort outerBottomLeftLeftInnerIndex,
                out ushort outerBottomLeftLeftOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices
                );

            // Left
            createOuterSide(
                _vertices, _triangles,
                cornerBottomLeft, cornerSizeBottomLeft, cornerTopLeft, cornerSizeTopLeft, -1, 0,
                widthLeft,
                offset, offsetEverything,
                innerColor, outerColor,
                vertexDistance,
                Config.UseRadialGradients,
                borderLeftColor,
                outerBottomLeftLeftInnerIndex,
                outerBottomLeftLeftOuterIndex,
                out ushort outerTopLeftLeftInnerIndex,
                out ushort outerTopLeftLeftOuterIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices,
                forceSubdivision,
                createSideA: false
                );

            // Top-Left
            approxCornerArc = (cornerSizeTopLeft.x + widthLeft + cornerSizeTopLeft.y + widthTop) * 0.8f;
            createOuterCorner(
                _vertices, _triangles,
                cornerTopLeft, cornerSizeTopLeft,
                widthLeft,
                widthTop,
                offset, offsetEverything,
                innerColor, outerColor,
                preserveHardCorners && topLeftWasHardCorner,
                outerTopLeftLeftInnerIndex, outerTopLeftLeftOuterIndex,
                Mathf.Max(1, Mathf.CeilToInt(approxCornerArc / vertexDistance)),
                borderTopColor,
                3,
                out ushort innerEndIndex,
                out ushort outerEndIndex,
                _innerIndices, _outerIndices, _outerToInnerIndices
                );

            // Check if last vertex position matches first vertex. If not then add a triangle to fill the gap.
            // TODO: This happens sometimes if the overlap is bigger than the corner size. Needs investigation.
            if (Vector3.SqrMagnitude(_vertices[innerCornerStartIndex].position - _vertices[innerEndIndex].position) > 1f)
            {
                _triangles.Add(innerEndIndex);
                _triangles.Add(outerEndIndex);
                _triangles.Add(innerCornerStartIndex);
            }
            if (Vector3.SqrMagnitude(_vertices[outerCornerStartIndex].position - _vertices[outerEndIndex].position) > 1f)
            {
                _triangles.Add(innerEndIndex);
                _triangles.Add(outerEndIndex);
                _triangles.Add(outerCornerStartIndex);
            }

            // Fill center
            if (fillCenter)
            {
                var v = new Vertex();
                v.position = contentRect.center + (offsetEverything ? offset : Vector2.zero);
                v.tint = Config.UseRadialGradients ? Config.InnerColors.Evaluate(0f) : innerColor;
                _vertices.Add(v);
                ushort innerIndex = (ushort)(_vertices.Count - 1);

                // triangles from center to every inner vertex
                int iCount = _innerIndices.Count - 1; // <- are ordered CW
                for (int i = 0; i < iCount; i++)
                {
                    _triangles.Add(innerIndex);
                    _triangles.Add(_innerIndices[i]);
                    _triangles.Add(_innerIndices[i + 1]);
                }
            }

            if (Config.UseRadialGradients)
            {
                int indexCount = _innerIndices.Count;
                float stepSize = 1f / (indexCount - 1);
                for (int i = 0; i < _innerIndices.Count; i++)
                {
                    var v = _vertices[_innerIndices[i]];
                    v.tint = Config.InnerColors.Evaluate(stepSize * i);
                    _vertices[_innerIndices[i]] = v;
                }

                indexCount = _outerIndices.Count;
                stepSize = 1f / (indexCount - 1);
                for (int i = 0; i < indexCount; i++)
                {
                    var v = _vertices[_outerIndices[i]];
                    v.tint = Config.OuterColors.Evaluate(stepSize * i);
                    _vertices[_outerIndices[i]] = v;
                }
            }

            if (OnBeforeMeshWrite != null && target.panel.contextType == ContextType.Player)
            {
                copyVerticesToMeshCallbackTmp();
                OnBeforeMeshWrite?.Invoke(this, _tmpVerticesForMeshCallback, _triangles, _outerIndices, _innerIndices, _outerToInnerIndices);
                writeMeshData(mgc, _tmpVerticesForMeshCallback, _triangles);
            }
            else
            {
                writeMeshData(mgc, _vertices, _triangles);
            }

            
        }

        private void addToVertexPosition(ushort index, float x, float y)
        {
            var vertex = _vertices[index];
            var pos = vertex.position;
            pos.x += x;
            pos.y += y;
            vertex.position = pos;
            _vertices[index] = vertex;
        }

        private void writeMeshData(MeshGenerationContext mgc, List<Vertex> vertices, List<ushort> triangles)
        {
            int vCount = vertices.Count;
            int tCount = triangles.Count;
            
            MeshWriteData mwd = mgc.Allocate(vCount, tCount);

            for (int i = 0; i < vCount; i++)
            {
                mwd.SetNextVertex(vertices[i]);
            }

            for (int i = 0; i < tCount; i++)
            {
                mwd.SetNextIndex(triangles[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="cornerA"></param>
        /// <param name="cornerSizeA"></param>
        /// <param name="cornerB"></param>
        /// <param name="cornerSizeB"></param>
        /// <param name="directionX"></param>
        /// <param name="directionY"></param>
        /// <param name="glowWidth"></param>
        /// <param name="offset"></param>
        /// <param name="offsetEverything"></param>
        /// <param name="innerColor"></param>
        /// <param name="outerColor"></param>
        /// <param name="vertexDistance"></param>
        /// <param name="useRadialGradients"></param>
        /// <param name="borderColor"></param>
        /// <param name="innerCornerAIndex"></param>
        /// <param name="outerCornerAIndex"></param>
        /// <param name="innerCornerBIndex"></param>
        /// <param name="outerCornerBIndex"></param>
        /// <param name="innerIndices"></param>
        /// <param name="outerIndices"></param>
        /// <param name="outerToInnerIndices"></param>
        /// <param name="forceSubdivision"></param>
        /// <param name="createSideA">Should only be true on the very first quad. All others will use innerCornerAIndex and outerCornerAIndex from the preceeding curve</param>
        protected void createOuterSide(
            List<Vertex> vertices, List<ushort> triangles, 
            Vector2 cornerA, Vector2 cornerSizeA, Vector2 cornerB, Vector2 cornerSizeB, 
            int directionX, int directionY,
            float glowWidth,
            Vector2 offset, bool offsetEverything,
            Color innerColor, Color outerColor, float vertexDistance, bool useRadialGradients,
            Color borderColor,
            ushort innerCornerAIndex,
            ushort outerCornerAIndex,
            out ushort innerCornerBIndex,
            out ushort outerCornerBIndex,
            List<ushort> innerIndices,
            List<ushort> outerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices,
            bool forceSubdivision,
            bool createSideA)
        {
            Vertex v;

            // All mesh generation code adds 2 vertices at a time (inner and outer). But for
            // the start of the mesh we have to create two initial vertices to start from.
            // That's what "createSideA" does.
            if (createSideA)
            {
                // inner
                v = new Vertex();
                v.position = new Vector3(
                    cornerA.x + cornerSizeA.x * directionX + (offsetEverything ? offset.x : 0f),
                    cornerA.y + cornerSizeA.y * directionY + (offsetEverything ? offset.y : 0f),
                    Vertex.nearZ);
                v.tint = innerColor * borderColor;
                vertices.Add(v);
                ushort innerIndex = (ushort)(vertices.Count - 1);
                innerIndices.Add(innerIndex);

                // outer
                v = new Vertex();
                v.position = new Vector3(
                    cornerA.x + cornerSizeA.x * directionX + glowWidth * directionX + offset.x, 
                    cornerA.y + cornerSizeA.y * directionY + glowWidth * directionY + offset.y,
                    Vertex.nearZ);
                v.tint = outerColor * borderColor;
                vertices.Add(v);
                ushort outerIndex = (ushort)(vertices.Count - 1);
                outerIndices.Add(outerIndex);

                outerToInnerIndices.Add(outerIndex, innerIndex);
            }

            Vector3 innerStartPos = vertices[innerCornerAIndex].position;
            Vector3 outerStartPos = vertices[outerCornerAIndex].position;

            Vector3 innerEndPos = new Vector3(
                    cornerB.x + cornerSizeB.x * directionX + (offsetEverything ? offset.x : 0f), 
                    cornerB.y + cornerSizeB.y * directionY + (offsetEverything ? offset.y : 0f),
                    Vertex.nearZ);
            Vector3 outerEndPos = new Vector3(
                    cornerB.x + cornerSizeB.x * directionX + glowWidth * directionX + offset.x,
                    cornerB.y + cornerSizeB.y * directionY + glowWidth * directionY + offset.y,
                    Vertex.nearZ);

            int sideDivisions = 1;
            bool doSubdivision = useRadialGradients || forceSubdivision;
            if (doSubdivision)
            {
                float sideLength = Vector2.Distance(innerStartPos, innerEndPos);
                sideDivisions = doSubdivision ? Mathf.Max(1, Mathf.RoundToInt(sideLength / vertexDistance)) : 1;
            }

            outerCornerBIndex = 0;
            innerCornerBIndex = 0;

            ushort prevInnerIndex;
            ushort prevOuterIndex;

            ushort nextInnerIndex = innerCornerAIndex;
            ushort nextOuterIndex = outerCornerAIndex;

            for (int i = 1; i <= sideDivisions; i++)
            {
                // outer
                v = new Vertex();
                v.position = Vector3.Lerp(outerStartPos, outerEndPos, (float)i / sideDivisions);
                v.tint = outerColor * borderColor;
                vertices.Add(v);
                outerCornerBIndex = (ushort)(vertices.Count - 1);
                outerIndices.Add(outerCornerBIndex);

                // inner
                v = new Vertex();
                v.position = Vector3.Lerp(innerStartPos, innerEndPos, (float)i / sideDivisions);
                v.tint = innerColor * borderColor;
                vertices.Add(v);
                innerCornerBIndex = (ushort)(vertices.Count - 1);
                innerIndices.Add(innerCornerBIndex);

                outerToInnerIndices.Add(outerCornerBIndex, innerCornerBIndex);

                prevInnerIndex = nextInnerIndex;
                prevOuterIndex = nextOuterIndex;

                nextInnerIndex = (ushort)(vertices.Count - 1);
                nextOuterIndex = (ushort)(vertices.Count - 2);

                // triangles
                triangles.Add(prevInnerIndex);
                triangles.Add(prevOuterIndex);
                triangles.Add(nextOuterIndex);

                triangles.Add(nextOuterIndex);
                triangles.Add(nextInnerIndex);
                triangles.Add(prevInnerIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="cornerPos"></param>
        /// <param name="cornerSize"></param>
        /// <param name="glowWidthX"></param>
        /// <param name="glowWidthY"></param>
        /// <param name="offset"></param>
        /// <param name="offsetEverything"></param>
        /// <param name="innerColor"></param>
        /// <param name="outerColor"></param>
        /// <param name="preserveHardCorners"></param>
        /// <param name="innerStartIndex"></param>
        /// <param name="outerStartindex"></param>
        /// <param name="verticesPerCorner"></param>
        /// <param name="borderColor"></param>
        /// <param name="quadrant">In clock-wise order starting with 0 = top-right, 1 = bottom-right, ...</param>
        /// <param name="innerEndIndex"></param>
        /// <param name="outerEndIndex"></param>
        /// <param name="innerIndices"></param>
        /// <param name="outerIndices"></param>
        /// <param name="outerToInnerIndices"></param>
        private void createOuterCorner(
            List<Vertex> vertices, List<ushort> triangles,
            Vector2 cornerPos, Vector2 cornerSize,
            float glowWidthX, float glowWidthY,
            Vector2 offset, bool offsetEverything,
            Color innerColor, Color outerColor,
            bool preserveHardCorners,
            ushort innerStartIndex, ushort outerStartindex,
            int verticesPerCorner,
            Color borderColor,
            int quadrant,
            out ushort innerEndIndex,
            out ushort outerEndIndex,
            List<ushort> innerIndices,
            List<ushort> outerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices)
        {
            bool roundCorner = cornerSize.x > 0 && cornerSize.y > 0;

            float quadrantAngle = Mathf.PI * 0.5f * (quadrant - 1);
            float stepSizeInQuadrant = 1f / (verticesPerCorner + 1) * Mathf.PI * 0.5f;

            ushort prevInnerIndex;
            ushort prevOuterIndex;

            ushort nextInnerIndex = innerStartIndex;
            ushort nextOuterIndex = outerStartindex;

            bool useHardCorner = preserveHardCorners && !roundCorner;
            if (useHardCorner)
                verticesPerCorner = 0; // results in just one vertex

            for (int c = 1; c < verticesPerCorner + 2; c++)
            {
                float x = Mathf.Cos(quadrantAngle + stepSizeInQuadrant * c);
                float y = Mathf.Sin(quadrantAngle + stepSizeInQuadrant * c);

                // Only round corners need new inner vertices.
                if (roundCorner)
                {
                    var innerPos = new Vector3(
                        cornerPos.x + x * cornerSize.x + (offsetEverything ? offset.x : 0f),
                        cornerPos.y + y * cornerSize.y + (offsetEverything ? offset.y : 0f),
                        Vertex.nearZ);

                    var vInner = new Vertex();
                    vInner.position = innerPos;
                    vInner.tint = innerColor * borderColor;

                    vertices.Add(vInner);
                }

                Vector3 outerPos;
                if (useHardCorner)
                {
                    outerPos = new Vector3(
                         cornerPos.x + Mathf.Sign(x) * (cornerSize.x + glowWidthX) + offset.x,
                         cornerPos.y + Mathf.Sign(y) * (cornerSize.y + glowWidthY) + offset.y,
                         Vertex.nearZ);
                }
                else
                {
                    outerPos = new Vector3(
                          cornerPos.x + x * (cornerSize.x + glowWidthX) + offset.x,
                          cornerPos.y + y * (cornerSize.y + glowWidthY) + offset.y,
                          Vertex.nearZ);
                }

                var vOuter = new Vertex();
                vOuter.position = outerPos;
                vOuter.tint = outerColor * borderColor;

                vertices.Add(vOuter);

                prevInnerIndex = nextInnerIndex;
                prevOuterIndex = nextOuterIndex;

                nextInnerIndex = roundCorner ? (ushort)(vertices.Count - 2) : innerStartIndex;
                nextOuterIndex = (ushort)(vertices.Count - 1);

                if (roundCorner)
                {
                    innerIndices.Add(nextInnerIndex);
                }
                outerIndices.Add(nextOuterIndex);

                outerToInnerIndices.Add(nextOuterIndex, nextInnerIndex);

                // triangles
                triangles.Add(prevInnerIndex);
                triangles.Add(prevOuterIndex);
                triangles.Add(nextOuterIndex);

                if (roundCorner)
                {
                    triangles.Add(nextOuterIndex);
                    triangles.Add(nextInnerIndex);
                    triangles.Add(prevInnerIndex);
                }
            }

            innerEndIndex = nextInnerIndex;
            outerEndIndex = nextOuterIndex;
        }

        /// <summary>
        /// The new distance between inner and outer is the current distance multiplied by the factore.
        /// A factor of 0 means no change.<br />
        /// Below 0 means the distance between inner and outer will shrink.
        /// Above 0 mean the distance will grow.
        /// </summary>
        public static Vector3 DisplaceVertexOutwardsNormalized(
            List<Vertex> vertices,
            Dictionary<ushort, ushort> outerToInner,
            ushort outerVertexIndex,
            float displacementFactor = 1f)
        {
            var outerVertex = vertices[outerVertexIndex];
            var innerVertex = vertices[outerToInner[outerVertexIndex]];

            var vector = outerVertex.position - innerVertex.position;
            vector *= displacementFactor;

            outerVertex.position += vector;
            vertices[outerVertexIndex] = outerVertex;

            return vector;
        }

        public static void DisplaceVertex(List<Vertex> vertices, ushort vertexIndex, Vector3 vector)
        {
            var vertex = vertices[vertexIndex];
            vertex.position += vector;
            vertices[vertexIndex] = vertex;
        }

        public static void DisplaceVertex(List<Vertex> vertices, ushort vertexIndex, float x, float y)
        {
            var vertex = vertices[vertexIndex];

            vertex.position.x += x;
            vertex.position.y += y;

            vertices[vertexIndex] = vertex;
        }

        public static void SetVertexColor(List<Vertex> vertices, ushort vertexIndex, Color color)
        {
            var vertex = vertices[vertexIndex];
            vertex.tint = color;
            vertices[vertexIndex] = vertex;
        }

        public static void SetVertexPosition(List<Vertex> vertices, ushort vertexIndex, Vector3 position)
        {
            var vertex = vertices[vertexIndex];
            vertex.position = position;
            vertices[vertexIndex] = vertex;
        }
    }
}