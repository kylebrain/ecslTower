using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{

    public static Player localPlayer;
    public GameObject attacker;
    public GameObject defender;
    public GameObject pauseButton;
    public GameObject singlePlayerStartWave;
    public Text functionText;

    // @ should work but might have to remove it
    [SyncVar]
    public PlayerType @PlayerType;

    [SyncVar]
    public bool isHost = false;

    public override void OnStartLocalPlayer()
    {
        //Debug.LogWarning(GetType() + ": " + MethodBase.GetCurrentMethod().Name);

        localPlayer = this;
        //InitializePlayer(PlayerType);
        functionText.text = (isHost || isServer) ? "Hosting" : "Client";

        CmdDeleteSinglePlayerWave();

        InitializePlayer(PlayerType);

        /*
        if (PlayerType == PlayerType.Defender)
        {
            CmdDeleteSinglePlayerWave();
        }
        */

        /*
        int playerCount = FindObjectOfType<NetworkManager>().numPlayers;

        if(PlayerType == PlayerType.None)
        {
            Debug.LogWarning("Player was not given a type!");
            InitializePlayer(playerCount == 1 ? PlayerType.Defender : PlayerType.Attacker);
        }

        */


    }

    [Command]
    void CmdDeleteSinglePlayerWave()
    {
        int playerCount = FindObjectOfType<NetworkManager>().numPlayers;
        if (playerCount > 1)
        {
            Destroy(singlePlayerStartWave);
            RpcDeleteWave();
        } else
        {
            if(PlayerType == PlayerType.None)
            {
                PlayerType = PlayerType.Defender;
            }
            singlePlayerStartWave.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcDeleteWave()
    {
        if (singlePlayerStartWave != null)
        {
            Destroy(singlePlayerStartWave);
        }
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

        //Debug.LogWarning(GetType() + ": " + MethodBase.GetCurrentMethod().Name);
        if (!isLocalPlayer)
        {
            Destroy(attacker);
            Destroy(defender);
            Destroy(pauseButton);
            Destroy(singlePlayerStartWave);
            Destroy(functionText.gameObject);
            GetComponent<Attacker>().enabled = false;
            GetComponent<Defender>().enabled = false;
            GetComponent<WaveController>().enabled = false;

            name = "Player " + FindObjectOfType<NetworkManager>().numPlayers;
        }
    }
}
