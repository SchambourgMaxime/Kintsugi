using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationControl : MonoBehaviour
{
    public event Action OnRotationPressed;
    public event Action OnRotationReleased;
    
    [SerializeField] private float maxRotation = 180;
    [SerializeField] private List<Transform> cameraCenters;

    private void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("RotationSlider");
            slider.RegisterCallback<PointerDownEvent>(_ => OnRotationPressed?.Invoke());
            slider.RegisterCallback<PointerUpEvent>(_ => OnRotationReleased?.Invoke());
            slider.RegisterValueChangedCallback(evt =>
            {
                foreach (Transform center in cameraCenters)
                {
                    Vector3 euler = center.eulerAngles;
                    center.eulerAngles = new Vector3(euler.x, maxRotation * evt.newValue, euler.z);
                }
            });
        }
    }
}
