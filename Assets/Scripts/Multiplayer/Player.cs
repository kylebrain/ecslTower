using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public static Player localPlayer;
    public GameObject attacker;
    public GameObject defender;
    public GameObject pauseButton;

    [SyncVar]
    public PlayerType PlayerType;

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
        InitializePlayer(PlayerType);

        /*
        int playerCount = FindObjectOfType<NetworkManager>().numPlayers;

        if(PlayerType == PlayerType.None)
        {
            Debug.LogWarning("Player was not given a type!");
            InitializePlayer(playerCount == 1 ? PlayerType.Defender : PlayerType.Attacker);
        }

        */

        
    }

    public void InitializePlayer(PlayerType playerType)
    {
        PlayerType = playerType;

        int playerCount = FindObjectOfType<NetworkManager>().numPlayers;

        switch (playerType)
        {
            case PlayerType.Defender:
                defender.SetActive(true);
                GetComponent<Attacker>().enabled = false;
                Destroy(attacker);
                name = "DefenderPlayer " + playerCount;
                break;
            case PlayerType.Attacker:
                attacker.SetActive(true);
                GetComponent<Defender>().enabled = false;
                GetComponent<WaveController>().enabled = false;
                Destroy(defender);
                name = "AttackerPlayer " + playerCount;
                break;
            case PlayerType.None:
                Debug.LogError("Player should have a valid type!");
                return;
            default:
                Debug.LogError("There is no case for this enum value\nAdd one or remove the enum value!");
                return;
        }
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(attacker);
            Destroy(defender);
            Destroy(pauseButton);
            GetComponent<Attacker>().enabled = false;
            GetComponent<Defender>().enabled = false;
            GetComponent<WaveController>().enabled = false;

            name = "Player " + FindObjectOfType<NetworkManager>().numPlayers;
        }
    }
}
