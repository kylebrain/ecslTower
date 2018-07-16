using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public GameObject attacker;
    public GameObject defender;

    public override void OnStartLocalPlayer()
    {

        if (NetworkServer.connections.Count != 1) //change to 1 after testing
        {
            defender.SetActive(true);
            GetComponent<Attacker>().enabled = false;
            Destroy(attacker);
            name = "DefenderPlayer " + NetworkServer.connections.Count;
        }
        else
        {
            attacker.SetActive(true);
            GetComponent<Defender>().enabled = false;
            Destroy(defender);
            name = "AttackerPlayer " + NetworkServer.connections.Count;
        }
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(attacker);
            Destroy(defender);
            GetComponent<Attacker>().enabled = false;
            GetComponent<Defender>().enabled = false;
        }

        name = "Player " + NetworkServer.connections.Count;
    }

    public void SpawnWithAuthority(NetworkIdentity identity)
    {
        if (isLocalPlayer)
        {
            NetworkServer.SpawnWithClientAuthority(identity.gameObject, FindObjectOfType<NetworkManager>().client.connection);
        }
    }

    [Command]
    public void CmdSpawnWithAuthority(NetworkIdentity identity)
    {
        //set to null when ran on the server
        //something about invalid pass types to commands

        if (identity == null)
        {
            Debug.LogError("Object to spawn is null!");
            return;
        }

        GameObject obj = Instantiate(identity.gameObject);
        NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);
    }

    /*
    [Command]
    public void CmdSpawn(NetworkIdentity identity, bool withAuthority)
    {
        if(isLocalPlayer)
        {
            if(withAuthority)
            {
                NetworkServer.SpawnWithClientAuthority(identity.gameObject, gameObject);
            } else
            {
                NetworkServer.Spawn(identity.gameObject);
            }
        }
    }*/
}
