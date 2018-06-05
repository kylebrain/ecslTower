using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct that holds an Agent to be spawned and a WavePath that it will follow
/// </summary>
public struct PreAgent {
    /// <summary>
    /// Agent prefab that will be spawned
    /// </summary>
    public Agent agentPrefab;
    /// <summary>
    /// The Path that the agent will follow
    /// </summary>
    public WavePath agentPath;

    public AgentAttribute agentAttribute;

    /// <summary>
    /// Parametrized constructor must be used to have a valid PreAgent
    /// </summary>
    /// <param name="agent">Sets Agent struct variable to this</param>
    /// <param name="path">Sets WavePath struct variable to this</param>
    public PreAgent(Agent agent, WavePath path, AgentAttribute attribute)
    {
        agentPrefab = agent;
        agentPath = path;
        agentAttribute = attribute;
    }
}
