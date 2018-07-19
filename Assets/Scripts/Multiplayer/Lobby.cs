using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : NetworkLobbyManager
{
    public GameObject lobby;
    public GameObject select;
    public GameObject playerList;
    public Button backButton;

    public override void OnStopClient()
    {
        lobby.SetActive(false);
        select.SetActive(true);
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        lobby.SetActive(true);
        select.SetActive(false);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        backButton.onClick.AddListener(StopClient);
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        lobby.SetActive(false);
        select.SetActive(false);

        //bool ret = base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        gamePlayer.GetComponent<Player>().PlayerType = lobbyPlayer.GetComponent<LobbyPlayer>().playerType;

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);

    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        backButton.onClick.AddListener(StopHost);
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        //conn.playerControllers[0].unetView.gameObject.SetActive(SceneManager.GetActiveScene().name == lobbyScene);

        lobby.SetActive(SceneManager.GetActiveScene().name == lobbyScene);

        if(SceneManager.GetActiveScene().name == lobbyScene)
        {
            backButton.onClick.RemoveAllListeners();
            //Debug.Log("Setting listeners!");
            if (conn.playerControllers[0].unetView.isServer)
            {
                backButton.onClick.AddListener(StopHost);
            } else
            {
                backButton.onClick.AddListener(StopClient);
            }
        }

        //transform.Find("PlayerList").gameObject.SetActive(SceneManager.GetActiveScene().name == lobbyScene);
    }
}
