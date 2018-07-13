using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject attacker;
    public GameObject defender;

    public override void OnStartLocalPlayer()
    {

        if(NetworkServer.connections.Count != 1) //change to 1 after testing
        {
            defender.SetActive(true);
            Destroy(attacker);
        } else
        {
            attacker.SetActive(true);
            Destroy(defender);
        }
    }

    private void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(attacker);
            Destroy(defender);
        }
    }

    public void SpawnWithAuthority(NetworkIdentity identity)
    {
        if (isLocalPlayer)
        {
            NetworkServer.SpawnWithClientAuthority(identity.gameObject, gameObject);
        }
    }

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
    }
}
