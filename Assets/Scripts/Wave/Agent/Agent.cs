using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moving unit that follows a WavePath and perform an action
/// </summary>
//[RequireComponent(typeof(NavMeshAgent))]
public abstract class Agent : VisualAgent
{

    /*-----------public variables-----------*/
    

    public int scoreMod = 5;

    public float destinationScaling = 0.01f;

    public Node CurrentNode
    {
        get
        {
            return currentNode;
        }
    }

    public float minY = -100f;


    /*-----------private variables-----------*/
    /// <summary>
    /// The WavePath the Agent will follow
    /// </summary>
    private WavePath wavePath;
    /// <summary>
    /// The Node that is currently the destination
    /// </summary>
    private Node currentNode;
    bool terminated = false;
    bool thrown = false;


    /*-----------private MonoBehavior functions-----------*/

    /// <summary>
    /// Checks the distance to the current Node, once in range switches to the next
    /// </summary>
    private void FixedUpdate()
    {
        if (!terminated && (currentNode.transform.position - transform.position).sqrMagnitude < destinationScaling)
        {
            Node nextNode = wavePath.GetNextNode();
            if (nextNode != null)
            {
                currentNode = nextNode;
                transform.LookAt(currentNode.transform.position); //???
            }
            else
            {
                Terminate();
            }

        }
        transform.position += Vector3.Normalize(currentNode.transform.position - transform.position) * Speed * Time.deltaTime;

        //points the object in the way it is falling
        if(terminated && thrown)
        {
            Rigidbody rigid;
            if ((rigid = GetComponent<Rigidbody>()) != null)
            {
                transform.rotation = Quaternion.LookRotation(rigid.velocity);
            } else
            {
                Debug.LogError("Could not find the rigidbody of a thrown object!");
            }
        }

        //deletes the object if it falls too far
        if(transform.position.y < minY)
        {
            Destroy(gameObject);
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
        //navAgent = GetComponent<NavMeshAgent>();
        wavePath = newWavePath;
        Node startNode = wavePath.GetNextNode();
        currentNode = startNode;
        transform.LookAt(currentNode.transform.position);
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
        AudioSource audio;

        //add animation
        if (Health.health > 0 && !RepairButton.Rebuilding)
        {
            DestinationAction();
            audio = GetComponents<AudioSource>()[0];
        } else
        {
            audio = GetComponents<AudioSource>()[1];
        }
        Destroy(GetComponent<Renderer>()); //or explode or other effect here
        StartCoroutine(playTerminationAudio(audio));
    }

    IEnumerator playTerminationAudio(AudioSource audio)
    {
        if (audio.clip != null)
        {
            audio.Play();
            yield return new WaitForSeconds(audio.clip.length);
        } else
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    protected override void ApplySize(Vector3 size)
    {
        transform.localScale = size;
    }

    public void Throw(Vector3 velocity)
    {
        terminated = true;
        thrown = true;
        transform.parent = null; //to make sure the game doesn't wait for the Agent to reach the barrier
        Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
        rigid.velocity = velocity;
    }

}
