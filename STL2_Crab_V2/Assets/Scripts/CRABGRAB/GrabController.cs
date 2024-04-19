using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform handTransform; // The transform representing the character's hand
    public float grabRange = 2f; // The range within which the character can grab objects

    private bool isGrabbing = false;
    private Rigidbody currentGrabbedObject;
    private Vector3 originalGrabOffset;
    private Quaternion originalRotationOffset;
    private GameObject item;

    void Update()
    {
        // Check for grab input
        if (Input.GetButtonDown("Fire1"))
        {
            ToggleGrab();
        }

        // Moves grabbed object
        if (isGrabbing && currentGrabbedObject != null)
        {
            Vector3 desiredPosition = handTransform.TransformPoint(originalGrabOffset);
            // Add 2 to the y-coordinate of the desired position
            desiredPosition.y += 2f;
            currentGrabbedObject.MovePosition(desiredPosition);
            currentGrabbedObject.MoveRotation(handTransform.rotation * originalRotationOffset);
        }

        // Throw object if grab button is pressed again while holding an object
        if (isGrabbing && currentGrabbedObject != null && Input.GetButtonDown("Fire2"))
        {
            ThrowMode();
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

    void ThrowMode()
    {
        // Implement throw logic here
    }
}
