using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RingDisplay : MonoBehaviour
{
    public RingDisplayAgent currentRingAgent;

    private void Awake()
    {
        currentRingAgent = transform.Find("RingAgent").GetComponent<RingDisplayAgent>();
        if(currentRingAgent == null)
        {
            Debug.LogError("Could not find RingAgent, perhaps it was moved or renamed?");
        }
    }

    public void UpdateDisplay(AgentAttribute attribute)
    {
        currentRingAgent.InitializeAttributes(attribute);
    }
}
