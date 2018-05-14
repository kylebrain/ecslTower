using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouterBuilding : Building
{

    /// <summary>
    /// Decides whther to blacklist or whitelist.
    /// True = blacklist. False = whitelist.
    /// </summary>
    [Tooltip("True = blacklist, False = whitelist")]
    public bool blacklist = true;

    /// <summary>
    /// A list of attribute combinations which are blacklisted
    /// or whitelisted depending on the "blacklist" variable.
    /// To specify more general rules that only care about some of
    /// the attributes, put "dontCare" as the attribute for the
    /// other fields of AgentAttribute.
    /// </summary>
    public List<AgentAttribute> filter = new List<AgentAttribute>();

    /// <summary>
    /// The number of packets processed per second.
    /// The rate decreases based on the number of items in
    /// the filter.
    /// </summary>
    public float RoutingRate = 3;

    public Queue<Agent> deleteQueue = new Queue<Agent>();
    private List<Agent> processedList = new List<Agent>();


    private float timeSinceLastFilter;

    /// <summary>
    /// The calculated required time before another agent can be filtered
    /// </summary>
    private float timeBetweenFilters;

    private void InRadius()
    {
        GameObject[] agentArray = GameObject.FindGameObjectsWithTag("Agent");
        foreach (GameObject obj in agentArray)
        {
            if (Vector3.SqrMagnitude(transform.position - obj.transform.position) <= Mathf.Sqrt(Radius))
            {
                Agent delAgent = obj.GetComponent<Agent>();
                if (delAgent == null)
                {
                    Debug.LogError("Cannot find Agent script of object tagged Agent!");
                    continue;
                }
                if (!processedList.Contains(delAgent) && !deleteQueue.Contains(delAgent))
                {
                    delAgent.navAgent.speed = 0.1f; //change from a magic number
                    deleteQueue.Enqueue(delAgent);
                }
            }
        }
    }

    protected override void derivedStart()
    {
        timeSinceLastFilter = 0f;
        timeBetweenFilters = 1f / RoutingRate;
    }




    protected override void updateAction()
    {
        InRadius();

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
        if (RoutingRate != prevRoutingRate)
        {
            timeBetweenFilters = 1f / RoutingRate;
        }

        //Add the time elapsed since the last frame to the time since the last agent
        timeSinceLastFilter += Time.deltaTime;

        //If the time since the last processing is g
        if (timeSinceLastFilter >= timeBetweenFilters)
        {
            timeSinceLastFilter = 0f;
            Agent delAgent = null;
            while (deleteQueue.Count > 0 && delAgent == null)
            {
                delAgent = deleteQueue.Dequeue();
            }
            if(delAgent != null){
                bool filtered = false;
                foreach (AgentAttribute attribute in filter)
                {
                    if (delAgent.Attribute.Equals(attribute))
                    {
                        Debug.Log("Deleted!");
                        Destroy(delAgent.gameObject);
                        filtered = true;
                        break;
                    }
                }
                if (!filtered)
                {
                    delAgent.SetSpeed(delAgent.Attribute.Speed);
                    processedList.Add(delAgent);
                }
                //processedList.RemoveAll(null);
            }


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

    protected override void HideUI(GameObject canvas)
    {
        RoutingOptions options = canvas.transform.Find("RoutingOptions").GetComponent<RoutingOptions>();
        if (!options.Over)
        {
            radiusLine.enabled = false;
            canvas.transform.Find("Sell").gameObject.GetComponent<GameButton>().Hide();
            canvas.transform.Find("RoutingOptions").gameObject.SetActive(false);
        }
    }

    protected override void ShowUI(GameObject canvas)
    {
        radiusLine.enabled = true;
        canvas.transform.Find("Sell").gameObject.GetComponent<GameButton>().Show();
        canvas.transform.Find("RoutingOptions").gameObject.SetActive(true);
    }



}
