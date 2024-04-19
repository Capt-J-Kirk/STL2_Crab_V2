using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shark : MonoBehaviour
{
    Animator animation;
    public GameObject crab_LeftPart;
    public GameObject crab_RightPart;
    [SerializeField]
    private GameObject target;

    public float distance_L;
    public float distance_R;
    public float distance;

    private float attackDistance = 15;
    private Vector3 attackVector;
    
    private void Awake()
    {
        animation = GetComponent<Animator>();
        float speed =animation.speed;
        Debug.Log("Animation speed: " + speed);
        animation.speed = 5;


    }

    private void FixedUpdate()
    {
        GetAttackData();

        if (distance < attackDistance)
        {
            //gameObject.transform.LookAt(target.transform, Vector3.up);

            // Calculate the rotation needed to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(attackVector, Vector3.up);

            // Rotate towards the target using RotateTowards
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1f * Time.deltaTime);

            // Move towards the target
            transform.Translate(attackVector * 1f * Time.deltaTime);
        }
    }


    void GetAttackData()
    {
        distance_L = Vector3.Distance(crab_LeftPart.transform.position, transform.position);
        distance_R = Vector3.Distance(crab_RightPart.transform.position, transform.position);
        distance = Mathf.Min(distance_L, distance_R);
        if (distance_L < distance_R)
        {
            attackVector = crab_LeftPart.transform.position - transform.position;
            target = crab_LeftPart;
        }
        else
        {
            attackVector = crab_RightPart.transform.position - transform.position;
            target = crab_RightPart;
        }
    }

}
