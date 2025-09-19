#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Kamgam.UIToolkitBlurredBackground
{
    // Create a new type of Settings Asset.
    public class UIToolkitBlurredBackgroundSettings : ScriptableObject
    {
        public enum ShaderVariant { Performance, Gaussian };

        public const string Version = "1.2.0"; 
        public const string SettingsFilePath = "Assets/UIToolkitBlurredBackgroundSettings.asset";

        [SerializeField, Tooltip(_logLevelTooltip)]
        public Logger.LogLevel LogLevel;
        public const string _logLevelTooltip = "Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'";

        public const string _addShaderBeforeBuildTooltip = "Should the blur shader be added to the list of always included shaders before a build is started?\n\n" +
            "Disable only if you do not use any blurred images in your project but you still want to keep the asset around.";
        [Tooltip(_addShaderBeforeBuildTooltip)]
        public bool AddShaderBeforeBuild = true;

        [RuntimeInitializeOnLoadMethod]
        static void bindLoggerLevelToSetting()
        {
            // Notice: This does not yet create a setting instance!
            Logger.OnGetLogLevel = () => GetOrCreateSettings().LogLevel;
        }

        static UIToolkitBlurredBackgroundSettings cachedSettings;

        public static UIToolkitBlurredBackgroundSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                string typeName = typeof(UIToolkitBlurredBackgroundSettings).Name;

                cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitBlurredBackgroundSettings>(SettingsFilePath);

                // Still not found? Then search for it.
                if (cachedSettings == null)
                {
                    string[] results = AssetDatabase.FindAssets("t:" + typeName);
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitBlurredBackgroundSettings>(path);
                    }
                }

                if (cachedSettings != null)
                {
                    SessionState.EraseBool(typeName + "WaitingForReload");
                }

                // Still not found? Then create settings.
                if (cachedSettings == null)
                {
                    // Are the settings waiting for a recompile to finish? If yes then return null;
                    // This is important if an external script tries to access the settings before they
                    // are deserialized after a re-compile.
                    bool isWaitingForReloadAfterCompilation = SessionState.GetBool(typeName + "WaitingForReload", false);
                    if (isWaitingForReloadAfterCompilation)
                    {
                        Debug.LogWarning(typeName + " is waiting for assembly reload.");
                        return null;
                    }

                    cachedSettings = ScriptableObject.CreateInstance<UIToolkitBlurredBackgroundSettings>();
                    cachedSettings.LogLevel = Logger.LogLevel.Warning;
                    cachedSettings.AddShaderBeforeBuild = true;

                    AssetDatabase.CreateAsset(cachedSettings, SettingsFilePath);
                    AssetDatabase.SaveAssets();

                    Logger.OnGetLogLevel = () => cachedSettings.LogLevel;
                }
            }

            return cachedSettings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [MenuItem("Tools/UI Toolkit Blurred Background/Settings", priority = 101)]
        public static void OpenSettings()
        {
            var settings = UIToolkitBlurredBackgroundSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "UI Toolkit Blurred Background Settings could not be found or created.", "Ok");
            }
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
    [CustomEditor(typeof(UIToolkitBlurredBackgroundSettings))]
    public class UIToolkitBlurredBackgroundSettingsEditor : Editor
    {
        public UIToolkitBlurredBackgroundSettings settings;

        public void OnEnable()
        {
            settings = target as UIToolkitBlurredBackgroundSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + UIToolkitBlurredBackgroundSettings.Version);
            base.OnInspectorGUI();
        }
    }
#endif

    static class UIToolkitBlurredBackgroundSettingsProvider
    {
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateUIToolkitBlurredBackgroundSettingsProvider()
        {
            var provider = new UnityEditor.SettingsProvider("Project/UI Toolkit Blurred Background", SettingsScope.Project)
            {
                label = "UI Toolkit Blurred Background",
                guiHandler = (searchContext) =>
                {
                    var settings = UIToolkitBlurredBackgroundSettings.GetSerializedSettings();

                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField("Version: " + UIToolkitBlurredBackgroundSettings.Version);
                    if (drawButton(" Open Manual ", icon: "_Help"))
                    {
                        Installer.OpenManual();
                    }

                    var settingsObj = settings.targetObject as UIToolkitBlurredBackgroundSettings;

                    drawField("LogLevel", "Log Level", UIToolkitBlurredBackgroundSettings._logLevelTooltip, settings, style);
                    drawField("AddShaderBeforeBuild", "Add Shader Before Build", UIToolkitBlurredBackgroundSettings._addShaderBeforeBuildTooltip, settings, style);

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