using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GamePaused = false;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject[] othersToHide;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        Player player;
        if ((player = GetComponent<Player>()) != null)
        {
            player.CmdPause(true);
        }
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        Player player;
        if ((player = GetComponent<Player>()) != null)
        {
            player.CmdPause(false);
        }
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void LoadLevelSelect()
    {
        Resume();
        SceneLoader.LoadScene("LevelSelect");
    }

    public void ShowOptions(bool show)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(!show);
        }
        foreach(GameObject obj in othersToHide)
        {
            obj.SetActive(!show);
        }
        optionsMenu.SetActive(show);
    }

}
