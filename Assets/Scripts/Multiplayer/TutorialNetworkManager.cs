using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TutorialNetworkManager : MonoBehaviour {

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneChange;
    }

    private void Start()
    {
        GetComponent<NetworkManager>().StartHost();
    }

    private void OnSceneChange(Scene scene)
    {
        //GetComponent<NetworkManager>().StopHost();
        //Destroy(gameObject);
    }

}
