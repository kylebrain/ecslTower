using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds both the possible traits but values of those traits to be used by Agents
/// </summary>
/// <seealso cref="VisualAgent"/>
public struct AgentAttribute {

    /// <summary>
    /// the possible colors af an agent
    /// </summary>
    public enum PossibleColors { red, green, blue, dontCare };

    /// <summary>
    /// The current active color
    /// </summary>
    public PossibleColors Color;

    /// <summary>
    /// The possible sizes of an agent
    /// </summary>
    public enum PossibleSizes { small, medium, large, dontCare };

    /// <summary>
    /// The current active size
    /// </summary>
    public PossibleSizes Size;

    /// <summary>
    /// The possible speeds of an agent
    /// </summary>
    public enum PossibleSpeeds { slow, normal, fast, dontCare };

    /// <summary>
    /// The current active speed
    /// </summary>
    public PossibleSpeeds Speed;

    /// <summary>
    /// Checks equality of AgentAttribute objects
    /// </summary>
    /// <param name="obj">The other AgentAttribute object</param>
    /// <returns>True if the Attributes are the same or either is a dontCare</returns>

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        AgentAttribute other = (AgentAttribute)obj;

        return other.Color == Color && other.Size == Size && other.Speed == Speed;
    }

    public bool Contains(AgentAttribute other)
    {
        return (other.Color == Color || other.Color == PossibleColors.dontCare || Color == PossibleColors.dontCare)
            && (other.Size == Size || other.Size == PossibleSizes.dontCare || Size == PossibleSizes.dontCare)
            && (other.Speed == Speed || other.Speed == PossibleSpeeds.dontCare || Speed == PossibleSpeeds.dontCare);
    }

    public static AgentAttribute NullAttribute
    {
        get
        {
            AgentAttribute ret;
            ret.Color = PossibleColors.dontCare;
            ret.Size = PossibleSizes.dontCare;
            ret.Speed = PossibleSpeeds.dontCare;
            return ret;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "Color: " + Color + ", Size: " + Size + ", Speed: " + Speed;
    }

}
