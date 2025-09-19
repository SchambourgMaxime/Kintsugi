using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public partial class ParticleImage : Image
    {
        public bool _addAttractor = false;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Add-Attractor")]
#endif
        public bool AddAttractor
        {
            get => _addAttractor;

            set
            {
                if (value != _addAttractor)
                {
                    _addAttractor = value;
                    MarkDirtyRepaint();
                    if (value)
                        enableAttractor();
                    else
                        disableAttractor();
                }
            }
        }

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Attractor-Element-Type")]
#endif
        public UIElementType AttractorElementType { get; set; } = UIElementType.VisualElement;

        protected bool _attractorElementIsDirty = true;
        protected VisualElement _attractorElement;
        public VisualElement AttractorElement
        {
            get
            {
                // Query the element again if the query parameters have changed
                if (_attractorElementIsDirty)
                {
                    _attractorElementIsDirty = false;
                    _attractorElement = panel.visualTree.QueryType(AttractorElementType, AttractorElementName, AttractorElementClass);
                }

                return _attractorElement;
            }

            set
            {
                _attractorElement = value;
                _attractorElementIsDirty = false;
            }
        }

        protected string _attractorElementName;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Attractor-Element-Name")]
#endif
        public string AttractorElementName
        {
            get => _attractorElementName;
            set
            {
                if (value != _attractorElementName)
                {
                    _attractorElementName = value;
                    _attractorElementIsDirty = true;
                }
            }
        }

        protected string _attractorElementClass;
#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute("Attractor-Element-Class")]
#endif
        public string AttractorElementClass
        {
            get => _attractorElementClass;
            set
            {
                if (value != _attractorElementClass)
                {
                    _attractorElementClass = value;
                    _attractorElementIsDirty = true;
                }
            }
        }

        protected ParticleSystemForceField _particleSystemForceField;
        public ParticleSystemForceField ParticleSystemForceField
        {
            get
            {
                if (ParticleSystem == null)
                    return null;

                if (_particleSystemForceField == null)
                {
                    _particleSystemForceField = ParticleSystem.gameObject.GetComponentInChildren<ParticleSystemForceField>(includeInactive: true);
                }

                return _particleSystemForceField;
            }
        }

        protected void enableAttractor()
        {
            if (ParticleSystem == null)
                return;

            // Find or create force field
            var forceField = ParticleSystem.gameObject.GetComponentInChildren<ParticleSystemForceField>(includeInactive: true);

            var forces = ParticleSystem.externalForces;
            forces.enabled = true;
            forces.influenceFilter = ParticleSystemGameObjectFilter.List;
            if (forces.influenceCount == 0 || forces.GetInfluence(0) == null)
            {
                if (forceField == null)
                {
                    var go = new GameObject("Particle Attractor ForceField");
                    go.SetActive(false);
                    go.transform.parent = ParticleSystem.gameObject.transform;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = new Vector3(0f, 0f, 0f);
                    forceField = go.AddComponent<ParticleSystemForceField>();
                    forceField.gravity = 0.05f;
                    forceField.drag = 0.5f;
                    forceField.endRange = 100f;
                    forceField.multiplyDragByParticleSize = false;
                    go.SetActive(true);
                }
                if (forces.influenceCount > 0)
                    forces.SetInfluence(0, forceField);
                else
                    forces.AddInfluence(forceField);
            }

            if (forceField != null)
                forceField.gameObject.SetActive(true);
        }

        protected void disableAttractor()
        {
            if (ParticleSystem == null)
                return;

            var forces = ParticleSystem.externalForces;
            forces.enabled = false;

            var forceField = ParticleSystem.gameObject.GetComponentInChildren<ParticleSystemForceField>(includeInactive: true);
            if (forceField != null)
            {
                forceField.gameObject.SetActive(false);
            }
        }

        protected void updateAttractorPosition(Vector3 origin)
        {
            if (!AddAttractor || ParticleSystem == null || ParticleSystemForceField == null || AttractorElement == null)
                return;

#if UNITY_EDITOR
            if ((PreviewInGameView && IsPartOfUIBuilder()) || (!PreviewInGameView && !IsPartOfUIBuilder()))
                return;
#endif

            float width = resolvedStyle.width;
            float height = resolvedStyle.height;

            if (float.IsNaN(width))
                return;

            Vector3 center = new Vector3(width * 0.5f, height * 0.5f, 0f);

            Vector3 deltaInWorldSpace;
            if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                var deltaToCenter = (Vector3)getCenterPosLocalDelta(this, AttractorElement);
                deltaInWorldSpace = deltaToCenter / PixelsPerUnit;
                deltaInWorldSpace.y *= -1; // Convert from Y = Down to Y = Up.
            }
            else
            {
                var deltaToCenter = (Vector3)getCenterPosLocalDelta(this, AttractorElement);
                var deltaToOrigin = center + deltaToCenter - origin;

                deltaInWorldSpace = deltaToOrigin / PixelsPerUnit;
                deltaInWorldSpace.y *= -1; // Convert from Y = Down to Y = Up.
            }

            // The attractor is a child of the particle system transform.
            // Therefore, if simulation space is world space, we have to take the
            // particle systems position into account.
            deltaInWorldSpace -= new Vector3(
                ParticleSystemForImage.transform.position.x - ParticleSystemForImage.DefaultPosition.x,
                ParticleSystemForImage.transform.position.y - ParticleSystemForImage.DefaultPosition.y,
                0);

            ParticleSystemForceField.gameObject.transform.localPosition = deltaInWorldSpace;
        }
    }
}
