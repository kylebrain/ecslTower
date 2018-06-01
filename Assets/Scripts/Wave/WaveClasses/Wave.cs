using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for Agents that have a destination to be Spawned
/// </summary>
public class Wave : MonoBehaviour {

    public float timeBetweenAgent = 0.5f;
    private float timeSpent = 0f;

    /*-----------private variable-----------*/
    /// <summary>
    /// Contains the Wave of Agents to be spawned that follow their assigned Path
    /// </summary>
    private Queue<AgentPath> waveQueue = new Queue<AgentPath>();

    public int WaveCount
    {
        get
        {
            return waveQueue.Count;
        }
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
    }


    /*-----------public functions-----------*/
    /// <summary>
    /// Adds an AgentPath to the list
    /// </summary>
    /// <param name="agent">The Agent prefab to be added</param>
    /// <param name="path">The WavePath this Agent will ultimately follow</param>
    /// <returns></returns>
    /*public AgentPath AddNewAgent(Agent agent, WavePath path)
    {
        AgentPath newAgentPath = new AgentPath(agent, path);
        waveQueue.Enqueue(newAgentPath);
        return newAgentPath;
    }*/

    public void CreateWaveWithList(List<AgentPath> agentPaths)
    {
        waveQueue = new Queue<AgentPath>(agentPaths);
    }

    /// <summary>
    /// Spawn and start an Agent on its Path
    /// </summary>
    /// <param name="newAgentPath">The AgentPath to be spawned</param>
    public void Spawn(AgentPath newAgentPath)
    {
        //Debug.Log("Spawning!");
        WavePath newPath = new WavePath(newAgentPath.agentPath);
        Node startNode = newPath.GetNextNode();
        Agent newAgent = Instantiate(newAgentPath.agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;
        newAgent.InitializeAttributes(newAgentPath.agentAttribute);
        newAgent.BeginMovement(newPath);

        if(waveQueue.Count <= 0)
        {
            Destroy(gameObject);
        }

    }
}
