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

    private float baseDistance = 0f;
    private float endDistance = 0f;
    
    private void Start()
    {
        baseDistance = Vector3.Distance(cameraCenters[0].GetComponentInChildren<Camera>().transform.position,
            cameraCenters[0].transform.position);
        
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("ZoomSlider");
            slider.RegisterCallback<PointerDownEvent>(_ => OnZoomPressed?.Invoke());
            slider.RegisterCallback<PointerUpEvent>(_ => OnZoomReleased?.Invoke());
            slider.RegisterValueChangedCallback(evt =>
            {
                foreach (Transform center in cameraCenters)
                {
                    Vector3 camToCenter = (center.GetComponentInChildren<Camera>().transform.position -
                                           center.transform.position).normalized;
                    
                    float length = baseDistance - (maxZoom * evt.newValue);
                    Vector3 offset = length * camToCenter;
                    center.GetComponentInChildren<Camera>().transform.position =
                        center.transform.position + offset;
                }
            });
        }
    }
}
