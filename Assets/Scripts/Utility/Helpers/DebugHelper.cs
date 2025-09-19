using System.Collections.Generic;
using UnityEngine;

namespace VV.Utility
{
    public enum LogColorType
    {
        Normal,
        Important,
        Critical,
        Debugging,
        Validated
    }

    public static class DebugHelper
    {
        private static readonly Dictionary<LogColorType, string> ColorMappings = new()
        {
            { LogColorType.Normal, "white" },
            { LogColorType.Important, "blue" },
            { LogColorType.Critical, "red" },
            { LogColorType.Debugging, "yellow" },
            { LogColorType.Validated, "green" }
        };

        /// <summary> 
        /// Display a given message to the console, applying the given color type to it.
        /// </summary>
        /// <param name="message">The string of the message to display.</param> 
        /// <param name="logColorType">The LogColorType of the color to apply to the message.</param> 
        public static void Log(string message, LogColorType logColorType = LogColorType.Normal)
        {
            if (ColorMappings.TryGetValue(logColorType, out string color))
            {
                Debug.Log(ColorLog(message, color) + "\n" + GetStackTrace());
            }
            else
            {
                Debug.Log(message + "\n" + GetStackTrace());
            }
        }

        /// <summary> 
        /// Returns a colored message string for console output.
        /// </summary>
        /// <param name="message">The message text.</param> 
        /// <param name="color">The color name string.</param> 
        private static string ColorLog(string message, string color)
        {
            return $"<color={color}>{message}</color>";
        }

        /// <summary>
        /// Get stack trace of the calling method, excluding DebugHelper itself.
        /// </summary>
        private static string GetStackTrace()
        {
            // Get stack trace and filter out DebugHelper to avoid confusing Unity's console.
            string stackTrace = StackTraceUtility.ExtractStackTrace();
            return stackTrace.Replace(nameof(DebugHelper) + ".", "");
        }
    }
}
