using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
	
    public static bool GamePaused = false;
    public GameObject pauseMenu;

	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
	}

    public void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void LoadLevelSelect()
    {
        Resume();
        SceneLoader.LoadScene("LevelSelect");
    }

}
