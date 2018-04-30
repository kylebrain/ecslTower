using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    private Queue<AgentPath> waveQueue = new Queue<AgentPath>();

    //** test area **//
    public Agent agent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(waveQueue.Count > 0)
            {
                Spawn(waveQueue.Dequeue());
            }
        }
    }

    public AgentPath AddNewAgent(Agent agent, WavePath path)
    {
        AgentPath newAgentPath = new AgentPath(agent, path);
        waveQueue.Enqueue(newAgentPath);
        return newAgentPath;
    }

    //** End of test area **//

    public void Spawn(AgentPath newAgentPath)
    {
        Debug.Log("Spawning!");
        WavePath newPath = new WavePath(newAgentPath.agentPath);
        Node startNode = newPath.GetNextNode();
        Agent newAgent = Instantiate(newAgentPath.agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;
        newAgent.BeginMovement(newPath);
    }
}
