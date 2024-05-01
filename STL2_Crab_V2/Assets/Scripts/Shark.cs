using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float killDistance = 2;
    private Vector3 attackVector;

    // Ref. to tethers
    //public TetherBreak_V2 tether_Side_1;
    //public TetherBreak_V2 tether_Center;
    //public TetherBreak_V2 tether_Side_2;
    private List<TetherBreak_V2> tethers = new();
    private int remainingTethers = 0;

    // Ref. to GameData
    public GameData gameData;

    float killTimer = 1.5f;
    float timeBetweenKills = 5f;
    public AudioSource audioSource_1;
    public AudioClip crack;
    public AudioClip underWater;


    private void Awake()
    {
        animation = GetComponent<Animator>();
        float speed =animation.speed;
        Debug.Log("Animation speed: " + speed);
        animation.speed = 5;

        //tethers.Add(tether_Side_1);
        //tethers.Add(tether_Center);
        //tethers.Add(tether_Side_2);

        gameData = FindObjectOfType<GameData>();
    }

    private void FixedUpdate()
    {
        GetAttackData();
        if (remainingTethers == 0) return;

        // Move
        if (distance < attackDistance && distance > killDistance)
        {
            //gameObject.transform.LookAt(target.transform, Vector3.up);

            // Calculate the rotation needed to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(attackVector, Vector3.up);

            // Rotate towards the target using RotateTowards
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1f * Time.deltaTime);

            // Move towards the target
            transform.Translate(attackVector * 1f * Time.deltaTime);
        }

        // Kill
        if (distance < killDistance && remainingTethers > 0)
        {
            killTimer += Time.deltaTime;
            if (killTimer > timeBetweenKills) Kill();
        }
    }

    void Kill()
    {
        if (remainingTethers > 1)
        {
            audioSource_1.clip = crack;
            audioSource_1.Play();
        } else {
            audioSource_1.clip = crack;
            audioSource_1.Play();
            Invoke(nameof (PlayEndSound), crack.length);
        }
        string message = "The SHARK took a life";
        tethers[0].BreakRope();
        tethers.RemoveAt(0);
        gameData.message = message;
        gameData.showMessage = true;
        killTimer = 0;
    }

    void PlayEndSound()
    {
        audioSource_1.clip = underWater;
        audioSource_1.Play();
    }


    void GetAttackData()
    {
        tethers.Clear();
        remainingTethers = 0;
        TetherBreak_V2[] remaining = FindObjectsByType<TetherBreak_V2>(FindObjectsSortMode.None);
        foreach (var item in remaining)
        {
            if (!item.ropeBroken)
            {
                remainingTethers++;
                tethers.Add(item);
            }
        }


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
