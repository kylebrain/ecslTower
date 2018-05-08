using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouterBuilding: Building {

    /// <summary>
    /// Decides whther to blacklist or whitelist.
    /// True = blacklist. False = whitelist.
    /// </summary>
    public bool blacklist = true;

    /// <summary>
    /// A list of attribute combinations which are blacklisted
    /// or whitelisted depending on the "blacklist" variable.
    /// To specify more general rules that only care about some of
    /// the attributes, put "dontCare" as the attribute for the
    /// other fields of AgentAttribute.
    /// </summary>
    private List<AgentAttribute> filter;



    

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
