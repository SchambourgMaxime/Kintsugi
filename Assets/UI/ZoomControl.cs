using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ZoomControl : MonoBehaviour
{
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
            Slider slider = doc.rootVisualElement.Q<Slider>("RotationSlider");
            slider.RegisterValueChangedCallback(evt =>
            {
                for (int i = 0; i < cameraCenters.Count; i++)
                {
                    Vector3 camToCenter = (cameraCenters[i].GetComponentInChildren<Camera>().transform.position -
                                           cameraCenters[i].transform.position).normalized;
                    
                        float length = baseDistance - (maxZoom * evt.newValue);
                        Vector3 offset = length * camToCenter;
                        cameraCenters[i].GetComponentInChildren<Camera>().transform.position =
                            cameraCenters[i].transform.position + offset;
                }
            });
        }
    }
}
