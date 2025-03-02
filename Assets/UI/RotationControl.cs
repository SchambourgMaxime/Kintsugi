using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationControl : MonoBehaviour
{
    [SerializeField] private float maxRotation = 180;
    [SerializeField] private List<Transform> cameraCenters;

    private void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("RotationSlider");
            slider.RegisterValueChangedCallback(evt =>
            {
                for (int i = 0; i < cameraCenters.Count; i++)
                {
                    Vector3 euler = cameraCenters[i].eulerAngles;
                    cameraCenters[i].eulerAngles = new Vector3(euler.x, maxRotation * evt.newValue, euler.z);
                }
            });
        }
    }
}
