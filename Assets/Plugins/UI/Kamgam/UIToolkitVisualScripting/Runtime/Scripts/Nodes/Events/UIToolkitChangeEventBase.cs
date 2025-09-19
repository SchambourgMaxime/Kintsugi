#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public abstract class UIToolkitChangeEventBase<T> : UIToolkitEventBase<T>
    {
        [DoNotSerialize]
        public ValueOutput previousValue;

        protected ChangeEvent<T> _lastEvent;

        protected override void Definition()
        {
            base.Definition();

            // TODO: Investigate if we can pass more data info EventBus.Trigger and
            // then receive it in AssignArguments. The current solution is not very nice.
            previousValue = ValueOutput<T>(nameof(previousValue), (flow) => {
                var value = _lastEvent != null ? _lastEvent.previousValue : default;
                flow.SetValue(previousValue, value);
                return value;
            });
        }

        protected virtual void triggerEvent(ChangeEvent<T> evt)
        {
            _lastEvent = evt;
            EventBus.Trigger(getHookName(), evt.newValue);
        }

        protected override void AssignArguments(Flow flow, T data)
        {
            flow.SetValue(result, data);
        }
    }
}
#endif