using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReadyButton : MonoBehaviour
{
    public Text text;

    public Button Button
    {
        get
        {
            return GetComponent<Button>();
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Ready);
        SetReadyAppearance(transform.parent.GetComponent<LobbyPlayer>().readyToBegin);
    }

    public void Ready()
    {
        //LobbyPlayer.localPlayer.readyToBegin = !LobbyPlayer.localPlayer.readyToBegin;
        SetReady(!LobbyPlayer.localPlayer.readyToBegin);
    }

    private void SetReady(bool ready)
    {
        if(ready == LobbyPlayer.localPlayer.readyToBegin)
        {
            return;
        }

        //Debug.Log(SelectedLevel.instance.SelectedLevelName + "\n" + LobbyPlayer.localPlayer.playerType);

        if (ready && SelectedLevel.instance.SelectedLevelName != LevelLookup.defaultLevelName && LobbyPlayer.localPlayer.playerType != PlayerType.None)
        {
            LobbyPlayer.localPlayer.SendReadyToBeginMessage();
            //Debug.Log("Ready!", LobbyPlayer.localPlayer);
        }
        else
        {
            LobbyPlayer.localPlayer.SendNotReadyToBeginMessage();
            //Debug.Log("Not ready!", LobbyPlayer.localPlayer);

        }

        //SetReadyAppearance(ready);
    }

    public void SetReadyAppearance(bool ready)
    {
        if(ready)
        {
            text.text = "Ready!";
            GetComponent<Image>().color = Color.green;
        } else
        {
            text.text = "Not ready!";
            GetComponent<Image>().color = Color.red;
        }
    }
}
