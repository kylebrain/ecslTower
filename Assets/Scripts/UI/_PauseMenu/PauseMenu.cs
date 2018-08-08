using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{

    public static bool IsGamePaused = false;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject[] othersToHide;

    private delegate void OnPause(bool paused);
    private static event OnPause PauseGame;

    public override void OnStartLocalPlayer()
    {
        PauseGame = PauseListener;
        Resume();
    }

    private void OnDestroy()
    {
        PauseGame = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
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
        IsGamePaused = true;
        Time.timeScale = 0f;
        CmdPause(true);
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        CmdPause(false);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void LoadLevelSelect()
    {
        SceneLoader.ExitGame();
    }

    public void ShowOptions(bool show)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(!show);
        }
        foreach (GameObject obj in othersToHide)
        {
            obj.SetActive(!show);
        }
        optionsMenu.SetActive(show);
    }

    // networked functions, must be attached to a Player object

    [Command]
    public void CmdPause(bool paused)
    {
        RpcPauseOnClients(paused);
    }

    [ClientRpc]
    void RpcPauseOnClients(bool paused)
    {
        if (PauseGame != null)
        {
            PauseGame(paused);
        }
    }

    void PauseListener(bool paused)
    {
        if (IsGamePaused == paused || !isLocalPlayer)
        {
            return;
        }

        if (paused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

}
