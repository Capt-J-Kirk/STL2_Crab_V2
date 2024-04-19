using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForFinishLevel : MonoBehaviour
{
    GameData gameData;
    public bool levelFinishedLeft;
    public bool levelFinishedRight;
    public bool levelFinished;

    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
        levelFinishedLeft = false;
        levelFinishedRight = false;
        levelFinished = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Border trigger");
        if (!levelFinished)
        {
            if (other.CompareTag("LeftPart") || other.name == "LeftPart")
            {
                
                levelFinishedLeft = true;
                if (levelFinishedRight)
                {
                    levelFinished = true;
                    gameData.FinishLevel(true);
                }
            }

            if (other.CompareTag("RightPart") || other.name == "RightPart")
            {
                levelFinishedRight = true;
                if (levelFinishedLeft)
                {
                    levelFinished = true;
                    gameData.FinishLevel(true);
                }
            }
        }
    }
}
