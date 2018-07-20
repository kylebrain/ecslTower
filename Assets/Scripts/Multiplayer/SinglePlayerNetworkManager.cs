using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SinglePlayerNetworkManager : NetworkManager {

    public static SinglePlayerNetworkManager instance;

    public Button backButton;

    private void Start()
    {
        instance = this;
        backButton.onClick.AddListener(ButtonDelegate);
    }

    void ButtonDelegate()
    {
        SceneLoader.LoadScene("ModeSelect");
    }
}
