using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public class ParticlesDemoScreen2_ProgressBar : MonoBehaviour
    {
        public UIDocument Document;

        protected ProgressBar _progressBar;
        protected ParticleImage _progressParticles;
        protected Coroutine _progressAnimation;

        public IEnumerator Start()
        {
            _progressBar = Document.rootVisualElement.Q<ProgressBar>();
            _progressParticles = _progressBar.Q<ParticleImage>();

            _progressBar.RegisterCallback<ChangeEvent<float>>(onProgressChanged);
            _progressBar.RegisterCallback<ClickEvent>(onClick);

            // Wait a bit before animation the progress
            yield return new WaitForSeconds(1f);

            _progressAnimation = StartCoroutine(animateProgress());
        }

        private IEnumerator animateProgress()
        {
            _progressParticles.Stop(stopBehaviour: ParticleSystemStopBehavior.StopEmittingAndClear);
            _progressBar.value = 0;

            // 0 - 100% progress animation
            float speed = 100f;
            while (_progressBar.value < 100f)
            {
                yield return null;

                // Add some fake pauses in the middle to make it feel a bit more realistic
                if (_progressBar.value > 30f && _progressBar.value < 50f)
                {
                    if (UnityEngine.Random.value > 0.9f)
                    {
                        yield return new WaitForSeconds(UnityEngine.Random.value * 0.2f);
                    }
                }
                // Make it faster in the second half
                if (_progressBar.value > 50f)
                    speed = 200;

                _progressBar.value += (speed * Time.deltaTime) / 3;
            }

            _progressAnimation = null;
        }

        private void onClick(ClickEvent evt)
        {
            if (_progressAnimation != null)
            {
                StopCoroutine(_progressAnimation);
            }
            _progressAnimation = StartCoroutine(animateProgress());
        }

        private void onProgressChanged(ChangeEvent<float> evt)
        {
            // -0.7f to have the particle positioned a bit inside the end of the progress fill.
            _progressParticles.PositionX = Mathf.Clamp(evt.newValue - 0.7f, 0f, 100f);

            // Stop particles once we are done.
            if (evt.newValue >= 100f)
            {
                _progressParticles.Stop();
            }
            // Make sure particles are playing.
            else if (evt.newValue > 1f && !_progressParticles.IsPlaying())
            {
                _progressParticles.Play();
            }
        }
    }
}
