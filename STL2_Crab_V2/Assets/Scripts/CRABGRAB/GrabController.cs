using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform handTransform; // The transform representing the character's hand
    public float grabRange = 2f; // The range within which the character can grab objects
    public float throwForce = 10f; // Force applied when throwing the object

    private bool isGrabbing = false;
    private bool isAiming = false;
    private Rigidbody currentGrabbedObject;
    private Vector3 originalGrabOffset;
    private Quaternion originalRotationOffset;
    private GameObject item;
    private Vector3 worldPoint;
    public Camera worldCam;
    public GameObject Reticle;

    private void Start()
    {
        Reticle.SetActive(false);
    }

    void Update()
    {
        // Check for grab input
        if (Input.GetAxis("LeftTrigger") > 0.1f && !isGrabbing)
        {
            ToggleGrab();
        }

        // Check for aiming input
        if (isGrabbing)
        {
            if (Input.GetAxis("RightTrigger") > 0.1f)
            {
                isAiming = true;
                Reticle.SetActive(true);
            }
            else if (isAiming && Input.GetAxis("RightTrigger") <= 0.1f)
            {
                isAiming = false;
                ThrowObject(worldPoint);
                Reticle.SetActive(false);
            }
        }

        // Moves grabbed object
        if (isGrabbing && currentGrabbedObject != null && !isAiming)
        {
            Vector3 desiredPosition = handTransform.TransformPoint(originalGrabOffset);
            desiredPosition.y += 2f; // Adjust the y-position if necessary
            currentGrabbedObject.MovePosition(desiredPosition);
            currentGrabbedObject.MoveRotation(handTransform.rotation * originalRotationOffset);
        }

        if (isAiming && currentGrabbedObject != null)
        {
            
            worldPoint = CalculateThrowDirection();
        }
    }

    void OnTriggerStay(Collider target)
    {
        if (!isGrabbing)
        {
            HoldItem(target.gameObject);
        }
    }

    void HoldItem(GameObject grabTarget)
    {
        item = grabTarget;
    }

    void ToggleGrab()
    {
        if (!isGrabbing && item != null)
        {
            if (item.GetComponent<Rigidbody>() != null)
            {
                currentGrabbedObject = item.GetComponent<Rigidbody>();
                currentGrabbedObject.isKinematic = true;
                originalGrabOffset = handTransform.InverseTransformDirection(currentGrabbedObject.position - handTransform.position);
                originalRotationOffset = Quaternion.Inverse(handTransform.rotation) * currentGrabbedObject.rotation;
                isGrabbing = true;
            }
        }
        else
        {
            if (currentGrabbedObject != null)
            {
                currentGrabbedObject.isKinematic = false;
                currentGrabbedObject = null;
                isGrabbing = false;
            }
        }
    }

    void ThrowObject(Vector3 throwDirection)
    {
        if (currentGrabbedObject != null)
        {
            currentGrabbedObject.isKinematic = false;
            currentGrabbedObject.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);
            currentGrabbedObject = null;
            isGrabbing = false;
        }
    }
    Vector3 CalculateThrowDirection()
    {
        
        Camera cam = worldCam;
        Vector3 throwDirection = cam.transform.forward;
        return throwDirection;
    }
}
