using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialNetworkManager : MonoBehaviour {

    private void Start()
    {
        GetComponent<NetworkManager>().StartHost();
    }
}
