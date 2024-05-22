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

    void Update()
    {
        // Check for grab input
        if (Input.GetButtonDown("Fire1"))
        {
            ToggleGrab();
        }

        // Check for aiming input
        if (isGrabbing)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isAiming = true;
                // Enable Reticle
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                isAiming = false;
                ThrowObject(worldPoint);
                //Disable Reticle
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
        // Get the camera
        Camera cam = Camera.main;

        // Get a ray from the center of the screen
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        // Assume the world point is on the same plane as the reference object
        Plane plane = new Plane(Vector3.up, this.gameObject.transform.position);

        // Calculate the intersection point
        float distance;
        Vector3 worldPoint = Vector3.zero;
        if (plane.Raycast(ray, out distance))
        {
            worldPoint = ray.GetPoint(distance);
        }

        // Calculate the direction vector from the reference object to the world point
        Vector3 direction = worldPoint - this.gameObject.transform.position;
        return direction;
    }
}
