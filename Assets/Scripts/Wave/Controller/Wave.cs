using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Container for PreAgent to be spawned
/// </summary>
public class Wave : NetworkBehaviour
{

    private bool WavePaused = false;

    /// <summary>
    /// How much time passes between each spawn
    /// </summary>
    /// <remarks>
    /// Different for each Map
    /// </remarks>
    public float timeBetweenAgent;

    /// <summary>
    /// Get only count of the waveQueue
    /// </summary>
    public int AgentsRemaining
    {
        get
        {
            return waveQueue.Count;
        }
    }

    /// <summary>
    /// Seconds spend waiting for the next spawn
    /// </summary>
    private float timeSpent = 0f;

    /// <summary>
    /// Contains the Wave of Agents to be spawned that follow their assigned Path
    /// </summary>
    private Queue<PreAgent> waveQueue = new Queue<PreAgent>();


    private void Awake()
    {
        //sets the spawnRate based on the level value
        timeBetweenAgent = LevelLookup.spawnRate;
    }

    /// <summary>
    /// Spawns an Agent when waveQueue has PreAgents in it
    /// </summary>
    /// <remarks>
    /// Calls Spawn after timeBetweenAgent seconds
    /// </remarks>
    private void Update()
    {

        //if (!WavePaused)
        //{

        if (timeSpent >= timeBetweenAgent && waveQueue.Count > 0)
        {
            timeSpent = 0;
            Spawn(waveQueue.Dequeue());
        }
        timeSpent += Time.deltaTime; //increments until it reaches the timeBetweenAgent

        //}

        //self-destructs if it has no Agents in game or queued
        if (transform.childCount == 0 && AgentsRemaining == 0)
        {
            if (hasAuthority)
            {
                Destroy(gameObject);
            }
        }

    }

    /// <summary>
    /// Sets the waveQueue to a copy of a List of PreAgents created in WaveController
    /// </summary>
    /// <seealso cref="WaveController"/>
    /// <param name="preAgents">PreAgents to become the Wave</param>
    public void CreateWaveWithList(List<PreAgent> preAgents)
    {
        waveQueue = new Queue<PreAgent>(preAgents);
    }

    public void AddNewAgent(PreAgent preAgent)
    {
        waveQueue.Enqueue(preAgent);
    }

    /// <summary>
    /// Spawn and start an Agent on its Path
    /// </summary>
    /// <param name="newPreAgent">The PreAgent to be spawned</param>
    public void Spawn(PreAgent newPreAgent)
    {
            //creates a new path based on the one in the PreAgent
        WavePath newPath = new WavePath(newPreAgent.agentPath);
        //gets the first Node which the Agent where the Agent will be spawned
        Node startNode = newPath.GetNextNode();
        //Spawns the Agent at the first Node
        Agent newAgent = Instantiate(newPreAgent.agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;

        //initializes the Agent through its methods
        newAgent.transform.parent = transform;
        
        newAgent.InitializeAttributes(newPreAgent.agentAttribute);

        if(newPath != null && newPath.NodeList.Count > 0)
        {
            newAgent.BeginMovement(newPath);
        }

        //NetworkServer.Spawn(newAgent.gameObject);

        if (hasAuthority)
        {
            NetworkServer.Spawn(newAgent.gameObject);

            Node[] nodeArray = newPreAgent.agentPath.NodeList.ToArray();
            Vector2Int[] coordArray = nodeArray.Select(x => x.Coordinate).ToArray();
            int[] pathX = coordArray.Select(x => x.x).ToArray();
            int[] pathY = coordArray.Select(x => x.y).ToArray();


            RpcUpdateAttributes(newAgent.gameObject, newAgent.Attribute, pathX, pathY, !hasAuthority);
        }

        //CmdSpawn(newAgent.GetComponent<NetworkIdentity>());
    }

    [ClientRpc]
    void RpcUpdateAttributes(GameObject identity, AgentAttribute attribute, int[] pathX, int[] pathY, bool movementBegan)
    {
        identity.GetComponent<Agent>().InitializeAttributes(attribute);
        if (!movementBegan)
        {
            WorldGrid worldGrid = GameObject.FindGameObjectWithTag("WorldGrid").GetComponent<WorldGrid>();
            List<Node> nodeArray = new List<Node>();
            for(int i = 0; i < pathX.Length; i++)
            {
                nodeArray.Add(worldGrid.getAt(pathX[i], pathY[i]));
            }
            identity.GetComponent<Agent>().BeginMovement(new WavePath(new Queue<Node>(nodeArray)));
        }
        


    }

    [Command]
    public void CmdSpawn(NetworkIdentity agentIdentity)
    {
        if (agentIdentity != null)
        {
            NetworkServer.Spawn(agentIdentity.gameObject);
        }
        else
        {
            Debug.LogError("Agent passed is null!");
        }
    }

    public void PauseSpawning(bool pause = true)
    {
        WavePaused = pause;
    }

    public void Pause(bool pause = true)
    {
        if (pause)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Agent>().Speed = 0f;
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Agent agent = transform.GetChild(i).GetComponent<Agent>();
                agent.SetSpeed(agent.Attribute.Speed);
            }
        }
    }
}
