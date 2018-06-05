using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RingDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Over = false;

    public RingDisplayAgent currentRingAgent;

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to true
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Over = true;
    }

    /// <summary>
    /// Basic workaround so that the menu does not close when clicked on, sets Over to false
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Over = false;
    }

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
