#if UNITY_EDITOR
#if KAMGAM_VISUAL_SCRIPTING
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#endif
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Kamgam.UIToolkitVisualScripting
{
    // Create a new type of Settings Asset.
    public class UIToolkitVisualScriptingSettings : ScriptableObject
    {
        public enum ShaderVariant { Performance, Gaussian };

        public const string Version = "1.0.3"; 
        public const string SettingsFilePath = "Assets/UIToolkitVisualScriptingSettings.asset";

        [SerializeField, Tooltip(_logLevelTooltip)]
        public Logger.LogLevel LogLevel;
        public const string _logLevelTooltip = "Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'";

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

        static UIToolkitVisualScriptingSettings cachedSettings;

        public static UIToolkitVisualScriptingSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                string typeName = typeof(UIToolkitVisualScriptingSettings).Name;

                cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitVisualScriptingSettings>(SettingsFilePath);

                // Still not found? Then search for it.
                if (cachedSettings == null)
                {
                    string[] results = AssetDatabase.FindAssets("t:" + typeName);
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<UIToolkitVisualScriptingSettings>(path);
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

                    cachedSettings = ScriptableObject.CreateInstance<UIToolkitVisualScriptingSettings>();
                    cachedSettings.LogLevel = Logger.LogLevel.Warning;

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
            string typeName = typeof(UIToolkitVisualScriptingSettings).Name;
            SessionState.SetBool(typeName + "WaitingForReload", true);
        }

        // We use this callback instead of CompilationPipeline.compilationFinished because
        // compilationFinished runs before the assemply has been reloaded but DidReloadScripts
        // runs after. And only after we can access the Settings asset.
        [UnityEditor.Callbacks.DidReloadScripts(999000)]
        public static void DidReloadScripts()
        {
            string typeName = typeof(UIToolkitVisualScriptingSettings).Name;
            SessionState.EraseBool(typeName + "WaitingForReload");
        }

        static void onSettingsCreated()
        {
#if !KAMGAM_VISUAL_SCRIPTING
            Debug.LogWarning("You need to install the Visual Scripting package from the package manager if you want to use this asset.");
            Debug.LogWarning("NOTICE: You will have to call Tools > UI Toolkit Visual Scripting > Debug > Rebuild Nodes afterwards to complete the installation.");

            bool openPackageManager = EditorUtility.DisplayDialog("Visual Scripting is not installed!",
                "You need to install the Visual Scripting package from the package manager if you want to use this asset." + 
                "\n\nNOTICE: You will have to call Tools > UI Toolkit Visual Scripting > Debug > Rebuild Nodes afterwards to complete the installation.",
                "Open Package Manager", "Cancel");

            if(openPackageManager)
            {
                UnityEditor.PackageManager.UI.Window.Open("com.unity.visualscripting");
                CrossCompileCallbacks.RegisterCallback(RebuildNodes);
            }
#else
            RebuildNodes();
#endif

            bool openManual = EditorUtility.DisplayDialog(
                    "UI Toolkit Visual Scripting",
                    "Thank you for choosing UI Toolkit Visual Scripting.\n\n" +
                    "You'll find the tool under Tools > UI Toolkit Visual Scripting > Open\n\n" +
                    "Please start by reading the manual.\n\n" +
                    "It would be great if you could find the time to leave a review.",
                    "Open manual", "Cancel"
                    );

            if (openManual)
            {
                OpenManual();
            }
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/Setup", priority = 1)]
        public static void RebuildNodes()
        {
#if KAMGAM_VISUAL_SCRIPTING
            // If Visual Scripting is not yet initialized then ask the user to do it.
            if (BoltFlow.instance == null || BoltFlow.instance.paths == null)
            {
                EditorUtility.DisplayDialog(
                    "Please initialize Visual Scripting!",
                    "Go to Project Settings > Visual Scripting and press the 'Initialize Visual Scripting' button. Then (that's important) go to: Tools > UI Toolkit Visual Scripting > Setup.",
                    "Okay"
                );
                Debug.LogWarning("Please go to Project Settings > Visual Scripting and press the 'Initialize Visual Scripting' button. Then (that's important) go to: Tools > UI Toolkit Visual Scripting > Setup.");
            }
            else
            {
                try
                {
                    var assemblyOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.assemblyOptions));
                    var assemblies = (List<LooseAssemblyName>)assemblyOptionsMetadata.value;
                    var assembly = System.Reflection.Assembly.GetAssembly(typeof(UIToolkitVisualElementExtensions));
                    int exists = assemblies.Count(asm => asm.name == assembly.GetName().Name);
                    if(exists == 0)
                    {
                        var looseAssembly = new LooseAssemblyName(assembly.GetName().Name);
                        assemblyOptionsMetadata.Add(looseAssembly);
                        BoltCore.Configuration.Save();
                        Codebase.UpdateSettings();
                    }

                    // Rebuild the nodes
                    UnitBase.Rebuild();
                }
                catch (Exception e)
                {
                    bool openManual = EditorUtility.DisplayDialog("Node setup failed", "Please check the manual on how to set things up manually.", "Open Manual", "Cancel");
                    if (openManual)
                    {
                        OpenManual();
                    }
                    throw e;
                }
            }
#else
            Debug.LogError("Node Build aborted: Please install the Visual Scripting package first!");
#endif
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/Manual", priority = 101)]
        public static void OpenManual()
        {
            Application.OpenURL("https://kamgam.com/unity/UIToolkitVisualScriptingManual.pdf");
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/Settings", priority = 101)]
        public static void OpenSettings()
        {
            var settings = UIToolkitVisualScriptingSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "UI Toolkit Visual Scripting Settings could not be found or created.", "Ok");
            }
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/Please leave a review :-)", priority = 410)]
        public static void LeaveReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/255702?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/More Asset by KAMGAM", priority = 420)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/37829?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UI Toolkit Visual Scripting/Version: " + Version, priority = 510)]
        public static void LogVersion()
        {
            Debug.Log("UI Toolkit Visual Scripting Version: " + Version);
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
    [CustomEditor(typeof(UIToolkitVisualScriptingSettings))]
    public class UIToolkitVisualScriptingSettingsEditor : Editor
    {
        public UIToolkitVisualScriptingSettings settings;

        public void OnEnable()
        {
            settings = target as UIToolkitVisualScriptingSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + UIToolkitVisualScriptingSettings.Version);
            base.OnInspectorGUI();
        }
    }
#endif

    static class UIToolkitVisualScriptingSettingsProvider
    {
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateUIToolkitVisualScriptingSettingsProvider()
        {
            var provider = new UnityEditor.SettingsProvider("Project/UI Toolkit Visual Scripting", SettingsScope.Project)
            {
                label = "UI Toolkit Visual Scripting",
                guiHandler = (searchContext) =>
                {
                    var settings = UIToolkitVisualScriptingSettings.GetSerializedSettings();

                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField("Version: " + UIToolkitVisualScriptingSettings.Version);
                    if (drawButton(" Open Manual ", icon: "_Help"))
                    {
                        UIToolkitVisualScriptingSettings.OpenManual();
                    }

                    var settingsObj = settings.targetObject as UIToolkitVisualScriptingSettings;

                    drawField("LogLevel", "Log Level", UIToolkitVisualScriptingSettings._logLevelTooltip, settings, style);

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