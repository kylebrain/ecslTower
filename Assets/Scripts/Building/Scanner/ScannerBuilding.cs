using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerBuilding : Building
{

    //public RingDisplayAgent[] ringDisplayAgents;
    //bool[] filledDisplays;
    public int scanAgentCount = 3;
    public float agentRadius = 30f;
    public float agentBuffer = 15f;

    public RectTransform displayHolder;
    public MovingRingDisplayAgent displayAgentPrefab;
    //public GameObject circleGameObject;

    private List<Agent> scannedAgents = new List<Agent>();
    AgentAttribute nullAttribute;

    private List<MovingRingDisplayAgent> hudAgents = new List<MovingRingDisplayAgent>();
    private float agentSize;

    protected override void derivedStart()
    {
        nullAttribute.Color = AgentAttribute.PossibleColors.dontCare;
        nullAttribute.Size = AgentAttribute.PossibleSizes.dontCare;
        nullAttribute.Speed = AgentAttribute.PossibleSpeeds.dontCare;
        agentSize = 2 * (agentRadius + agentRadius);
        RectTransform parentRect = displayHolder.transform.parent.GetComponent<RectTransform>();
        parentRect.sizeDelta = new Vector2(scanAgentCount * agentSize, parentRect.sizeDelta.y);

        /*
        filledDisplays = new bool[ringDisplayAgents.Length];
        nullAttribute.Color = AgentAttribute.PossibleColors.dontCare;
        nullAttribute.Size = AgentAttribute.PossibleSizes.large;
        nullAttribute.Speed = AgentAttribute.PossibleSpeeds.normal;
        foreach (RingDisplayAgent ringDisplay in ringDisplayAgents)
        {
            ringDisplay.InitializeAttributes(nullAttribute);
        }
        */
    }

    protected override void updateAction()
    {
        List<Agent> scanningAgents = GetAgentsInRadius();
        foreach (Agent currentAgent in scanningAgents)
        {
            if (!scannedAgents.Contains(currentAgent))
            {
                scannedAgents.Add(currentAgent);
                MovingRingDisplayAgent currentDisplayAgent = Instantiate(displayAgentPrefab, displayHolder.transform);
                currentDisplayAgent.InitDisplayAgent();
                currentDisplayAgent.displayAgent.InitializeAttributes(currentAgent.Attribute);
                currentDisplayAgent.SetPosition(new Vector2(-agentSize * (scanAgentCount + 1 / 2f), 0));
                currentDisplayAgent.SetDesiredPos(new Vector2(-agentSize * ((hudAgents.Count > scanAgentCount ? scanAgentCount : hudAgents.Count) + 1 / 2f), 0f));
                foreach (MovingRingDisplayAgent movingAgent in hudAgents)
                {
                    if (movingAgent.displayAgent.Attribute.Speed == currentDisplayAgent.displayAgent.Attribute.Speed)
                    {
                        currentDisplayAgent.displayAgent.startingRotation = movingAgent.displayAgent.GetRotation;
                        break;
                    }
                }
                hudAgents.Add(currentDisplayAgent);

                for (int i = 0; i < hudAgents.Count; i++)
                {
                    MovingRingDisplayAgent delAgent = hudAgents[i];
                    if (delAgent.rectTransform.anchoredPosition.x > 0 && delAgent.IsAtDestination)
                    {
                        //Debug.Log(delAgent.center.x + "\n" + delAgent.desiredPos);
                        hudAgents.Remove(delAgent);
                        Destroy(delAgent.gameObject);
                        i--;
                    }
                }

                if (hudAgents.Count > scanAgentCount)
                {
                    foreach (MovingRingDisplayAgent movingAgent in hudAgents)
                    {
                        movingAgent.SetDesiredPos(movingAgent.DesiredPos + Vector2.right * agentSize);
                    }
                }

            }
        }
    }

    protected override void HighlightBuilding(bool highlight)
    {
        Material mat = transform.Find("Model").GetComponent<Renderer>().material;
        //change to the name of the visual representation
        if (highlight)
        {
            mat.SetColor("_Color", Color.grey);
        } else
        {
            mat.SetColor("_Color", Color.white);
        }
    }
}
