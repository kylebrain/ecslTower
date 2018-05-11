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

    /*-----------public variables-----------*/
    public float health;
    //public Alliance alliance;
    public Color color;
    public float speed = 3.5f;


    /*-----------private variables-----------*/
    /// <summary>
    /// The NavMeshAgent for moving the Agent
    /// </summary>
    private NavMeshAgent navAgent;
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
        wavePath = newWavePath;

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
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

}
