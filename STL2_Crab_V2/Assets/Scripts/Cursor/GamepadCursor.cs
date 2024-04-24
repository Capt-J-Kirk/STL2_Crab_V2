using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
public class GamepadCursor : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private float padding = 50f;
    
    public Vector2 rightStickValue = Vector2.zero;

    private PlayerInputHandler playerInputHandler;

    private bool previousMouseState;
    private Mouse virtualMouse;
    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputHandler = GetComponent<PlayerInputHandler>();

        if (playerInputHandler.index == 0)
        {
            cursorTransform = GameObject.Find("CursorLeft").GetComponent<RectTransform>();
            canvas = GameObject.Find("LeftCanvas").GetComponent<Canvas>();
            canvasRectTransform = GameObject.Find("LeftCanvas").GetComponent<RectTransform>();
        }
        else if (playerInputHandler.index == 1)
        {
            cursorTransform = GameObject.Find("CursorRight").GetComponent<RectTransform>();
            canvas = GameObject.Find("RightCanvas").GetComponent<Canvas>();
            canvasRectTransform = GameObject.Find("RightCanvas").GetComponent<RectTransform>();
        }

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if(!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }
        
        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = rightStickValue;
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width-padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height-padding);
        
        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool aButtonIsPressed2 = Gamepad.current.aButton.IsPressed();
        bool aButtonIsPressed = Gamepad.current.buttonSouth.IsPressed();

        //Debug.Log(aButtonIsPressed);

        if (previousMouseState !=  aButtonIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIsPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, null, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }
}
