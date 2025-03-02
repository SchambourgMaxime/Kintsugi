using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveControl : MonoBehaviour
{
    [SerializeField] private float maxHeight = .5f;
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
                    Vector3 pos = cameraCenters[i].position;
                    cameraCenters[i].position = new Vector3(pos.x, maxHeight * evt.newValue, pos.z);
                }
            });
        }
    }
}
