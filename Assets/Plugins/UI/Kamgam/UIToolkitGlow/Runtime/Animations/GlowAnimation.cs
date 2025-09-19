using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UIToolkitGlow
{
#if KAMGAM_VISUAL_SCRIPTING
    // Why? See: https://forum.unity.com/threads/unable-to-provide-a-default-for-getvalue-on-object-valueinput.1140022/#post-9138727
    [Unity.VisualScripting.Inspectable]
#endif

    /// <summary>
    /// Base class for animations. You can also makes your very own by implementing IGlowAnimation
    /// but it is recommended to use this since it takes care of all the scaffolding for you.
    /// </summary>
    public abstract class GlowAnimation : IGlowAnimation
    {
        static List<IGlowAnimation> _animationsOnManipulators = new List<IGlowAnimation>();

        /// <summary>
        /// Use this to add a copy of the animationTemplate to the manipulator.
        /// </summary>
        /// <param name="animationTemplateName">The name of the animation that will be searched for in configRoot.</param>
        /// <param name="manipulator">The manipulator to which the animation will be added.</param>
        /// <param name="configRoot">(optional, default: null) The configRoot to use. If not specified then it will try to find the root on a GlowDocument in the active scene.</param>
        /// <param name="linkToTemplate">(optional, default: false) If true then the copy will listen for changes in the original and update accordingly.</param>
        /// <param name="reuseExisting">(optional, default: true) Defines what to do if the manipulator already has an animation with the same name: Reuse? Delete and re-add?</param>
        /// <returns>The added animation. May be NULL if no animation with nam = animationName was found.</returns>
        public static IGlowAnimation AddAnimationCopyTo(
            string animationTemplateName,
            GlowManipulator manipulator,
            GlowConfigRoot configRoot = null,
            bool linkToTemplate = false,
            bool reuseExisting = true)
        {
            IGlowAnimation animation = null;

            if (configRoot == null)
                configRoot = GlowConfigRoot.FindConfigRoot();
            if (configRoot != null)
            {
                var animationTemplate = configRoot.GetAnimationByName(animationTemplateName);
                return AddAnimationCopyTo(animationTemplate, manipulator, linkToTemplate, reuseExisting);
            }

            return animation;
        }

        public static void GetAnimationsOnManipulator(GlowManipulator manipulator, List<IGlowAnimation> animations)
        {
            GetAnimationsOnManipulator<IGlowAnimation>(manipulator, animations);
        }

        public static void GetAnimationsOnManipulator<T>(GlowManipulator manipulator, List<T> animations)
            where T: IGlowAnimation
        {
            if (manipulator == null)
                return;

            foreach (var animation in _animationsOnManipulators)
            {
                if (animation.Manipulator == null)
                    continue;

                if (animation.Manipulator == manipulator && animation is T)
                {
                    animations.Add((T)animation);
                }
            }
        }

        public static IGlowAnimation GetAnimationOnManipulatorByName(GlowManipulator manipulator, string animationName)
        {
            return GetAnimationOnManipulatorByName<IGlowAnimation>(manipulator, animationName);
        }

        public static T GetAnimationOnManipulatorByName<T>(GlowManipulator manipulator, string animationName)
            where T : IGlowAnimation
        {
            _tmpAnimationsResult.Clear();
            GetAnimationsOnManipulator(manipulator, _tmpAnimationsResult);
            foreach (var animation in _tmpAnimationsResult)
            {
                if (animation.Name == animationName)
                {
                    return (T)animation;
                }
            }

            return default;
        }

        static List<IGlowAnimation> _tmpAnimationsResult = new List<IGlowAnimation>();

        /// <summary>
        /// Use this to add a copy of the animationTemplate to the manipulator.
        /// </summary>
        /// <param name="animationTemplate">The animation that will be copied from.</param>
        /// <param name="manipulator">The manipulator to which the animation will be added.</param>
        /// <param name="linkToTemplate">(optional, default: false) If true then the copy will listen for changes in the original and update accordingly.</param>
        /// <param name="reuseExisting">(optional, default: true) Defines what to do if the manipulator already has an animation with the same name: Reuse? Delete and re-add?</param>
        /// <returns>The added animation. May be NULL if no animation with name = animationName was found.</returns>
        public static IGlowAnimation AddAnimationCopyTo(
            IGlowAnimation animationTemplate,
            GlowManipulator manipulator,
            bool linkToTemplate = false,
            bool reuseExisting = true)
        {
            if (manipulator == null)
                return null;

            IGlowAnimation animation = null;

            if (animationTemplate != null)
            {
                // What do we do if the manipulator already has an animation with the same name? Reuse? Delete and re-add?
                animation = GetAnimationOnManipulatorByName(manipulator, animationTemplate.Name);
                if (animation != null && animation.Manipulator != null && animation.Name == animationTemplate.Name)
                {
                    if (!reuseExisting)
                    {
                        animation.RemoveFromManipulator(manipulator);
                        animation = null;
                    }
                }

                // Create new copy if needed
                if (animation == null)
                {
                    animation = animationTemplate.Copy();
                    animation.AddToManipulator(manipulator);
                }

                if (linkToTemplate)
                {
                    animationTemplate.OnValueChanged -= animation.CopyValuesFrom;
                    animationTemplate.OnValueChanged += animation.CopyValuesFrom;
                }
            }

            return animation;
        }

        /// <summary>
        /// Adds the animation to the manipulator.<br />
        /// If the animation has already been added to another manipulator then the animation will be automatically removed from that previous manipulator.<br />
        /// If you want to control multiple manipulators you will have to make a COPY of the animation and assign one copy to each manipulator. Consider using "AddAnimationCopyToManipulator()".
        /// </summary>
        /// <param name="animationName">The name of the animation that will be searched for in configRoot.</param>
        /// <param name="manipulator">The manipulator to which the animation will be added.</param>
        /// <param name="configRoot">(optional, default: null) The configRoot to use. If not specified then it will try to find the root on a GlowDocument in the active scene.</param>
        /// <returns>The added animation. May be NULL if no animation with nam = animationName was found.</returns>
        public static IGlowAnimation AddAnimationTo(
            string animationName,
            GlowManipulator manipulator,
            GlowConfigRoot configRoot = null)
        {
            IGlowAnimation animation = null;

            if (configRoot == null)
                configRoot = GlowConfigRoot.FindConfigRoot();
            if (configRoot != null)
            {
                animation = configRoot.GetAnimationByName(animationName);
                if(animation != null)
                    animation.AddToManipulator(manipulator);
            }

            return animation;
        }


        public event System.Action<GlowAnimation> OnValueChanged;

        /// <summary>
        /// The identifier by which the animation will be searched for.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Use this instead of Time.deltaTime
        /// </summary>
        public float DeltaTime => (1f / FrameRate) * Time.timeScale;

        /// <summary>
        /// The default frame rate to fall back on if Application.targetFrameRate is not set.
        /// </summary>
        public static int DefaultFrameRate = 30;

        protected int _frameRate = -1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int FrameRate
        {
            get
            {
                if (_frameRate <= 0)
                {
                    if (Application.targetFrameRate < 0)
                    {
                        return DefaultFrameRate;
                    }
                    else
                    {
                        return Application.targetFrameRate;
                    }
                }
                else
                {
                    return _frameRate;
                }
            }
            set
            {
                if (_frameRate == value)
                    return;

                _frameRate = value;

                if (_scheduledAnimation != null)
                {
                    RemoveScheduledAnimation();
                    Play();
                }

                TriggerOnValueChanged();
            }
        }

        protected GlowManipulator _manipulator;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public GlowManipulator Manipulator
        {
            get => _manipulator;
            private set => _manipulator = value;
        }

        protected IVisualElementScheduledItem _scheduledAnimation;

        public void TriggerOnValueChanged()
        {
            OnValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Use this to add a copy of the animation to the manipulator.
        /// </summary>
        /// <param name="manipulator">The manipulator to which the animation will be added.</param>
        /// <param name="linkToTemplate">(optional, default: false) If true then the copy will listen for changes in the original and update accordingly.</param>
        /// <param name="reuseExisting">(optional, default: true) Defines what to do if the manipulator already has an animation with the same name: Reuse? Delete and re-add?</param>
        /// <returns>The added animation. May be NULL if no animation with nam = animationName was found.</returns>
        public virtual IGlowAnimation AddCopyToManipulator(
            GlowManipulator manipulator,
            bool linkToTemplate = false,
            bool reuseExisting = true)
        {
            return AddAnimationCopyTo(this, manipulator, linkToTemplate, reuseExisting);
        }

        /// <summary>
        /// Adds the animation to the manipulator.<br />
        /// If the animation has already been added to another manipulator then the animation will be automatically removed from that previous manipulator.<br />
        /// If you want to control multiple manipulators you will have to make a COPY of the animation and assign one copy to each manipulator. Consider using "AddAnimationCopyToManipulator()".
        /// </summary>
        public virtual void AddToManipulator(GlowManipulator manipulator)
        {
            if(_manipulator != null && _manipulator != manipulator)
            {
                RemoveFromManipulator(manipulator);
            }

            _manipulator = manipulator;
            if (!_animationsOnManipulators.Contains(this))
            {
                _animationsOnManipulators.Add(this);
            }

            manipulator.OnElementAttachToPanel -= onManipulatorAttached;
            manipulator.OnElementAttachToPanel += onManipulatorAttached;

            manipulator.OnElementDetachFromPanel -= onManipulatorDetached;
            manipulator.OnElementDetachFromPanel += onManipulatorDetached;

            manipulator.OnUnregisterCallbacksOnTarget -= onManipulatorRemoved;
            manipulator.OnUnregisterCallbacksOnTarget += onManipulatorRemoved;

            // Register mesh change event for animations
            manipulator.OnBeforeMeshWrite -= OnUpdateMesh;
            manipulator.OnBeforeMeshWrite += OnUpdateMesh;

            Play();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void RemoveFromManipulator()
        {
            RemoveFromManipulator(_manipulator);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void RemoveFromManipulator(GlowManipulator manipulator)
        {
            if (manipulator == null)
                return;

            RemoveScheduledAnimation();
            manipulator.OnBeforeMeshWrite -= OnUpdateMesh;

            _manipulator = null;
            if (_animationsOnManipulators.Contains(this))
            {
                _animationsOnManipulators.Remove(this);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Play()
        {
            if (_scheduledAnimation == null)
            {
                if (_manipulator != null && _manipulator.target != null)
                {
                    // Start animation by scheduling a mark dirty at regular intervals.
                    long intervalMS = 1000 / FrameRate;
                    _scheduledAnimation = _manipulator.target.schedule.Execute(updateAnimation).Every(intervalMS);
                }
            }
            else
            {
                _scheduledAnimation.Resume();
            }
        }

        protected virtual void updateAnimation()
        {
            if (_manipulator == null)
                return;

            Update();
            _manipulator.MarkDirtyAnimation();
        }

        public abstract IGlowAnimation Copy();
        public virtual void CopyValuesFrom(IGlowAnimation source)
        {
            Name = source.Name;
            FrameRate = source.FrameRate;
        }

        public abstract void Update();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Pause()
        {
            if (_scheduledAnimation != null)
            {
                _scheduledAnimation.Pause();
            }
        }

        public void RemoveScheduledAnimation()
        {
            if (_scheduledAnimation != null)
            {
                _scheduledAnimation.Pause();
                _scheduledAnimation = null;
            }
        }

        protected virtual void onManipulatorAttached(GlowManipulator manipulator, AttachToPanelEvent evt)
        {
            AddToManipulator(manipulator);
            Play();
        }

        protected virtual void onManipulatorDetached(GlowManipulator manipulator, DetachFromPanelEvent evt)
        {
            Pause();
            RemoveFromManipulator(manipulator);
        }

        protected virtual void onManipulatorRemoved(GlowManipulator manipulator)
        {
            Pause();
            RemoveFromManipulator(manipulator);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void OnUpdateMesh(
            GlowManipulator manipulator,
            List<Vertex> vertices,
            List<ushort> triangles,
            List<ushort> outerIndices,
            List<ushort> innerIndices,
            Dictionary<ushort, ushort> outerToInnerIndices);
    }
}