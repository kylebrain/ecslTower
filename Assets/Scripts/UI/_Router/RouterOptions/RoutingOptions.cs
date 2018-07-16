using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutingOptions : RolodexHandler {
    public float radiusIncrease = 0.5f;
    /// <summary>
    /// The parent Router that the Dropdown is attached to
    /// </summary>
    private RouterBuilding parentTower;
    public RingDisplayAgent worldSpaceDisplayAgent;

    protected override void DerivedStart()
    {
        parentTower = transform.root.gameObject.GetComponent<RouterBuilding>();
        if (worldSpaceDisplayAgent != null && parentTower != null)
        {
            worldSpaceDisplayAgent.radius = parentTower.Radius + radiusIncrease;
        }
    }

    protected override void DerivedUpdateFilter(AgentAttribute agentAttribute)
    {
        if (parentTower == null)
        {
            parentTower = transform.root.gameObject.GetComponent<RouterBuilding>();
        }
        parentTower.CmdUpdateFilter(agentAttribute);

        //not sure if this will update on other system?
            //possibly is only updating on the local client and server and nothing else
        parentTower.filter = agentAttribute;

        /*
        if (parentTower != null)
        {
            parentTower.filter = agentAttribute;
        }
        if (worldSpaceDisplayAgent != null)
        {
            worldSpaceDisplayAgent.InitializeAttributes(agentAttribute);
        } */
    }

}
