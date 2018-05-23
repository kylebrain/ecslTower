using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moving unit that follows a WavePath and perform an action
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Agent : MonoBehaviour
{
    //testing home pc

    /*-----------public variables-----------*/
    public AgentAttribute Attribute;


    /*-----------private variables-----------*/
    /// <summary>
    /// The NavMeshAgent for moving the Agent
    /// </summary>
    public NavMeshAgent navAgent;
    /// <summary>
    /// The WavePath the Agent will follow
    /// </summary>
    private WavePath wavePath;
    /// <summary>
    /// The Node that is currently the destination
    /// </summary>
    private Node CurrentNode
    {
        get
        {
            return currentNode;
        }
        set
        {
            navAgent.SetDestination(value.transform.position);
            currentNode = value;
        }
    }
    private Node currentNode;


    /*-----------private MonoBehavior functions-----------*/

    private void Start()
    {
        InitializeAttributes(Attribute = GenerateAttribute());
        //Use function for the AI, but add an Attribute to AgentPath to allow for customization before pushing
    }

    /// <summary>
    /// Checks the distance to the current Node, once in range switches to the next
    /// </summary>
    private void FixedUpdate()
    {
        if ((currentNode.transform.position - transform.position).sqrMagnitude < 1)
        {
            Node nextNode = wavePath.GetNextNode();
            if (nextNode != null)
            {
                CurrentNode = nextNode;
                transform.LookAt(CurrentNode.transform.position); //???
            }
            else
            {
                Terminate();
            }

        }
    }


    /*-----------public function-----------*/
    /// <summary>
    /// Initializes the movement and begins following Path
    /// </summary>
    /// <remarks>
    /// Cannot use a constructor because Agent must be a GameObject
    /// </remarks>
    /// <param name="newWavePath">WavePath to be followed</param>
    public void BeginMovement(WavePath newWavePath)
    {
        navAgent = GetComponent<NavMeshAgent>();
        wavePath = newWavePath;
        Node startNode = wavePath.GetNextNode();
        CurrentNode = startNode;
        transform.LookAt(CurrentNode.transform.position);
    }


    /*-----------public abstract function-----------*/
    /// <summary>
    /// What the Agent will do when it reaches its destination
    /// </summary>
    public abstract void DestinationAction();


    /*-----------private function-----------*/
    /// <summary>
    /// Performs DestinationAction and Destroys the object itself
    /// </summary>
    private void Terminate()
    {
        //add animation
        DestinationAction();
        Destroy(gameObject);
    }

    /// <summary>
    /// Randomly generates an AgentAttribute for the Agent
    /// </summary>
    /// <returns>A random AgentAttribute</returns>
    private AgentAttribute GenerateAttribute()
    {
        AgentAttribute attributes;
        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.possibleColors)).Length - 1;
        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)).Length - 1;
        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)).Length - 1;

        attributes.Color = (AgentAttribute.possibleColors)Random.Range(0, numberColors);
        attributes.Size = (AgentAttribute.possibleSizes)Random.Range(0, numberSizes);
        attributes.Speed = (AgentAttribute.possibleSpeeds)Random.Range(0, numberSpeed);

        return attributes;
    }

    /// <summary>
    /// Sets all useable values of Agent based on the Enum values
    /// </summary>
    /// <param name="attributes">The desire attributes</param>
    public void InitializeAttributes(AgentAttribute attributes)
    {
        SetColor(attributes.Color);
        SetSize(attributes.Size);
        SetSpeed(attributes.Speed);
    }

    /// <summary>
    /// Sets the NavAgent speed based on speed Enum
    /// </summary>
    /// <param name="speed">Desired speed</param>
    public void SetSpeed(AgentAttribute.possibleSpeeds speed)
    {
        navAgent = GetComponent<NavMeshAgent>();
        switch (speed)
        {
            case AgentAttribute.possibleSpeeds.slow:
                navAgent.speed = 1.5f;
                break;
            case AgentAttribute.possibleSpeeds.normal:
                navAgent.speed = 3.5f;
                break;
            case AgentAttribute.possibleSpeeds.fast:
                navAgent.speed = 5.5f;
                break;
            default:
                Debug.LogError("Agent speed not recognized!");
                break;
        }
    }

    /// <summary>
    /// Sets the Transform scale based on size Enum
    /// </summary>
    /// <param name="size">Desired size</param>
    public void SetSize(AgentAttribute.possibleSizes size)
    {
        Vector3 newScale = Vector3.one;
        switch (size)
        {
            case AgentAttribute.possibleSizes.small:
                newScale = new Vector3(0.5f, 1, 0.5f);
                break;
            case AgentAttribute.possibleSizes.medium:
                newScale = new Vector3(1f, 1, 1f);
                break;
            case AgentAttribute.possibleSizes.large:
                newScale = new Vector3(1.5f, 1, 1.5f);
                break;
            default:
                Debug.LogError("Agent scale not recognized!");
                break;
        }
        transform.localScale = newScale;
    }

    /// <summary>
    /// Sets Render Material based on color Enum
    /// </summary>
    /// <param name="color">Desired Color</param>
    public void SetColor(AgentAttribute.possibleColors color)
    {
        Renderer rend = GetComponent<Renderer>();
        Material selectedMaterial = null;
        switch (color)
        {
            case AgentAttribute.possibleColors.red:
                selectedMaterial = Resources.Load<Material>("Agent/Red");
                break;
            case AgentAttribute.possibleColors.green:
                selectedMaterial = Resources.Load<Material>("Agent/Green");
                break;
            case AgentAttribute.possibleColors.blue:
                selectedMaterial = Resources.Load<Material>("Agent/Blue");
                break;
            default:
                Debug.LogError("Agent color not recognized!");
                break;
        }
        if (selectedMaterial != null)
        {
            rend.material = selectedMaterial;
        }
        else
        {
            Debug.LogError("Could not find Agent material. Perhaps it was moved?");
        }
    }
}
