using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Agent : MonoBehaviour {
    public float health;
    //private Alliance alliance;
    public Color color;
    public float Speed;

    private NavMeshAgent navAgent;
    private WavePath wavePath;

    public abstract void DestinationAction();

    public void BeginMovement(WavePath newWavePath)
    {
        wavePath = newWavePath;
        navAgent = GetComponent<NavMeshAgent>();
        Node firstDestination = wavePath.GetNextNode();
        navAgent.SetDestination(firstDestination.transform.position);
        transform.LookAt(firstDestination.transform.position);
    }

    private void Terminate()
    {
        //add animation
        Destroy(this);
    }

}
