using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

/// <summary>
/// Moving unit that follows a WavePath and perform an action
/// </summary>
//[RequireComponent(typeof(NavMeshAgent))]
public abstract class Agent : VisualAgent
{

    /*-----------public variables-----------*/
    

    public int scoreMod = 5;

    public float destinationScaling = 0.01f;


    /*-----------private variables-----------*/
    /// <summary>
    /// The NavMeshAgent for moving the Agent
    /// </summary>
        //public NavMeshAgent navAgent;
    /// <summary>
    /// The WavePath the Agent will follow
    /// </summary>
    private WavePath wavePath;
    /// <summary>
    /// The Node that is currently the destination
    /// </summary>
    private Node CurrentNode;
    bool terminated = false;


    /*-----------private MonoBehavior functions-----------*/

    private void Start()
    {
        //InitializeAttributes(Attribute = GenerateAttribute());
        //Use function for the AI, but add an Attribute to AgentPath to allow for customization before pushing
    }

    /// <summary>
    /// Checks the distance to the current Node, once in range switches to the next
    /// </summary>
    private void FixedUpdate()
    {
        if (!terminated && (CurrentNode.transform.position - transform.position).sqrMagnitude < destinationScaling)
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
        transform.position += Vector3.Normalize(CurrentNode.transform.position - transform.position) * Speed * Time.deltaTime;
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
        //navAgent = GetComponent<NavMeshAgent>();
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
        terminated = true;
        //add animation
        DestinationAction();
        Destroy(GetComponent<Renderer>()); //or explode or other effect here
        StartCoroutine(playTerminationAudio());
    }

    IEnumerator playTerminationAudio()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        Destroy(gameObject);
    }

    protected override void ApplySize(Vector3 size)
    {
        transform.localScale = size;
    }

}
