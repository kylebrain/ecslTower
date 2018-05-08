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
    public List<AgentAttribute> filter;

    /// <summary>
    /// The number of packets processed per second.
    /// The rate decreases based on the number of items in
    /// the filter.
    /// </summary>
    public float RoutingRate = 3;

    
    private float timeSinceLastFilter;

    /// <summary>
    /// The calculated required time before another agent can be filtered
    /// </summary>
    private float timeBetweenFilters;


    protected override void derivedStart() {
        timeSinceLastFilter = 0f;
        timeBetweenFilters = 1f / RoutingRate;
    }




    protected override void updateAction() {
        //Store the value of RoutingRate at the beggining of this frame
        float prevRoutingRate = RoutingRate;

        /*
         * Check to see if the user has clicked anything to:
         *  - add to the filter
         *  - change the mode blacklist/whitelist (determined by blacklist true/false)
         *  
         * Update the RoutingRate based on the size of filter.
         */

        //Recalculate the time between processing agents if the RoutingRate changed
        if(RoutingRate != prevRoutingRate) {
            timeBetweenFilters = 1f / RoutingRate;
        }

        //Add the time elapsed since the last frame to the time since the last agent
        timeSinceLastFilter += Time.deltaTime;

        //If the time since the last processing is g
        if(timeSinceLastFilter >= timeBetweenFilters) {
            timeSinceLastFilter = 0f;

            /*
             * For each agent waiting to pass, make sure:
             * 
             *  - If blacklist is true:
             *     + Start with everything allowed
             *     + Filter out any agents exactly matching the given attributes
             *     + Filter out any agents matching all attributes that are not "dontCare"
             *     + more specific rules override general ones
             *     
             *  - If blacklist is false:
             *     + Start with nothing allowed
             *     + Same thing as above except specifically allow agents rather than 
             *       filtering out agents
             */
        }
    }





}
