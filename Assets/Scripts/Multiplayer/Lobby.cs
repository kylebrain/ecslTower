using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Lobby : NetworkLobbyManager {

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        gamePlayer.GetComponent<Player>().InitializePlayer(lobbyPlayer.GetComponent<LobbyPlayer>().PlayerType);

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);

    }
}
