using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitParticles
{
    public class DragManipulator : IManipulator
    {
        private VisualElement _target;
        private PickingMode _lastPickingMode;
        private bool _isDragging;
        private Vector3 _offset;

        public VisualElement target
        {
            get => _target;
            set
            {
                if (_target != null)
                {
                    if (_target == value)
                        return;

                    _target.UnregisterCallback<PointerDownEvent>(DragBegin);
                    _target.UnregisterCallback<PointerUpEvent>(DragEnd);
                    _target.UnregisterCallback<PointerMoveEvent>(PointerMove);
                }

                _target = value;

                _target.RegisterCallback<PointerDownEvent>(DragBegin);
                _target.RegisterCallback<PointerUpEvent>(DragEnd);
                _target.RegisterCallback<PointerMoveEvent>(PointerMove);
            }
        }

        private void DragBegin(PointerDownEvent ev)
        {
            _lastPickingMode = target.pickingMode;
            target.pickingMode = PickingMode.Ignore;

            _isDragging = true;
            _offset = ev.localPosition;

            target.CapturePointer(ev.pointerId);
        }

        private void DragEnd(IPointerEvent ev)
        {
            if (!_isDragging)
                return;

            target.ReleasePointer(ev.pointerId);
            target.pickingMode = _lastPickingMode;

            _isDragging = false;
        }

        private void PointerMove(PointerMoveEvent ev)
        {
            if (!_isDragging)
                return;

            Vector3 delta = ev.localPosition - _offset;
            target.transform.position += delta;
        }
    }
}
