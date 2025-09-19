using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    [CustomEditor(typeof(ParticleSystemForImage))]
    public class ParticleSystemForImageEditor : Editor
    {
        ParticleSystemForImage system;

        public void OnEnable()
        {
            system = target as ParticleSystemForImage;

#if UNITY_EDITOR
            // Unitys in scene particle gizmos to auto-start the system once it is selected.
            // We want to keep our own EditorPlayParticles in sync so we copy that behaviour here.
            if (system.ParticleSystem.isPlaying)
                system.EditorPlayParticles = true;
#endif

            if (system.EditorPlayParticles)
                system.ParticleSystem.Play();

            ParticleManager.Instance.RegisterParticleSystem(system);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (system.ParticleSystem == null)
                return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button((system.ParticleSystem.isPlaying || system.ParticleSystem.isPaused) ? "Restart" : "Start"))
            {
                system.ParticleSystem.Stop();
                system.ParticleSystem.Simulate(0, true, restart: true); 
                system.ParticleSystem.Play();
            }
            if (system.ParticleSystem.isPlaying || system.EditorPlayParticles)
            {
                if (GUILayout.Button("Pause"))
                {
                    system.EditorPlayParticles = false;
                    system.ParticleSystem.Pause();
                }
            }
            else
            {
                if (GUILayout.Button("Play"))
                {
                    system.EditorPlayParticles = true;
                    system.ParticleSystem.Play();
                }
            }
            if (GUILayout.Button("Stop"))
            {
                system.EditorPlayParticles = false;
                system.ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            GUILayout.EndHorizontal();

            if (!hasParticleImage(system.Guid))
            {
                var col = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("There was not ParticleImage found.");
                if (GUILayout.Button("Delete Particles"))
                {
                    var scene = system.gameObject.scene;
                    Utils.SmartDestroy(system.gameObject);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                }
                GUI.color = col;
            }

#if UNITY_EDITOR
            if (system.ParticleSystem != null && system.ParticleSystem.isPlaying)
                system.EditorPlayParticles = system.ParticleSystem.isPlaying;
#endif
        }

        bool hasParticleImage(string guid)
        {
            var documents = Utils.FindObjectsOfTypeFast<UIDocument>(includeInactive: true);

            UIDocument document = null;
            ParticleImage imgInDoc;
            foreach (var doc in documents)
            {
                imgInDoc = doc.rootVisualElement.Q<ParticleImage>(className: guid);
                if (imgInDoc != null)
                {
                    document = doc;
                    break;
                }
            }

            return document != null;
        }
    }
}
