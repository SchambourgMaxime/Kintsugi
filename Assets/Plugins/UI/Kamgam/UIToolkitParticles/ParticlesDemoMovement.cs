using UnityEngine;

namespace Kamgam.UIToolkitParticles
{
    public class ParticlesDemoMovement : MonoBehaviour
    {
        protected float v;

        void Update()
        {
            v += 4f * Time.deltaTime;
            var pos = transform.localPosition;
            pos.x = Mathf.Sin(v) * 10f;
            transform.localPosition = pos;
        }
    }
}
