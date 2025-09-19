using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public class ParticlesScriptDemo : MonoBehaviour
    {
        public Texture Texture;
        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = Utils.FindObjectOfTypeFast<UIDocument>();
                }
                return _document;
            }
        }

        IEnumerator Start()
        {
            // Make the logger only log errors
            Logger.CurrentLogLevel = Logger.LogLevel.Error;



            var root = Document.rootVisualElement;

            var particleImage = new ParticleImage();
            // Configure the UI Toolkit ParticleImage properties.
            // Notice that particleImage.ParticleSystem will be NULL here.
            particleImage.Texture = Texture;

            // Make sure you add it BEFORE you initialize it (see below)!
            particleImage.style.flexGrow = 1;
            root.Add(particleImage);

            // This creates the necessary game object with a ParticleSystem in the scene.
            particleImage.InitializeIfNecessary();

            // particleImage.ParticleSystem will be available after initialization if initialization was successful.
            // Let's change the speed of the simulation.
            var main = particleImage.ParticleSystem.main;
            main.simulationSpeed = 10f;

            // Play() is not really necessary if your particle system is set to "play on awake".
            particleImage.Play();



            // change the color and speed after 3 seconds
            yield return new WaitForSeconds(2);
            main.startColor = Color.yellow;
            main.simulationSpeed = 5f;

            // pause
            yield return new WaitForSeconds(2);
            particleImage.Pause();

            // resume
            yield return new WaitForSeconds(2);
            particleImage.Play();
            main.startColor = Color.blue;

            // soft stop
            yield return new WaitForSeconds(2);
            particleImage.ParticleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
