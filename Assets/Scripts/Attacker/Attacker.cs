using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Attacker : MonoBehaviour {

    [SerializeField]
    RolodexHandler[] routingOptions;
    public MaliciousAgent maliciousAgentPrefab;
    public Wave wavePrefab;

    protected MapDisplay mapDisplay;

    private void Awake()
    {
        mapDisplay = GameObject.FindWithTag("MapDisplay").GetComponent<MapDisplay>();
        if (mapDisplay == null)
        {
            Debug.LogError("Could not find MapDisplay object in the scene. Either the tag was changed or the object is missing.");
        }
    }

    private void Update()
    {
        for(int i = 0; i < routingOptions.Length; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha5 + i))
            {
                PreAgent preAgent = new PreAgent(maliciousAgentPrefab, mapDisplay.selectedPath, routingOptions[i].currentAttribute);
                Wave wave = Instantiate(wavePrefab);
                List<PreAgent> agentList = new List<PreAgent> { preAgent };
                wave.CreateWaveWithList(agentList);
                //wave.GetComponent<NetworkIdentity>().AssignClientAuthority(transform.parent.GetComponent<NetworkIdentity>().connectionToClient);
                if (transform.parent.GetComponent<Player>().isLocalPlayer)
                {
                    transform.parent.GetComponent<Player>().SpawnWithAuthority(wave.GetComponent<NetworkIdentity>());
                    //transform.parent.GetComponent<Player>().Spawn(wave.GetComponent<NetworkIdentity>(), true);
                }
                //wave.Spawn(preAgent);
            }
        }
    }
}
