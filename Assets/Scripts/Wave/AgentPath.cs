using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AgentPath {
    public Agent agentPrefab;
    public WavePath agentPath;

    public AgentPath(Agent agent, WavePath path)
    {
        agentPrefab = agent;
        agentPath = path;
    }
}
