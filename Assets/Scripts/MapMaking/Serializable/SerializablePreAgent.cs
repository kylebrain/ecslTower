using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SerializablePreAgent {

    /// <summary>
    /// Agent prefab that will be spawned
    /// </summary>
    public GameObject agentPrefab;
    /// <summary>
    /// The Path that the agent will follow
    /// </summary>
    public Vector2[] agentPath;
    /// <summary>
    /// Agent will be created with this Attribute
    /// </summary>
    public AgentAttribute agentAttribute;

    public SerializablePreAgent(PreAgent preAgent)
    {
        agentPrefab = preAgent.agentPrefab.gameObject;
        agentPath = preAgent.agentPath.ToVector2Array();
        agentAttribute = preAgent.agentAttribute;
    }

    public PreAgent ToPreAgent()
    {
        return new PreAgent(agentPrefab.GetComponent<Agent>(), new WavePath(agentPath), agentAttribute);
    }
}
