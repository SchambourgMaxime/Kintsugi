using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<Transform> cameraCenters;
    
    private float baseDistance = 0f;
    private float endDistance = 0f;
    
    [SerializeField] private float maxZoom = -1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseDistance = Vector3.Distance(cameraCenters[0].GetComponentInChildren<Camera>().transform.position,
            cameraCenters[0].transform.position);
    }

    public void RotateCameras(float value = 1f)
    {
        foreach (Transform center in cameraCenters)
        {
            Vector3 camToCenter = (center.GetComponentInChildren<Camera>().transform.position -
                                   center.transform.position).normalized;
                    
            float length = baseDistance - (maxZoom * value);
            Vector3 offset = length * camToCenter;
            center.GetComponentInChildren<Camera>().transform.position =
                center.transform.position + offset;
        }
    }
}
