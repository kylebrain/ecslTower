using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public List<WavePath> pathList = new List<WavePath>();

    //** test area **//
    public Agent agent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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
        Debug.Log("Spawning!");
        Node startNode = path.GetNextNode();
        Agent newAgent = Instantiate(agentPrefab, startNode.transform.position, Quaternion.identity) as Agent;
        WavePath newPath = path;
        newAgent.BeginMovement(newPath);
    }
}
