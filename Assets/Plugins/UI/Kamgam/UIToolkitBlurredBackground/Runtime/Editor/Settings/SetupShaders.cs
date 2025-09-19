#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.UIToolkitBlurredBackground
{
    /// <summary>
    /// Since we do not add any objects directly referencing the materials/shaders we
    /// need to make sure the shaders are added  to builds so they can be found at runtime.
    /// </summary>
    public static class SetupShaders
    {
        public class SetupShadersOnBuild : IPreprocessBuildWithReport
        {
            public int callbackOrder => int.MinValue + 10;

            public void OnPreprocessBuild(BuildReport report)
            {
                if (UIToolkitBlurredBackgroundSettings.GetOrCreateSettings().AddShaderBeforeBuild)
                    SetupShaders.AddShaders();
            }
        }

        [MenuItem("Tools/UI Toolkit Blurred Background/Debug/Add shaders to always included shader", priority = 401)]
        public static void AddShaders()
        {
#if !KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            // BuiltIn
            AddShaders(BlurredBackgroundBufferBuiltIn.ShaderName);
#elif KAMGAM_RENDER_PIPELINE_URP
            // URP
            AddShaders(BlurredBackgroundPassURP.ShaderName);
#else
            // HDRP
            AddShaders(BlurredBackgroundPassHDRP.ShaderName);
#endif
        }

        public static void AddShaders(string shaderName)
        {
            // Thanks to: https://forum.unity.com/threads/modify-always-included-shaders-with-pre-processor.509479/#post-3509413

            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;

            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
            bool hasShader = false;
            for (int i = 0; i < arrayProp.arraySize; ++i)
            {
                var arrayElem = arrayProp.GetArrayElementAtIndex(i);
                if (shader == arrayElem.objectReferenceValue)
                {
                    hasShader = true;
                    break;
                }
            }

            if (!hasShader)
            {
                int arrayIndex = arrayProp.arraySize;
                arrayProp.InsertArrayElementAtIndex(arrayIndex);
                var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
                arrayElem.objectReferenceValue = shader;

                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();

                Debug.Log("Added the '"+ shaderName + "' shader to always included shaders (see Project Settings > Graphics). UI Toolkit Blurred Background requires it to render the blur. Hope that's okay.");
            }
        }
    }
}
#endif
