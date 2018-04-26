using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Agent : MonoBehaviour
{
    public float health;
    //private Alliance alliance;
    public Color color;
    public float Speed;

    private NavMeshAgent navAgent;
    private WavePath wavePath;
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

    public abstract void DestinationAction();

    private void FixedUpdate()
    {
        if ((currentNode.transform.position - transform.position).sqrMagnitude < 1)
        {
            Node nextNode = wavePath.GetNextNode();
            if(nextNode != null)
            {
                CurrentNode = nextNode;
            } else
            {
                Terminate();
            }
            
        }
    }

    public void BeginMovement(WavePath newWavePath)
    {
        wavePath = newWavePath;
        navAgent = GetComponent<NavMeshAgent>();
        Node startNode = wavePath.GetNextNode();
        CurrentNode = startNode;
        transform.LookAt(startNode.transform.position);
    }

    private void Terminate()
    {
        //add animation
        Debug.Log("Reached destination!");
        Destroy(gameObject);
    }

}
