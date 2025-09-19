#if KAMGAM_VISUAL_SCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public abstract class UIToolkitEventBase<T> : EventUnit<T>
    {
        [DoNotSerialize]
        public ValueOutput result;

        [DoNotSerialize]
        public ValueOutput target;

        protected VisualElement _cachedVisualElement;

        [DoNotSerialize, AllowsNull]
        public ValueInput element;

        [DoNotSerialize]
        public ControlInput registerCallbacksIn;

        [DoNotSerialize]
        public ControlOutput registerCallbacksOut;

        protected override bool register => true;

        protected abstract string getHookName();
		
		protected string _uniqueHookName;

        protected string getUniqueHookName()
        {
            if(string.IsNullOrEmpty(_uniqueHookName))
                _uniqueHookName = getHookName() + guid.ToString(); ;

            return _uniqueHookName;
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(getUniqueHookName());
        }

        protected override void Definition()
        {
            registerCallbacksOut = ControlOutput("Registered");

            base.Definition();

            // Setting the value on our port.
            result = ValueOutput<T>(nameof(result));

            element = ValueInput<VisualElement>(nameof(element), null).AllowsNull();

            registerCallbacksIn = ControlInput("Register Callbacks", inputAction);
        }

        protected virtual ControlOutput inputAction(Flow flow)
        {
            var newTarget = flow.GetValue<VisualElement>(element);
            registerEvents(newTarget);

            return registerCallbacksOut;
        }

        public override void StartListening(GraphStack stack)
        {
            var newTarget = Flow.FetchValue<VisualElement>(element, stack.ToReference());
            registerEvents(newTarget);

            base.StartListening(stack);
        }

        protected virtual ControlOutput registerEvents(VisualElement element)
        {
            if (!register)
                return null;

            // unregister from old element
            if (_cachedVisualElement != null)
            {
                unregisterCallbacks(_cachedVisualElement);
            }

            _cachedVisualElement = element;

            // register to new element
            if (_cachedVisualElement != null)
            {
                registerCallbacks(_cachedVisualElement);
            }

            return null;
        }

        protected abstract void registerCallbacks(VisualElement ve);
        protected abstract void unregisterCallbacks(VisualElement ve);
        protected virtual void triggerEvent(T data)
        {
            EventBus.Trigger(getUniqueHookName(), data);
        }

        protected virtual void triggerEvent()
        {
            EventBus.Trigger(getUniqueHookName());
        }

        protected override void AssignArguments(Flow flow, T data)
        {
            flow.SetValue(result, data);
        }
    }
}
#endif