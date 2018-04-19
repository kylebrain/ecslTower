using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public void Spawn(Agent agentPrefab, WavePath path)
    {
        Agent newAgent = Instantiate(agentPrefab) as Agent;
        newAgent.BeginMovement(path);
    }
}
