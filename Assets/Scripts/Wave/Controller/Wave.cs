using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for PreAgent to be spawned
/// </summary>
public class Wave : MonoBehaviour {

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
        if (timeSpent >= timeBetweenAgent && waveQueue.Count > 0)
        {
            timeSpent = 0;
            Spawn(waveQueue.Dequeue());
        }
        timeSpent += Time.deltaTime; //increments until it reaches the timeBetweenAgent

        //self-destructs if it has no Agents in game or queued
        if(transform.childCount == 0 && AgentsRemaining == 0)
        {
            Destroy(gameObject);
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
        newAgent.BeginMovement(newPath);
        newAgent.InitializeAttributes(newPreAgent.agentAttribute);
    }
}
