using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agent type that damages Health of the player
/// </summary>
public class MaliciousAgent : Agent {

    /// <summary>
    /// How much higher the marker is placed above the Model Collider
    /// </summary>
    /// <seealso cref="AgentModel"/>
    public float markerOffset = 0.1f;
    /// <summary>
    /// Represents the malicious packet
    /// </summary>
    /// <remarks>
    /// Only displays when the Map enables the marker
    /// </remarks>
    /// <seealso cref="Map"/>
    /// <seealso cref="LevelLookup"/>
    private GameObject maliciousMarker;

    private void Awake()
    {
        //initializes the maliciousMarker
        maliciousMarker = transform.Find("MaliciousMarker").gameObject;
        //shows the marker if the level enables it
        if (LevelLookup.markMalicious)
        {
            maliciousMarker.SetActive(true);
        }
    }

    /// <summary>
    /// Removes scoreMod from Health
    /// </summary>
    /// <remarks>
    /// Only called when the servers are up and the Agent reaches its destination
    /// </remarks>
    /// <seealso cref="Score"/>
    public override void DestinationAction()
    {
        Score.Health -= scoreMod;
    }

    /// <summary>
    /// Hides the maliciousMarker
    /// </summary>
    /// <remarks>
    /// Called no matter what if the Agent reaches its destination
    /// </remarks>
    protected override void DerivedTerminated()
    {
        maliciousMarker.SetActive(false); //should already be false for non-marked
    }

    /// <summary>
    /// Places the maliciousMarker based on the size of the Model collider
    /// </summary>
    /// <param name="size">Desire size, currently not used</param>
    /// <param name="model">The model attached to the Agent</param>
    protected override void DerivedApplySize(float size, AgentModel model)
    {
       if (LevelLookup.markMalicious)
       {
            Collider modelCollider = model.GetComponent<Collider>();
            float height = transform.TransformDirection(modelCollider.bounds.size).y / 2f;
            maliciousMarker.transform.localPosition = Vector3.up * (height + markerOffset);
       }
    }
}
