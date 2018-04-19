using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public List<WavePath> pathList = new List<WavePath>();

    //** test area **//
    public Agent agent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if(pathList.Count > 0)
            {
                Spawn(agent, pathList[0]);
            }
        }
    }

    //** End of test area **//

    public void Spawn(Agent agentPrefab, WavePath path)
    {
        Agent newAgent = Instantiate(agentPrefab, path.GetNextNode().transform) as Agent;
        newAgent.BeginMovement(path);
    }
}
