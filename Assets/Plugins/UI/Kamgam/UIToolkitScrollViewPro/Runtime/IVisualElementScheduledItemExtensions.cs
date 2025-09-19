using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace Kamgam.UIToolkitScrollViewPro
{
    public static class IVisualElementSchedulerExtensions
    {
        public class Condition
        {
            public Func<bool> Predicate;
            public Action Action;

            public bool Update()
            {
                if (Predicate())
                {
                    try
                    {
                        Action();
                    }
                    finally
                    {
                        Release();
                    }
                    return true;
                }

                return false;
            }

            public void Release()
            {
                Predicate = null;
                Action = null;
            }

            public bool IsReleased => Predicate == null;
        }

        static List<Condition> pool = new List<Condition>();

        static Condition getFromPool(System.Func<bool> predicate, System.Action action)
        {
            // Reuse
            int count = pool.Count;
            for (int i = 0; i < count; i++)
            {
                if (pool[i].IsReleased)
                {
                    pool[i].Predicate = predicate;
                    pool[i].Action = action;
                    return pool[i];
                }
            }

            // New
            var cond = new Condition()
            {
                Predicate = predicate,
                Action = action
            };
            pool.Add(cond);
            return cond;
        }


        static void doNothing() { }

        public static IVisualElementScheduledItem When(this IVisualElementScheduler scheduler, System.Func<bool> predicate, System.Action action)
        {
            long frameRateInMS = Mathf.FloorToInt(1000 / Mathf.Max(30, Application.targetFrameRate));
            return scheduler.Execute(doNothing).Every(frameRateInMS).Until(getFromPool(predicate, action).Update);
        }
    }
}
