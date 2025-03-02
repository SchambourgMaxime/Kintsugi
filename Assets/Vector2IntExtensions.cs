using System.Runtime.CompilerServices;
using UnityEngine;

namespace ForIndustrie.Utility
{
    public static class Vector2IntExtensions
    {
        private const float Epsilon = 0.0001f;

        // Sets any values of the Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int With(this Vector2Int vector, int? x = null, int? y = null)
        {
            return new Vector2Int(x ?? vector.x, y ?? vector.y);
        }

        // Adds to any values of the Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Add(this Vector2Int vector, int? x = null, int? y = null)
        {
            return new Vector2Int(vector.x + (x ?? 0), vector.y + (y ?? 0));
        }

        // checks magnitude difference between 2 Vectors
        // returns : 1 if v1.magnitude > v1.magnitude
        // 0 if v1.magnitude == v2.magnitude
        // -1 if v1.magnitude < v2.magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(this Vector2Int v1, Vector2Int v2)
        {
            if (v1.sqrMagnitude > v2.sqrMagnitude)
                return 1;
            else if (Mathf.Approximately(v1.sqrMagnitude, v2.sqrMagnitude))
                return 0;
            else if (v1.sqrMagnitude < v2.sqrMagnitude)
                return -1;

            return 0;
        }

        // checks magnitude is less than parameter vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsShorterThan(this Vector2Int v1, Vector2Int v2)
        {
            return v1.sqrMagnitude < v2.sqrMagnitude;
        }

        // checks magnitude between 2 Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSizeApproximately(this Vector2Int v1, Vector2Int v2)
        {
            return Mathf.Approximately(v1.sqrMagnitude, v2.sqrMagnitude);
        }

        // checks magnitude is greater than parameter vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLongerThan(this Vector2Int v1, Vector2Int v2)
        {
            return v1.sqrMagnitude > v2.sqrMagnitude;
        }

        // checks magnitude is shorter than given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsShorterThan(this Vector2Int v2, float magnitude)
        {
            return v2.sqrMagnitude < (magnitude * magnitude);
        }

        // checks magnitude between 2 Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector2Int v1)
        {
            return v1.sqrMagnitude < (Epsilon * Epsilon);
        }

        // checks magnitude between 2 Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSizeApproximately(this Vector2Int v1, float magnitude)
        {
            return Mathf.Approximately(v1.sqrMagnitude, (magnitude * magnitude));
        }

        // checks magnitude is greater than parameter vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLongerThan(this Vector2Int v1, float magnitude)
        {
            return v1.sqrMagnitude > (magnitude * magnitude);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector2Int v2)
        {
            return new Vector2(v2.x, v2.y);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2Int v2)
        {
            return new Vector3(v2.x, 0f, v2.y);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this Vector2Int v2)
        {
            return new Vector3Int(v2.x, 0, v2.y);
        }
    }
}