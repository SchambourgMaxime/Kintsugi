using System.Collections.Generic;
using UnityEngine.Events;

namespace VV.Utility
{
    public class MultiEventListener
    {
        private readonly HashSet<string> triggeredEvents = new();
        private readonly HashSet<string> requiredEvents;
        private readonly UnityAction onAllEventsTriggered;

        public MultiEventListener(IEnumerable<string> eventNames, UnityAction onAllEventsTriggered)
        {
            requiredEvents = new HashSet<string>(eventNames);
            this.onAllEventsTriggered = onAllEventsTriggered;
        }

        public void Trigger(string eventName)
        {
            if (requiredEvents.Contains(eventName))
            {
                triggeredEvents.Add(eventName);
                if (triggeredEvents.SetEquals(requiredEvents))
                {
                    onAllEventsTriggered?.Invoke();
                }
            }
        }
    }
}