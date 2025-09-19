using System.Runtime.CompilerServices;
using UnityEngine;

namespace VV.Utility
{
    public static class RayExtensions
    {
        private const float Epsilon = 0.0001f;

        // Sets any values of the Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetIntersection(this Ray ray, Plane plane)
        {
            plane.Raycast(ray, out float rayPos);
            return ray.GetPoint(rayPos);
        }
        
        // Sets any values of the Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetIntersection(this Ray ray, Vector3 planeNormal, Vector3 planeOrigin)
        {
            return ray.GetIntersection(new Plane(planeNormal, planeOrigin));
        }
    }
}