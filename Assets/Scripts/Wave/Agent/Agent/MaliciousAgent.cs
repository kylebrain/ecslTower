using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaliciousAgent : Agent {

    private GameObject maliciousMarker;
    public float markerOffset = 0.1f;

    /*-----------public override function-----------*/
    /// <summary>
    /// Specific action for this type of Agent
    /// </summary>
    public override void DestinationAction()
    {
        Score.Health -= scoreMod;
    }

    private void Awake()
    {
        maliciousMarker = transform.Find("MaliciousMarker").gameObject;
        if (LevelLookup.markMalicious)
        {
            maliciousMarker.SetActive(true);
        }
    }

    protected override void DerivedTerminated()
    {
        maliciousMarker.SetActive(false); //should already be false for non-marked
    }

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
