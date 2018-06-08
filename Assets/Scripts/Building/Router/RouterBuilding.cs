using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic unit of the CyberSecurity sim
/// </summary>
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
    public float processSpeed = 0.1f;

    public float throwAngle = 45f;
    public float throwMagnitude = 10f;
    public float throwRange = 120f;

    /// <summary>
    /// Queue of Agents to be processed, processed one at a time
    /// </summary>
    public Queue<Agent> processQueue = new Queue<Agent>();
    /// <summary>
    /// List of Agents that have been processed and will not be marked to process again 
    /// </summary>
    private List<Agent> processedList = new List<Agent>();

    private AudioSource allowed;
    private AudioSource denied;

    private float timeSinceLastFilter;

    /// <summary>
    /// The calculated required time before another agent can be filtered
    /// </summary>
    private float timeBetweenFilters;

    /// <summary>
    /// Finds all Agents and checks if they are in range, adds to processQueue and slows to form clump around ROuter
    /// </summary>
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
                if (!processedList.Contains(delAgent) && !processQueue.Contains(delAgent))
                {
                    delAgent.Speed = processSpeed;
                    processQueue.Enqueue(delAgent);
                }
            }
        }
    }

    protected override void derivedStart()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        allowed = audios[0];
        denied = audios[1];
        timeSinceLastFilter = 0f;
        timeBetweenFilters = 1f / RoutingRate;
    }




    protected override void updateAction()
    {
        if (!Placed)
        {
            return;
        }

        /*
        //test area
        if (selected)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    RoutingRate = i;
                    break;
                }
            }
        }

        */

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
            timeBetweenFilters = 1f / RoutingRate;

            /*
             * Dequeues processQueue until valid Agent is found
             * Checks to see if it triggers any filters
             * Deletes if it does
             * If not it resets the speed and marks it as processed by adding to the processedList
             */

            Agent delAgent = null;
            while (processQueue.Count > 0 && delAgent == null)
            {
                delAgent = processQueue.Dequeue();
            }
            if (delAgent != null)
            {
                bool filtered = false;
                foreach (AgentAttribute attribute in filter)
                {
                    if (delAgent.Attribute.Equals(attribute))
                    {
                        //Debug.Log("Deleted: " + delAgent.Attribute);
                        denied.Play();
                        ThrowDeniedAgent(delAgent);
                        filtered = true;
                        break;
                    }
                }
                if (!filtered)
                {
                    //Debug.Log("Let in: " + delAgent.Attribute);
                    allowed.Play();
                    delAgent.SetSpeed(delAgent.Attribute.Speed);
                }
                processedList.Add(delAgent);
                //add a way to delete Agents from the List who have been destroyed by reaching destination
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

    private void ThrowDeniedAgent(Agent agent)
    {
        float theta = throwAngle * Mathf.Deg2Rad;
        float yComponent = throwMagnitude * Mathf.Sin(theta);
        float horizontalMagnitude = throwMagnitude * Mathf.Cos(theta);
        Vector3 xzComponent = -horizontalMagnitude * Vector3.Normalize(agent.CurrentNode.transform.position - transform.position);
        Vector3 velocity = new Vector3(xzComponent.x, yComponent, xzComponent.z);



        velocity = Quaternion.Euler(0f, Random.Range(-throwRange / 2f, throwRange / 2f), 0f) * velocity;
        Debug.Log(velocity);
        agent.Throw(velocity);
    }

    protected void SetChildActive(string [] elements, bool active, GameObject canvas)
    {
        foreach(string element in elements)
        {
            canvas.transform.Find(element).gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// Shows the RoutingOptions with the Sell option
    /// </summary>
    /// <param name="canvas">The canvas on which it is displayed</param>
    protected override void derivedHide(GameObject canvas)
    {
        RingDisplay options = canvas.transform.Find("RingDisplay").GetComponent<RingDisplay>();
        if (options.Over)
        {
            ShowUI(canvas);
            return;
        }
        SetChildActive(new[] { "RingDisplay", "Tooltips" }, false, canvas);
    }

    /// <summary>
    /// Hides the RoutingOptions with the Sell option
    /// </summary>
    /// <param name="canvas">The canvas on which it is displayed</param>
    protected override void derivedShow(GameObject canvas)
    {
        SetChildActive(new[] { "RingDisplay", "Tooltips" }, true, canvas);
    }



}
