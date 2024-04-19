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
    public TMP_Text levelTimeText;
    public GameObject leftReadyButtonGO;
    public GameObject rightReadyButtonGO;


    // Level performance
    public float gameTimer = 0;
    public float levelTimer = 0;
    private List<float> levelDurations = new();
    private bool levelStarted = false;
    public int playersReady = 0;
    public bool saveFile;


    private void Awake()
    {
        tethers = new(FindObjectsByType<TetherBreak_V2>(FindObjectsSortMode.None));
        saveFile = true;
    }

    // Post processing
    // script
    // scene
    

    private void FixedUpdate()
    {
        int tethersLeft = 0;
        foreach (TetherBreak_V2 itr in tethers)
        {
            if (itr.ropeBroken == false) tethersLeft++;
        }

        if (tethersLeft <= 0 && saveFile) EndGame();

        if (playersReady == 2)
        {
            gameTimer += Time.deltaTime;
        }
        if (levelStarted)
        {
            levelTimer += Time.deltaTime;
            levelTimeText.text = "Timer: " + levelTimer.ToString("F1");
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

    public void FinishLevel()
    {
        levelStarted = false;
        levelDurations[levelDurations.Count - 1] = levelTimer;
        levelTimer = 0f;
    }

    void EndGame()
    {
        playersReady = 0;
        FinishLevel();

        SaveGameData();
        // In case we want to quit the game.
        //Application.Quit();
    }

    void SaveGameData()
    {
        saveFile = false;
        string playerData = "";
        int i = 1;
        foreach (float itr in levelDurations)
        {
            playerData += "Level " + i.ToString() + ", " + itr;
            i++;
        }

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
        UnityEditor.AssetDatabase.Refresh();
    }
}
