using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Lobby : NetworkLobbyManager
{

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //bool ret = base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        gamePlayer.GetComponent<Player>().PlayerType = lobbyPlayer.GetComponent<LobbyPlayer>().playerType;

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);

    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        //conn.playerControllers[0].unetView.gameObject.SetActive(SceneManager.GetActiveScene().name == lobbyScene);
        transform.Find("PlayerList").gameObject.SetActive(SceneManager.GetActiveScene().name == lobbyScene);
    }
}
