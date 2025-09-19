#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    [IncludeInSettings(include: false)]
    /// <summary>
    /// A wrapper for pointer events.
    /// </summary>
    public class PointerEventWrapper : EventBase<PointerEventWrapper>
    {
        public IPointerEvent PointerEvent;

        public int pointerId => PointerEvent != null ? PointerEvent.pointerId : default;

        public string pointerType => PointerEvent != null ? PointerEvent.pointerType : default;

        public bool isPrimary => PointerEvent != null ? PointerEvent.isPrimary : default;

        public int button => PointerEvent != null ? PointerEvent.button : default;

        public int pressedButtons => PointerEvent != null ? PointerEvent.pressedButtons : default;

        public Vector3 position => PointerEvent != null ? PointerEvent.position : default;

        public Vector3 localPosition => PointerEvent != null ? PointerEvent.localPosition : default;

        public Vector3 deltaPosition => PointerEvent != null ? PointerEvent.deltaPosition : default;

        public float deltaTime => PointerEvent != null ? PointerEvent.deltaTime : default;

        public int clickCount => PointerEvent != null ? PointerEvent.clickCount : default;

        public float pressure => PointerEvent != null ? PointerEvent.pressure : default;

        public float tangentialPressure => PointerEvent != null ? PointerEvent.tangentialPressure : default;

        public float altitudeAngle => PointerEvent != null ? PointerEvent.altitudeAngle : default;

        public float azimuthAngle => PointerEvent != null ? PointerEvent.azimuthAngle : default;

        public float twist => PointerEvent != null ? PointerEvent.twist : default;

        public Vector2 radius => PointerEvent != null ? PointerEvent.radius : default;

        public Vector2 radiusVariance => PointerEvent != null ? PointerEvent.radiusVariance : default;

        public EventModifiers modifiers => PointerEvent != null ? PointerEvent.modifiers : default;

        public bool shiftKey => PointerEvent != null ? PointerEvent.shiftKey : default;

        public bool ctrlKey => PointerEvent != null ? PointerEvent.ctrlKey : default;

        public bool commandKey => PointerEvent != null ? PointerEvent.commandKey : default;

        public bool altKey => PointerEvent != null ? PointerEvent.altKey : default;

        public bool actionKey => PointerEvent != null ? PointerEvent.actionKey : default;
    }
}
#endif