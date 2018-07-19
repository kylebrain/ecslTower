using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectPlayScript : MonoBehaviour {

    public Button playAndHostButton;
    public Button connectButton;
    public UnityEngine.UI.InputField connectionInputField;

    private void Start()
    {
        //lobby = transform.root.GetComponent<Lobby>();
        playAndHostButton.onClick.AddListener(OnClickHost);
        connectButton.onClick.AddListener(OnClickJoin);
        connectionInputField.onEndEdit.AddListener(OnEndEditIP);
    }

    public void OnClickHost()
    {
        Lobby.instance.StartHost();
    }

    public void OnEndEditIP(string text)
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnClickJoin();
        }
    }

    public void OnClickJoin()
    {
        string ipAddress = connectionInputField.text;
        if(string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = connectionInputField.placeholder.GetComponent<Text>().text;
        }
        Lobby.instance.networkAddress = ipAddress;

        Lobby.instance.connectingDisplay.SetActive(true);

        Lobby.instance.StartClient();
    }


}
