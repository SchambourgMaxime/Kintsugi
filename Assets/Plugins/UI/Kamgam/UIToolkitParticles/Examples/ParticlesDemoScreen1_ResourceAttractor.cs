using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public class ParticlesDemoScreen1_ResourceAttractor : MonoBehaviour
    {
        public UIDocument Document;

        void Start()
        {
            var coinsBtn = Document.rootVisualElement.Q<Button>(name: "GiveCoinsButton");
            coinsBtn.RegisterCallback<ClickEvent>(onCoinsClicked);

            var gemsBtn = Document.rootVisualElement.Q<Button>(name: "GiveGemsButton");
            gemsBtn.RegisterCallback<ClickEvent>(onGemsClicked);
        }

        private void onCoinsClicked(ClickEvent evt)
        {
            var coinParticles = Document.rootVisualElement.Q<ParticleImage>(name: "CoinParticles");
            coinParticles.Play();
        }

        private void onGemsClicked(ClickEvent evt)
        {
            var gemParticles = Document.rootVisualElement.Q<ParticleImage>(name: "GemParticles");
            gemParticles.Play();
        }
    }
}
