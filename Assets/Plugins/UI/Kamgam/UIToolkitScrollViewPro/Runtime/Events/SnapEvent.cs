using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class SnapEvent : EventBase<SnapEvent>
    {
        public ScrollViewPro scrollView;

        public static void Dispatch(VisualElement target, ScrollViewPro scrollView)
        {
            using (SnapEvent evt = SnapEvent.GetPooled())
            {
                evt.scrollView = scrollView;
                evt.target = target;

                target.SendEvent(evt);
            }
        }
    }
}