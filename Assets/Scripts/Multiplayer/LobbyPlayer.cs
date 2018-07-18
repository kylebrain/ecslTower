using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerType {None, Defender, Attacker};

public class LobbyPlayer : NetworkLobbyPlayer {

    public PlayerType PlayerType;
}
