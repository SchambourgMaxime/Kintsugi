using System.Runtime.CompilerServices;
using UnityEngine;

namespace VV.Utility
{
    public static class CameraExtensions
    {
        private const float Epsilon = 0.0001f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScreenPointToPlaneIntersection(this Camera cam, Vector3 position, Plane plane)
        {
            return cam.ScreenPointToRay(position).GetIntersection(plane);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScreenPointToPlaneIntersection(this Camera cam, Vector3 position, Vector3 planeNormal, Vector3 planeOrigin)
        {
            return cam.ScreenPointToRay(position).GetIntersection(new Plane(planeNormal, planeOrigin));
        }
    }
}