using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouterBuilding: Building {

    /// <summary>
    /// A list of exact attribute combinations which are blacklisted
    /// </summary>
    private List<AgentAttribute> blockedAttributes;

    /// <summary>
    /// A list of colors that are blacklisted in general
    /// </summary>
    private List<AgentAttribute.possibleColors> blockedColors;

    /// <summary>
    /// A list of sizes that are blacklisted in general
    /// </summary>
    private List<AgentAttribute.possibleSizes> blockedSizes;

    /// <summary>
    /// A list of speeds that are blacklisted in general
    /// </summary>
    private List<AgentAttribute.possibleSpeeds> blockedSpeeds;



    

    protected override void derivedStart() {
        
    }

    protected override void updateAction() {
        /*
         * Check to see if the user has clicked anything to add to any
         * of the "blocked" lists.
         */


        /*
         * For each agent waiting to pass, make sure:
         *  - their attribute doesn't EXACTLY match any attributes in "blockedAttributes"
         *  - none of their attributes equal anything in "blockedColors", "blockedSizes", or "blockedSpeeds"
         */
    }
}
