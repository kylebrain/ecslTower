using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Displays a RingDisplayAgent based on attributes
/// </summary>
/// <remarks>
/// Requires a child RingDisplayAgent called RingAgent, set up in the prefab
/// </remarks>
/// <seealso cref="RingDisplayAgent"/>
public class RingDisplay : MonoBehaviour
{
    public RingDisplayAgent currentRingAgent;

    private void Awake()
    {
        //sets the RingDisplayAgent variable based on its child
        currentRingAgent = GetComponentInChildren<RingDisplayAgent>();
        if(currentRingAgent == null)
        {
            Debug.LogError("Could not find RingAgent, perhaps it was moved or renamed?");
        }
    }

    /// <summary>
    /// Updates its RingDisplayAgent based on a passed attribute
    /// </summary>
    /// <param name="attribute">Desired attribute</param>
    public void UpdateDisplay(AgentAttribute attribute)
    {
        currentRingAgent.InitializeAttributes(attribute);
    }
}
