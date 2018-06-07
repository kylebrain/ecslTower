using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for Agents that have a destination to be Spawned
/// </summary>
public class Wave : MonoBehaviour {

    public float timeBetweenAgent = LevelLookup.spawnRate;
    private float timeSpent = 0f;

    /*-----------private variable-----------*/
    /// <summary>
    /// Contains the Wave of Agents to be spawned that follow their assigned Path
    /// </summary>
    private Queue<PreAgent> waveQueue = new Queue<PreAgent>();

    public int AgentsRemaining
    {
        get
        {
            return waveQueue.Count;
        }
    }

    private void Awake()
    {
        timeBetweenAgent = LevelLookup.spawnRate;
    }

    /*-----------private MonoBehavoir function-----------*/
    /// <summary>
    /// Spawns a queued Agent when Enter is pressed
    /// </summary>
    /// <remarks>
    /// To be changed later to automatically spawn
    /// </remarks>
    private void Update()
    {
        if (timeSpent >= timeBetweenAgent && waveQueue.Count > 0)
        {
            timeSpent = 0;
            Spawn(waveQueue.Dequeue());
        }
        timeSpent += Time.deltaTime;

        if(transform.childCount == 0 && AgentsRemaining == 0)
        {
            Destroy(gameObject);
        }

    }

    public void CreateWaveWithList(List<PreAgent> preAgents)
    {
        waveQueue = new Queue<PreAgent>(preAgents);
    }

    /// <summary>
    /// Spawn and start an Agent on its Path
    /// </summary>
    /// <param name="newPreAgent">The PreAgent to be spawned</param>
    public void Spawn(PreAgent newPreAgent)
    {
        WavePath newPath = new WavePath(newPreAgent.agentPath);
        Node startNode = newPath.GetNextNode();
        Agent newAgent = Instantiate(newPreAgent.agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;
        newAgent.transform.parent = transform;
        newAgent.InitializeAttributes(newPreAgent.agentAttribute);
        newAgent.BeginMovement(newPath);
    }
}
