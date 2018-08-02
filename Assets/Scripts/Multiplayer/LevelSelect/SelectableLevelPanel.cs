using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectableLevelPanel : MonoBehaviour {

    private void Start()
    {
        GetComponent<Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        GetComponent<Button>().onClick.AddListener(SelectLevel);
    }

    public void SelectLevel()
    {
        string levelName = GetComponent<LevelPanel>().LevelName;
        LobbyPlayer.localPlayer.CmdSetLevel(levelName);

        // not sure if this will work with more than one client
        //SelectedLevel.instance.UpdateSelectedLevel(levelName);
    }

}
