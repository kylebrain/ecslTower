using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Moving unit that follows a WavePath and perform an action
/// </summary>
public abstract class Agent : VisualAgent
{

    #region Public Varibles

    /// <summary>
    /// How much the Agent effects the Score: Money or Health
    /// </summary>
    public int scoreMod = 5;

    /// <summary>
    /// How close an Agent can get to a Node before moving to the next Node
    /// </summary>
    /// <remarks>
    /// Lower: Agent follow more straight paths
    /// Too low: Agent will never reach their destination
    /// </remarks>
    public float destinationScaling = 0.01f;

    /// <summary>
    /// Get only of the Node the Agent is moving towards
    /// </summary>
    public Node CurrentNode
    {
        get
        {
            return currentNode;
        }
    }

    /// <summary>
    /// Agent is Destroyed when it reaches below this Y value in World Space
    /// </summary>
    public float minY = -100f;

    /// <summary>
    /// The particle animation played with the Destination Action
    /// </summary>
    public ParticleSystem destinationParticles;

    #endregion

    #region Private Variables

    /// <summary>
    /// The WavePath the Agent will follow
    /// </summary>
    private WavePath wavePath;
    /// <summary>
    /// The Node that is currently the destination
    /// </summary>
    private Node currentNode;
    /// <summary>
    /// The Agent no longer tracks a Node
    /// </summary>
    private bool terminated = false;
    /// <summary>
    /// The Agent is thrown
    /// </summary>
    /// <remarks>
    /// Thrown Agents should have a Rigidbody component
    /// Thrown Agents face the direction they are falling
    /// </remarks>
    private bool thrown = false;
    /// <summary>
    /// The visual representation of the Agent
    /// </summary>
    /// <remarks>
    /// ApplyColor and ApplySize used on this object
    /// </remarks>
    private AgentModel model = null;

    #endregion

    #region Movement

    /// <summary>
    /// Updates the position and rotation of the object
    /// </summary>
    private void FixedUpdate()
    {
        if(currentNode == null)
        {
            return;
        }

        //Checks the distance to the current Node, once in range switches to the next
        if (!terminated && (currentNode.transform.position - transform.position).sqrMagnitude < destinationScaling)
        {
            Node nextNode = wavePath.GetNextNode();
            //if the next node exists, point at it and move towards it
            if (nextNode != null)
            {
                currentNode = nextNode;
                transform.LookAt(currentNode.transform.position);
            }
            //if it is null, the Agent has reached its destination and terminates
            else
            {
                Terminate();
            }

        }
        transform.position += Vector3.Normalize(currentNode.transform.position - transform.position) * Speed * Time.deltaTime;

        //points the object in the way it is falling
        if (terminated && thrown)
        {
            Rigidbody rigid;
            if ((rigid = GetComponent<Rigidbody>()) != null)
            {
                transform.rotation = Quaternion.LookRotation(rigid.velocity);
            }
            else
            {
                Debug.LogError("Could not find the rigidbody of a thrown object!");
            }
        }

        //deletes the object if it falls too far
        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Initializes the movement and begins following Path
    /// </summary>
    /// <remarks>
    /// Cannot use a constructor because Agent must be a GameObject
    /// </remarks>
    /// <param name="newWavePath">WavePath to be followed, must start with the first destination Node</param>
    public void BeginMovement(WavePath newWavePath)
    {
        wavePath = newWavePath;
        Node startNode = wavePath.GetNextNode();
        currentNode = startNode; //Wave will remove the first Node before passing the WavePath will the first destination
        transform.LookAt(currentNode.transform.position);
    }

    #endregion

    #region Termination and Destination

    /// <summary>
    /// Decides what to do before terminating the object
    /// </summary>
    private void Terminate()
    {
        terminated = true;
        AudioSource audio;
        bool particles;
        if (Score.Health > 0 && !RepairButton.Rebuilding) //if the servers are up
        {
            DestinationAction();
            audio = GetComponents<AudioSource>()[0];
            particles = true;
        }
        else //if the servers are down do not play animation
        {
            audio = GetComponents<AudioSource>()[1];
            particles = false;
        }
        Destroy(GetComponent<Renderer>()); //or explode or other effect here
        model.gameObject.SetActive(false);
        DerivedTerminated();
        StartCoroutine(playTerminationAnimation(audio, particles));
    }

    /// <summary>
    /// What the derived Agent will do when it reaches its destination and the servers are up
    /// </summary>
    public abstract void DestinationAction();

    /// <summary>
    /// Allows the derived Agent to perform an action when terminated
    /// </summary>
    protected virtual void DerivedTerminated() { }

    /// <summary>
    /// Plays audio and/or particle effect
    /// </summary>
    /// <param name="audio">The audio to be played</param>
    /// <param name="particles">If the particle animation will be played</param>
    IEnumerator playTerminationAnimation(AudioSource audio, bool particles = true)
    {
        if (audio.clip != null || destinationParticles != null)
        {
            if (audio.clip != null)
            {
                audio.pitch = AudioManager.VaryPitch(0.1f);
                audio.Play();
            }
            //particles must be enabled and exist
            if (particles && destinationParticles != null)
            {
                destinationParticles = Instantiate(destinationParticles, transform);
                destinationParticles.Play();
            }
            //waits for the audio and/or the particles to finish playing
            yield return new WaitForSeconds(GetDestinationTime(audio, particles));
        }
        else
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Determines the longest wait time based on audio and particles
    /// </summary>
    /// <param name="audio">The audio to be played</param>
    /// <param name="particles">If the particle animation will be played</param>
    /// <returns></returns>
    private float GetDestinationTime(AudioSource audio, bool particles = true)
    {
        float ret = audio.clip.length;
        if (particles && destinationParticles != null)
        {
            //if the particles are wanted and longer than the audio length, then return the particles' duration
            ret = Mathf.Max(ret, destinationParticles.main.duration);
        }
        return ret;
    }

    /// <summary>
    /// Attaches a rigidbody and "throws" the Agent
    /// </summary>
    /// <param name="velocity">The direction and magnitude to be thrown</param>
    public void Throw(Vector3 velocity)
    {
        terminated = true;
        thrown = true; //if the Agent has a rigidbody, thrown must be true
        transform.parent = null; //to make sure the game doesn't wait for the Agent to reach the barrier
        model.GetComponent<Collider>().enabled = false;
        Rigidbody rigid;
        if ((rigid = GetComponent<Rigidbody>()) == null)
        {
            rigid = gameObject.AddComponent<Rigidbody>();
        }
        rigid.velocity = velocity;
    }

    #endregion

    #region AgentModel and Atrribute override

    /// <summary>
    /// Instantiates the AgentModel from Resources and sets the model variable
    /// </summary>
    protected override void CreateAgentModel()
    {
        //should only create one model
        if(model != null)
        {
            return;
        }
        model = Resources.Load<AgentModel>("AgentModel/" + LevelLookup.agentModel);
        if(model == null)
        {
            Debug.LogError("Could not find the requested AgentModel!");
            return;
        }
        model = Instantiate(model, transform);
    }

    /// <summary>
    /// Applies the size to the model and any derived classes
    /// </summary>
    /// <param name="size">Desired size</param>
    protected override void ApplySize(float size)
    {
        model.SetModelSize(size);
        DerivedApplySize(size, model);
    }

    /// <summary>
    /// Derived class can define what to do based on the size
    /// </summary>
    /// <param name="size">Desired size</param>
    /// <param name="model">Model to be changed</param>
    protected virtual void DerivedApplySize(float size, AgentModel model) { }

    /// <summary>
    /// Sets the Model color
    /// </summary>
    /// <param name="color">Desired color</param>
    protected override void ApplyColor(Color color)
    {
        model.SetModelColor(color);
        DerivedApplyColor(color, model);
    }

    /// <summary>
    /// Derived class can define what to do based on the size
    /// </summary>
    /// <param name="color">Desired color</param>
    /// <param name="model">Model to be changed</param>
    protected virtual void DerivedApplyColor(Color color, AgentModel model) { }

    #endregion
}
