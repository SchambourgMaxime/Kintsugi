using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
    public enum SinusMode { NoChange, Absolute, ClampPositive, ClampNegative }

    public static class SinusUtils
    {
        public static float ApplySinusMode(float value, SinusMode mode)
        {
            switch (mode)
            {
                case SinusMode.Absolute:
                    return Mathf.Abs(value);

                case SinusMode.ClampPositive:
                    return Mathf.Max(0f, value);

                case SinusMode.ClampNegative:
                    return Mathf.Min(0f, value);

                case SinusMode.NoChange:
                default:
                    return value;
            }
        }
    }
}