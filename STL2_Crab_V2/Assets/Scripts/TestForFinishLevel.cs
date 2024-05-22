using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForFinishLevel : MonoBehaviour
{
    GameData gameData;
    public bool levelFinishedLeft;
    public bool levelFinishedRight;
    public bool levelFinished;
    AudioSource audioSource;

    string tagLeft = "LeftPart_MovementCylinder";
    string tagRight = "RightPart_MovementCylinder";

    Animator animatorLeft;
    Animator animatorRight;

    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
        levelFinishedLeft = false;
        levelFinishedRight = false;
        levelFinished = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Border trigger: name: " + other.name + ", tag:" + other.tag);
        if (!levelFinished)
        {
            if (other.CompareTag(tagLeft))
            {
                levelFinishedLeft = true;
                animatorLeft = other.GetComponent<Animator>();
                if (levelFinishedRight) Finishlevel();
            }

            if (other.CompareTag(tagRight))
            {
                levelFinishedRight = true;
                animatorRight = other.GetComponent<Animator>();
                if (levelFinishedLeft) Finishlevel();
            }
        }
    }


    void Finishlevel()
    {
        levelFinished = true;
        audioSource.Play();
        gameData.FinishLevel(true);
    }


}


