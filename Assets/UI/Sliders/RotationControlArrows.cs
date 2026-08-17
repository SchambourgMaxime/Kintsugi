using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationControlArrows : MonoBehaviour
{
    [SerializeField] private float rotationTime = 1f;
    [SerializeField] private List<Transform> cameraCenters;

    private float rotationTimeStart;
    private float rotationStart;
    private float rotationEnd;

    private void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            Button buttonLeft = doc.rootVisualElement.Q<Button>("ButtonLeft");
            Button buttonRight = doc.rootVisualElement.Q<Button>("ButtonRight");
            
            buttonLeft.RegisterCallback<PointerUpEvent>(evt => RotateView(1));
            buttonRight.RegisterCallback<PointerUpEvent>(evt => RotateView(-1));
        }
    }

    private void RotateView(int dir)
    {
        StartCoroutine(rotate_view_cr(dir));
    }

    private IEnumerator rotate_view_cr(int dir)
    {
        rotationStart     =  cameraCenters[0].eulerAngles.y;
        rotationEnd       = rotationStart + (90f * dir);
        rotationTimeStart =  Time.time;

        while (Time.time - rotationTimeStart < rotationTime)
        {
            float yaw = Mathf.Lerp(rotationStart, rotationEnd, (Time.time - rotationTimeStart) / rotationTime);
            SetRotation(yaw);
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetRotation(float rot)
    {
        foreach (Transform center in cameraCenters)
        {
            Vector3 euler = center.eulerAngles;
            center.eulerAngles = new Vector3(euler.x, rot, euler.z);
        }
    }
}
