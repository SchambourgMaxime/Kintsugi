using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    public static class GlowUtils
    {
        public static Dictionary<VisualElement, EventCallback<MouseOverEvent>> _toggleClassHoverMouseOverFunctions = new Dictionary<VisualElement, EventCallback<MouseOverEvent>>();
        public static Dictionary<VisualElement, EventCallback<MouseOutEvent>> _toggleClassHoverMouseOutFunctions = new Dictionary<VisualElement, EventCallback<MouseOutEvent>>();

        public static void RegisterToggleClassOnHover<T>(UQueryBuilder<T> queryBuilder, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Over
                // - Clear
                if (_toggleClassHoverMouseOverFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOverEvent>(_toggleClassHoverMouseOverFunctions[element]);
                    _toggleClassHoverMouseOverFunctions.Remove(element);
                }
                // - Create
                var onMouseOverAction = new EventCallback<MouseOverEvent>((MouseOverEvent evt) => onMouseOverForHoverClass(evt, classNames));
                element.RegisterCallback<MouseOverEvent>(onMouseOverAction);
                _toggleClassHoverMouseOverFunctions.Add(element, onMouseOverAction);

                // Out
                // - Clear
                if (_toggleClassHoverMouseOutFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOutEvent>(_toggleClassHoverMouseOutFunctions[element]);
                    _toggleClassHoverMouseOutFunctions.Remove(element);
                }
                // - Create
                var onMouseOutAction = new EventCallback<MouseOutEvent>((MouseOutEvent evt) => onMouseOutForHoverClass(evt, classNames));
                element.RegisterCallback<MouseOutEvent>(onMouseOutAction);
                _toggleClassHoverMouseOutFunctions.Add(element, onMouseOutAction);
            }
        }

        public static void UnregisterToggleClassOnHover<T>(UQueryBuilder<T> queryBuilder, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Over
                // - Clear
                if (_toggleClassHoverMouseOverFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOverEvent>(_toggleClassHoverMouseOverFunctions[element]);
                    _toggleClassHoverMouseOverFunctions.Remove(element);
                }

                // Out
                // - Clear
                if (_toggleClassHoverMouseOutFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOutEvent>(_toggleClassHoverMouseOutFunctions[element]);
                    _toggleClassHoverMouseOutFunctions.Remove(element);
                }

                removeClasses(element, classNames);
            }
        }

        static void onMouseOverForHoverClass(MouseOverEvent evt, string[] classNames)
        {
            var element = evt.target as VisualElement;
            addClasses(element, classNames);
        }

        static void onMouseOutForHoverClass(MouseOutEvent evt, string[] classNames)
        {
            var element = evt.target as VisualElement;
            removeClasses(element, classNames);
        }

        public static Dictionary<VisualElement, EventCallback<FocusEvent>> _toggleClassFocusFunctions = new Dictionary<VisualElement, EventCallback<FocusEvent>>();
        public static Dictionary<VisualElement, EventCallback<BlurEvent>> _toggleClassBlurFunctions = new Dictionary<VisualElement, EventCallback<BlurEvent>>();

        public static void RegisterToggleClassOnFocus<T>(UQueryBuilder<T> queryBuilder, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Focus
                // - Clear
                if (_toggleClassFocusFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<FocusEvent>(_toggleClassFocusFunctions[element]);
                    _toggleClassFocusFunctions.Remove(element);
                }
                // - Create
                var focusAction = new EventCallback<FocusEvent>((FocusEvent evt) => onFocusForToggleClass(evt, classNames));
                element.RegisterCallback<FocusEvent>(focusAction);
                _toggleClassFocusFunctions.Add(element, focusAction);

                // Blur
                // - Clear
                if (_toggleClassBlurFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<BlurEvent>(_toggleClassBlurFunctions[element]);
                    _toggleClassBlurFunctions.Remove(element);
                }
                // - Create
                var blurAction = new EventCallback<BlurEvent>((BlurEvent evt) => onBlurForToggleClass(evt, classNames));
                element.RegisterCallback<BlurEvent>(blurAction);
                _toggleClassBlurFunctions.Add(element, blurAction);
            }
        }

        public static void UnregisterToggleClassOnFocus<T>(UQueryBuilder<T> queryBuilder, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Focus
                // - Clear
                if (_toggleClassFocusFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<FocusEvent>(_toggleClassFocusFunctions[element]);
                    _toggleClassFocusFunctions.Remove(element);
                }

                // Blur
                // - Clear
                if (_toggleClassBlurFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<BlurEvent>(_toggleClassBlurFunctions[element]);
                    _toggleClassBlurFunctions.Remove(element);
                }

                removeClasses(element, classNames);
            }
        }

        static void onFocusForToggleClass(FocusEvent evt, string[] classNames)
        {
            var element = evt.target as VisualElement;
            addClasses(element, classNames);
        }

        static void onBlurForToggleClass(BlurEvent evt, string[] classNames)
        {
            var element = evt.target as VisualElement;
            removeClasses(element, classNames);
        }

        static void addClasses(VisualElement element, string[] classNames)
        {
            if (element == null)
                return;

            foreach (var className in classNames)
            {
                GlowPanel.AddClass(element, className);
            }
        }

        static void removeClasses(VisualElement element, string[] classNames)
        {
            foreach (var className in classNames)
            {
                GlowPanel.RemoveClass(element, className);
            }
        }



        public static Dictionary<VisualElement, EventCallback<MouseOverEvent>> _toggleGlowAndAnimationHoverMouseOverFunctions = new Dictionary<VisualElement, EventCallback<MouseOverEvent>>();
        public static Dictionary<VisualElement, EventCallback<MouseOutEvent>> _toggleGlowAndAnimationHoverMouseOutFunctions = new Dictionary<VisualElement, EventCallback<MouseOutEvent>>();
        public static Dictionary<VisualElement, IGlowAnimation> _toggleAnimationHoverAnimations = new Dictionary<VisualElement, IGlowAnimation>();

        public static void RegisterToggleGlowAndAnimationOnHover<T>(UQueryBuilder<T> queryBuilder, string animationName, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Over
                // - Clear
                if (_toggleGlowAndAnimationHoverMouseOverFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOverEvent>(_toggleGlowAndAnimationHoverMouseOverFunctions[element]);
                    _toggleGlowAndAnimationHoverMouseOverFunctions.Remove(element);
                }
                // - Create
                var onMouseOverAction = new EventCallback<MouseOverEvent>((MouseOverEvent evt) => onMouseOverForAnimation(_toggleAnimationHoverAnimations, evt, animationName, classNames));
                element.RegisterCallback<MouseOverEvent>(onMouseOverAction);
                _toggleGlowAndAnimationHoverMouseOverFunctions.Add(element, onMouseOverAction);

                // Out
                // - Clear
                if (_toggleGlowAndAnimationHoverMouseOutFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOutEvent>(_toggleGlowAndAnimationHoverMouseOutFunctions[element]);
                    _toggleGlowAndAnimationHoverMouseOutFunctions.Remove(element);
                }
                // - Create
                var onMouseOutAction = new EventCallback<MouseOutEvent>((MouseOutEvent evt) => onMouseOutForAnimation(_toggleAnimationHoverAnimations, evt, animationName, classNames));
                element.RegisterCallback<MouseOutEvent>(onMouseOutAction);
                _toggleGlowAndAnimationHoverMouseOutFunctions.Add(element, onMouseOutAction);
            }
        }

        public static void UnregisterToggleGlowAndAnimationOnHover<T>(UQueryBuilder<T> queryBuilder, string animationName, params string[] classNames) where T : VisualElement
        {
            var results = queryBuilder.Build();
            foreach (var element in results)
            {
                if (element == null)
                    continue;

                // Over
                // - Clear
                if (_toggleGlowAndAnimationHoverMouseOverFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOverEvent>(_toggleGlowAndAnimationHoverMouseOverFunctions[element]);
                    _toggleGlowAndAnimationHoverMouseOverFunctions.Remove(element);
                }
                
                // Out
                // - Clear
                if (_toggleGlowAndAnimationHoverMouseOutFunctions.ContainsKey(element))
                {
                    element.UnregisterCallback<MouseOutEvent>(_toggleGlowAndAnimationHoverMouseOutFunctions[element]);
                    _toggleGlowAndAnimationHoverMouseOutFunctions.Remove(element);
                }

                removeAnimation(_toggleAnimationHoverAnimations, animationName, classNames, element, deleteAnimation: true);
            }
        }

        static void onMouseOverForAnimation(Dictionary<VisualElement, IGlowAnimation> animationTable, MouseOverEvent evt, string animationName, string[] classNames)
        {
            var element = evt.target as VisualElement;
            if (element == null)
                return;

            addClasses(element, classNames);
            addAnimation(animationTable, animationName, classNames, element);
        }

        static void onMouseOutForAnimation(Dictionary<VisualElement, IGlowAnimation> animationTable, MouseOutEvent evt, string animationName, string[] classNames)
        {
            var element = evt.target as VisualElement;
            if (element == null)
                return;

            removeClasses(element, classNames);
            removeAnimation(animationTable, animationName, classNames, element, deleteAnimation: false);
        }

        static void addAnimation(Dictionary<VisualElement, IGlowAnimation> animationTable, string animationName, string[] classNames, VisualElement element)
        {
            var glow = element as Glow;

            // Animation
            if (glow != null)
            {
                // If it's a glow element then add the animation via the attribute
                glow.animationName = animationName;
            }
            else
            {
                // If it's a glow added via modifiers then add the animation to the modifier.
                animationTable.TryGetValue(element, out var animation);
                if (animation != null && animation.Name == animationName)
                {
                    // Resume if animation already exists
                    animation.Play();
                }
                else
                {
                    // Animation with different name? -> remove old animation
                    if (animation != null && animation.Name != animationName)
                    {
                        animation.RemoveFromManipulator();
                        animation = null;
                        if (animationTable.ContainsKey(element))
                            animationTable.Remove(element);
                    }

                    // Add new animation
                    if (animation == null)
                    {
                        if (GlowPanel.TryGetManipulator(element, out var manipulator))
                        {
                            animation = GlowAnimation.AddAnimationTo(animationName, manipulator);
                            if (animation != null)
                            {
                                if (animationTable.ContainsKey(element))
                                    animationTable.Remove(element);
                                animationTable.Add(element, animation);
                            }
                        }
                    }
                }
            }
        }

        static void removeAnimation(Dictionary<VisualElement, IGlowAnimation> animationTable, string animationName, string[] classNames, VisualElement element, bool deleteAnimation)
        {
            var glow = element as Glow;

            // Animation
            if (glow != null)
            {
                // If it's a glow element then add the animation via the attribute
                glow.animationName = null;
            }
            else
            {
                // Pause if animation exists
                animationTable.TryGetValue(element, out var animation);
                if (animation != null && animation.Name == animationName)
                {
                    animation.Pause();
                    if (deleteAnimation)
                    {
                        animation.RemoveFromManipulator();
                        if (animationTable.ContainsKey(element))
                            animationTable.Remove(element);
                    }
                    
                }
            }
        }
    }
}