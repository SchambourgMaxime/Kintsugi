using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SliderControl : MonoBehaviour
{
    public event Action OnPressed;
    public event Action OnReleased;
    
    [SerializeField] private float maxHeight = .5f;
    [SerializeField] private List<Transform> cameraCenters;

    private void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("RotationSlider");
            slider.RegisterCallback<PointerDownEvent>(_ => OnPressed?.Invoke());
            slider.RegisterCallback<PointerUpEvent>(_ => OnReleased?.Invoke());
            slider.RegisterValueChangedCallback(evt =>
            {
                foreach (Transform center in cameraCenters)
                {
                    Vector3 pos = center.position;
                    center.position = new Vector3(pos.x, maxHeight * evt.newValue, pos.z);
                }
            });
        }
    }
}
