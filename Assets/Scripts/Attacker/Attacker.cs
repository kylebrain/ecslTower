using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Attacker : NetworkBehaviour
{

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

        /*
        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            List<WavePath> wavePaths = new List<WavePath>(mapDisplay.WavePathList);
            Vector2Array[] vector2Array = new Vector2Array[wavePaths.Count];
            for(int i = 0; i < vector2Array.Length; i++)
            {
                vector2Array[i].array = wavePaths[i].ToVector2Array();
            }

            CmdBogey(vector2Array);
        } */

        for (int i = 0; i < routingOptions.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5 + i))
            {
                if(mapDisplay.selectedPath != null)
                {
                    CmdSpawnWave(i, routingOptions[i].currentAttribute, mapDisplay.selectedPath.ToVector2Array());
                } else
                {
                    Debug.LogWarning("No selected path!");
                }
                
            }
        }
    }

    [Command]
    void CmdSpawnWave(int i, AgentAttribute agentAttribute, Vector2[] coords)
    {
        PreAgent preAgent = new PreAgent(maliciousAgentPrefab, new WavePath(coords), agentAttribute);
        Wave wave = Instantiate(wavePrefab);
        //Debug.Log(preAgent);
        List<PreAgent> agentList = new List<PreAgent> { preAgent };
        wave.CreateWaveWithList(agentList);
        NetworkServer.SpawnWithClientAuthority(wave.gameObject, connectionToClient);

    }

    /*
    [Command]
    void CmdBogey(Vector2Array[] coords)
    {
        WavePath[] wavePaths = coords.Select(x => new WavePath(x.array)).ToArray();
        foreach(WavePath w in wavePaths)
        {
            Debug.Log(w);
        }
    } */
}

public struct Vector2Array
{
    public Vector2[] array;
}
