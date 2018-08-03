using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Attacker : PreWaveCreator //inherit from PreWaveCreator to use helper functions
{

    public readonly static int StartingHackerCurrency = 0;
    public static int HackerCurrency = StartingHackerCurrency;

    readonly float spawnRate = 2;
    readonly float passiveRate = 1f;

    float passiveTimer = 0;
    float spawnTimer = 0;

    public readonly static int AdvanceTimeCost = 10;
    public readonly static int MutateCost = 10;

    public HackCooldown hackCooldown;
    public Text currencyText;

    public Text mutateAgentText;
    public Text advanceHackText;

    [SerializeField]
    RingDisplayAgent[] agents;
    //public MaliciousAgent maliciousAgentPrefab;
    //public Wave wavePrefab;


    //protected MapDisplay mapDisplay;

    // queue deployed Agents here
        // spawn Agents (contained in Waves at a constant rate)
            // if this queue is empty spawn a random benignAgent or else spawn from the queue
    private Queue<PreAgent> deployedAgents = new Queue<PreAgent>();

    /*
    private void Awake()
    {
        mapDisplay = GameObject.FindWithTag("MapDisplay").GetComponent<MapDisplay>();
        if (mapDisplay == null)
        {
            Debug.LogError("Could not find MapDisplay object in the scene. Either the tag was changed or the object is missing.");
        }
    }

    */

    private void Start()
    {
        mutateAgentText.text = MutateCost.ToString();
        advanceHackText.text = AdvanceTimeCost.ToString();
    }

    public void DeployAgent(int agentIndex)
    {
        if (hackCooldown != null && hackCooldown.DeployCount() > 0)
        {
            if (mapDisplay.selectedPath != null && mapDisplay.selectedPath.Valid)
            {
                //CmdSpawnWave(true, agents[agentIndex].Attribute, mapDisplay.selectedPath.ToVector2Array());
                deployedAgents.Enqueue(new PreAgent(maliciousAgent, mapDisplay.selectedPath, agents[agentIndex].Attribute));
                hackCooldown.DeployCount(-1);
            }
            else
            {
                Debug.LogWarning("No selected path!");
            }

        } else
        {
            Debug.LogWarning("No hack available right now!");
        }
    }

    private void Update()
    {
        currencyText.text = HackerCurrency.ToString();

        passiveTimer += Time.deltaTime;
        if (passiveTimer * passiveRate >= 1)
        {
            HackerCurrency++;
            passiveTimer %= 1 / passiveRate;
        }

        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnRate)
        {
            Spawn();


            spawnTimer %= spawnRate;
        }



    }

    void Spawn()
    {
        if(deployedAgents.Count > 0)
        {
            // uses a preagent
            PreAgent toSpawn = deployedAgents.Dequeue();
            CmdSpawnWave(true, toSpawn.agentAttribute, toSpawn.agentPath.ToVector2Array());
        } else
        {
            AgentAttribute attr;
            do
            {
                attr = GenerateAttribute();
            } while (attr.Equals(agents[0].Attribute) || attr.Equals(agents[1].Attribute));
            CmdSpawnWave(false, attr, GetRandomWavePath(mapDisplay).ToVector2Array());
        }
        
    }

    [Command]
    void CmdSpawnWave(bool malicious, AgentAttribute agentAttribute, Vector2[] coords)
    {
        Agent prefab;
        if(malicious)
        {
            prefab = maliciousAgent;
        } else
        {
            prefab = benignAgent;
        }

        PreAgent preAgent = new PreAgent(prefab, new WavePath(coords), agentAttribute);
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
