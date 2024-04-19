using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.GraphicsBuffer;

public class CrabControl_Backup : MonoBehaviour
{
    private Crab_Input inputDPad = null;

    // Game pad variables
    [SerializeField] private Vector2 leftVector = Vector2.zero;
    [SerializeField] private Vector2 leftVector_prev = Vector2.zero;
    [SerializeField] private Vector2 rightVector = Vector2.zero;

    // Camera
    public Camera camera;
    private Quaternion camStartRot;
    public Vector2 camRotaton;

    // Crab body
    public enum Sides { Left, Right };
    [SerializeField] Sides crabPart = new Sides();
    private Rigidbody rb;
    public GameObject crabLeft;
    public GameObject crabRight;
    public GameObject partToMove;
    // Crab rotation axes
    public GameObject frontAxisObject;
    public GameObject rearAxisObject;

    // Max speeds and timers
    public float speedTimer = 0;
    public float rotateTimer = 0;
    float maxSpeed = 2;                // Units per second.
    public float maxAngularSpeed = 20;  // Degrees per second.

    // Controller
    private bool gotControllerID = false;
    public int controllerID = 0;
    private float direction = 0;


    // Particle system
    [SerializeField] private ParticleSystem crabParticle;
    [HideInInspector] public bool isSpraying = false;


    private void Awake()
    {
        inputDPad = new Crab_Input();
        var main = crabParticle.main;
        main.loop = true;

        if (crabPart == Sides.Left)
        {
            partToMove = crabLeft;
            direction = -1;
        }
        if (crabPart == Sides.Right)
        {
            partToMove = crabRight;
            direction = 1;
        }

        rb = partToMove.GetComponent<Rigidbody>();

        camera = GetComponentInChildren<Camera>();
        camStartRot = camera.transform.rotation;
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

    private void FixedUpdate()
    {
        if (crabPart != Sides.Left) return;

        Move();
        RotateCamera();
    }

    private void Move()
    {
        Rotate();
        ChangeVelocity();
        //ApplyDrag();
    }

    private void Rotate()
    {
        float maxRadPerSec = maxAngularSpeed * Mathf.PI / 180;
        float timeToMaxAngularSpeed = 1;


        // Rotate around center (with capping of rotation speed)
        //if (Mathf.Abs(leftVector.x) < 0.2f) leftVector.x = 0f;
        //rb.AddRelativeTorque(new Vector3(0f, leftVector.x * (rotateTimer / timeToMaxAngularSpeed), 0f), ForceMode.Force);

        // Capping angular y_velocity
        //if (Mathf.Abs(rb.angularVelocity.y) > maxRadPerSec) rb.angularVelocity = new Vector3(rb.angularVelocity.x, maxRadPerSec * (rb.angularVelocity.y / Mathf.Abs(rb.angularVelocity.y)), rb.angularVelocity.z);

        //rotateTimer += Time.deltaTime;
        //if (rotateTimer > timeToMaxAngularSpeed) rotateTimer = timeToMaxAngularSpeed;

        // Rotate around some axis
        //partToMove.transform.RotateAround(frontAxisObject.transform.position, Vector3.up, maxAngularSpeed * Time.deltaTime * leftVector.x);
        partToMove.transform.RotateAround(partToMove.transform.position, Vector3.up, maxAngularSpeed * Time.deltaTime * leftVector.x);



    }

    private void ChangeVelocity()
    {
        float speedChangeTime = 0.5f;
        if (Mathf.Sign(leftVector_prev.y) != Mathf.Sign(leftVector.y)) speedChangeTime *= 2;

        // Current velocity
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        float local_x = Mathf.Lerp(leftVector_prev.y * maxSpeed, leftVector.y * maxSpeed, Mathf.Pow(speedTimer / speedChangeTime, 1.5f));
        localVelocity.x = local_x;

        // Cap speed
        if (Mathf.Abs(localVelocity.x) > maxSpeed) localVelocity = new Vector3(
            maxSpeed * Mathf.Sign(localVelocity.x),
            localVelocity.y,
            localVelocity.z
            );
        if (Mathf.Abs(localVelocity.y) > maxSpeed) localVelocity = new Vector3(
            localVelocity.x,
            maxSpeed * Mathf.Sign(localVelocity.y),
            localVelocity.z
            );
        if (Mathf.Abs(localVelocity.z) > maxSpeed) localVelocity = new Vector3(
            localVelocity.x,
            localVelocity.y,
            maxSpeed * Mathf.Sign(localVelocity.z)
            );

        // Apply new velocity
        rb.velocity = direction * transform.TransformDirection(localVelocity);

        speedTimer += Time.deltaTime;
        if (speedTimer > speedChangeTime) speedTimer = speedChangeTime;
    }

    private void ApplyDrag()
    {
        if (rb.velocity.sqrMagnitude > 0)
        {
            rb.AddForce(0.01f * -rb.velocity, ForceMode.VelocityChange);
        }
    }

    private void RotateCamera()
    {
        // Yaw
        camera.transform.RotateAround(camera.transform.position, Vector3.up, maxAngularSpeed * Time.deltaTime * rightVector.x);

        // Pitch
        camera.transform.RotateAround(camera.transform.position, camera.transform.right, maxAngularSpeed * Time.deltaTime * rightVector.y);
    }



    // ########## Left stick callbacks ##########
    private void OnLeftStickPerformed(InputAction.CallbackContext value)
    {

        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

        Debug.Log("Controller ID: " + value.control.device.deviceId);

        if (leftVector.x == 0) rotateTimer = 0; // Starting new rotation.

        leftVector_prev = leftVector;
        leftVector = value.ReadValue<Vector2>();
    }

    private void OnLeftStickCancelled(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

        leftVector_prev = leftVector;
        leftVector = Vector2.zero;
        speedTimer = 0f;
        rotateTimer = 0;
    }

    // ########## Right stick callbacks ##########
    private void OnRightStickPerformed(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

        rightVector = value.ReadValue<Vector2>();
    }

    private void OnRightStickCancelled(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

        //Vector2 rightVector = value.ReadValue<Vector2>();
        //float newX = rightVector.x;
        //float newY = rightVector.y;

        rightVector = Vector2.zero;
    }

    private void OnRTriggerPerformed(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

        if (!crabParticle.isPlaying)
        {
            crabParticle.Play();
            isSpraying = true;
        }
        else
        {
            crabParticle.Stop();
            isSpraying = false;
        }
    }

    private void OnRTriggerCancelled(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

    }

    private void OnXButtonPerformed(InputAction.CallbackContext value)
    {
        Debug.Log("Controller ID: " + value.control.device.GetHashCode());
        if (!gotControllerID)
        {
            List<int> controllerIDs = new();
            CrabControl_Backup[] controllers = FindObjectsByType<CrabControl_Backup>(FindObjectsSortMode.InstanceID);
            foreach (CrabControl_Backup itr in controllers)
                // ###################### UNCOMMENT ########################
                if (itr.controllerID != 0) controllerIDs.Add(itr.controllerID);

            if (!controllerIDs.Contains(value.control.device.deviceId))
            {
                controllerID = value.control.device.deviceId;
                gotControllerID = true;
            }
        }

        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;



    }

    private void OnXButtonCancelled(InputAction.CallbackContext value)
    {
        //if (value.control.device.deviceId != controllerID) return;
        if (crabPart != Sides.Left) return;

    }


}
