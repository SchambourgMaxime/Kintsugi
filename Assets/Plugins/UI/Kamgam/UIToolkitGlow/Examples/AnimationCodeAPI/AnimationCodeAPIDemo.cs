using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{ 
    public class AnimationCodeAPIDemo : MonoBehaviour
    {
        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = this.GetComponent<UIDocument>();
                }
                return _document;
            }
        }

        public IEnumerator Start()
        {
            var element1 = Document.rootVisualElement.Q<Glow>("Animation1");
            var element2 = Document.rootVisualElement.Q<Glow>("Animation2");

            // Speed up the first animation
            var anim1 = element1.GetAnimation<BlobAnimation>();
            anim1.Speed *= 2f;

            yield return new WaitForSeconds(2f);

            // Change the template speed (notice how all the animations are changed).
            Debug.Log("Slowing down");
            var animationTemplate = GlowConfigRoot.FindConfigRoot().GetAnimationByName<BlobAnimation>(element1.animationName);
            animationTemplate.Speed = 0.4f;

            yield return new WaitForSeconds(2f);

            // Change anim 1 speed back to normal.
            Debug.Log("Speed up first animation.");
            anim1.Speed = 2f;
        }
    }
}