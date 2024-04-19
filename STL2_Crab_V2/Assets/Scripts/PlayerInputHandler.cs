using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private CrabControl crabControl;
    private GamepadCursor gamepadCursor;
    public int index;

    // private Crab_Input inputDPad = null;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        gamepadCursor = GetComponent<GamepadCursor>();
        var controllers = FindObjectsOfType<CrabControl>();
        foreach (CrabControl itr in controllers) Debug.Log("crab controllers: " + itr.name);
        index = playerInput.playerIndex;
        crabControl = controllers.FirstOrDefault(m => m.GetPlayerIndex() == index);

        Debug.Log(index);
        // inputDPad = new Crab_Input();
    }

    /**
    // Subscribe
    private void OnEnable()
    {
        inputDPad.Enable();
        // Left stick
        inputDPad.PlayerContol.LeftStick.performed += OnLeftStickPerformed;
        inputDPad.PlayerContol.LeftStick.canceled += OnLeftStickCancelled;
        // Right stick
        inputDPad.PlayerContol.RightStick.performed += OnRightStickPerformed;
        inputDPad.PlayerContol.RightStick.canceled += OnRightStickCancelled;
        // Right trigger (spray)
        inputDPad.PlayerContol.Particle.performed += OnRTriggerPerformed;
        inputDPad.PlayerContol.Particle.canceled += OnRTriggerCancelled;
        //
        inputDPad.PlayerContol.X_Button.performed += OnXButtonPerformed;
        inputDPad.PlayerContol.X_Button.canceled += OnXButtonCancelled;
    }

    // Unsubscribe
    private void OnDisable()
    {
        inputDPad.Disable();
        // Left stick
        inputDPad.PlayerContol.LeftStick.performed -= OnLeftStickPerformed;
        inputDPad.PlayerContol.LeftStick.canceled -= OnLeftStickCancelled;
        // Right stick
        inputDPad.PlayerContol.RightStick.performed -= OnRightStickPerformed;
        inputDPad.PlayerContol.RightStick.canceled -= OnRightStickCancelled;
        // Right trigger (spray)
        inputDPad.PlayerContol.Particle.performed -= OnRTriggerPerformed;
        inputDPad.PlayerContol.Particle.canceled -= OnRTriggerCancelled;
        //
        inputDPad.PlayerContol.X_Button.performed -= OnXButtonPerformed;
        inputDPad.PlayerContol.X_Button.canceled -= OnXButtonCancelled;
    }
    **/


    // ########## Left stick callbacks ##########
    public void OnLeftStickPerformed(InputAction.CallbackContext value)
    {
        //Debug.Log("Left stick performed AAA");
        if (crabControl == null) return;
        //Debug.Log("Left stick performed BBB");

        if (crabControl.leftVector.x == 0) crabControl.rotateTimer = 0; // Starting new rotation.
        crabControl.leftVector_prev = crabControl.leftVector;
        crabControl.leftVector = value.ReadValue<Vector2>();
    }

    public void OnLeftStickCancelled(InputAction.CallbackContext value)
    {
        //Debug.Log("Left stick cancelled AAA");
        if (crabControl == null) return;
        //Debug.Log("Left stick cancelled BBB");

        crabControl.leftVector_prev = crabControl.leftVector;
        crabControl.leftVector = Vector2.zero;
        crabControl.speedTimer = 0f;
        crabControl.rotateTimer = 0;
    }

    // ########## Right stick callbacks ##########
    public void OnRightStickPerformed(InputAction.CallbackContext value)
    {
        if (crabControl == null) return;

        crabControl.rightVector = value.ReadValue<Vector2>();
        gamepadCursor.rightStickValue = value.ReadValue<Vector2>();
    }

    public void OnRightStickCancelled(InputAction.CallbackContext value)
    {
        if (crabControl == null) return;

        //Vector2 rightVector = value.ReadValue<Vector2>();
        //float newX = rightVector.x;
        //float newY = rightVector.y;

        crabControl.rightVector = Vector2.zero;
    }

    public void OnRTriggerPerformed(InputAction.CallbackContext value)
    {
        if (crabControl == null) return;

        if (!crabControl.crabParticle.isPlaying)
        {
            crabControl.crabParticle.Play();
            crabControl.isSpraying = true;
        }
        else
        {
            crabControl.crabParticle.Stop();
            crabControl.isSpraying = false;
        }
    }

    public void OnRTriggerCancelled(InputAction.CallbackContext value)
    {


    }

    public void OnXButtonPerformed(InputAction.CallbackContext value)
    {
        /**
        var devices = InputSystem.devices.ToArray();
        foreach (InputDevice itr in devices) Debug.Log("Devices: " + itr.deviceId);

        if (!idToControl.ContainsKey(value.control.device.deviceId) &&
            crabParts.Count > 0)
        {
            GameObject crabGO = crabParts[0];
            crabParts.RemoveAt(0);
            idToControl.Add(value.control.device.deviceId, crabGO.GetComponent<CrabControl>());
        }
        **/

    }

    public void OnXButtonCancelled(InputAction.CallbackContext value)
    {


    }
    
    public void OnLeftShoulderPerformed(InputAction.CallbackContext context)
    {
        if (crabControl == null) return;
    
        if (context.performed)
        {
            Debug.Log("Performed Left Bumper");
            crabControl.radialMenu.GetComponent<RadialMenu>().Open();
            crabControl.rightStickRotateCamera = false;
            crabControl.cursor.SetActive(true);
        }
    
        if (context.canceled) 
        {
            Debug.Log("Cancelled Left Bumper");
            crabControl.radialMenu.GetComponent<RadialMenu>().Close();
            crabControl.rightStickRotateCamera = true;
            crabControl.cursor.SetActive(false);
        }
    }


}
