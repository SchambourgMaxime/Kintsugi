using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ZoomControl : MonoBehaviour
{
    public event Action OnZoomPressed;
    public event Action OnZoomReleased;
    
    [SerializeField] private float maxZoom = -1f;
    [SerializeField] private List<Transform> cameraCenters;
    
    private void Start()
    {
        
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("ZoomSlider");
            slider.RegisterCallback<PointerDownEvent>(_ => OnZoomPressed?.Invoke());
            slider.RegisterCallback<PointerUpEvent>(_ => OnZoomReleased?.Invoke());
            slider.RegisterValueChangedCallback(evt =>
            {

            });
        }
    }
}
