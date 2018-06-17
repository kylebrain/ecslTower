using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Derived Agent class that is harmless or helpful to the player
/// </summary>
public class BenignAgent : Agent {


    /*-----------public override function-----------*/
    /// <summary>
    /// Specific action for this type of Agent
    /// </summary>
    public override void DestinationAction()
    {
        Score.score += scoreMod;
    }
}
