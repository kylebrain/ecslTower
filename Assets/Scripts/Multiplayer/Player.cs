using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public GameObject attacker;
    public GameObject defender;
    public GameObject pauseButton;

    public override void OnStartLocalPlayer()
    {
        int playerCount = FindObjectOfType<NetworkManager>().numPlayers;

        if (playerCount == 1) //change to 1 after testing
        {
            defender.SetActive(true);
            GetComponent<Attacker>().enabled = false;
            Destroy(attacker);
            name = "DefenderPlayer " + playerCount;
        }
        else
        {
            attacker.SetActive(true);
            GetComponent<Defender>().enabled = false;
            GetComponent<WaveController>().enabled = false;
            Destroy(defender);
            name = "AttackerPlayer " + playerCount;
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

            name = "Player " + NetworkServer.connections.Count;
        }
    }
}
