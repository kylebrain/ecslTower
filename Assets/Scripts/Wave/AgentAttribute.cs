using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AgentAttribute {



    /// <summary>
    /// the possible colors af an agent
    /// </summary>
    public enum possibleColors { red, green, blue, dontCare };

    /// <summary>
    /// The current active color
    /// </summary>
    public possibleColors Color;


    /// <summary>
    /// The possible sizes of an agent
    /// </summary>
    public enum possibleSizes { small, medium, large, dontCare };

    /// <summary>
    /// The current active size
    /// </summary>
    public possibleSizes Size;

    /// <summary>
    /// The possible speeds of an agent
    /// </summary>
    public enum possibleSpeeds { small, medium, large, dontCare };

    /// <summary>
    /// The current active speed
    /// </summary>
    public possibleSpeeds Speed;
}
