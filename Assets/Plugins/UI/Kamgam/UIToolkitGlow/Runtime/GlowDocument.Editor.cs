using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlowDocument))]
    public class GlowRegistryProviderEditor : UnityEditor.Editor
    {
        GlowDocument obj;

        public void OnEnable()
        {
            obj = target as GlowDocument;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button( new GUIContent("Refresh Preview", "Sometimes the preview in UI Builder or the Game View does not update automatically. Use this to force and update in the UI Builder and the Game View.")))
            {
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    obj.UpdateGlowOnChildren();
                }
                else
                {
                    UIEditorPanelObserver.RefreshGlowManipulators();
                }
            }
        }
    }
#endif

}