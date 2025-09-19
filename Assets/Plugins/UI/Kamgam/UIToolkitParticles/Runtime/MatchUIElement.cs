using UnityEngine;

namespace Kamgam.UIToolkitParticles
{
    [ExecuteAlways]
    public class MatchUIElement : MonoBehaviour
    {
        public bool MatchX = true;
        public bool MatchY = true;

        [Tooltip("0/0 = top left, 1/1 = bottom right")]
        public Vector2 NormalizedPosInUI = new Vector2(0.5f, 0.5f);

        protected ParticleSystemForImage _system;
        public ParticleSystemForImage System
        {
            get
            {
                if (_system == null)
                {
                    _system = this.GetComponentInParent<ParticleSystemForImage>(includeInactive: true);
                }
                return _system;
            }
        }

        void Update()
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || UnityEditor.EditorApplication.isCompiling)
                return;
#endif
            var delta = System.GetLocalPos3D(NormalizedPosInUI);
            var pos = transform.localPosition;
            
            if (MatchX)
                pos.x = delta.x;
            else
                pos.x = 0f;

            if (MatchY)
                pos.y = delta.y;
            else
                pos.y = 0;

            transform.localPosition = pos;
        }
    }
}
