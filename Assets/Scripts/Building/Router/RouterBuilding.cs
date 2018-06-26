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

    public RoutingOptions routingOptions;

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

    public float highlightLightSaberValue = 0.7f;
    public Color highlightEmissionColor;

    public RingDisplayAgent childDisplayAgent;
    public LaserScript laserScript;
    //public VolumetricLines.VolumetricLineBehavior laser;
    public List<GameObject> Poles = new List<GameObject>();

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

    private float initLightSaberValue;

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

        initLightSaberValue = laserScript.laser.LightSaberFactor;
    }

    protected override void UpdateRotation(Node node)
    {
        if(node.Occupied != Node.nodeStates.navigation)
        {
            //router cannot be placed or rotated here
            //set to default value?
                //have to also handle what to do if no node is found
                //possibly with a null check but calling from building would need to be changed
            return;
        }
        List<Arrow> intersectingArrows = new List<Arrow>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            Arrow arrow = obj.GetComponent<Arrow>();
            if (node.IsBetween(arrow))
            {
                intersectingArrows.Add(arrow);
            }
        }
        SetRotation(intersectingArrows);
    }

    private void SetRotation(List<Arrow> arrowList)
    {
        if(arrowList.Count == 0)
        {
            return;
        }
        Vector3 pointVector;
        if(arrowList.Count == 1)
        {
            pointVector = arrowList[0].GetCardinality();
        } else
        {
            pointVector = arrowList[1].GetCardinality() + arrowList[0].GetCardinality(); //not the most eligant solution but it works to make sure that any thing over 1 is diagonal
        }
        transform.localRotation = Quaternion.LookRotation(new Vector3(pointVector.x, 0f, pointVector.y));
        if(childDisplayAgent != null)
        {
            childDisplayAgent.startingRotation = transform.eulerAngles.y;
        } else
        {
            Debug.LogError("Cannot find the childDisplayAgent please attach the Ring Agent in the inspector!");
        }
    }

    protected override void HighlightBuilding(bool highlight)
    {
        if (highlight)
        {
            foreach(GameObject obj in Poles)
            {
                Renderer rend = obj.GetComponent<Renderer>();
                rend.material.SetColor("_EmissionColor", highlightEmissionColor); //replace with variable when done
            }
            laserScript.laser.LightSaberFactor = highlightLightSaberValue;
        } else
        {
            foreach (GameObject obj in Poles)
            {
                Renderer rend = obj.GetComponent<Renderer>();
                rend.material.SetColor("_EmissionColor", Color.black); //default value
            }
            laserScript.laser.LightSaberFactor = initLightSaberValue;
        }
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
                        filtered = true;
                        break;
                    }
                }
                if(filtered == blacklist)
                {
                    DenyAgent(delAgent);
                } else
                {
                    AllowAgent(delAgent);
                }
                processedList.Add(delAgent);
                processedList.RemoveAll(agent => agent == null);
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

    private void AllowAgent(Agent agent)
    {
        allowed.pitch = AudioManager.VaryPitch(0.1f);
        allowed.Play();
        StartCoroutine(ChangeLaserColor(agent));
        agent.SetSpeed(agent.Attribute.Speed);
    }

    private void DenyAgent(Agent agent)
    {
        denied.pitch = AudioManager.VaryPitch(0.1f);
        denied.Play();
        ThrowDeniedAgent(agent);
        laserScript.laser.LineColor = Color.red;
    }

    

    IEnumerator ChangeLaserColor(Agent processedAgent)
    {
        laserScript.laser.LineColor = Color.green;
        yield return new WaitUntil(() => laserScript.hitAgent == processedAgent || laserScript.laser.LineColor == Color.red);
        yield return new WaitUntil(() => laserScript.hitAgent == null || laserScript.laser.LineColor == Color.red);
        laserScript.laser.LineColor = Color.red;
    }

    private void ThrowDeniedAgent(Agent agent)
    {
        float theta = throwAngle * Mathf.Deg2Rad;
        float yComponent = throwMagnitude * Mathf.Sin(theta);
        float horizontalMagnitude = throwMagnitude * Mathf.Cos(theta);
        Vector3 directionalVector;
        if(agent.CurrentNode.transform.position == transform.position)
        {
            directionalVector = Vector3.Normalize(transform.position - agent.transform.position);
        } else
        {
            directionalVector = Vector3.Normalize(agent.CurrentNode.transform.position - transform.position);
        }
        Vector3 xzComponent = -horizontalMagnitude * directionalVector;
        Vector3 velocity = new Vector3(xzComponent.x, yComponent, xzComponent.z);



        velocity = Quaternion.Euler(0f, Random.Range(-throwRange / 2f, throwRange / 2f), 0f) * velocity;
        agent.Throw(velocity);
    }

    private void OnDestroy()
    {
        foreach(Agent delAgent in processQueue)
        {
            delAgent.SetSpeed(delAgent.Attribute.Speed);
            processedList.Add(delAgent);
        }
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
