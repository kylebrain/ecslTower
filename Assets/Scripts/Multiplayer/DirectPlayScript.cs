using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectPlayScript : MonoBehaviour {

    public Button playAndHostButton;
    public Button connectButton;
    public UnityEngine.UI.InputField connectionInputField;

    private Lobby lobby;

    private void Start()
    {
        //lobby = transform.root.GetComponent<Lobby>();
        lobby = FindObjectOfType<Lobby>();
        playAndHostButton.onClick.AddListener(OnClickHost);
        connectButton.onClick.AddListener(OnClickJoin);
        //connectionInputField.onEndEdit.AddListener(OnClickJoin);
    }

    public void OnClickHost()
    {
        lobby.StartHost();
    }

    public void OnClickJoin()
    {
        string ipAddress = connectionInputField.text;
        if(string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = connectionInputField.placeholder.GetComponent<Text>().text;
        }
        lobby.networkAddress = ipAddress;
        lobby.StartClient();
    }


}
