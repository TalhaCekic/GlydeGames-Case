using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public bool isPause;
    public GameObject PauseMenuObj;

    private void Start()
    {
        PauseMenuObj.SetActive(false);
    }

    public void AfterScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    } 
    public void BeforeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseSelectMenu();
        }
    }

    private void PauseSelectMenu()
    {
        isPause = !isPause;
        if (isPause)
        {
            PauseMenuObj.SetActive(true);
        }
        else
        {
            PauseMenuObj.SetActive(false);
        }
    }
}
