using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Crab_Input inputDPad = null;

    // Controllers
    public Dictionary<int, CrabControl> idToControl = new();
    public List<GameObject> crabParts = new();

    private void Awake()
    {
        inputDPad = new Crab_Input();
    }


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


    // ########## Left stick callbacks ##########
    private void OnLeftStickPerformed(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

        Debug.Log("Controller ID: " + id);

        if (idToControl[id].leftVector.x == 0) idToControl[id].rotateTimer = 0; // Starting new rotation.

        idToControl[id].leftVector_prev = idToControl[id].leftVector;
        idToControl[id].leftVector = value.ReadValue<Vector2>();
    }

    private void OnLeftStickCancelled(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

        idToControl[id].leftVector_prev = idToControl[id].leftVector;
        idToControl[id].leftVector = Vector2.zero;
        idToControl[id].speedTimer = 0f;
        idToControl[id].rotateTimer = 0;
    }

    // ########## Right stick callbacks ##########
    private void OnRightStickPerformed(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

        idToControl[id].rightVector = value.ReadValue<Vector2>();
    }

    private void OnRightStickCancelled(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

        //Vector2 rightVector = value.ReadValue<Vector2>();
        //float newX = rightVector.x;
        //float newY = rightVector.y;

        idToControl[id].rightVector = Vector2.zero;
    }

    private void OnRTriggerPerformed(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

        if (!idToControl[id].crabParticle.isPlaying)
        {
            idToControl[id].crabParticle.Play();
            idToControl[id].isSpraying = true;
        }
        else
        {
            idToControl[id].crabParticle.Stop();
            idToControl[id].isSpraying = false;
        }
    }

    private void OnRTriggerCancelled(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

    }

    private void OnXButtonPerformed(InputAction.CallbackContext value)
    {
        var devices = InputSystem.devices.ToArray();
        foreach (InputDevice itr in devices) Debug.Log("Devices: " + itr.deviceId);

        if (!idToControl.ContainsKey(value.control.device.deviceId) &&
            crabParts.Count > 0)
        {
            GameObject crabGO = crabParts[0];
            crabParts.RemoveAt(0);
            idToControl.Add(value.control.device.deviceId, crabGO.GetComponent<CrabControl>());
        }

    }

    private void OnXButtonCancelled(InputAction.CallbackContext value)
    {
        int id;
        if (!idToControl.ContainsKey(value.control.device.deviceId)) return;
        else id = value.control.device.deviceId;

    }


}
