using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public partial class ScrollViewPro
    {
        protected float? _childrenBoundsWidth;
        protected float? _childrenBoundsHeight;

        protected void clearInfiniteChildInfoCache()
        {
            _childrenBoundsWidth = null;
            _childrenBoundsHeight = null;
        }

        protected void refreshChildInfosInInfiniteScollViewIfNeeded()
        {
            if (!_childrenBoundsHeight.HasValue || float.IsNaN(_childrenBoundsHeight.Value))
            {
                if (!_childrenBoundsHeight.HasValue)
                    _childrenBoundsHeight = calcChildrenBoundsHeight();

                if (!_childrenBoundsWidth.HasValue)
                    _childrenBoundsWidth = calcChildrenBoundsWidth();

                // We set the values of any absolutely positioned elements. We need this to
                // ensure style.top has a value that takes the absolute top and left into account.
                // Basically this copies any inline left/top position to style.left/top.
                var children = contentContainer.Children();
                foreach (var child in children)
                {
                    if (child.resolvedStyle.position == Position.Absolute)
                    {
                        float newTop = child.resolvedStyle.top - child.resolvedStyle.marginTop - child.style.top.value.value;
                        child.style.top = child.resolvedStyle.top - child.resolvedStyle.marginTop - child.style.top.value.value;
                        child.style.left = child.resolvedStyle.left - child.resolvedStyle.marginLeft - child.style.left.value.value;
                    }
                }
            }
        }

        protected void registerEventsForInfinity()
        {
            horizontalScroller.RegisterCallback<ChangeEvent<float>>(onHorizontalValueChangedInfinity);
            verticalScroller.RegisterCallback<ChangeEvent<float>>(onVerticalValueChangedInfinity);
        }

        protected void onHorizontalValueChangedInfinity(ChangeEvent<float> e)
        {
            if (!infinite)
                return;

            if (mode != ScrollViewMode.Horizontal && mode != ScrollViewMode.VerticalAndHorizontal)
                return;

            float delta = e.newValue - e.previousValue;
            // updateChildPositionsInInfinityX(-delta); // invert delta because scrollOffset is inverted too.
        }

        protected void onVerticalValueChangedInfinity(ChangeEvent<float> e)
        {
            if (!infinite)
                return;

            if (mode != ScrollViewMode.Vertical && mode != ScrollViewMode.VerticalAndHorizontal)
                return;

            float delta = e.newValue - e.previousValue;
            updateChildPositionsInInfinityY(-delta); // invert delta because scrollOffset is inverted too.
        }

        protected float calcChildrenBoundsWidth()
        {
            // Get child min max.
            float childMin = float.MaxValue;
            float childMax = float.MinValue;
            var children = contentContainer.Children();
            foreach (var child in children)
            {
                if (child.style.display == DisplayStyle.None)
                    continue;

                var childBB = child.worldBound;
                childMin = Mathf.Min(childMin, childBB.xMin - child.resolvedStyle.marginLeft);
                childMax = Mathf.Max(childMax, childBB.xMax + child.resolvedStyle.marginRight);
            }
            return childMax - childMin;
        }

        protected float calcChildrenBoundsHeight()
        { 
            // Get child min max.
            float childMin = float.MaxValue;
            float childMax = float.MinValue;
            var children = contentContainer.Children();
            foreach (var child in children)
            {
                if (child.style.display == DisplayStyle.None)
                    continue;

                var childBB = child.worldBound;
                childMin = Mathf.Min(childMin, childBB.yMin - child.resolvedStyle.marginTop);
                childMax = Mathf.Max(childMax, childBB.yMax + child.resolvedStyle.marginBottom);
            }
            return childMax - childMin;
        }

        // I HATE hidden state but it's the best I could come up with. TODO: investigate.
        protected Dictionary<VisualElement, float> _tmpInfinityPositionChangesX = new Dictionary<VisualElement, float>(10);
        protected int _lastUpdateChildPositionsInInfinityXFrame = 0;

        protected bool updateChildPositionsInInfinityX(float scrollDirection)
        {
            // Abort there is not change.
            if (Mathf.Approximately(scrollDirection, 0f))
                return false;

            // Execute this only ONCE per frame or else the contents of _tmpInfinityPositionChanges
            // would be invalid.
            if (_lastUpdateChildPositionsInInfinityXFrame >= Time.frameCount)
                return false;

            _lastUpdateChildPositionsInInfinityXFrame = Time.frameCount;


            if (!_childrenBoundsWidth.HasValue || float.IsNaN(_childrenBoundsWidth.Value))
                _childrenBoundsWidth = calcChildrenBoundsWidth();

            bool didTeleportElements = false;
            var viewBB = contentViewport.worldBound;
            // Use for infinity fix (see below).
            _tmpInfinityPositionChangesX.Clear();

            // Detect all children that are out of bounds and teleport them to the other side.
            // Initial state (move direction of content is ->) _ |_| _ _  -> new state (async, 2 teleported from right to left): _ _ _ |_|
            var children = contentContainer.Children();
            foreach (var child in children)
            {
                if (child.style.display == DisplayStyle.None)
                    continue;

                var childBB = child.worldBound;
                if (scrollDirection < -0.001f)
                {
                    // Is outside
                    if (childBB.xMax < viewBB.xMin)
                    {
                        float pos = child.style.left.value.value;
                        child.style.left = pos + _childrenBoundsWidth.Value;
                        didTeleportElements = true;

                        //Debug.Log($"Teleporting {child.name} to right");

                        // Memorize change for infinity fix (see below)
                        _tmpInfinityPositionChangesX.Add(child, _childrenBoundsWidth.Value);
                    }
                }
                else if (scrollDirection > 0.001f)
                {
                    // Is outside
                    if (childBB.xMin > viewBB.xMax)
                    {
                        float pos = child.style.left.value.value;
                        child.style.left = pos - _childrenBoundsWidth.Value;
                        didTeleportElements = true;

                        //Debug.Log($"Teleporting {child.name} to left");

                        // Memorize change for infinity fix (see below)
                        _tmpInfinityPositionChangesX.Add(child, -_childrenBoundsWidth.Value);
                    }
                }
            }

            // Fix for selecting a child in the inverse direction of the update (primarily a problem in focusSnap).
            // Reason: If all children are on one side then focusing in the inverse direction
            // will not work since there is no child there to focus.
            // To solve this we take one child (if possible) and put it back on the other side.
            // The tricky part, which also trips up all the other code, is that changing the 
            // style.left.value does not immediately update the worldBounds. Instead it has to wait
            // for the layout engine to run. In that sense it is "async" and thus all other calculations
            // that are based on the worldBounds will be incorrect for the rest of the frame (scrollTo, sorting by position, ...).
            // To work around that we store the position change in a dictionary here and hand it over to any method that needs it.
            //
            // Initial state (async): _ _ _ |_|  -> put back state (async): _ _ |_| _
            // 1) Count children that could the put back.
            // 2) If more than one then put one back, if less then do nothing.
            {
                children = contentContainer.Children();
                VisualElement childToPutBackLeft = null;
                VisualElement childToPutBackRight = null;
                float distanceLeft = 0f;
                float distanceRight = 0f;
                float maxDistanceLeft = 0f;
                float maxDistanceRight = 0f;

                int outLeft = 0;
                int outRight = 0;

                foreach (var child in children)
                {
                    if (child.style.display == DisplayStyle.None)
                        continue;

                    var childBB = child.worldBound;
                    float childxMin = childBB.xMin + getFromDict(_tmpInfinityPositionChangesX, child, 0f);
                    float childxMax = childBB.xMax + getFromDict(_tmpInfinityPositionChangesX, child, 0f);

                    if (childxMin > viewBB.xMax)
                    {
                        outRight++;
                        distanceRight = childxMin - viewBB.xMax;
                        if (distanceRight > maxDistanceRight)
                        {
                            maxDistanceRight = distanceRight;
                            childToPutBackRight = child;
                        }
                    }
                    else if (childxMax < viewBB.xMin)
                    {
                        outLeft++;
                        distanceLeft = viewBB.xMin - childxMax;
                        if (distanceLeft > maxDistanceLeft)
                        {
                            maxDistanceLeft = distanceLeft;
                            childToPutBackLeft = child;
                        }
                    }
                }

                //Debug.Log("Out left: " + outLeft + " out right: " + outRight);

                if (outLeft > 0 && outLeft > outRight)
                {
                    // put one child from left to right
                    float pos = childToPutBackLeft.style.left.value.value;
                    if (_tmpInfinityPositionChangesX.ContainsKey(childToPutBackLeft))
                    {
                        // invert style change
                        childToPutBackLeft.style.left = pos - _tmpInfinityPositionChangesX[childToPutBackLeft];
                    }
                    else
                    {
                        // move to other side
                        childToPutBackLeft.style.left = pos + _childrenBoundsWidth.Value;
                    }

                    //Debug.Log($"Moving {childToPutBackLeft.name} to the right");
                }
                else if (outRight > 0 && outRight > outLeft)
                {
                    // put one child from right to left
                    float pos = childToPutBackRight.style.left.value.value;
                    if (_tmpInfinityPositionChangesX.ContainsKey(childToPutBackRight))
                    {
                        // invert style change
                        childToPutBackRight.style.left = pos - _tmpInfinityPositionChangesX[childToPutBackRight];
                    }
                    else
                    {
                        // move to other side
                        childToPutBackRight.style.left = pos - _childrenBoundsWidth.Value;
                    }

                    //Debug.Log($"Moving {childToPutBackRight.name} to the left");
                }

                sortAllFocusables(_tmpInfinityPositionChangesX, _tmpInfinityPositionChangesY);

                _tmpInfinityPositionChangesX.Clear();
            }

            return didTeleportElements;
        }

        protected Dictionary<VisualElement, float> _tmpInfinityPositionChangesY = new Dictionary<VisualElement, float>(10);
        protected int _lastUpdateChildPositionsInInfinityYFrame = 0;

        protected bool updateChildPositionsInInfinityY(float scrollDirection)
        {

            if (!_childrenBoundsHeight.HasValue || float.IsNaN(_childrenBoundsHeight.Value))
                _childrenBoundsHeight = calcChildrenBoundsHeight();

            bool didTeleportElements = false;
            var viewBB = contentViewport.worldBound;
            // Use for infinity fix (see below).
            _tmpInfinityPositionChangesY.Clear();

            // Detect all children that are out of bounds and teleport them to the other side.
            var children = contentContainer.Children();
            foreach (var child in children)
            {
                if (child.style.display == DisplayStyle.None)
                    continue;

                var childBB = child.worldBound;
                if (scrollDirection < -0.001f)
                {
                    // Is outside
                    if (childBB.yMax < viewBB.yMin)
                    {
                        float pos = child.style.top.value.value;
                        child.style.top = pos + _childrenBoundsHeight.Value;
                        // Memorize change for infinity fix (see below)
                        _tmpInfinityPositionChangesY.Add(child, _childrenBoundsHeight.Value);
                        didTeleportElements = true;
                    }
                }
                else if (scrollDirection > 0.001f)
                {
                    // Is outside
                    if (childBB.yMin > viewBB.yMax)
                    {
                        float pos = child.style.top.value.value;
                        child.style.top = pos - _childrenBoundsHeight.Value;
                        // Memorize change for infinity fix (see below)
                        _tmpInfinityPositionChangesY.Add(child, -_childrenBoundsHeight.Value);
                        didTeleportElements = true;
                    }
                }
            }

            // Fix for selecting a child in the inverse direction of the update (primarily a problem in focusSnap).
            // See explanation in Y (above)
            {
                children = contentContainer.Children();
                VisualElement childToPutBackTop = null;
                VisualElement childToPutBackBottom = null;
                float distanceTop = 0f;
                float distanceBottom = 0f;
                float maxDistanceTop = 0f;
                float maxDistanceBottom = 0f;

                int outTop = 0;
                int outBottom = 0;

                foreach (var child in children)
                {
                    if (child.style.display == DisplayStyle.None)
                        continue;

                    var childBB = child.worldBound;
                    float childyMin = childBB.yMin + getFromDict(_tmpInfinityPositionChangesY, child, 0f);
                    float childyMax = childBB.yMax + getFromDict(_tmpInfinityPositionChangesY, child, 0f);

                    if (childyMin > viewBB.yMax)
                    {
                        outBottom++;
                        distanceBottom = childyMin - viewBB.yMax;
                        if (distanceBottom > maxDistanceBottom)
                        {
                            maxDistanceBottom = distanceBottom;
                            childToPutBackBottom = child;
                        }
                    }
                    else if (childyMax < viewBB.yMin)
                    {
                        outTop++;
                        distanceTop = viewBB.yMin - childyMax;
                        if (distanceTop > maxDistanceTop)
                        {
                            maxDistanceTop = distanceTop;
                            childToPutBackTop = child;
                        }
                    }
                }

                //Debug.Log("Out top: " + outTop + " out bottom: " + outBottom);

                if (outTop > 0 && outTop > outBottom)
                {
                    // put one child from top to bottom
                    float pos = childToPutBackTop.style.top.value.value;
                    if (_tmpInfinityPositionChangesY.ContainsKey(childToPutBackTop))
                    {
                        // invert style change
                        childToPutBackTop.style.top = pos - _tmpInfinityPositionChangesY[childToPutBackTop];
                    }
                    else
                    {
                        // move to other side
                        childToPutBackTop.style.top = pos + _childrenBoundsHeight.Value;
                    }

                    //Debug.Log($"Moving {childToPutBackTop.name} to the bottom");
                }
                else if (outBottom > 0 && outBottom > outTop)
                {
                    // put one child from bottom to top
                    float pos = childToPutBackBottom.style.top.value.value;
                    if (_tmpInfinityPositionChangesY.ContainsKey(childToPutBackBottom))
                    {
                        // invert style change
                        childToPutBackBottom.style.top = pos - _tmpInfinityPositionChangesY[childToPutBackBottom];
                    }
                    else
                    {
                        // move to other side
                        childToPutBackBottom.style.top = pos - _childrenBoundsHeight.Value;
                    }

                    //Debug.Log($"Moving {childToPutBackBottom.name} to the top");
                }

                sortAllFocusables(_tmpInfinityPositionChangesX, _tmpInfinityPositionChangesY);

                _tmpInfinityPositionChangesY.Clear();
            }

            return didTeleportElements;
        }
    }
}
