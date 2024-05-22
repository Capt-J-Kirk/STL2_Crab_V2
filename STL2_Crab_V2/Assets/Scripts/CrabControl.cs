using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CrabControl : MonoBehaviour
{
    [SerializeField]
    private int playerIndex = -1;

    // Game pad variables
    public Vector2 leftVector = Vector2.zero;
    public Vector2 leftVector_prev = Vector2.zero;
    public Vector2 rightVector = Vector2.zero;
    public bool rightStickRotateCamera = true;

    // Camera
    public Camera camera;
    private Quaternion camStartRot;
    public Vector2 camRotaton;
    float cameraOffsetFactor = 2.5f;
    [SerializeField] GameObject camYawObj;
    [SerializeField] GameObject camPitchObj;

    // Crab body
    private Rigidbody rb;

    // Crab rotation axes
    public GameObject frontAxisObject;
    public GameObject rearAxisObject;

    // Max speeds and timers
    public float speedTimer = 0;
    public float rotateTimer = 0;
    float maxSpeed = 2;                 // Units per second.
    public float maxAngularSpeed = 20;  // Degrees per second.

    // Controller
    [SerializeField]
    private float direction = 0;


    // Particle system
    public ParticleSystem crabParticle;
    [HideInInspector] public bool isSpraying = false;

    public GameData gameData;
    private bool allowMovementOnDeath = false;   // For testing.

    public GameObject radialMenu; // RadialMenu
    public GameObject cursor; // Cursor

    public Animator animator;


    private void Awake()
    {
        var main = crabParticle.main;
        main.loop = true;

        rb = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
        camStartRot = camera.transform.rotation;
        camPitchObj.transform.localPosition = new Vector3(camPitchObj.transform.localPosition.x, camPitchObj.transform.localPosition.y, camPitchObj.transform.localPosition.z * cameraOffsetFactor);

        gameData = GameObject.FindAnyObjectByType<GameData>();
        animator.speed = 0;
    }




    private void FixedUpdate()
    {
        if (gameData.playersReady >= 2 || allowMovementOnDeath)
        {
            Move();
            if (rightStickRotateCamera)
            {
                RotateCamera();
            }
        }
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
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
        rb.transform.RotateAround(rb.transform.position, transform.up, maxAngularSpeed * Time.deltaTime * leftVector.x);



    }

    private void ChangeVelocity()
    {
        float speedChangeTime = 0.5f;
        if (Mathf.Sign(leftVector_prev.y) != Mathf.Sign(leftVector.y)) speedChangeTime *= 2;

        // Current velocity
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        float local_x = Mathf.Lerp(leftVector_prev.y * maxSpeed, leftVector.y * maxSpeed, Mathf.Pow(speedTimer/speedChangeTime, 1.5f));
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
        animator.speed = rb.velocity.sqrMagnitude;

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
        //camera.transform.RotateAround(camera.transform.position, camera.transform.up, maxAngularSpeed * Time.deltaTime * rightVector.x);
        //camera.transform.RotateAround(camera.transform.position, camYawObj.transform.up, maxAngularSpeed * Time.deltaTime * rightVector.x);
        camYawObj.transform.RotateAround(camYawObj.transform.position, camYawObj.transform.up, maxAngularSpeed * Time.deltaTime * rightVector.x);

        // Pitch
        //camera.transform.RotateAround(camera.transform.position, camera.transform.right, maxAngularSpeed * Time.deltaTime * rightVector.y);
        camera.transform.RotateAround(camera.transform.position, camPitchObj.transform.right, maxAngularSpeed * Time.deltaTime * rightVector.y);
    }






}
