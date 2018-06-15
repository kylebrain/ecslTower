using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaliciousAgent : Agent {

    /*-----------public override function-----------*/
    /// <summary>
    /// Specific action for this type of Agent
    /// </summary>
    public override void DestinationAction()
    {
        Health.health -= scoreMod;
    }

    private void Start()
    {
        if (LevelLookup.markMalicious)
        {
            transform.Find("MaliciousMarker").gameObject.SetActive(true);
        }
    }

    protected override void DerivedTerminated()
    {
        transform.Find("MaliciousMarker").gameObject.SetActive(false); //should already be false for non-marked
    }
}
