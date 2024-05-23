using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool _isPaused;
    [SerializeField]
    private GameObject child;

    private void Awake()
    {
        child = this.gameObject.transform.GetChild(0).gameObject;
    }

    public void PauseGame()
    {
        if (!_isPaused)
        {
            //Time.timeScale = 0;
            _isPaused = true;
            child.SetActive(true);
        }
        else
        {
            //Time.timeScale = 1;
            _isPaused = false;
            child.SetActive(false);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
