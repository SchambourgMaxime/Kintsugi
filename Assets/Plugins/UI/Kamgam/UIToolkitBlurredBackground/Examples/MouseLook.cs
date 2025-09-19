using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamgam.UIToolkitBlurredBackground
{
    public class MouseLook : MonoBehaviour
    {
        private float mouseSensitivity = 100.0f;
        private float clampAngle = 80.0f;

        private float rotX = 0.0f; // rotation around the right/x axis
        private float rotY = 0.0f; // rotation around the up/y axis

        private float startRotX = 0.0f; // rotation around the right/x axis

        void Start()
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            startRotX = rot.x;
            rotY = rot.y;
            rotX = rot.x;
        }

        void Update()
        {

            if (Time.realtimeSinceStartup > 3f)
            {

#if ENABLE_INPUT_SYSTEM
                float mouseX = Mouse.current.delta.x.ReadValue() * 0.3f;
                float mouseY = -Mouse.current.delta.y.ReadValue() * 0.3f;
#else
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");
#endif

                float dY = mouseX * mouseSensitivity * Time.deltaTime;
                float dX = mouseY * mouseSensitivity * Time.deltaTime;

                rotY += dY;
                rotX += dX;

                rotX = Mathf.Clamp(rotX, startRotX - clampAngle, startRotX + clampAngle);

                Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
                transform.rotation = localRotation;
            }
        }
    }
}