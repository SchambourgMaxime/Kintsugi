using System.Collections;
using UnityEngine;

namespace VV.Utility
{
    public static class MonoBehaviourExtensions
    {
        public static void Invoke(this MonoBehaviour mb, System.Action f, float delay)
        {
            mb.StartCoroutine(InvokeRoutine(f, delay));
        }

        private static IEnumerator InvokeRoutine(System.Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }
        
        public static void DestroyAllChildren(this MonoBehaviour mb)
        {
            foreach (Transform child in mb.transform) { Object.Destroy(child.gameObject); }
        }
    }
}