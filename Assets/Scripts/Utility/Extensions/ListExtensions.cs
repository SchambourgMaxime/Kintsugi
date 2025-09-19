using System;
using System.Collections.Generic;
using System.Linq;

namespace VV.Utility
{
    public static class ListExtensions
    {
        public static void AddUnique<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
        }

        public static void AddUnique<T>(this IList<T> list, T item)
        {
            AddUnique(list, new[] { item });
        }
        
        public static T GetRandom<T>(this List<T> list, List<T> excludedItems = null)
        {
            Random rng = new Random();
            if (excludedItems != null)
            {
                List<T> excludedList = list.Except(excludedItems).ToList();
                return excludedList[rng.Next(list.Count)];
            }

            return list[rng.Next(list.Count)];
        }
    }
}