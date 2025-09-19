using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitGlow
{
    /// <summary>
    /// A base manipulator that keeps a list of active manipulators and exposes various events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ManipulatorBase<T> : Manipulator
        where T : ManipulatorBase<T>
    {
        /// <summary>
        /// All manipulators that are linked to an element AND whose 'target' element is currently attached to a panel.
        /// </summary>
        public static List<T> ActiveManipulators = new List<T>();

        public event Action<T> OnRegisterCallbacksOnTarget;
        public event Action<T> OnUnregisterCallbacksOnTarget;

        public event Action<T, AttachToPanelEvent> OnElementAttachToPanel;
        public event Action<T, DetachFromPanelEvent> OnElementDetachFromPanel;

        protected override void RegisterCallbacksOnTarget()
        {
            addToManipulators();

            target.UnregisterCallback<AttachToPanelEvent>(onAttach);
            target.RegisterCallback<AttachToPanelEvent>(onAttach);

            target.UnregisterCallback<DetachFromPanelEvent>(onDetached);
            target.RegisterCallback<DetachFromPanelEvent>(onDetached);

            OnRegisterCallbacksOnTarget?.Invoke((T)this);
        }

        protected virtual void onAttach(AttachToPanelEvent evt)
        {
            addToManipulators();
            OnElementAttachToPanel?.Invoke((T)this, evt);
        }

        protected virtual void onDetached(DetachFromPanelEvent evt)
        {
            removeFromManipulators();
            OnElementDetachFromPanel?.Invoke((T)this, evt);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(onAttach);
            target.UnregisterCallback<DetachFromPanelEvent>(onDetached);

            removeFromManipulators();

            OnUnregisterCallbacksOnTarget?.Invoke((T)this);
        }

        protected void addToManipulators()
        {
            if (!ActiveManipulators.Contains((T)this))
            {
                ActiveManipulators.Add((T)this);
            }
        }

        protected void removeFromManipulators()
        {
            if (ActiveManipulators.Contains((T)this))
            {
                ActiveManipulators.Remove((T)this);
            }
        }

        public static T GetManipulatorOnElement(VisualElement element)
        {
            foreach (var m in ActiveManipulators)
            {
                if (m != null && m.target != null && m.target == element)
                    return m;
            }

            return null;
        }
    }
}