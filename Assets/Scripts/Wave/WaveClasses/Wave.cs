using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for Agents that have a destination to be Spawned
/// </summary>
public class Wave : MonoBehaviour {


    /*-----------private variable-----------*/
    /// <summary>
    /// Contains the Wave of Agents to be spawned that follow their assigned Path
    /// </summary>
    private Queue<AgentPath> waveQueue = new Queue<AgentPath>();


    /*-----------private MonoBehavoir function-----------*/
    /// <summary>
    /// Spawns a queued Agent when Enter is pressed
    /// </summary>
    /// <remarks>
    /// To be changed later to automatically spawn
    /// </remarks>
    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            if(waveQueue.Count > 0)
            {
                Spawn(waveQueue.Dequeue());
            } else
            {
                Debug.LogError("Wave queue is empty!");
            }
        }
    }


    /*-----------public functions-----------*/
    /// <summary>
    /// Adds an AgentPath to the list
    /// </summary>
    /// <param name="agent">The Agent prefab to be added</param>
    /// <param name="path">The WavePath this Agent will ultimately follow</param>
    /// <returns></returns>
    public AgentPath AddNewAgent(Agent agent, WavePath path)
    {
        AgentPath newAgentPath = new AgentPath(agent, path);
        waveQueue.Enqueue(newAgentPath);
        return newAgentPath;
    }

    /// <summary>
    /// Spawn and start an Agent on its Path
    /// </summary>
    /// <param name="newAgentPath">The AgentPath to be spawned</param>
    public void Spawn(AgentPath newAgentPath)
    {
        Debug.Log("Spawning!");
        WavePath newPath = new WavePath(newAgentPath.agentPath);
        Node startNode = newPath.GetNextNode();
        Agent newAgent = Instantiate(newAgentPath.agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;
        newAgent.BeginMovement(newPath);
    }
}
