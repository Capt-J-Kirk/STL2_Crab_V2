using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameData : MonoBehaviour
{
    public List<TetherBreak_V2> tethers;
    public List<GameObject> UI_Objects;
    public TMP_Text messageUI_Text;
    public GameObject leftReadyButtonGO;
    public GameObject rightReadyButtonGO;

    // Endgame dance
    GameObject[] dancingParts = new GameObject[10];
    public GameObject[] dancingLights = new GameObject[5];
    public GameObject dirLight;


    // Level performance
    public float gameTimer = 0;
    public float levelTimer = 0;
    private List<float> levelDurations = new();
    public bool levelStarted = false;
    public int playersReady = 0;
    public bool saveFile;

    // Finish Level
    string message_LevelComplete = "You finished the level, congrats!";
    string message_LevelAndGameComplete = "You finished Level and Game. Congrats;";
    string message_Died = "The Crab died.";
    float showMessageTime = 1.5f;
    float showMessageTimer = 0;
    public string message = "";
    public bool showMessage = false;
    float danceTime = 10;
    public int tethersLeft;

    // Finish game
    int totalLevels = 1;
    int currentLevel;



    private void Awake()
    {
        tethers = new(FindObjectsByType<TetherBreak_V2>(FindObjectsSortMode.None));
        saveFile = true;
        playersReady = 0;
        currentLevel = 1;
        dancingParts = GameObject.FindGameObjectsWithTag("RaveDancer");
        foreach (GameObject itr in dancingParts)
        {
            itr.GetComponent<Animator>().speed = 0;

        }

        // Need to count tethers, so end smoke doesn't start.
        foreach (TetherBreak_V2 itr in tethers) if (itr.ropeBroken == false) tethersLeft++;
    }

    // Post processing
    // script
    // scene
    

    private void FixedUpdate()
    {
        // Count unbroken tethers, and end game if there is none.
        tethersLeft = 0;
        foreach (TetherBreak_V2 itr in tethers)
        {
            if (itr.ropeBroken == false) tethersLeft++;
        }
        if (tethersLeft == 0 && saveFile) FinishLevel(false);

        // Always increment game timer, if 2 players are ready.
        if (playersReady == 2)
        {
            gameTimer += Time.deltaTime;
        }

        // Increment levelTimer if level started.
        if (levelStarted)
        {
            levelTimer += Time.deltaTime;
        }

        // Show timer or message.
        if (!showMessage)
        {
            if (levelStarted) messageUI_Text.text = "Timer: " + levelTimer.ToString("F1");
        }
        else
        {
            showMessageTimer += Time.deltaTime;
            messageUI_Text.text = message;
            if (showMessageTimer > showMessageTime)
            {
                showMessage = false;
                showMessageTimer = 0;
            }
        }
    }

    public void RegisterLeftReady()
    {
        playersReady++;
        leftReadyButtonGO.SetActive(false);
        if (playersReady == 2)
        {
            LevelStart();
        }
    }

    public void RegisterRightReady()
    {
        playersReady++;
        rightReadyButtonGO.SetActive(false);
        if (playersReady == 2)
        {
            LevelStart();
        }
    }

    void LevelStart()
    {
        levelStarted = true;
        levelDurations.Add(0f);
    }


    public void FinishLevel(bool isAlive)
    {
        // Show message.
        showMessage = true;

        // End game, in one way or another.
        if (currentLevel == totalLevels || !isAlive)
        {
            levelStarted = false;
            levelDurations[levelDurations.Count - 1] = levelTimer;
            levelTimer = 0f;
            if (!isAlive)
            {
                message = message_Died;
                EndGame("False");
            }
            else
            {
                message = message_LevelAndGameComplete;
                EndGame("True");
            }

        }
        // Continue with next level.
        else
        {
            levelStarted = true;
            message = message_LevelComplete;
            levelDurations[levelDurations.Count - 1] = levelTimer;
            levelTimer = 0f;
            currentLevel++;
        }
    }


    void EndGame(string gameComplete)
    {
        SaveGameData(gameComplete);

        if (gameComplete == "True")
        {
            foreach (GameObject itr in dancingParts)
            {
                itr.GetComponent<Animator>().speed = 1;
            }
            foreach (GameObject itr in dancingLights)
            {
                itr.SetActive(true);
            }
            dirLight.GetComponent<Light>().intensity = 0;

            Invoke(nameof(Dancing), danceTime);
        }
        else
        {
            Invoke(nameof(Dancing), 0);
        }

        // In case we want to quit the game.
        //Application.Quit();
    }


    void Dancing()
    {
        playersReady = 0;
        levelStarted = false;
    }


    void SaveGameData(string gameComplete)
    {
        saveFile = false;
        string playerData = "";
        int i = 1;
        foreach (float itr in levelDurations)
        {
            playerData += "Level " + i.ToString() + ", " + itr + ", ";
            i++;
        }
        playerData += "GameComplete: " + gameComplete;

        string dataFolderPath = Application.dataPath + "/Data";
        if (!Directory.Exists(dataFolderPath)) Directory.CreateDirectory(dataFolderPath);

        int fileCount = Directory.GetFiles(dataFolderPath).Length / 2;

        string dateTime = DateTime.Now.ToString();
        string newDateTime = dateTime.Replace("/", "-");
        newDateTime = newDateTime.Replace(" ", "_");
        newDateTime = newDateTime.Replace(":", "-");
        Debug.Log(newDateTime);

        string filePath = Path.Combine(dataFolderPath, fileCount.ToString() + "_GameData_" + newDateTime + ".txt");
        
        File.WriteAllText(filePath, playerData);

        // Refresh the AssetDatabase to make sure Unity detects the newly created file
        //UnityEditor.AssetDatabase.Refresh();
    }
}
