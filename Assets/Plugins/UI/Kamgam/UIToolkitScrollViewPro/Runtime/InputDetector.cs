using System;
using System.Linq;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#endif

namespace Kamgam.UIToolkitScrollViewPro
{
    public class InputDetector
#if !ENABLE_INPUT_SYSTEM
         : MonoBehaviour
#else
         : IObserver<InputEventPtr>
#endif
    {
        public enum InputDevice
        {
            /// <summary>
            /// Mouse or other pointer event triggering devices.
            /// </summary>
            Pointer,
            Touch,
            Keyboard,
            Controller,
        }

        /// <summary>
        /// Info about what the last used input device was.
        /// </summary>
        public static InputDevice LastInputDevice
        {
            get => instance._lastInputDevice;
        }

        /// <summary>
        /// Returns true if the last used input device is in the list of devices.
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static bool IsUsing(params InputDevice[] devices)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i] == LastInputDevice)
                    return true;
            }

            return false;
        }

        protected InputDevice _lastInputDevice;

        protected InputDevice getDefaultInputDevice()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return InputDevice.Pointer;
#else
            return InputDevice.Touch;
#endif
        }


#if ENABLE_INPUT_SYSTEM

        // NEW INPUT SYSTEM

        static InputDetector _instance;
        static InputDetector instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("new instance");
                    _instance = new InputDetector();
                    _instance.Init();
                }

                return _instance;
            }
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void create()
        {
            if (_instance == null)
            {
                var _ = instance;
            }
        }

        public void Init()
        {
            _lastInputDevice = getDefaultInputDevice();
            InputSystem.onEvent.Subscribe(this);
        }

        private bool onInputEvent(InputEventPtr evtPtr, InputDevice device)
        {
            return true;
        }

        public void OnCompleted() {}
        public void OnError(Exception error) {}

        public void OnNext(InputEventPtr value)
        {
            for (int i = 0; i < InputSystem.devices.Count; i++)
            {
                if (InputSystem.devices[i].deviceId == value.deviceId)
                {
                    var device = InputSystem.devices[i];

                    // Thanks to:
                    // https://forum.unity.com/threads/detect-most-recent-input-device-type.753206/#post-9167348

                    // Some devices like to spam events like crazy.
                    // Example: PS4 controller on PC keeps triggering events without meaningful change.
                    var eventType = value.type;
                    if (eventType == StateEvent.Type)
                    {
                        // Go through the changed controls in the event and look for ones actuated
                        // above a magnitude of a little above zero.
                        if (!value.EnumerateChangedControls(device: device, magnitudeThreshold: 0.0001f).Any())
                            continue;
                    }

                    if (device == Mouse.current)
                    {
                        _lastInputDevice = InputDevice.Pointer;
                    }
                    else if (device == Keyboard.current)
                    {
                        _lastInputDevice = InputDevice.Keyboard;
                    }
                    else if (device == Gamepad.current)
                    {
                        _lastInputDevice = InputDevice.Controller;
                    }
                    else
                    {
                        // Guess it's touch.
                        _lastInputDevice = InputDevice.Touch;
                    }
                    break;
                }

            }
            
        }

#else

        // OLD INPUT SYSTEM

        static InputDetector _instance;
        static InputDetector instance
        {
            get
            {
                if (_instance == null || _instance._isDestroyed)
                {
                    var go = new GameObject("UITK ScrollView: InputDetector");
                    _instance = go.AddComponent<InputDetector>();
                    _instance.hideFlags = HideFlags.HideAndDontSave;

                    DontDestroyOnLoad(go);

                    _instance.Init();
                }

                return _instance;
            }
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void create()
        {
            if (_instance == null)
            {
                var _ = instance;
            }
        }

        protected bool _isDestroyed;

        public void Init()
        {
            _lastInputDevice = getDefaultInputDevice();
        }

        public void Update()
        {
            Detect();
        }

        protected Vector3 _lastMousePos;

        const string HorizontalAxisName = "Horizontal";
        const string VerticalAxisName = "Vertical";

        public void Detect()
        {
            if (Input.touchSupported && Input.touchCount > 0)
            {
                _lastInputDevice = InputDevice.Touch;
            }
            else if (
                   Input.mousePresent && (
                   Input.GetMouseButton(0)
                || Input.GetMouseButton(1)
                || Input.GetMouseButton(2)
                || Input.GetMouseButton(3)
                || (Input.mousePosition - _lastMousePos).sqrMagnitude > 1f
                || Input.mouseScrollDelta.sqrMagnitude > 0.01f
                )
                )
            {
                _lastMousePos = Input.mousePosition;
                _lastInputDevice = InputDevice.Pointer;
            }
            else if (
                   Input.GetKey(KeyCode.JoystickButton0)
                || Input.GetKey(KeyCode.JoystickButton1)
                || Input.GetKey(KeyCode.JoystickButton2)
                || Input.GetKey(KeyCode.JoystickButton3)
                || Input.GetKey(KeyCode.JoystickButton4)
                || Input.GetKey(KeyCode.JoystickButton5)
                || Input.GetKey(KeyCode.JoystickButton6)
                || Input.GetKey(KeyCode.JoystickButton7)
                || Input.GetKey(KeyCode.JoystickButton8)
                || Input.GetKey(KeyCode.JoystickButton9)
                || Input.GetKey(KeyCode.JoystickButton10)
                || Input.GetKey(KeyCode.JoystickButton11)
                || Input.GetKey(KeyCode.JoystickButton12)
                || Input.GetKey(KeyCode.JoystickButton13)
                || Input.GetKey(KeyCode.JoystickButton14)
                || Input.GetKey(KeyCode.JoystickButton15)
                || Input.GetKey(KeyCode.JoystickButton16)
                || Input.GetKey(KeyCode.JoystickButton17)
                || Input.GetKey(KeyCode.JoystickButton18)
                || Input.GetKey(KeyCode.JoystickButton19)
                )
            {
                _lastInputDevice = InputDevice.Controller;
            }
            else if (Input.anyKey)
            {
                _lastInputDevice = InputDevice.Keyboard;
            }
            else
            {
                // Thumb stick detection.
                if (Mathf.Abs(Input.GetAxisRaw(HorizontalAxisName)) > 0.2f
                    || Mathf.Abs(Input.GetAxisRaw(VerticalAxisName)) > 0.2f)
                {
                    _lastInputDevice = InputDevice.Controller;
                }
            }
        }

        public void OnDestroy()
        {
            _isDestroyed = true;
        }
#endif
    }
}
