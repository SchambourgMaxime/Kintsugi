using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public static event EventHandler<Vector2> OnPointerDown;
    public static event EventHandler<Vector2> OnPointerPressed;
    public static event EventHandler<Vector2> OnPointerUp;
        
    public InputManager instance { get; private set; }
    
    private static Vector2 _previousPointerPos = Vector2.negativeInfinity;
    private static Vector2 _pointerPos;
    private bool isPointerPressed;
    private bool isPointerMouse;

    public static Vector2 pointerDelta => _pointerPos - _previousPointerPos;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePointer();
        HandleEvents();
    }

    private void UpdatePointer()
    {
        isPointerMouse = Input.GetMouseButtonDown((int)MouseButton.LeftMouse) ||
                         Input.GetMouseButton((int)MouseButton.LeftMouse) ||
                         Input.GetMouseButtonUp((int)MouseButton.LeftMouse);
        isPointerPressed = Input.touches.Length > 0 || isPointerMouse;

        if (!isPointerPressed) return;
        
        // Updating values
        _previousPointerPos = _pointerPos;
        _pointerPos = !isPointerMouse ? Input.touches[0].position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    
    private void HandleEvents()
    {
        if (!isPointerPressed) return;
        
        EventHandler<Vector2> ev = null;
        if(!isPointerMouse)
        {
            if (Input.touches[0].phase == TouchPhase.Began) ev = OnPointerDown;
            else if (Input.touches[0].phase == TouchPhase.Ended) ev = OnPointerUp;
            else ev = OnPointerPressed;
        }
        else
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) ev = OnPointerDown;
            else if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse)) ev = OnPointerUp;
            else ev = OnPointerPressed;
        }
        
        ev?.Invoke(this, _pointerPos);
    }
}
