using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectableLevelPanel : NetworkBehaviour {

    private void Start()
    {
        GetComponent<Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        GetComponent<Button>().onClick.AddListener(SelectLevel);
    }

    public void SelectLevel()
    {
        LobbyPlayer.localPlayer.CmdSetLevel(GetComponent<LevelPanel>().LevelName);
    }
	
}
