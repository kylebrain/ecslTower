﻿using System.Collections;
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
        if (parentTower != null)
        {
            parentTower.filter = agentAttribute;
        }
        if (worldSpaceDisplayAgent != null)
        {
            worldSpaceDisplayAgent.InitializeAttributes(agentAttribute);
        }
    }

}
