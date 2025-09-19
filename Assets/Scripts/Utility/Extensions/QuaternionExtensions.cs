using System.Runtime.CompilerServices;
using UnityEngine;

namespace VV.Utility
{
    public static class QuaternionExtensions
    {
        // Returns only the angle in degrees of the quaternion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this Quaternion quaternion)
        {
            quaternion.ToAngleAxis(out float angle, out Vector3 axis);
            return angle;
        }

        // Returns only the axis of the quaternion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetAxis(this Quaternion quaternion)
        {
            quaternion.ToAngleAxis(out float angle, out Vector3 axis);
            return axis;
        }

        // Changes the quaternion so that the angle is contained between min and max values
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ClampAngle(this Quaternion quaternion, float min, float max)
        {
            quaternion.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > min && angle < max)
                return quaternion;

            angle = Mathf.Clamp(angle, min, max);
            quaternion = Quaternion.AngleAxis(angle, axis);
            return quaternion;
        }
    }
}