using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerBuilding : Building
{

    public RingDisplayAgent[] ringDisplayAgents;
    bool[] filledDisplays;
    public List<Agent> scannedAgents = new List<Agent>();
    AgentAttribute nullAttribute;

    protected override void derivedStart()
    {
        filledDisplays = new bool[ringDisplayAgents.Length];
        nullAttribute.Color = AgentAttribute.PossibleColors.dontCare;
        nullAttribute.Size = AgentAttribute.PossibleSizes.dontCare;
        nullAttribute.Speed = AgentAttribute.PossibleSpeeds.dontCare;
        foreach (RingDisplayAgent ringDisplay in ringDisplayAgents)
        {
            ringDisplay.InitializeAttributes(nullAttribute);
        }
    }

    protected override void updateAction()
    {
        List<Agent> scanningAgents = GetAgentsInRadius();
        foreach(Agent currentAgent in scanningAgents)
        {
            if(!scannedAgents.Contains(currentAgent))
            {
                scannedAgents.Add(currentAgent);
                bool displayFilled = true;
                for(int i = 0; i < filledDisplays.Length; i++)
                {
                    if (!filledDisplays[i])
                    {
                        ringDisplayAgents[i].InitializeAttributes(currentAgent.Attribute);
                        filledDisplays[i] = true;
                        displayFilled = false;
                        break;
                    }
                }
                if (displayFilled)
                {
                    for(int i = ringDisplayAgents.Length - 1; i > 0; i--)
                    {
                        ringDisplayAgents[i].InitializeAttributes(ringDisplayAgents[i - 1].Attribute);
                    }
                    ringDisplayAgents[0].InitializeAttributes(currentAgent.Attribute);
                }

            }
        }
    }

    protected override void HighlightBuilding(bool highlight)
    {
        
    }
}
