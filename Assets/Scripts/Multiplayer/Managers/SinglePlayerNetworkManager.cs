using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SinglePlayerNetworkManager : NetworkManager {

    public static SinglePlayerNetworkManager instance;

    //public Button backButton;

    //private delegate void BackButtonDelegate();
    //private BackButtonDelegate backButtonDelegate;

    private void Start()
    {
        instance = this;
        //backButton.onClick.AddListener(ButtonDelegate);
        //backButtonDelegate = ButtonDelegate;
    }

    /*

    void ButtonDelegate()
    {
        SceneLoader.LoadScene("ModeSelect");
    }

    public void ButtonAction()
    {
        backButtonDelegate();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        backButton.onClick.AddListener(ButtonDelegate);
    }

    */
}
