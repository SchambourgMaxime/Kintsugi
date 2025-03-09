using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveControl : MonoBehaviour
{
    public event Action OnMovePressed;
    public event Action OnMoveReleased;
    
    [SerializeField] private float maxHeight = .5f;
    [SerializeField] private List<Transform> cameraCenters;

    private void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            Slider slider = doc.rootVisualElement.Q<Slider>("MoveSlider");
            slider.RegisterCallback<PointerDownEvent>(_ => OnMovePressed?.Invoke());
            slider.RegisterCallback<PointerUpEvent>(_ => OnMoveReleased?.Invoke());
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
