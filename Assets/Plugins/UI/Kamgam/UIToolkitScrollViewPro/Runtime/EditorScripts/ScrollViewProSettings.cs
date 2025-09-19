#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Kamgam.UIToolkitScrollViewPro
{
    /// <summary>
    /// ScrollViewPro is a full rewrite of the Unity ScrollView.
    /// It aims to be API compatible and thus uses some of ScrollViews enums and types.
    /// </summary>
    public class ScrollViewProSettings
    {
        public const string Version = "1.3.2";

#if UNITY_EDITOR
        [MenuItem("Tools/UI Toolkit ScrollViewPro/Manual", priority = 210)]
        public static void OpenManual()
        {
            Application.OpenURL("https://kamgam.com/unity/UIToolkitScrollViewProManual.pdf");
        }

        [MenuItem("Tools/UI Toolkit ScrollViewPro/Please leave a review :-)", priority = 310)]
        public static void LeaveReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/262334?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit ScrollViewPro/More Assets by KAMGAM", priority = 510)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/37829?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit ScrollViewPro/Version: " + Version, priority = 510)]
        public static void LogVersion()
        {
            Debug.Log("UI Toolkit ScrollViewPro Version: " + Version);
        }
#endif
    }
}
