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
    public enum possibleSpeeds { slow, normal, fast, dontCare };

    /// <summary>
    /// The current active speed
    /// </summary>
    public possibleSpeeds Speed;

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        AgentAttribute other = (AgentAttribute)obj;
        return (other.Color == Color || other.Color == possibleColors.dontCare || Color == possibleColors.dontCare)
            && (other.Size == Size || other.Size == possibleSizes.dontCare || Size == possibleSizes.dontCare)
            && (other.Speed == Speed || other.Speed == possibleSpeeds.dontCare || Speed == possibleSpeeds.dontCare);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
