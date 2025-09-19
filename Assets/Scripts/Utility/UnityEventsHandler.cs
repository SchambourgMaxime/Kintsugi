using System;
using UnityEngine;
using UnityEngine.Events;

namespace VV.Utility
{
    public class UnityEventsHandler : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onDestroy;
        
        private void Awake() => onAwake?.Invoke(); 
        private void Start() => onStart?.Invoke();
        private void OnDestroy() => onDestroy?.Invoke(); 
    }
}