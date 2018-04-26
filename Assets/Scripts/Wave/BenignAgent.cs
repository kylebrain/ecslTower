using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenignAgent : Agent {

    public override void DestinationAction()
    {
        Debug.Log("Reached destination!");
    }
}
