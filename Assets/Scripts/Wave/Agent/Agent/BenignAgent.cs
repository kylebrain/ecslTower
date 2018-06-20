using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Derived Agent class that is helpful to the player
/// </summary>
public class BenignAgent : Agent {

    /// <summary>
    /// Adds to the Money of the Agent based on the scoreMode
    /// </summary>
    /// <seealso cref="Score"/>
    public override void DestinationAction()
    {
        Score.Money += scoreMod;
    }
}
