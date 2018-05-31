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
        Score.score -= scoreMod;
    }
}
