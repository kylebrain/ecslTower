using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Attacker : NetworkBehaviour
{

    public readonly static int StartingHackerCurrency = 0;
    public static int HackerCurrency = StartingHackerCurrency;

    readonly float passiveRate = 2;

    float passiveTimer = 0;

    public readonly static int AdvanceTimeCost = 10;
    public readonly static int MutateCost = 10;

    public HackCooldown hackCooldown;
    public Text currencyText;

    [SerializeField]
    RingDisplayAgent[] agents;
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

    public void DeployAgent(int agentIndex)
    {
        if (hackCooldown != null && hackCooldown.DeployCount() > 0)
        {
            if (mapDisplay.selectedPath != null && mapDisplay.selectedPath.Valid)
            {
                CmdSpawnWave(agentIndex, agents[agentIndex].Attribute, mapDisplay.selectedPath.ToVector2Array());
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
