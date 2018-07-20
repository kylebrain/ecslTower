using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ModeSelect : MonoBehaviour {

    private void Start()
    {
        NetworkManager[] managers = FindObjectsOfType<NetworkManager>();

        foreach(NetworkManager manager in managers)
        {
            Destroy(manager.gameObject);
        }
    }
}
