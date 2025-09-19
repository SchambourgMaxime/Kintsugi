using UnityEngine;

namespace VV.Utility
{
    public static class TransformExtensions
    {
        private const float Epsilon = 0.0001f;

        public static void SetTransform(this Transform lhs, Transform rhs)
        {
            lhs.transform.position = rhs.position;
            lhs.transform.rotation = rhs.rotation;
            lhs.transform.localScale = rhs.localScale;
        }
    }
}