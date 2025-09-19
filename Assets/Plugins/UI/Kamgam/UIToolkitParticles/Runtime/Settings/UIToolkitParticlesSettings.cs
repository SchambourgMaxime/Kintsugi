#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Kamgam.UIToolkitParticles
{
    // Create a new type of Settings Asset.
    public class UIToolkitParticlesSettings : ScriptableObject
    {
        public enum ShaderVariant { Performance, Gaussian };

        public const string Version = "1.2.3"; 
        public const string SettingsFilePath = "Assets/UIToolkitParticlesSettings.asset";

        [SerializeField, Tooltip(_logLevelTooltip)]
        public Logger.LogLevel LogLevel;
        public const string _logLevelTooltip = "Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'";

        [SerializeField, Tooltip(_showNoDocumentWarningTooltip)]
        public bool ShowNoDocumentWarning;
        public const string _showNoDocumentWarningTooltip = "Should the 'No UIDocument found ..' warning be shown or not?";

        [RuntimeInitializeOnLoadMethod]
        static void bindLoggerLevelToSetting()
        {
            // Notice: This does not yet create a setting instance! 
            Logger.OnGetLogLevel = () => GetOrCreateSettings().LogLevel;
        }

        [InitializeOnLoadMethod]
        static void autoCreateSettings()
        {
            GetOrCreateSettings();
        }

        static UIToolkitParticlesSettings cachedSettings;

        public static UIToolkitParticlesSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                string typeName = typeof(UIToolkitParticlesSettings).Name;

                cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitParticlesSettings>(SettingsFilePath);

                // Still not found? Then search for it.
                if (cachedSettings == null)
                {
                    string[] results = AssetDatabase.FindAssets("t:" + typeName);
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitParticlesSettings>(path);
                    }
                }

                if (cachedSettings != null)
                {
                    SessionState.EraseBool(typeName + "WaitingForReload");
                }

                // Still not found? Then create settings.
                if (cachedSettings == null)
                {
                    CompilationPipeline.compilationStarted -= onCompilationStarted;
                    CompilationPipeline.compilationStarted += onCompilationStarted;

                    // Are the settings waiting for a recompile to finish? If yes then return null;
                    // This is important if an external script tries to access the settings before they
                    // are deserialized after a re-compile.
                    bool isWaitingForReloadAfterCompilation = SessionState.GetBool(typeName + "WaitingForReload", false);
                    if (isWaitingForReloadAfterCompilation)
                    {
                        Debug.LogWarning(typeName + " is waiting for assembly reload.");
                        return null;
                    }

                    cachedSettings = ScriptableObject.CreateInstance<UIToolkitParticlesSettings>();
                    cachedSettings.LogLevel = Logger.LogLevel.Warning;
                    cachedSettings.ShowNoDocumentWarning = true;

                    AssetDatabase.CreateAsset(cachedSettings, SettingsFilePath);
                    AssetDatabase.SaveAssets();

                    Logger.OnGetLogLevel = () => cachedSettings.LogLevel;

                    // Import packages and then show welcome screen.
                    PackageImporter.ImportDelayed(onSettingsCreated);
                }
            }

            return cachedSettings;
        }

        private static void onCompilationStarted(object obj)
        {
            string typeName = typeof(UIToolkitParticlesSettings).Name;
            SessionState.SetBool(typeName + "WaitingForReload", true);
        }

        // We use this callback instead of CompilationPipeline.compilationFinished because
        // compilationFinished runs before the assemply has been reloaded but DidReloadScripts
        // runs after. And only after we can access the Settings asset.
        [UnityEditor.Callbacks.DidReloadScripts(999000)]
        public static void DidReloadScripts()
        {
            string typeName = typeof(UIToolkitParticlesSettings).Name;
            SessionState.EraseBool(typeName + "WaitingForReload");
        }

        static void onSettingsCreated()
        {
            copyGizmos();

            bool openManual = EditorUtility.DisplayDialog(
                    "UI Toolkit Particles",
                    "Thank you for choosing UI Toolkit Particles.\n\n" +
                    "You'll find the tool under Tools > UI Toolkit Particles > Open\n\n" +
                    "Please start by reading the manual.\n\n" +
                    "It would be great if you could find the time to leave a review.",
                    "Open manual", "Cancel"
                    );

            if (openManual)
            {
                OpenManual();
            }
        }

        [MenuItem("Tools/UI Toolkit Particles/Debug/CopyGizmos", priority = 801)]
        static void copyGizmos()
        {
            var sourceFile = Application.dataPath + "/Kamgam/UIToolkitParticles/Editor/ParticleSystemForImage icon.png";
            var targetDir = Application.dataPath + "/Gizmos/Kamgam/UIToolkitParticles/";
            var targetFile = targetDir + "ParticleSystemForImage icon.png";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetDir));
            System.IO.File.Copy(sourceFile, targetFile, true);

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/UI Toolkit Particles/Manual", priority = 101)]
        public static void OpenManual()
        {
            Application.OpenURL("https://kamgam.com/unity/UIToolkitParticlesManual.pdf");
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [MenuItem("Tools/UI Toolkit Particles/Settings", priority = 101)]
        public static void OpenSettings()
        {
            var settings = UIToolkitParticlesSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "UI Toolkit Particles Settings could not be found or created.", "Ok");
            }
        }

        [MenuItem("Tools/UI Toolkit Particles/Please leave a review :-)", priority = 410)]
        public static void LeaveReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/252260?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit Particles/More Asset by KAMGAM", priority = 420)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/37829?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit Particles/Version: " + Version, priority = 510)]
        public static void LogVersion()
        {
            Debug.Log("UI Toolkit Particles Version: " + Version);
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
#if UNITY_2021_2_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(this);
#else
            AssetDatabase.SaveAssets();
#endif
        }

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(UIToolkitParticlesSettings))]
    public class UIToolkitParticlesSettingsEditor : Editor
    {
        public UIToolkitParticlesSettings settings;

        public void OnEnable()
        {
            settings = target as UIToolkitParticlesSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + UIToolkitParticlesSettings.Version);
            base.OnInspectorGUI();
        }
    }
#endif

    static class UIToolkitParticlesSettingsProvider
    {
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateUIToolkitParticlesSettingsProvider()
        {
            var provider = new UnityEditor.SettingsProvider("Project/UI Toolkit Particles", SettingsScope.Project)
            {
                label = "UI Toolkit Particles",
                guiHandler = (searchContext) =>
                {
                    var settings = UIToolkitParticlesSettings.GetSerializedSettings();

                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField("Version: " + UIToolkitParticlesSettings.Version);
                    if (drawButton(" Open Manual ", icon: "_Help"))
                    {
                        UIToolkitParticlesSettings.OpenManual();
                    }

                    var settingsObj = settings.targetObject as UIToolkitParticlesSettings;

                    drawField("LogLevel", "Log Level", UIToolkitParticlesSettings._logLevelTooltip, settings, style);
                    drawField("ShowNoDocumentWarning", "Show no UI Document warning", UIToolkitParticlesSettings._showNoDocumentWarningTooltip, settings, style);

                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting.
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "shader", "triplanar", "rendering" })
            };

            return provider;
        }

        static void drawField(string propertyName, string label, string tooltip, SerializedObject settings, GUIStyle style)
        {
            EditorGUILayout.PropertyField(settings.FindProperty(propertyName), new GUIContent(label));
            if (!string.IsNullOrEmpty(tooltip))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(tooltip, style);
                GUILayout.EndVertical();
            }
            GUILayout.Space(10);
        }

        static bool drawButton(string text, string tooltip = null, string icon = null, params GUILayoutOption[] options)
        {
            GUIContent content;

            // icon
            if (!string.IsNullOrEmpty(icon))
                content = EditorGUIUtility.IconContent(icon);
            else
                content = new GUIContent();

            // text
            content.text = text;

            // tooltip
            if (!string.IsNullOrEmpty(tooltip))
                content.tooltip = tooltip;

            return GUILayout.Button(content, options);
        }
    }
}
#endif