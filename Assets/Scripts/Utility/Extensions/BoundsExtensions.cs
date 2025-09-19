using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VV.Utility
{
    public static class BoundsExtensions
    {
        private const float Epsilon = 0.0001f;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector3> GetVertices(this Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            return new List<Vector3>(8)
            {
                new(min.x, max.y, max.z),
                new(min.x, max.y, min.z),
                new(min.x, min.y, max.z),
                new(min.x, min.y, min.z),
                new(max.x, max.y, max.z),
                new(max.x, max.y, min.z),
                new(max.x, min.y, max.z),
                new(max.x, min.y, min.z)
            };;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEachVertices(this Bounds bounds, Action<Vector3> action)
        {
            foreach (Vector3 vertex in GetVertices(bounds)) action?.Invoke(vertex);
        }
    }
}