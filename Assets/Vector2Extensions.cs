using System.Runtime.CompilerServices;
using UnityEngine;

namespace ForIndustrie.Utility
{
    public static class Vector2Extensions
    {
        private const float Epsilon = 0.0001f;

        // Sets any values of the Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector.x, y ?? vector.y);
        }

        // Adds to any values of the Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Add(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(vector.x + (x ?? 0), vector.y + (y ?? 0));
        }

        #region Comparison

        #region CompareWithVector

        // checks magnitude difference between 2 Vectors
        // returns : 1 if v1.magnitude > v1.magnitude
        // 0 if v1.magnitude == v2.magnitude
        // -1 if v1.magnitude < v2.magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(this Vector2 v1, Vector2 v2)
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
        public static bool ShorterThan(this Vector2 v1, Vector2 v2)
        {
            return v1.sqrMagnitude < v2.sqrMagnitude;
        }

        // checks magnitude between 2 Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSizeApproximately(this Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.sqrMagnitude, v2.sqrMagnitude);
        }

        // checks magnitude is greater than parameter vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LongerThan(this Vector2 v1, Vector2 v2)
        {
            return v1.sqrMagnitude > v2.sqrMagnitude;
        }

        #endregion CompareWithVector

        #region CompareWithScalar

        // checks magnitude is shorter than given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShorterThan(this Vector2 v2, float magnitude)
        {
            return v2.sqrMagnitude < (magnitude * magnitude);
        }

        // checks magnitude between 2 Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector2 v1)
        {
            return v1.sqrMagnitude < (Epsilon * Epsilon);
        }

        // checks magnitude between 2 Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSizeApproximately(this Vector2 v1, float magnitude)
        {
            return Mathf.Approximately(v1.sqrMagnitude, (magnitude * magnitude));
        }

        // checks magnitude is greater than parameter vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LongerThan(this Vector2 v1, float magnitude)
        {
            return v1.sqrMagnitude > (magnitude * magnitude);
        }

        #endregion CompareWithScalar

        #endregion Comparison

        #region Scale

        // Scales vector using a simple scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(this Vector2 v1, float scale)
        {
            v1.Scale(new Vector2(scale, scale));
        }

        // Scales vector using a simple scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetScaled(this Vector2 v1, Vector2 scale)
        {
            Vector2 v = new Vector2(v1.x, v1.y);
            v.Scale(scale);
            return v;
        }

        // Scales vector using a simple scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetScaled(this Vector2 v1, float scale)
        {
            Vector2 v = new Vector2(v1.x, v1.y);
            v.Scale(new Vector2(scale, scale));
            return v;
        }

        #endregion Scale

        #region Size

        // Resizes this vector with given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resize(this Vector2 v1, float magnitude)
        {
            v1.Scale(magnitude / v1.magnitude);
        }

        // returns new vector with given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetWithSize(this Vector2 v1, float magnitude)
        {
            return (new Vector2(v1.x, v1.y)).GetScaled(magnitude / v1.magnitude);
        }

        // Reduces vector by amount of magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReduceBy(this ref Vector2 v1, float magnitude)
        {
            v1 -= (v1.normalized * magnitude);
        }

        // Get new vector with size reduced by given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetReducedBy(this Vector2 v1, float magnitude)
        {
            return v1 - (v1.normalized * magnitude);
        }

        // Reduces vector by amount of magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Augment(this ref Vector2 v1, float magnitude)
        {
            v1 += (v1.normalized * magnitude);
        }

        // Get new vector with size reduced by given magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetAugmentedBy(this Vector2 v1, float magnitude)
        {
            return v1 + (v1.normalized * magnitude);
        }

        #endregion Size

        // Converts a Vector2 to a Vector2 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToInt(this Vector2 v2)
        {
            return Vector2Int.FloorToInt(v2);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToInt(this Vector2 v2)
        {
            return Vector2Int.RoundToInt(v2);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CeilToInt(this Vector2 v2)
        {
            return Vector2Int.CeilToInt(v2);
        }

        // Converts a Vector2 to a Vector3 with a y value of 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }
    }
}